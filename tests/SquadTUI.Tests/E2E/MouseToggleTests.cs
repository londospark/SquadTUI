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
public class MouseToggleTests
{
    [Fact]
    public async Task MouseToggle_UpdatesOptionsEnableMouse()
    {
        var state = new AppState();
        state.Settings.MouseEnabled = true;
        state.Members = Right<AppError, IReadOnlyList<SquadMember>>(SampleData.Members);
        state.Tasks = Right<AppError, IReadOnlyList<SquadTask>>(SampleData.Tasks);
        state.Decisions = Right<AppError, IReadOnlyList<DecisionEntry>>(SampleData.Decisions);
        state.Skills = Right<AppError, IReadOnlyList<Skill>>(SampleData.Skills);
        state.LogEntries = Right<AppError, IReadOnlyList<OrchestrationLogEntry>>(SampleData.LogEntries);
        state.SprintHistory = Right<AppError, IReadOnlyList<SprintMetrics>>(SampleData.SprintHistory);
        state.CharterContent = Some(SampleData.GetCharterFor("Sonic"));

        Hex1bAppOptions? capturedOptions = null;

        await using var terminal = Hex1bTerminal.CreateBuilder()
            .WithHex1bApp((app, options) =>
            {
                options.Theme = ThemeManager.GetTheme(state.SelectedThemeIndex);
                options.EnableMouse = true;
                capturedOptions = options;
                return ctx => AppLayout.Build(ctx, state, app, options);
            })
            .WithHeadless()
            .WithDimensions(120, 30)
            .Build();

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var runTask = terminal.RunAsync(cts.Token);
        await Task.Delay(200);

        Assert.NotNull(capturedOptions);
        Assert.True(capturedOptions!.EnableMouse);

        // Open settings modal
        var openSettings = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.S)
            .Build();
        await openSettings.ApplyAsync(terminal);
        await Task.Delay(300);

        Assert.True(state.ShowSettingsModal, "Settings modal should be open");
        Assert.Equal(0, state.SettingsModalSelectedIndex);

        // Navigate to Mouse Support (index 2) using DownArrow (List widget navigation) and toggle it
        var navDown1 = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.DownArrow)
            .Build();
        await navDown1.ApplyAsync(terminal);
        await Task.Delay(200);

        var navDown2 = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.DownArrow)
            .Build();
        await navDown2.ApplyAsync(terminal);
        await Task.Delay(200);

        var toggle = new Hex1bTerminalInputSequenceBuilder()
            .Key(Hex1bKey.Enter)
            .Build();
        await toggle.ApplyAsync(terminal);
        await Task.Delay(200);

        // Mouse should now be disabled
        Assert.False(state.Settings.MouseEnabled);
        Assert.False(capturedOptions.EnableMouse);

        cts.Cancel();
        try { await runTask; } catch (OperationCanceledException) { }
    }
}
