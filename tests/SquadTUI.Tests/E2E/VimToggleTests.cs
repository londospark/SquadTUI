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

[Collection("E2E")]
public class VimToggleTests
{
    [Fact]
    public async Task VimDisabled_JKey_DoesNotNavigateInRoster()
    {
        var state = new AppState();
        state.Settings.VimBindings = false;
        state.Members = Right<AppError, IReadOnlyList<SquadMember>>(SampleData.Members);
        state.Tasks = Right<AppError, IReadOnlyList<SquadTask>>(SampleData.Tasks);
        state.Decisions = Right<AppError, IReadOnlyList<DecisionEntry>>(SampleData.Decisions);
        state.Skills = Right<AppError, IReadOnlyList<Skill>>(SampleData.Skills);
        state.LogEntries = Right<AppError, IReadOnlyList<OrchestrationLogEntry>>(SampleData.LogEntries);
        state.SprintHistory = Right<AppError, IReadOnlyList<SprintMetrics>>(SampleData.SprintHistory);
        state.CharterContent = Some(SampleData.GetCharterFor("Sonic"));

        await using var terminal = Hex1bTerminal.CreateBuilder()
            .WithHex1bApp((app, options) =>
            {
                options.Theme = ThemeManager.GetTheme(state.SelectedThemeIndex);
                options.EnableMouse = true;
                return ctx => AppLayout.Build(ctx, state, app, options);
            })
            .WithHeadless()
            .WithDimensions(120, 30)
            .Build();

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        // Navigate to Roster first
        var enterSeq = new Hex1bTerminalInputSequenceBuilder()
            .Enter()
            .Build();
        await enterSeq.ApplyAsync(terminal);
        await Task.Delay(200);

        // Record initial index
        var initialIndex = state.RosterSelectedIndex;

        // Press J — should NOT change selection because VimBindings is false
        var jSeq = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.J)
            .Build();
        await jSeq.ApplyAsync(terminal);
        await Task.Delay(200);

        Assert.Equal(initialIndex, state.RosterSelectedIndex);

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task VimEnabled_JKey_NavigatesInRoster()
    {
        var state = new AppState();
        state.Settings.VimBindings = true;
        state.Members = Right<AppError, IReadOnlyList<SquadMember>>(SampleData.Members);
        state.Tasks = Right<AppError, IReadOnlyList<SquadTask>>(SampleData.Tasks);
        state.Decisions = Right<AppError, IReadOnlyList<DecisionEntry>>(SampleData.Decisions);
        state.Skills = Right<AppError, IReadOnlyList<Skill>>(SampleData.Skills);
        state.LogEntries = Right<AppError, IReadOnlyList<OrchestrationLogEntry>>(SampleData.LogEntries);
        state.SprintHistory = Right<AppError, IReadOnlyList<SprintMetrics>>(SampleData.SprintHistory);
        state.CharterContent = Some(SampleData.GetCharterFor("Sonic"));

        await using var terminal = Hex1bTerminal.CreateBuilder()
            .WithHex1bApp((app, options) =>
            {
                options.Theme = ThemeManager.GetTheme(state.SelectedThemeIndex);
                options.EnableMouse = true;
                return ctx => AppLayout.Build(ctx, state, app, options);
            })
            .WithHeadless()
            .WithDimensions(120, 30)
            .Build();

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        // Navigate to Roster first
        var enterSeq = new Hex1bTerminalInputSequenceBuilder()
            .Enter()
            .Build();
        await enterSeq.ApplyAsync(terminal);
        await Task.Delay(200);

        // Press J — should change selection because VimBindings is true
        var jSeq = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.J)
            .Build();
        await jSeq.ApplyAsync(terminal);
        await Task.Delay(200);

        Assert.True(state.RosterSelectedIndex > 0);

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }
}
