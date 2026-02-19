# 🔥 SquadTUI

> *Praise the Sun!* — A Dark Souls–themed terminal UI for managing AI squad teams, forged in .NET.

**SquadTUI** is a powerful, elegant terminal user interface built with [Hex1b](https://hex1b.dev/) for managing AI development teams. Inspired by the cooperative spirits of Dark Souls, each team member is a seasoned warrior with specialized skills and roles. Track your squad's activity, monitor progress, review decisions, and manage tasks — all from your terminal.

[![Project Board](https://img.shields.io/badge/Project%20Board-GitHub-blue)](https://github.com/users/londospark/projects/5)
[![GitHub](https://img.shields.io/badge/Repo-GitHub-black)](https://github.com/londospark/SquadTUI)
[![.NET 10](https://img.shields.io/badge/.NET-10-purple)](https://dotnet.microsoft.com/)

## 🎬 Demo

[![asciicast](https://asciinema.org/a/lVT6isM9iBu0634a.svg)](https://asciinema.org/a/lVT6isM9iBu0634a)

## ✨ Features

- **📊 Dashboard** — Real-time overview of squad activity, active tasks, and team metrics
- **👥 Roster** — Browse team members with their roles, skills, and current status (Active, Idle, Working, Offline)
- **📋 Decisions** — Review and track architectural decisions, context, and rationale
- **📜 Charter Viewer** — Read detailed member charters including identity, expertise, and guidelines
- **🔧 Skills** — Catalog of team capabilities and specializations
- **📈 Activity Log** — Chronological orchestration log with task assignments and completions
- **📉 Metrics** — Sprint velocity, task completion rates, and team performance insights
- **⚙️ Settings** — Configure theme, vim bindings, mouse, emoji display, and markdown rendering (`S` key)
- **❓ Help Screen** — In-app keybinding reference accessible with `F1`
- **🎨 Theming** — Four beautiful themes (Ocean, Heist, Sunset, HighContrast) — cycle with `T` key
- **📱 Responsive Layout** — Adapts to wide (≥120 cols), medium (80-119), and narrow (<80) terminals
- **⌨️ Keyboard Navigation** — Vim-friendly shortcuts (`j`/`k`/`h`/`l`) with arrow key support
- **🖱️ Mouse Support** — Clickable nav tabs, scroll, and select with your mouse

## 📸 Screenshots

### Dashboard
![Dashboard](docs/images/screenshot-dashboard.svg)

### Roster
![Roster](docs/images/screenshot-roster.svg)

### Decisions
![Decisions](docs/images/screenshot-decisions.svg)

### Help
![Help](docs/images/screenshot-help.svg)

*Screenshots captured on a 120×30 terminal with the Ocean theme using [hex1b CLI](https://hex1b.dev/). Additional screenshots available in `docs/images/` — see [docs/SCREENSHOTS.md](docs/SCREENSHOTS.md) for how to capture fresh screenshots.*

## 🚀 Installation

### Build from Source

```bash
git clone https://github.com/londospark/SquadTUI.git
cd SquadTUI
dotnet build -c Release
dotnet run --project src/SquadTUI
```

### Download Pre-built Binaries

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
# From the repo root
dotnet run --project src/SquadTUI
```

If no `.ai-team/` directory is detected, SquadTUI will offer to create one for you, or you can set one up with [Squad](https://github.com/bradygaster/squad):

```bash
npx github:bradygaster/squad
```

### Keyboard Shortcuts

| Key | Action |
|-----|--------|
| `1`–`6` | Jump to screen (Dashboard, Roster, Decisions, Skills, Log, Metrics) |
| `j` / `k` | Move selection down/up in lists |
| `h` / `l` | Switch to previous/next screen |
| `↑` / `↓` | Arrow keys for list navigation |
| `←` / `→` | Arrow keys for screen switching |
| `Enter` | Select / activate current item |
| `T` | Cycle theme (Ocean → Heist → Sunset → HighContrast) |
| `S` | Open Settings screen |
| `F1` | Toggle Help screen |
| `E` | Edit charter (from Member Detail) |
| `C` | Create squad scaffold (from NoSquad screen) |
| `Escape` | Go back (Detail → Roster → Dashboard) |
| `Q` | Quit |
| 🖱️ Click | Click nav bar tabs to switch screens |

### Navigation Flow

1. **Dashboard** — Start here for an overview
2. **Roster** — Select a member → **Member Detail** → press `E` for **Charter**
3. **Decisions** — Browse architectural decisions by date
4. **Skills** — View team capabilities and specializations
5. **Activity Log** — Track recent orchestration events
6. **Metrics** — Analyze sprint velocity and completion rates
7. **Settings** (`S`) — Configure app preferences
8. **Help** (`F1`) — Quick keybinding reference

## 📈 Metrics & Analytics

SquadTUI includes a dedicated **Metrics** screen (press `6`) and a **Dashboard** summary for tracking squad performance at a glance.

**What the Metrics Screen Shows:**
- **Sprint Progress** — Visual progress bar showing completion percentage with task counts
- **Task Breakdown** — Done, in-progress, pending, and blocked counts with status icons
- **Velocity** — Completed tasks per sprint cycle with trend direction
- **Per-Member Contributions** — Bar chart of completed + active tasks per team member
- **Member Status** — Individual breakdown of each member's task distribution

**Responsive Layouts:**
- **Wide (≥120 cols)** — 3-column: progress + chart + member detail
- **Medium (80–119)** — 2-column: progress + chart side-by-side
- **Narrow (<80)** — Single column: stacked summary

## 🛠️ Tech Stack

- **Framework:** [.NET 10](https://dotnet.microsoft.com/)
- **TUI Library:** [Hex1b 0.87.0](https://hex1b.dev/) — Modern, declarative terminal UI framework
- **Language:** C# 14
- **Testing:** xUnit + Hex1b headless testing (`TestAppBuilder`)
- **Platform Support:** Windows, macOS, Linux (x64 and ARM64)
- **Architecture:** Clean separation — Models, Services, Screens, Rendering, Themes

## 🤝 Contributing

We welcome contributions! Please see [CONTRIBUTING.md](CONTRIBUTING.md) for our Git Flow workflow, CI pipeline details, and code style guidelines. See also [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md) for a technical overview.

### Quick Start for Contributors

```bash
git checkout develop
git pull origin develop
git checkout -b feature/my-feature
# Make changes
dotnet test --filter "FullyQualifiedName!~DotnetBuild&FullyQualifiedName!~DotnetTest"
git add .
git commit -m "feat: add my feature"
git push origin feature/my-feature
# Create PR to develop
```

## 🔗 Links

- **[Project Board](https://github.com/users/londospark/projects/5)** — Track progress and upcoming work
- **[GitHub Repository](https://github.com/londospark/SquadTUI)** — Source code and issues
- **[Hex1b](https://hex1b.dev/)** — The terminal UI framework powering SquadTUI
- **[SquadUI](https://github.com/csharpfritz/SquadUI)** — Inspiration for squad-based AI workflows

## 🎬 Credits

**SquadTUI** is forged in the fires of Lordran — where every team member, like an undead warrior, plays a critical role in linking the flame. Our AI squad follows the same cooperative philosophy found in Dark Souls.

### ☀️ The Dark Souls Squad

| Role | Name | Specialty |
|------|------|-----------|
| 🎯 Lead | **Solaire** | Architecture, code review, big picture |
| 💻 Backend Dev | **Andre** | Services, data layer, integrations |
| 🎨 Frontend Dev | **Siegmeyer** | UI/UX, screens, theming |
| 🧪 QA Engineer | **Patches** | Testing, quality assurance |
| 📣 UX Designer | **Firekeeper** | User experience, design systems |
| 📝 Docs | **Scribe** | Documentation, knowledge management |
| 🔧 DevOps | **Ralph** | CI/CD, tooling, automation |

*Special thanks to [csharpfritz/SquadUI](https://github.com/csharpfritz/SquadUI) for inspiration.*

## 📄 License

MIT License — see [LICENSE](LICENSE) for details.

Copyright © 2026 LondoSpark

---

**Built with ☀️ by the SquadTUI team**

*"Praise the Sun!"* \\[T]/
