# StudySpeech

StudySpeech is my project for the .NET web development course. It's a study notes app where you can write notes, sort them into folders and tags, and have them read out loud with AI text-to-speech (Azure Cognitive Services Speech), if you'd rather listen to your notes than reread them. You can also download the generated speech as an mp3 file. the home page shows three sample notes so people who aren't logged in can still try out the text-to-speech feature before making an account.

## Features

- Register/login (ASP.NET Core Identity, passwords are hashed)
- Full CRUD on notes, folders and tags — everyone only sees their own stuff
- Sort notes into folders and tag them
- Listen to any note with AI-generated speech
- Download a note's speech as an mp3

## Tech stack

- ASP.NET Core MVC (.NET 10)
- Entity Framework Core with SQLite
- ASP.NET Core Identity
- Microsoft Cognitive Services Speech SDK for the text-to-speech part
- Tailwind CSS 4 (built with `@tailwindcss/cli`)

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (18+) and npm, to build the Tailwind CSS
- An [Azure Speech resource](https://portal.azure.com/) (key + region) if you want the text-to-speech to actually work. The app still builds and runs without one, the "Listen with AI" button will just throw an error until a key is set.

## Setup

1. **Clone it and install dependencies**

   ```bash
   git clone https://github.com/Rosalij/StudySpeechProj.git
   cd StudySpeechProj
   dotnet restore
   npm install
   ```

2. **Set your Azure Speech key/region**

   Using `dotnet user-secrets` for this (already set up via `UserSecretsId` in `StudySpeech.csproj`), so you never have to put real keys in `appsettings.json`:

   ```bash
   dotnet user-secrets set "AzureSpeech:Key" "<your-azure-speech-key>"
   dotnet user-secrets set "AzureSpeech:Region" "<your-azure-region>"
   ```

3. **Database**

   The app applies migrations automatically on startup (`db.Database.Migrate()` in `Program.cs`), so `app.db` gets created and updated the first time you run it.

   It also seeds a few sample notes/tags automatically if none exist yet (`SeedData.EnsureSampleNotes`), those are the sample notes that show up on the home page for logged-out visitors.


4. **Run it**

   ```bash
   npm run dev
   ```

   That runs the Tailwind watcher and `dotnet watch run` at the same time. You can also just do a normal build/run, the Tailwind build is wired up as an MSBuild step that runs automatically before every build:

   ```bash
   dotnet run
   ```

## Configuration

| Setting | Where | What it's for |
|---|---|---|
| `ConnectionStrings:DefaultConnection` | `appsettings.json` | SQLite connection string, defaults to `DataSource=app.db;Cache=Shared` |
| `AzureSpeech:Key` / `AzureSpeech:Region` | user secrets locally, environment variables in production | credentials for `AzureSpeechService` |

`appsettings.json` has empty `AzureSpeech` values on purpose, keeping the real keys out of git and setting them via user secrets locally / environment variables on the host.

## Deployment

The app is deployed to [Render](https://render.com) as a Docker web service, built straight from the `Dockerfile` in the repo root. It's a two-stage build: the first stage has the .NET SDK and Node, compiles the app and the Tailwind CSS, and the second stage is just the lean ASP.NET runtime image with the published output, listening on whatever `PORT` Render gives it.

The Azure Speech key/region are set as environment variables in Render's dashboard, not committed anywhere.

## Database & ER diagram

See [`docs/er-diagram.md`](docs/er-diagram.md) for the ER diagram and a rundown of each table.
