# 🎰 SquadTUI

> Ocean's Eleven meets .NET — A terminal UI for managing AI squad teams with style.

**SquadTUI** is a powerful, elegant terminal user interface built with [Hex1b](https://hex1b.dev/) for managing AI development teams. Inspired by Ocean's Eleven, each team member has specialized skills and roles. Track your squad's activity, monitor progress, review decisions, and manage tasks — all from your terminal.

## ✨ Features

- **📊 Dashboard** — Real-time overview of squad activity, active tasks, and team metrics
- **👥 Roster** — Browse team members with their roles, skills, and current status (Active, Idle, Working, Offline)
- **📋 Decisions** — Review and track architectural decisions, context, and rationale
- **📜 Charter Viewer** — Read detailed member charters including identity, expertise, and guidelines
- **🔧 Skills** — Catalog of team capabilities and specializations
- **📈 Activity Log** — Chronological orchestration log with task assignments and completions
- **📉 Metrics** — Sprint velocity, task completion rates, and team performance insights
- **🎨 Theming** — Three beautiful themes (Ocean, Heist, Daylight) — switch with `T` key
- **📱 Responsive Layout** — Adapts to wide (≥120 cols), medium (80-119), and narrow (<80) terminals
- **⌨️ Keyboard Navigation** — Vim-friendly shortcuts with arrow key support
- **🖱️ Mouse Support** — Click, scroll, and select with your mouse (where terminal supports it)

## 📸 Screenshots

### Dashboard
```
┌─────────────────────────────────────────────────────┐
│ 🏠 DASHBOARD — Squad Overview                       │
│                                                     │
│ 👥 Active Members: 11/11                           │
│ 🔄 Tasks in Progress: 8                            │
│ ✅ Completed Today: 12                             │
└─────────────────────────────────────────────────────┘
```

### Roster
```
┌─────────────────────────────────────────────────────┐
│ 👥 ROSTER — Team Members                            │
│                                                     │
│ 🎯 Danny — Lead (✅ Active)                         │
│ 💻 Rusty — Backend Engineer (🔵 Working)           │
│ 🎨 Linus — Frontend Dev (✅ Active)                 │
└─────────────────────────────────────────────────────┘
```

*More screenshots coming soon — contributions welcome!*

## 🚀 Installation

### Option 1: Install as .NET Tool (Recommended)

```bash
dotnet tool install --global SquadTUI
squadtui
```

### Option 2: Build from Source

```bash
git clone https://github.com/londospark/SquadTUI.git
cd SquadTUI
dotnet build -c Release
dotnet run --project src/SquadTUI
```

### Option 3: Download Pre-built Binaries

Download the latest release for your platform from the [Releases](https://github.com/londospark/SquadTUI/releases) page:

- **Windows:** `squadtui-X.Y.Z-win-x64.zip` or `squadtui-X.Y.Z-win-arm64.zip`
- **macOS:** `squadtui-X.Y.Z-osx-x64.tar.gz` or `squadtui-X.Y.Z-osx-arm64.tar.gz`
- **Linux:** `squadtui-X.Y.Z-linux-x64.tar.gz` or `squadtui-X.Y.Z-linux-arm64.tar.gz`

Extract and run:

```bash
# Windows
unzip squadtui-X.Y.Z-win-x64.zip
.\SquadTUI.exe

# macOS/Linux
tar xzf squadtui-X.Y.Z-osx-x64.tar.gz
./SquadTUI
```

## 📖 Usage

### Starting SquadTUI

```bash
# If installed as a tool
squadtui

# If running from source
dotnet run --project src/SquadTUI
```

### Keyboard Shortcuts

| Key | Action |
|-----|--------|
| `1` | Dashboard |
| `2` | Roster |
| `3` | Decisions |
| `4` | Charter Viewer |
| `5` | Skills |
| `6` | Activity Log |
| `7` | Metrics |
| `T` | Cycle themes (Ocean → Heist → Daylight) |
| `Q` | Quit |
| `↑` `↓` | Navigate lists |
| `←` `→` | Switch tabs (where applicable) |
| `Enter` | Select item |
| `Esc` | Back/Cancel |

### Navigation Flow

1. **Dashboard** — Start here for an overview
2. **Roster** — Select a member to view their charter
3. **Decisions** — Browse architectural decisions by date
4. **Activity Log** — Track recent orchestration events
5. **Metrics** — Analyze sprint velocity and completion rates

## 🛠️ Tech Stack

- **Framework:** [.NET 10](https://dotnet.microsoft.com/)
- **TUI Library:** [Hex1b](https://hex1b.dev/) — Modern, declarative terminal UI framework
- **Language:** C# 13
- **Platform Support:** Windows, macOS, Linux (x64 and ARM64)
- **Architecture:** Clean separation: Models, Services, Screens, Rendering

## 🤝 Contributing

We welcome contributions! Please see [CONTRIBUTING.md](CONTRIBUTING.md) for:

- Git Flow branching strategy
- PR guidelines and commit conventions
- CI/CD pipeline details
- Code style guidelines

### Quick Start for Contributors

```bash
git checkout develop
git pull origin develop
git checkout -b feature/my-feature
# Make changes
git add .
git commit -m "feat: add my feature"
git push origin feature/my-feature
# Create PR to develop
```

## 📄 License

MIT License — see [LICENSE](LICENSE) for details.

Copyright © 2026 LondoSpark

## 🎬 Credits

**SquadTUI** is built with inspiration from Ocean's Eleven — where every team member plays a critical role in pulling off the perfect heist. Our AI squad follows the same philosophy.

### The Squad

- **🎯 Danny** — Lead (Architecture, code review, big picture)
- **💻 Rusty** — Backend Engineer (Services, data layer, integrations)
- **🎨 Linus** — Frontend Dev (UI/UX, screens, theming)
- **🧪 Basher** — QA Engineer (Testing, quality assurance)
- **📣 Saul** — UX Designer (User experience, design systems)
- **📝 Scribe** — Documentation (Docs, knowledge management)
- **🔮 Oracle** — Research (Tech exploration, prototyping)
- **🛠️ Toolsmith** — DevOps (CI/CD, tooling, automation)
- **🔍 Detective** — Debugger (Issue investigation, root cause analysis)
- **🚀 Turk** — Performance (Optimization, profiling)
- **🎭 Livingston** — Integration (Cross-system communication)

*Special thanks to [csharpfritz/SquadUI](https://github.com/csharpfritz/SquadUI) for inspiration.*

## 🔗 Related Projects

- [Hex1b](https://hex1b.dev/) — The terminal UI framework powering SquadTUI
- [SquadUI](https://github.com/csharpfritz/SquadUI) — Inspiration for squad-based AI workflows

---

**Built with ❤️ by the SquadTUI team**

*"You don't plan a heist without the right crew."* 🎰
