using Hex1b;
using Hex1b.Automation;
using Hex1b.Input;
using LanguageExt;
using static LanguageExt.Prelude;
using SquadTUI.Models;
using SquadTUI.Screens;
using SquadTUI.Tests.Fixtures;
using SquadTUI.Themes;

namespace SquadTUI.Tests.E2E;

/// <summary>
/// Builds a headless Hex1b terminal running the SquadTUI app for E2E testing.
/// </summary>
public static class TestAppBuilder
{
    public static Hex1bTerminal Build(int width = 120, int height = 30, bool squadDetected = true)
    {
        var state = new AppState();
        if (!squadDetected)
        {
            state.SquadDetected = false;
            state.CurrentScreen = Screen.NoSquad;
        }
        else
        {
            // Populate state with fixture data for E2E tests
            state.Members = Right<AppError, IReadOnlyList<SquadMember>>(SampleData.Members);
            state.Tasks = Right<AppError, IReadOnlyList<SquadTask>>(SampleData.Tasks);
            state.Decisions = Right<AppError, IReadOnlyList<DecisionEntry>>(SampleData.Decisions);
            state.Skills = Right<AppError, IReadOnlyList<Skill>>(SampleData.Skills);
            state.LogEntries = Right<AppError, IReadOnlyList<OrchestrationLogEntry>>(SampleData.LogEntries);
            state.SprintHistory = Right<AppError, IReadOnlyList<SprintMetrics>>(SampleData.SprintHistory);
            state.CharterContent = Some(SampleData.GetCharterFor("Sonic"));
        }

        var terminal = Hex1bTerminal.CreateBuilder()
            .WithHex1bApp((app, options) =>
            {
                options.Theme = ThemeManager.GetTheme(state.SelectedThemeIndex);
                options.EnableMouse = true;

                return ctx => AppLayout.Build(ctx, state, app, options);
            })
            .WithHeadless()
            .WithDimensions(width, height)
            .Build();

        return terminal;
    }
}
