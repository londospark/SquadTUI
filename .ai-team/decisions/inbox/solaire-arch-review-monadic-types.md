# Decision: Monadic Error Handling with Result<T> and Option<T>

**Date:** 2026-02-17
**By:** Solaire (Architecture Review Ceremony)
**Participants:** Andre, Firekeeper

## Context

LondoSpark wants no exceptions in the application flow — use LINQ and monadic types for validation and error handling. Current error handling in `Program.cs` uses `try/catch` around `Task.WhenAll()` with `state.ErrorMessage` as the only error channel. `DataBridge` methods are `async Task<T>` with no error wrapping. Services throw exceptions on parse failures.

## Decision

**Use `LanguageExt` NuGet package** for `Option<T>`, `Either<L, R>`, and related monadic types. Roll a thin `Result<T>` wrapper only if LanguageExt proves too heavy.

### Why LanguageExt over alternatives

| Option | Pros | Cons |
|--------|------|------|
| **LanguageExt** | Battle-tested, rich LINQ integration, `Option<T>`, `Either<L,R>`, `Try<T>`, `TryAsync<T>` | Large dependency, learning curve |
| **OneOf** | Lightweight discriminated unions | No LINQ integration, no `Option<T>` |
| **Roll our own** | Zero dependencies | Maintenance burden, incomplete |

LanguageExt wins because LondoSpark explicitly wants LINQ integration and monadic composition. The library is mature (10+ years, 3k+ stars) and aligns with Andre's and my functional preferences.

### Integration Plan

1. **DataBridge returns `Either<Error, T>` instead of raw `T`:**
   ```csharp
   public async Task<Either<AppError, IReadOnlyList<SquadMember>>> LoadRosterDataAsync()
   ```

2. **`AppError` sum type:**
   ```csharp
   public abstract record AppError(string Message);
   public record FileNotFound(string Path) : AppError($"File not found: {Path}");
   public record ParseError(string File, string Details) : AppError($"Parse error in {File}: {Details}");
   public record ServiceError(string Service, Exception Inner) : AppError($"{Service} failed: {Inner.Message}");
   ```

3. **AppState stores `Either` results:**
   ```csharp
   public Either<AppError, IReadOnlyList<SquadMember>> Members { get; set; } = Right(Array.Empty<SquadMember>());
   ```

4. **Screens use `.Match()` for rendering:**
   ```csharp
   state.Members.Match(
       Right: members => RenderMemberList(members),
       Left: error => RenderError(error.Message)
   );
   ```

5. **Program.cs startup removes try/catch:**
   ```csharp
   var membersResult = await bridge.LoadRosterDataAsync();
   state.Members = membersResult;
   // No exceptions, no catch blocks
   ```

6. **Charter loading uses `Option<string>`:**
   ```csharp
   public async Task<Option<string>> LoadCharterContentAsync(string memberName)
   ```

### Migration Strategy

**Phase 1 (Andre):** Add LanguageExt to csproj. Create `AppError` hierarchy. Update `DataBridge` return types.
**Phase 2 (Andre):** Update `AppState` to hold `Either<AppError, T>` for each data collection.
**Phase 3 (Siegmeyer):** Update screens to use `.Match()` pattern instead of `??` fallback.
**Phase 4 (Patches):** Update tests for new return types.

### Interaction with SampleData Isolation

This decision synergizes with SampleData isolation: the `?? SampleData.X` fallback pattern is replaced by `Either.Match(Right: render, Left: showEmpty)` — a strictly better pattern that makes error visibility explicit.

## Consequences

- All services become exception-free at the boundary
- Screens always know whether they have data or an error (not just empty vs. populated)
- Adds ~2MB to binary size (LanguageExt NuGet)
- Team needs to understand `Either`, `Option`, `Match`, `Bind`, `Map` — but that's the explicit goal

## Risks

- LanguageExt version churn (currently v4.x) — pin to a specific major version
- Hex1b widget building is imperative; monadic composition may feel awkward in render methods — use `.Match()` at the boundary, keep widget building imperative inside each arm
