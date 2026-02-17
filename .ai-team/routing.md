# Work Routing

How to decide who handles what.

## Routing Table

| Work Type | Route To | Examples |
|-----------|----------|----------|
| Architecture, scope, priorities | Solaire | Project structure, feature scoping, trade-offs, tech decisions |
| Hex1b widgets, TUI components, layout, theming | Siegmeyer | Building screens, widget composition, focus management, styling |
| Data layer, file parsing, .ai-team/ state | Andre | Reading/writing team files, models, services, data access |
| Testing, quality, edge cases | Patches | Unit tests, integration tests, Hex1b automation testing |
| Screen flows, navigation, information architecture | Firekeeper | UX design, screen layouts, information density, user flows |
| Code review | Solaire | Review PRs, check quality, suggest improvements |
| Session logging | Scribe | Automatic — never needs routing |

## Rules

1. **Eager by default** — spawn all agents who could usefully start work, including anticipatory downstream work.
2. **Scribe always runs** after substantial work, always as `mode: "background"`. Never blocks.
3. **Quick facts → coordinator answers directly.** Don't spawn an agent for "what port does the server run on?"
4. **When two agents could handle it**, pick the one whose domain is the primary concern.
5. **"Team, ..." → fan-out.** Spawn all relevant agents in parallel as `mode: "background"`.
6. **Anticipate downstream work.** If a feature is being built, spawn the tester to write test cases from requirements simultaneously.
