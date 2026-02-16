using Hex1b;
using Hex1b.Widgets;

namespace SquadTUI.Screens;

public static class NoSquadScreen
{
    public static Hex1bWidget Render(WidgetContext<VStackWidget> v, AppState state, Hex1bApp app)
    {
        return v.VStack(inner =>
        [
            inner.Text(""),
            inner.Text(""),
            inner.Text("\x1b[1m\x1b[36m  🎰 Welcome to SquadTUI\x1b[0m"),
            inner.Text(""),
            inner.Text("\x1b[93m  ⚠  No squad detected in this directory.\x1b[0m"),
            inner.Text(""),
            inner.Text("  \x1b[90mSquadTUI needs an\x1b[0m \x1b[1m.ai-team/\x1b[0m \x1b[90mdirectory to work with.\x1b[0m"),
            inner.Text("  \x1b[90mYou can create one using the squad CLI:\x1b[0m"),
            inner.Text(""),
            inner.Text("  \x1b[36m  npx github:bradygaster/squad\x1b[0m"),
            inner.Text(""),
            inner.Text("  \x1b[90mOr create the structure manually:\x1b[0m"),
            inner.Text(""),
            inner.Text("  \x1b[36m  mkdir .ai-team\x1b[0m"),
            inner.Text("  \x1b[36m  mkdir .ai-team/agents\x1b[0m"),
            inner.Text("  \x1b[36m  echo \"# Team\" > .ai-team/team.md\x1b[0m"),
            inner.Text(""),
            inner.Text("  \x1b[90mSee:\x1b[0m \x1b[4m\x1b[36mhttps://github.com/bradygaster/squad\x1b[0m"),
            inner.Text(""),
            inner.Text("\x1b[1m  [C] Create basic squad structure    [Q] Quit\x1b[0m"),
        ]).Fill();
    }
}
