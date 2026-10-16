# Cybersecurity Awareness Bot (Part 1)

This repository contains Part 1 of the Cybersecurity Awareness Bot project.

Requirements
- .NET 9 SDK
- C# 13

How to run locally
1. Ensure `Assets/greetings.wav` exists in the repository at `Assets/greetings.wav`.
2. Ensure the WAV file is set to copy to output (project is configured to copy Assets by wildcard).
3. From repository root:
   - dotnet restore
   - dotnet build
   - dotnet run --project "PROG6221 ALL PARTS/PROG6221 ALL PARTS/CybersecurityAwarenessBot.csproj"

Project files updated for Part 1
- `VoiceGreeting.cs` – robust playback, runtime WAV generation fallback, and TTS fallback on Windows.
- `ConsoleUI.cs` – improved prompt, colored output, and typing effect.
- `ResponseManager.cs` – keyword matching, memory-aware responses, and non-repeating tips.
- `ChatBot.cs`, `Program.cs` – improved flow and UX; ChatMemory persistence.

CI
- GitHub Actions workflow located at `.github/workflows/ci.yml` (Windows/.NET). Ensure the workflow is passing on GitHub; a local build has succeeded.

Commit summary
- Total commits in this repository: 35
- Example meaningful commits (at least six):
  - b1cc18d 2026-09-08 Add .gitignore file for Visual Studio project
  - df2accb 2026-09-08 Add voice greeting and multimedia assets
  - 805433f 2026-09-09 Added ggitignore and pre-push checklist; verify voice greeting and UI typing fix
  - f713fab 2026-10-14 Add GitHub Actions workflow for .NET build
  - 0efe142 2026-10-15 Add voice support: generate/copy greetings.wav and TTS fallback
  - 31c8311 2026-10-16 Add conversation memory persistence and avoid repeating tips; add CI workflow

Submission checklist (Part 1)
- [x] Minimum 6 meaningful commits (repository contains more than six commits; key commits listed above)
- [x] GitHub Actions CI configured (workflow file present at `.github/workflows/ci.yml`)
- [x] `Assets/greetings.wav` included or generated at runtime and copied to output by the project
- [ ] README contains run instructions and CI screenshot (add CI screenshot after a successful GitHub run)
- [ ] Unlisted YouTube video walkthrough (include link in README)
