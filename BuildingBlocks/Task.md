## Project Tasks

### Active Tasks

- [x] Fix AutoMapper MapperConfiguration constructor mismatch in `Infrastructure/Engine.cs` (🟡 Medium)
  - Created: 2025-11-03
  - Completed: 2025-11-03
  - Dependencies: `AutoMapper` 15.0.1 package in `BuildingBlocks.csproj`
  - Progress: 100%
  - Log:
    - 2025-11-03: Investigated ctor error, no local `MapperConfiguration` type found; disambiguated to `AutoMapper.MapperConfiguration` in `Infrastructure/Engine.cs`.
    - 2025-11-03: Build showed ctor requires 2 args in AutoMapper v15; added `ILoggerFactory` via `LoggerFactory.Create(...)` and passed to `MapperConfiguration`.
    - 2025-11-03: Invoked build to verify resolution.

### Completed Tasks (Archive)

- 2025-11-03: Fix AutoMapper MapperConfiguration constructor mismatch in `Infrastructure/Engine.cs`.


