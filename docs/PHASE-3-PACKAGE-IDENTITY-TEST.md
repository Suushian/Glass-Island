# Phase 3 — Package Identity Test Runbook

## Goal

Create a disposable Windows test build with package identity and the `userNotificationListener` capability so the notification permission flow can be validated without changing the normal unpackaged CI artifact or claiming the feature works prematurely.

## Why this is needed

The app currently targets `net8.0-windows10.0.19041.0` and CI publishes an unpackaged EXE. Microsoft's notification-listener API requires the app package to declare the `userNotificationListener` capability and the user to grant access. Microsoft now documents WinApp CLI as a supported route for existing .NET/WPF applications to test package identity and create MSIX packages.

References:
- [Notification listener requirements](https://learn.microsoft.com/en-us/windows/apps/develop/notifications/app-notifications/notification-listener)
- [WinApp CLI: .NET/WPF guide](https://learn.microsoft.com/en-us/windows/apps/dev-tools/winapp-cli/guides/dotnet)
- [WinApp CLI overview](https://learn.microsoft.com/en-us/windows/apps/dev-tools/winapp-cli/)

## Safety rule

Perform this experiment in a separate worktree or disposable branch, not directly on the Phase 3 implementation branch. WinApp initialization may modify the project file, add package references, create a package manifest, or recommend a newer Windows target framework. Review every diff and do not accept a target-framework upgrade or unrelated project rewrite without a separate compatibility decision. Do not commit or upload a development signing certificate (`.pfx`) or its password.

## Test setup on Windows

1. Start from a clean checkout of `feat/phase-3-notification-poc` in a separate worktree/branch.
2. Install the supported WinApp CLI from an elevated or normal PowerShell terminal as appropriate for the machine:

   ```powershell
   winget install Microsoft.winappcli --source winget
   winapp --help
   ```

3. Read the current [WinApp CLI .NET guide](https://learn.microsoft.com/en-us/windows/apps/dev-tools/winapp-cli/guides/dotnet), then initialize only the disposable test checkout using the documented .NET/WPF flow. Inspect the resulting `*.csproj`, manifest, assets, and package references before proceeding. If the CLI requires a target framework change, stop and record the exact requirement rather than silently changing the app's baseline.
4. In the generated `Package.appxmanifest`, declare the Microsoft-documented **User Notification Listener** capability. Verify the manifest schema/namespace matches the generated manifest; do not paste a capability element from a different manifest schema.
5. Use the CLI's documented package-identity debug flow to run the app with identity first. If debug identity is insufficient for the notification API on the tested Windows build, create a signed MSIX with the documented packaging flow and install it locally.
6. Ensure the app is launched as the packaged/identity-enabled application, not by double-clicking the old CI EXE. Confirm the installed package identity before judging permission behavior.
7. Do not distribute this development package. A locally generated development certificate is for testing only; production signing and publisher identity are separate release decisions.

## Runtime acceptance checklist

Record the Windows edition/build, CLI version, target framework, launch method, and result for each case. Do not include notification contents or personal data in logs or screenshots.

- [ ] First launch with access status `Unspecified`: the app presents Windows' permission flow.
- [ ] Allow: existing supported toast notifications are listed; a newly arriving notification updates the UI.
- [ ] Deny: no notification content is displayed and the rest of GlassIsland remains usable.
- [ ] Revoke access in Windows Settings: subsequent refresh/change handling stops exposing notification content and presents a clear status.
- [ ] Empty notification collection with permission allowed: friendly empty state.
- [ ] Missing title/body/app metadata: safe fallback; no crash.
- [ ] Add/dismiss a notification while the app is running: the list reflects the API's resulting state.
- [ ] Close the app during normal activity: no unhandled exception and event handlers are released.
- [ ] Relaunch: the saved Windows permission state is handled correctly without a repeated prompt loop.

## Result reporting

For each run, record only non-sensitive metadata:

| Field | Value |
|---|---|
| Windows version/build | Not tested yet |
| WinApp CLI version | Not tested yet |
| App package identity confirmed | Not tested yet |
| Capability present in installed package | Not tested yet |
| Permission prompt shown | Not tested yet |
| Allow / deny / revoke scenarios | Not tested yet |
| Existing and incoming notifications | Not tested yet |
| Known limitations | Pending Windows runtime validation |

A successful `dotnet build` or CI publish is **not** evidence that notification access works. Phase 3 remains unverified until this checklist is executed on Windows and the observed results are recorded.
