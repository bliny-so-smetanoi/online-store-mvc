# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

Project rules, layer layout and build/migration commands live in AGENTS.md (communicate with the user in Russian):

@AGENTS.md

## Tests

There is no test project yet: `Tests/ShoppingTests/` contains only `bin/`/`obj/` leftovers and is not in `OnlineStore.sln`. Verification is a solution build, a one-off webpack build, and checking the page in the running app.

## Request pipeline (backend)

- `Program.cs` composes everything: `AddKeycloak` → `AddPersistance` (sic, method name is misspelled) → `AddInfrastructure` → `ConfigureMediator(typeof(CreateTodoItemCommand))`. `ConfigureMediator` scans only the assemblies of the marker types passed in — handlers/validators in a new assembly need its marker type added there.
- Use cases live in `Application/UseCases/<Feature>/{Commands,Queries}/<Name>/`. Commands/queries implement `ICommand<Result>` / `IQuery<...>` from `Application/Abstractions` and return `CSharpFunctionalExtensions.Result`. Small use cases keep command, validator and handler in one file (e.g. `AddToCartCommand.cs`); larger ones split them into sibling files.
- Validators derive from `RequestValidatorBase<T>` (FluentValidation rules in the constructor, async business checks in `RequestValidateAsync`). `ValidationPipelineBehavior` runs both and **throws `ApplicationException`** on failure; `ApplicationExceptionFilter` turns `ApplicationException` into HTTP 400 with the message as body, anything else into 500. So validation errors reach the frontend as a 400 text response, not a `Result`.
- Repository interfaces are in `Application/Repositories`, implementations in `Persistence/Repositories`, registered in `Persistence/DependencyInjection.cs`. Queries use specification classes from `Domain/Specifications` (e.g. `ProductSpecification.ById`).
- Auth: cookie scheme + Keycloak OIDC (`oidc` challenge), configured from the `Authentication` config section; roles in `KeycloakExtension/RoleConstants.cs`. Local overrides go in `appsettings.Local.json` (optional, loaded first) or environment variables.
- Routing: attribute routes + an `areas` route (`Areas/Administrator` is the admin UI with its own `_Layout.cshtml`) + default route.

## Localization

- Server: `LocalizationMiddleware` reads the `culture` cookie (default `en`, sets `culture` and `ui_locales` cookies); `CultureController.ChangeLanguage` switches it. Strings come from `Resource/SharedResources.resx` (+ `.kk.resx` for Kazakh) via the generated `SharedResources` class.
- Client: `utils/localization` fetches `/Culture/GetResources?culture=...` and exposes `t(key, fallback)`; keys must exist in the `.resx` files.

## Frontend build specifics

- `webpack.config.js` turns **every** `.ts` file under `Scripts/src/` into its own entry, output to `wwwroot/js/src/<same path>.bundle.js` (SCSS → `.css` next to it). Views reference bundles explicitly, e.g. `~/js/src/views/home/index/index.bundle.js`; `src/index.ts` (+ `index.css`) is loaded by both `_Layout.cshtml` files on every page.
- Consequence: a module under `utils/` is also emitted as a standalone bundle, but its code only runs on a page if it is imported by a bundle that page loads (or the view adds its own `<script>`). Page-specific scripts go in `src/views/...` or `src/areas/...` mirroring the view path.
- Stack: jQuery, Bootstrap 5.3 (+ bootstrap-icons), RxJS for event streams, dayjs.
