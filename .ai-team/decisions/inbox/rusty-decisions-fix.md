# Fix Decisions Screen Parser

**Date:** 2026-02-17
**By:** Rusty
**Status:** Completed

## What I Did

Fixed the `DecisionService.ParseDecisionsMarkdown()` method to correctly parse the real `.ai-team/decisions.md` file format. The parser was expecting an old format but the actual file uses:

- `### {date}: {title}` headings (not `## ` headings)
- `**By:** {author}` fields (not `**Author:**`)
- `**What:** {content}` and `**Why:** {rationale}` fields
- `---` horizontal rules as separators

## Changes Made

**File:** `src/SquadTUI/Services/DecisionService.cs`

Rewrote the `ParseDecisionsMarkdown()` method to:
1. Parse `### date: title` format, extracting date and title from a single heading line
2. Recognize `**By:**`, `**What:**`, and `**Why:**` fields
3. Accumulate multi-line content for each section
4. Build formatted decision content from What/Why sections
5. Handle `---` separators gracefully

Added helper method `BuildDecisionContent()` to format the What/Why sections into readable content.

## Why This Matters

The Decisions screen was failing because the parser couldn't understand the real decision file format, so it returned empty or malformed data. Now the TUI correctly displays all decisions from `.ai-team/decisions.md` with proper date, title, author, and structured content.

## Verification

- `dotnet build src/SquadTUI` succeeds with no errors
- Parser correctly handles the 8 decisions currently in `.ai-team/decisions.md`
- Data flows through `DataBridge.LoadDecisionsDataAsync()` to `AppState.Decisions` to `DecisionsScreen`

## Follow-up

The TUI should now display real decisions when you navigate to the Decisions screen (press 3). The parser gracefully handles malformed entries and falls back to SampleData if the file doesn't exist.
