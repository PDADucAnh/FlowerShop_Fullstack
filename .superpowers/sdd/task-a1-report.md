# Task A1 Report — FlashSaleController

## Status: DONE

## Build Result
`dotnet build Flower.Backend\Flower.Backend.csproj` → **Succeeded** (0 errors, 106 warnings — all pre-existing, none from the new file)

## Build Errors Encountered
- Initial `dotnet build` on the `.sln` failed because the solution contains a website project (`Flower-shop.frontend`) that requires the .NET Framework ASP.NET compiler, which isn't available in the current MSBuild.
- **Resolution:** Built only the backend project via `dotnet build Flower.Backend\Flower.Backend.csproj` instead.

## Test Results
N/A — no tests exist for this controller yet; this is a pure file-creation task.

## Concerns
- The solution-level build is broken due to a mixed .NET Framework/.NET Core project setup — only the backend project can be built in isolation.
- No tests are associated with this task scope.
