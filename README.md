# Cybersecurity Awareness Bot (Part 1)

This repository contains Part 1 of the Cybersecurity Awareness Bot project.

Requirements
- .NET 9 SDK
- C# 13

How to run locally
1. Ensure `Assets/greetings.wav` exists in the repository at `Assets/greetings.wav`.
2. Ensure the WAV file is set to copy to output (see project file snippet below).
3. From repository root:
   - dotnet restore
   - dotnet build
   - dotnet run --project "<your-project-csproj>"

Project files updated for Part 1
- `VoiceGreeting.cs` – robust playback and error handling.
- `ConsoleUI.cs` – improved prompt and typing effect.
- `ResponseManager.cs` – keyword matching and random responses.
- `ChatBot.cs`, `Program.cs` – improved flow and UX.

CI
- GitHub Actions workflow located at `.github/workflows/ci.yml`. Make sure the workflow is green and include a screenshot of a successful run here before final submission.

Submission checklist (Part 1)
- [ ] Minimum 6 meaningful commits
- [ ] GitHub Actions CI configured and passing
- [ ] `Assets/greetings.wav` included and copied to output
- [ ] README contains run instructions and CI screenshot
- [ ] Unlisted YouTube video walkthrough (include link in README)