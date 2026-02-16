using Hex1b;
using Hex1b.Input;
using Hex1b.Widgets;
using SquadTUI.Rendering;
using SquadTUI.Themes;

namespace SquadTUI.Screens;

public static class HelpScreen
{
    public static Hex1bWidget Render(WidgetContext<VStackWidget> v, AppState state, Hex1bApp app)
    {
        var colors = ThemeManager.GetPanelColors(state.SelectedThemeIndex);
        var accent = colors.Accent;
        var dim = "\x1b[90m";
        var reset = PanelRenderer.Reset;
        var bold = PanelRenderer.Bold;

        return v.VStack(stack =>
        {
            var widgets = new List<Hex1bWidget>();

            // Title
            widgets.Add(stack.Text($"{bold}{accent}❓ Help & Keybindings{reset}\n"));

            // Navigation section
            widgets.Add(stack.Text($"{bold}{accent}NAVIGATION{reset}"));
            widgets.Add(stack.Text($"{dim}[1-6]{reset}        Jump to screen (Dashboard, Roster, Decisions, Skills, Log, Metrics)"));
            widgets.Add(stack.Text($"{dim}[h/l]{reset}        Previous/next screen"));
            widgets.Add(stack.Text($"{dim}[Escape]{reset}     Go back to previous screen\n"));

            // List navigation section
            widgets.Add(stack.Text($"{bold}{accent}LIST NAVIGATION{reset}"));
            widgets.Add(stack.Text($"{dim}[j/k]{reset}        Move selection down/up in lists"));
            widgets.Add(stack.Text($"{dim}[↑/↓]{reset}        Arrow keys also work for up/down\n"));

            // Actions section
            widgets.Add(stack.Text($"{bold}{accent}ACTIONS{reset}"));
            widgets.Add(stack.Text($"{dim}[Enter]{reset}       Activate/select current item"));
            widgets.Add(stack.Text($"{dim}[T]{reset}           Toggle theme"));
            widgets.Add(stack.Text($"{dim}[S]{reset}           Settings"));
            widgets.Add(stack.Text($"{dim}[E]{reset}           Edit charter (on member detail)"));
            widgets.Add(stack.Text($"{dim}[C]{reset}           Create squad (on NoSquad screen)"));
            widgets.Add(stack.Text($"{dim}[Q]{reset}           Quit SquadTUI\n"));

            // Help section
            widgets.Add(stack.Text($"{bold}{accent}HELP{reset}"));
            widgets.Add(stack.Text($"{dim}[?]{reset}           Toggle this help screen\n"));

            // Footer
            widgets.Add(stack.Text($"{dim}Press ? or Escape to dismiss{reset}"));

            return widgets.ToArray();
        }).WithInputBindings(keys =>
        {
            keys.Key(Hex1bKey.F1).Action(() =>
            {
                if (state.PreviousScreen.HasValue)
                {
                    state.CurrentScreen = state.PreviousScreen.Value;
                    state.PreviousScreen = null;
                }
                else
                {
                    state.CurrentScreen = Screen.Dashboard;
                }
            }, "Close Help");
            keys.Key(Hex1bKey.Escape).Action(() =>
            {
                if (state.PreviousScreen.HasValue)
                {
                    state.CurrentScreen = state.PreviousScreen.Value;
                    state.PreviousScreen = null;
                }
                else
                {
                    state.CurrentScreen = Screen.Dashboard;
                }
            }, "Close Help");
        });
    }
}
