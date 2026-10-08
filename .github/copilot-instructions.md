# Copilot instructions

## Build, test, and formatting

The solution targets .NET 10. CI runs formatting, build, and tests:

```sh
dotnet build
dotnet test
```

Run tests for one project or a single test with xUnit's fully qualified name filter:

```sh
dotnet test Modules\Auth\Weavly.Auth.Tests\Weavly.Auth.Tests.csproj
dotnet test Modules\Auth\Weavly.Auth.Tests\Weavly.Auth.Tests.csproj --filter "FullyQualifiedName~RegisterUserHandlerTests.HandleAsync_ShouldReturn_SuccessInstance_WhenNewUserWasCreated"
```

Formatting uses CSharpier (CI installs it with `dotnet tool install -g csharpier`):

```sh
csharpier check .
csharpier format .
```

The Makefile also provides `make build`, `make check`, and `make format`. Some restores require GitHub Packages credentials; configure them in local NuGet credentials and never commit tokens.

## Architecture

Weavly is a modular ASP.NET Core framework. `Weavly.Api/Program.cs` composes modules: `AddWeavly()` and `.AddModule<T>()` register them, `.Build()` configures services and Wolverine message discovery, and `UseWeavly()` activates module middleware/endpoints and runs module initialization/seeding after application start.

A module typically separates its public contracts into a `.Shared` project and implementation into the main project. Shared projects hold commands, responses, events, identifiers, and cross-module contracts; implementations hold handlers, endpoints, validation, models, persistence, and module setup. Modules communicate through Wolverine commands/events rather than directly depending on another module's implementation.

Feature endpoints implement `IWeavlyEndpoint` (commonly through the endpoint base classes) and dispatch commands through `IMessageBus`. Handlers implement `IWeavlyHandler<TCommand>` and return the shared `Result` types. The core builder disables Wolverine's conventional discovery and explicitly discovers commands and handlers through their marker interfaces, so retain those interfaces when adding features.

Persistence is MongoDB-based. `WeavlyRepository<TModule>` selects a database from the module type and collections from document types; module-specific repositories expose typed collections. Module `Configure` methods register services, `Use` methods add middleware/endpoints, and `InitializeAsync` performs seeding.

The CLI project contains solution/module scaffolding and templates. When changing generated module structure or conventions, check `Weavly.Cli/Commands/Module/NewCommand.cs` and `Weavly.Cli/Templates/` as well as existing modules.

## Repository conventions

- Keep public cross-module types in the corresponding `.Shared` project; keep implementation details in the module project.
- Organize implementation by feature, with command/response contracts and endpoint/handler implementations in matching feature folders.
- Use typed module repositories and strongly typed document identifiers rather than accessing MongoDB collections ad hoc.
- Tests are xUnit projects named `*.Tests`; they commonly inherit shared test bases, use NSubstitute for collaborators, and Shouldly for assertions. Prefer a project's existing test base and mock repository patterns.
- Package versions are centrally managed in `Directory.Packages.props`; project files generally omit package versions.
- CSharpier formatting is checked in CI. Its configuration is in `.csharpierrc.json`.
