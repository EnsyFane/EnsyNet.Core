# How to release a new version of EnsyNet.Core packages

1. Get all of the required changes merged on the main branch
2. In `Directory.Packages.props` bump `Version` and `AssemblyVersion` to the new package version you want to publish
    - This can be done either in the same PR as the changes you want to release or in a separate PR. If done in a separate PR that PR needs to get merged before proceeding to next step
3. Create a new tag and release in GitHub.
    - The new tag and release have to have the same name as `Version` and `AssemblyVersion` (example: if `Version` is `1.0.1` the the release should be named `v1.0.1`)
    - The description of the release should contain the autogenerated GitHub description and any other notes you deem necessary
4. After creating the new tag and release a new action should start running. When it completes successfully a new NuGet version should be published
