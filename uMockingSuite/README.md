# uMockingSuite

An Umbraco 17 backoffice package that delivers snarky, mocking "advice" whenever content editors create or update content. Because everyone needs a bit of unsolicited criticism with their publishing workflow.

## What It Does

- Hooks into Umbraco content notifications (create/save/publish)
- Generates snarky commentary about the content being edited
- Displays mocking advice in the backoffice UI via a custom dashboard/workspace

## Project Structure

```
uMockingSuite/
├── uMockingSuite.csproj          # Package class library (.NET 10, Umbraco 17)
├── Composers/                     # Umbraco composers for registering services & notifications
├── Notifications/                 # Notification handlers (content events)
├── Services/                      # Mocking engine and business logic
├── Client/
│   ├── src/                       # TypeScript/Lit backoffice UI components
│   └── umbraco-package.json       # Umbraco 17 package manifest
└── wwwroot/                       # Static assets served to the backoffice
```

## Development

This package is developed alongside the `Umbraco.AI.Demo` host site. The demo site references this project directly for local development.

```bash
dotnet build
```

## Stack

- **Backend:** C# / .NET 10, Umbraco 17.3.x
- **Frontend:** TypeScript, Lit (Umbraco backoffice UI framework)
- **Package format:** Umbraco 17 package manifest (`umbraco-package.json`)
