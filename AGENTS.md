# Agent Instructions

## Project

- This is a single .NET 10 Avalonia desktop application. The project file is [EventTracker.csproj](EventTracker.csproj).
- The application uses Avalonia Fluent controls, compiled bindings, ReactiveUI, CsvHelper, and LiveCharts. Preserve the existing UI patterns in `Views/` and `Items/`.
- See [README.md](README.md) for the user-facing overview and screenshots.

## Architecture

- `App.axaml.cs` creates `MainWindow` and injects the `TsvDatasource` implementation of `IDatasource`.
- `ViewModels/MainWindowViewModel.cs` composes the feature viewmodels.
- Most feature folders under `Items/` contain a singular model, plural `*View.axaml` plus code-behind, and `*ViewModel.cs`; external integrations are colocated when needed.
- `ViewModels/ItemViewModel.cs` owns shared item loading, filtering, selection, event creation, updates, image handling, and ReactiveUI commands. Extend it for item features instead of duplicating that behavior.
- Data is persisted as tab-delimited TSV through `Repositories/TsvDatasource.cs`. Item tables use model table names; event tables use `<Type>Events.tsv`.

## Development

- Build: `dotnet build EventTracker.csproj`
- Run with hot reload: `dotnet watch run --project EventTracker.csproj`
- Publish: `dotnet publish EventTracker.csproj`
- There is currently no test project; use the build as the baseline validation unless adding focused tests with a feature.

## Conventions And Pitfalls

- Use nullable reference types, file-scoped namespaces, PascalCase public members, `_camelCase` private fields, primary constructors where they match nearby code, and collection expressions where supported.
- Keep models singular and feature folders/views plural. Preserve established misspellings and public names such as `BookExtetrnal`, `GetItemSettigns`, and `is1001` unless an intentional compatibility change is required.
- Avalonia compiled bindings are enabled by default. Keep XAML binding paths and data contexts explicit and validate XAML changes with a build.
- Runtime data paths are derived from `Settings.Instance.DatasourcePath` by `Repositories/Paths.cs`; they include `Events`, `Events/Images`, `Events/.Temp`, and `Events/.Keys`.
- `Settings.json` is loaded relative to the executable. Do not edit generated runtime copies under `bin/`.
- Do not modify generated or ignored output such as `bin/`, `obj/`, `Debug/`, `Release/`, `artifacts/`, `.vs/`, or logs.
- Keep external API keys and personal event data out of source control.