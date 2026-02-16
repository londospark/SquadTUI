# Decision: Remove Self-Referential CI Tests

**By:** Solaire  
**Date:** 2026-02-18  
**Status:** Accepted

## What

Removed `DotnetBuild_Succeeds` and `DotnetTest_Passes` from `CIPipelineTests.cs`. Added a defensive `--filter` to `ci.yml` to exclude any future process-spawning tests.

## Why

Both tests spawned child `dotnet` processes inside the test runner:

- `DotnetBuild_Succeeds` ran `dotnet build --no-incremental` as a subprocess.
- `DotnetTest_Passes` ran `dotnet test --no-build` as a subprocess — which re-invoked the entire test suite, including itself, causing infinite recursion.

This broke CI on all 6 platform combinations. The test is logically circular: CI already runs `dotnet test`, so a test that asserts "dotnet test passes" adds zero signal — if the assertion could run, then by definition `dotnet test` is already passing.

## Changes

1. **`tests/SquadTUI.Tests/Integration/CIPipelineTests.cs`**: Deleted both `DotnetBuild_Succeeds` and `DotnetTest_Passes` methods. Replaced with a comment explaining why.
2. **`.github/workflows/ci.yml`**: Added `--filter "FullyQualifiedName!~DotnetBuild&FullyQualifiedName!~DotnetTest"` to the test step as a safety net against any future process-spawning tests.

## Consequences

- CI will no longer fail due to recursive test invocation.
- 8 CIPipelineTests remain, validating workflow structure (YAML validity, build/test steps exist, project/solution files exist).
- The `--filter` in ci.yml is defense-in-depth — even if someone re-adds process-spawning tests, CI won't recurse.

## Rule Going Forward

**No test may spawn `dotnet build`, `dotnet test`, or any other process that re-invokes the test runner.** Tests that validate CI infrastructure should inspect files and configuration, not execute builds.
