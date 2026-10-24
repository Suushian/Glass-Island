# Phase 3 — Packaged Notification Test Artifact

## Purpose

The `GlassIsland-Packaged-Notification-Test-win-x64` artifact is a development-only Windows x64 build that embeds sparse package identity and declares the `uap3:Capability Name="userNotificationListener"` capability. It is separate from the normal unpackaged EXE artifact and does not change the app's target framework or project dependencies.

The package build uses Microsoft's WinApp CLI sparse-package flow. WinApp CLI is currently public preview, so the generated manifest and package must be validated by the CI job and then on the target Windows machine.

## Download

1. Open the [packaged notification test workflow](https://github.com/Suushian/Glass-Island/actions/workflows/packaged-notification-test.yml).
2. Open the latest successful run for branch `feat/phase-3-packaged-notification-test`.
3. Download `GlassIsland-Packaged-Notification-Test-win-x64` from **Artifacts**.
4. Extract the archive into a permanent local folder. Do not move the folder after registering the sparse package.

## Install and launch

This is a locally signed development test package, not a production release. It includes the public certificate only; the private signing key is not distributed.

1. On Windows 10/11 x64, open PowerShell **as Administrator** in the extracted folder.
2. Trust the public test certificate in the local machine's Trusted People store:

   ```powershell
   Import-Certificate -FilePath .\GlassIsland-Dev.cer -CertStoreLocation Cert:\LocalMachine\TrustedPeople
   ```

3. Register the sparse identity package against the extracted folder:

   ```powershell
   Add-AppxPackage -Path .\GlassIsland-NotificationIdentity.msix -ExternalLocation (Get-Location).Path
   ```

4. Launch `GlassIsland.exe` from that folder.
5. Test the permission prompt, Allow, Deny, revocation, existing/incoming notifications, and app shutdown using the checklist in [PHASE-3-PACKAGE-IDENTITY-TEST.md](PHASE-3-PACKAGE-IDENTITY-TEST.md).

## Unregister and clean up

In elevated PowerShell from the same folder:

```powershell
Get-AppxPackage | Where-Object { $_.InstallLocation -eq (Get-Location).Path } | Remove-AppxPackage
Remove-Item Cert:\LocalMachine\TrustedPeople\* -ErrorAction SilentlyContinue
```

**Important:** Do not run the broad certificate removal command above on a machine with other trusted certificates. Identify the specific GlassIsland development certificate by its subject/thumbprint and remove only that certificate. The package removal query can also vary depending on how Windows reports the sparse package install location; inspect `Get-AppxPackage` before removal.

## What CI can and cannot prove

CI validates that the application publishes, the sparse manifest contains the notification listener capability, the package is produced, and an artifact is uploaded. GitHub-hosted CI does **not** validate the interactive Windows permission UI or access to the user's real notification database. Those scenarios require manual testing on Windows and a recorded result.

Do not distribute this development artifact as a release. The certificate is temporary and self-signed, the package has not undergone production signing, and Phase 3 is not complete until the runtime acceptance matrix is executed.
