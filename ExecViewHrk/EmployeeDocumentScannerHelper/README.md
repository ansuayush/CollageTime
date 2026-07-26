# HRNest Employee Document Scanner Helper (TWAIN)

Windows desktop helper that **detects connected TWAIN scanners**, scans pages with **NTwain**, and uploads them to the existing web API:

`POST /EmployeeDocuments/UploadAndSave`

Browsers cannot talk to TWAIN. This app fills that gap.

## Requirements

- Windows PC with a TWAIN-compatible scanner and drivers
- **32-bit (x86)** process — most scanner drivers are 32-bit only (this project sets `PlatformTarget=x86`)
- HRNest web app running (e.g. `http://localhost:51643/`)
- Admin user that can access Employee Documents
- `EmployeeDocuments` SQL table installed (see `App_Data/Sql/EmployeeDocuments_Install.sql`)

## Build & run

```bat
cd ExecViewHrk\EmployeeDocumentScannerHelper
dotnet build -c Release
dotnet run -c Release
```

Or open `EmployeeDocumentScannerHelper.csproj` in Visual Studio and run (platform **x86**).

Output:

`bin\Release\net8.0-windows\EmployeeDocumentScannerHelper.exe`

## Workflow

1. Enter HRNest site URL, User ID, Password → **Sign in**
2. **Refresh list** (or **TWAIN picker**) to detect scanners → **Connect**
3. **Scan page(s)** (optional scanner UI checkbox)
4. Search and select employee
5. Optionally check **sign after upload** (Admin or Employee + typed name)
6. **Upload PDF to employee folder** — pages are posted as JPEGs; the server merges to PDF and stores metadata

## API used

| Step | Endpoint |
|------|----------|
| Login | `POST /Account/Login` (cookie auth) |
| Session check | `GET /EmployeeDocuments/HelperPing` |
| Employee search | `GET /EmployeeDocuments/SearchEmployees?text=` |
| Upload | `POST /EmployeeDocuments/UploadAndSave` (multipart images + `employeeId`) |
| Optional sign | `POST /EmployeeDocuments/SignDocument` |

## Notes

- If no scanners appear: install manufacturer TWAIN drivers, confirm the app is **x86**, try **TWAIN picker**.
- WIA-only devices will not show in TWAIN lists.
- Keep the HRNest site URL reachable from the PC running this helper.
- For signatures, also run `App_Data/Sql/EmployeeDocuments_AddSignature.sql` if the table already existed without signature columns.
