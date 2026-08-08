---
name: solve-gaps
description: Reads docs/gaps.md, lets the user pick which open gaps to fix, then runs Opus triage + planning and parallel Sonnet/Haiku fixing subagents (2 at a time, isolated worktrees) to resolve them on one branch, finishing with /bump-version.
---

# Solve gaps

## When to use

Invoked explicitly (`/solve-gaps`) to work through one or more entries in `docs/gaps.md`'s
`## Open` section end-to-end: pick gaps → branch → triage → plan (if needed) → fix → merge →
version bump.

Read `docs/gaps.md`'s own "How to use this file" section before starting — it defines the exact
format a `### ` gap entry and its **Status:** line must follow. This skill is the automation on
top of those house rules, not a replacement for them.

## Pipeline overview

```
list open gaps → user picks → ensure one branch
  → one batched Opus triage call (effort / model / plan-needed, per gap)
  → queue of {plan?, fix} tasks, ≤2 agents in flight at once, worktree-isolated
      plan (Opus, "Plan" agent, worktree)  →  fix (Haiku/Sonnet, worktree) → merge + docs
  → /bump-version
```

## Step 1 — List open gaps

Read `docs/gaps.md`. Parse the `## Open` section (everything between the `## Open` heading and
the next `## ` heading): each `### ` line is one gap, its body runs until the next `### ` or the
next `## `.

- If the section is empty (e.g. contains only `_None currently._`), tell the user there's nothing
  to fix and stop.
- Otherwise, ask the user which to fix with `AskUserQuestion`, `multiSelect: true`, options being
  `"All open gaps"` followed by each gap's title. If `"All open gaps"` is selected, treat it as
  selecting every gap regardless of what else was picked.

Keep the gaps in the order they appear in the file — that order matters for naming and for
triage.

## Step 2 — Ensure a branch

- Run `git branch --show-current`.
- If it's **not** the default branch (`main`): reuse it as-is. Do not create a new branch. This
  skill run makes at most one *user-facing* branch, ever — reusing an existing non-main branch
  satisfies that by not creating one at all.
- If it **is** `main`: create exactly one branch, named:
  - Single gap selected: `fix/<short-slug>`, where `<short-slug>` is a short (2-5 word) kebab-case
    phrase capturing the gist of the gap — not a mechanical slugification of the whole title.
    E.g. for "No way to distinguish a DB-unavailable error from other unexpected DB errors", use
    something like `fix/database-unavailable-error`, not the full sentence.
  - Multiple gaps selected: `fix/gaps-<short-slug-of-first-selected-gap>-plus-N`, where "first
    selected" means the first of the chosen gaps in file order (not click order) and `N` is the
    count of additional chosen gaps. E.g. 3 gaps chosen → `...-plus-2`.

**This is the only branch this skill creates.** The isolated git worktrees used internally in
Step 4 each carry their own throwaway branch — those are implementation plumbing, get merged into
(or discarded without ever touching) the one branch from this step, and are deleted immediately
after. They don't count against "one branch per run" and are never left behind.

## Step 3 — Batched Opus triage

Make **one** `Agent` call (not one per gap) — `model: "opus"`, default `subagent_type`. Give it,
for every selected gap, the full `###` entry text (title + body, including the existing **Fix:**
proposal) plus enough repo context to judge scope (it can Read/Grep the repo itself).

Ask it to return, for each gap, in a clearly parseable per-gap block:
- **Effort**: trivial / small / medium / large
- **Model**: `haiku` or `sonnet` — the model that should implement the fix. Default mapping
  (trivial/small → `haiku`, medium/large → `sonnet`), but it may override with a one-line reason
  (e.g. a "trivial"-looking gap that's actually fiddly, or a "large" one that's mechanical
  repetition).
- **Plan needed**: yes/no — whether a fix plan should be written before implementation starts.
  Default mapping (trivial/small → no, medium/large → yes), same override allowance.
- **Reasoning**: one or two sentences.

This call is synchronous (`run_in_background: false`) — nothing else can proceed without its
result.

## Step 4 — Plan + fix, 2 agents in flight at a time

Build a per-gap task: `{ needs_plan, model }` from Step 3's output. Each gap becomes one or two
queued agent tasks:
- Needs a plan: `plan task` → (on completion) `fix task`, in that order, for that gap.
- Doesn't need a plan: `fix task` only.

Run this as a queue with **at most 2 agents in flight at once, across planning and fixing
combined**. Launch tasks with `run_in_background: true` and react only to completion
notifications — never poll or sleep waiting on them. Whenever a slot frees up (a task completes),
launch the next eligible queued task (a gap's fix task only becomes eligible once its plan task,
if any, has completed and its plan is in hand).

**Plan task** — `Agent` call:
- `subagent_type: "Plan"`, `model: "opus"`, `isolation: "worktree"`.
- Prompt: the gap's full entry text, the relevant files it names, and a request for a concrete
  implementation plan (what to add/change, where, and how to test it) — same depth as you'd want
  before doing the fix yourself.
- `Plan` agents can't write files, so with no changes made their worktree is auto-cleaned by the
  tool. Nothing to merge or clean up manually here — just hand the returned plan to that gap's fix
  task.

**Fix task** — `Agent` call:
- `subagent_type: "claude"`, `model` from Step 3's per-gap recommendation (`"haiku"` or
  `"sonnet"`), `isolation: "worktree"`.
- Base it off the shared branch's **current tip** (i.e. after any earlier gaps in this run have
  already been merged into it) — not off `main` and not off a stale snapshot — to minimize merge
  conflicts later.
- Prompt: the gap's full entry text, the plan if one was produced, and explicit instructions to:
  implement the fix, add/extend tests that cover it, run the build and the relevant test
  project(s), and make exactly one commit for the code + tests. Tell it explicitly **not** to
  touch `docs/gaps.md` — the orchestrator (you) handles that after merging, to avoid every fixing
  agent racing to append to the same section of the same file.

**On a fix task's completion:**
1. Skim the diff (trust but verify — the agent's summary describes intent, not necessarily what
   landed).
2. Merge its worktree branch into the shared branch.
3. On the shared branch, update `docs/gaps.md`: move that gap's `### ` entry from `## Open` to the
   end of `## Fixed`, following the file's own "How to use this file" rules exactly (status line,
   what was actually implemented, test pointer, no PR/branch/date noise). Commit this as its own
   small commit immediately following the merge — so each gap ends up as a fix-and-tests commit
   plus a docs commit, landing together.
4. Delete the worktree and its branch.
5. Free the slot; if the queue has an eligible task, launch it.

## Step 5 — Version bump

Once every selected gap's fix has been merged and every worktree cleaned up, invoke the
`bump-version` skill on the shared branch (no bump type specified — let it auto-detect from the
commits just made).

## Step 6 — Report

Summarize: which gaps were fixed, the branch name (created vs reused), the version bump result,
and that nothing has been pushed — offer to push / open a PR if the user wants that next. Don't
push or open a PR unless asked.
