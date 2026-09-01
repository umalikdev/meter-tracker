# Meter Tracker

A small family-shared web app to log electricity meter readings and see units
consumed since the last update, with full history (who updated it, when, and
what the value was before/after).

Built as a **Blazor WebAssembly** app (C#) — it's a static site, so it can be
hosted for free on **GitHub Pages**. Data lives in a free **Supabase**
(Postgres) project and is called directly from the browser — there's no
backend server to run or pay for. There's no login; anyone with the link can
view and update readings, and just types their name when they update one.

## 1. Create the free Supabase project

1. Go to https://supabase.com → sign up (free) → **New project**.
2. Once it's created, open **SQL Editor** → **New query**, paste the contents
   of [`supabase-schema.sql`](./supabase-schema.sql), and run it. This creates
   the `meters` and `reading_logs` tables.
3. Go to **Project Settings → API**. Copy:
   - **Project URL** (looks like `https://xxxxxxxx.supabase.co`)
   - **anon public** key (a long string under "Project API keys")

## 2. Plug those into the app

Open `MeterTracker/Services/SupabaseConfig.cs` and replace the placeholders:

```csharp
public const string Url = "https://YOUR_PROJECT_REF.supabase.co";
public const string AnonKey = "YOUR_SUPABASE_ANON_KEY";
```

> The anon key is meant to be public in client-side apps — it only allows what
> the Row Level Security policies in `supabase-schema.sql` permit (open
> read/write, since this is for casual home/family use with no login).

## 3. Run it locally (optional, to try it first)

You'll need the [.NET 8 SDK](https://dotnet.microsoft.com/download).

```bash
cd MeterTracker
dotnet run
```

Open the URL it prints (something like `http://localhost:5xxx`).

## 4. Push to GitHub and deploy to GitHub Pages (free hosting)

1. Create a new **public** GitHub repo and push this whole folder to it:

   ```bash
   git init
   git add .
   git commit -m "Meter tracker"
   git branch -M main
   git remote add origin https://github.com/YOUR_USERNAME/YOUR_REPO_NAME.git
   git push -u origin main
   ```

2. In the repo on GitHub: **Settings → Pages → Build and deployment → Source**,
   choose **GitHub Actions**.
3. The included workflow (`.github/workflows/deploy.yml`) will build and
   deploy automatically on every push to `main`, and sets the correct base
   path for you — you don't need to manually edit `index.html`.
4. After the "Deploy Meter Tracker to GitHub Pages" action finishes (check the
   **Actions** tab), your app will be live at:

   ```
   https://YOUR_USERNAME.github.io/YOUR_REPO_NAME/
   ```

5. Share that link with your family. No install, no login — just open it in a
   browser.

## How it works

- **Add a meter**: give it a name and last month's reading as the baseline.
- **Home page**: shows every meter as a card — current reading and units
  consumed since the last update.
- **Update Reading**: anyone can click it, type the new reading and their
  name. This both updates the meter's current value *and* appends a row to
  the history log (previous value, new value, who, and exact timestamp) —
  nothing is overwritten or lost.
- **History** page (per meter): the complete log of every update ever made.

## Notes / things you can tweak later

- There's a light guard rail: a new reading can't be lower than the current
  one (typos happen) — remove that check in `Home.razor` if you ever need a
  meter reset/rollover.
- "Your name" is a free-text field with autocomplete from names used before —
  simplest option with no login. If you want it locked to specific family
  members, turn it into a fixed dropdown in `Home.razor`.
- Want a chart of consumption over time? The history data is already there in
  `MeterDetails.razor` (`_history`) — a small `Chart.js` via JS interop or the
  `recharts`-style approach would drop right in.
