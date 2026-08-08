# Known gaps

Real gaps hit while building consumers of this package — not a general code-quality review.

## How to use this file

**Adding a gap:**
- Only log gaps actually hit while building/using a consumer of this package — not general
  code-quality nits, style preferences, or hypothetical edge cases. If you didn't hit it, it
  doesn't go here.
- One gap = one `###` entry under `## Open`, with a short, specific title naming the concrete
  behavior that's missing or wrong (not "improve error handling" — "no way to distinguish X from
  Y").
- Body should cover: what the current behavior is (with file/class/method names so it's
  actionable without re-deriving context), why it's a problem for a consumer, and a **Fix:**
  line/paragraph with a concrete proposed approach. End with **Status: to be fixed.**
- Append new entries to the end of `## Open` — don't reorder existing ones.

**Fixing a gap:**
- Move the entry from `## Open` to the end of `## Fixed`. Never delete a gap entry — this file
  doubles as a log of past rough edges and how they were resolved.
- Leave the original description and **Fix:** proposal untouched, even if the implementation
  ended up differing from what was proposed.
- Replace the status line with **Status: fixed.** followed by what was actually implemented
  (types/methods added, where) and a pointer to the test(s) that cover it. If the implementation
  diverged from the original **Fix:** proposal, say so and why.
- Don't add PR numbers, branch names, or dates — git history already has those. Keep this file
  focused on the technical "what" and "why", not the process around it.

## Open

_None currently._

## Fixed

### No way to distinguish a DB-unavailable error from other unexpected DB errors

`BaseRepository` (`EnsyNet.DataAccess.EntityFramework`) catches connection failures (connection
refused, timeout, DB server down, etc.) the same way it catches any other unclassified DB
exception — both fall through to the generic `catch (Exception e)` and get wrapped as
`UnexpectedDatabaseError`. Only `UniqueConstraintViolationError` (SQL 2601/2627),
`ForeignKeyConstraintViolationError` (SQL 547), and `OperationCanceledError` are split out;
everything else, including connectivity failures, collapses into one error type.

Consumers can't map "the database is unreachable" to a 503 without unwrapping
`UnexpectedDatabaseError`'s inner `Exception` themselves and inspecting it for
`SqlException`/`TimeoutException` connection-level codes — duplicating SQL-specific knowledge
that arguably belongs in this package, not in every consumer's service layer.

**Fix:** add a distinct error type (e.g. `DatabaseUnavailableError`) in
`EnsyNet.DataAccess.Abstractions/Errors`, and have `BaseRepository` catch connection-level
`SqlException`s (e.g. `Number` in the transient/connection-broken range, or a
`TimeoutException`/`SqlException` with no open connection) before the catch-all, returning that
new error type instead of `UnexpectedDatabaseError`.

**Status: fixed.** Added `DatabaseUnavailableError` (`EnsyNet.DataAccess.Abstractions/Errors`).
`BaseRepository` now catches it before the generic fallback whenever the underlying exception is
a `TimeoutException`, or a `SqlException` (unwrapped or nested in a `DbUpdateException`) with
`Number == -2` (client-side timeout) or `Class >= 20` (fatal server-side error — per SQL Server's
severity levels, always terminates the connection, e.g. connection refused, server down, or the
connection dropped mid-query). Covered by
`DatabaseUnavailableTests.DatabaseUnreachable_GetById_ReturnsDatabaseUnavailableError`.
