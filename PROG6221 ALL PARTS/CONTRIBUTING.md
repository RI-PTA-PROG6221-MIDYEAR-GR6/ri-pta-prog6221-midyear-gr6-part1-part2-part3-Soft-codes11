# Contributing

This project follows the assessment requirements for PROG6221 Part 1.

Guidelines:
- Minimum of six meaningful commits for each Part. Use clear messages (e.g., "Add voice greeting", "Improve ResponseManager keywords", "Add CI workflow").
- Add a GitHub Actions workflow under `.github/workflows/` that builds the solution.
- Add tags/releases as required by the assessment.
- Keep code formatting consistent; `.editorconfig` is provided.
- Include required assets (Assets/greetings.wav) and set them to copy to output in your project file.

Security and Sensitive Data (Important for public repositories):

- Never commit secrets, credentials, private keys, API keys, or passwords to the repository. This repository is public and any committed secrets become exposed.

- Use a `.gitignore` to exclude local user files and secrets (examples below). Do not rely on `.gitignore` to clean history — it only prevents future commits of ignored files.

- Store secrets in environment variables locally and in CI/CD (use GitHub Secrets in Actions). Avoid embedding credentials in code or configuration files checked into the repo.

- If a secret was accidentally committed:
  - Remove the file from the repository and stop using the leaked secret immediately (rotate or revoke it).
  - Remove the file from git history using a history-rewriting tool such as `git filter-repo` or the BFG Repo-Cleaner. See remediation steps below.

- Scan the repo before pushing using tools such as `git-secrets`, `truffleHog`, or GitHub's secret scanning. Run these locally or as a pre-push hook.

Remediation steps (high level):
- Stop and rotate the leaked credential immediately (change the password, revoke the token).
- Remove the file from the repo and commit the removal:

  ```bash
  git rm --cached path/to/secret.file
  git commit -m "Remove secret file from repository"
  ```

- Purge the secret from repository history (recommended: `git filter-repo`). Example using BFG (simplified):

  1. Install BFG: https://rtyley.github.io/bfg-repo-cleaner/
  2. Mirror clone your repo: `git clone --mirror https://github.com/owner/repo.git`
  3. Run BFG to remove `credentials.json`: `bfg --delete-files credentials.json repo.git`
  4. Push the cleaned repo: `cd repo.git && git push --force`

- Alternatively use `git filter-repo` (preferred modern tool). Follow its documentation carefully and backup your repository before rewriting history.

CI and GitHub Actions:
- Add sensitive values to GitHub repository secrets (Settings -> Secrets -> Actions) and access them in workflows via `${{ secrets.MY_SECRET }}`.
- Do not hardcode secrets in workflow files or source files.

.gitignore suggestions (example):

```text .gitignore
# Visual Studio / Rider / VSCode
.vs/
.vscode/
*.user
*.suo

# Build outputs
bin/
obj/

# OS files
Thumbs.db
.DS_Store

# Local secrets / config
local.settings.json
secrets.json
credentials.json
*.pem
*.key

# Avoid committing audio or large generated assets if they are not required
# (If required for assessment, keep as project Content with CopyToOutputDirectory)
```

CI Requirements:
- Ensure CI passes (green) before final submission.
- Include a screenshot of a successful run in `README.md`.

CI and publishing:
- Ensure CI passes (green) before final submission.
- Include a screenshot of a successful run in `README.md`.