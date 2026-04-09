# Task.md

- **Title**: Fix Novell LDAP symbol resolution errors (`GetAttribute` -> `getAttribute`)
- **Created**: 2025-11-03T00:00:00Z
- **Priority**: 🟡 Medium
- **Dependencies**: None

## Tasks
- [~] Update `ActiveDirectoryAppUser.cs` and `OpenLdapAppUser.cs` to call `getAttribute` instead of `GetAttribute`
  - Created: 2025-11-03
  - Started: 2025-11-03
  - Notes: IDE reports "Cannot resolve symbol 'GetAttribute'"; Novell.Directory.Ldap uses lowerCamelCase `getAttribute`.

## Log
- 2025-11-03: Marked task in-progress; preparing code edits.
- 2025-11-03: Replaced `GetAttribute` with `getAttribute` in `BuildingBlocks.Ldap/UserModel/ActiveDirectoryAppUser.cs` and `BuildingBlocks.Ldap/UserModel/OpenLdapAppUser.cs`.
- 2025-11-03: Attempted build, but `dotnet` SDK is not available on this environment (`dotnet: command not found`). Please run `dotnet build` locally to verify.
