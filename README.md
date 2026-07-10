# PartialBreastROIMaker

Varian Eclipse ESAPI plugin script that auto-generates the planning structures used in
partial breast irradiation. Given a structure set that already contains **BODY**,
**Cavity**, and **Chest Wall** contours, it creates (or overwrites) three derived
structures so they don't have to be built by hand for every plan.

Small single-purpose clinical script; last updated 2021, not actively maintained.

## What it creates

- **Skin** — the BODY contour expanded by 5 mm, minus BODY (a 5 mm shell at the body surface).
- **PTV** — Cavity expanded by 10 mm, intersected with BODY, then cropped 5 mm back from
  the body surface and with the Chest Wall subtracted.
- **PTVeval** — PTV minus the Cavity.

If a structure with the target ID already exists, its segment volume is overwritten
rather than duplicated. The script calls `patient.BeginModifications()` and is marked
`ESAPIScript(IsWriteable = true)`, so it writes to the structure set.

## Contents

- `Projects/PartialBreastVolumes/PartialBreastVolumes.cs` — the entire script (one `Execute` entry point plus the three structure-building methods).
- `Projects/PartialBreastVolumes/PartialBreastVolumes.csproj` — builds `PartialBreastVolumes.esapi.dll` as a binary plugin.
- `Plugins/PartialBreastVolumes.esapi.dll` — a precompiled copy of the plugin.

## Requirements

- Varian Eclipse with ESAPI; the project references the v16.1 API DLLs
  (`VMS.TPS.Common.Model.API` / `Types` from `Varian\RTM\16.1\esapi\API`).
- .NET Framework 4.6.1, x64.
- Structure set must contain structures with the exact IDs `BODY`, `Cavity`, and `Chest Wall`.

## Usage

Build the project (or use the DLL in `Plugins/`), then run it as a plugin script from
Eclipse on the open patient/structure set.
