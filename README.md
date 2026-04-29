# Business Card Manager

A full-stack web application for managing business card information. The backend is a .NET 8 Web API using SQL Server and Entity Framework Core. The frontend is an Angular application that supports manual card creation, file import preview, filtering, deletion, and CSV/XML export.

## Features

- Create business cards from the UI.
- Store name, gender, date of birth, email, phone, optional Base64 photo, and address.
- Validate optional photo payloads with a 1MB limit.
- List and filter cards by name, gender, date of birth, email, and phone.
- Delete stored cards.
- Import cards from CSV and XML.
- Import QR code images using ZXing. QR payloads exclude photo data.
- Preview manual cards and imported files before saving/importing.
- Export filtered result sets as CSV or XML.
- Export individual business cards as CSV or XML.

## Tech Stack

- Backend: .NET 8 Web API, C#
- Database: SQL Server Express
- ORM: Entity Framework Core
- Frontend: Angular 18
- CSV: CsvHelper
- QR: ZXing.Net on backend, `@zxing/browser` on frontend preview
- Image loading for backend QR import: ImageSharp
- Tests: xUnit

## Repository Structure

```text
backend/
  BusinessCardManager.Api/
    Controllers/
    Data/
    Dtos/
    Exporting/
    Importing/
    Mapping/
    Models/
    Services/
    Validators/
  BusinessCardManager.Tests/
database/
  schema.sql
frontend/
```

## Prerequisites

- .NET 8 SDK
- Node.js 20 or later
- npm
- SQL Server Express
- SQL Server Management Studio, optional but recommended

## Database Setup

The default connection string is in:

```text
backend/BusinessCardManager.Api/appsettings.json
```

Default value:

```text
Server=localhost\SQLEXPRESS;Database=BusinessCardManagerDb;Trusted_Connection=True;TrustServerCertificate=True
```

Apply migrations:

```powershell
dotnet tool restore
dotnet ef database update --project ".\backend\BusinessCardManager.Api"
```

Regenerate the SQL script:

```powershell
dotnet ef migrations script --project ".\backend\BusinessCardManager.Api" --output ".\database\schema.sql"
```

## Run Backend

From the repository root:

```powershell
dotnet run --project ".\backend\BusinessCardManager.Api"
```

Backend URLs:

```text
http://localhost:5222
http://localhost:5222/swagger
```

## Run Frontend

From the frontend folder:

```powershell
cd frontend
npm install
npm start
```

Frontend URL:

```text
http://localhost:4200
```

## Run Tests

```powershell
dotnet test ".\backend\BusinessCardManager.sln"
```

Build frontend:

```powershell
cd frontend
npm run build
```

## API Endpoints

```http
GET    /api/BusinessCards
GET    /api/BusinessCards/{id}
POST   /api/BusinessCards
DELETE /api/BusinessCards/{id}

POST   /api/BusinessCards/import/csv
POST   /api/BusinessCards/import/xml
POST   /api/BusinessCards/import/qr

GET    /api/BusinessCards/export/csv
GET    /api/BusinessCards/export/xml
GET    /api/BusinessCards/{id}/export/csv
GET    /api/BusinessCards/{id}/export/xml
```

Filtering is supported on list and filtered export endpoints:

```http
GET /api/BusinessCards?name=Ali&gender=Male&email=example
GET /api/BusinessCards/export/csv?phone=079
```

## Create Card Example

```json
{
  "name": "Test User",
  "gender": "Male",
  "dateOfBirth": "1998-01-15",
  "email": "test@example.com",
  "phone": "+962790000000",
  "photoBase64": null,
  "address": "Amman, Jordan"
}
```

## CSV Import Format

```csv
Name,Gender,DateOfBirth,Email,Phone,PhotoBase64,Address
CSV User,Male,1998-01-15,csv@example.com,+962790000001,,Amman
```

`Photo` is also accepted as an alternative header to `PhotoBase64`.

## XML Import Format

```xml
<BusinessCards>
  <BusinessCard>
    <Name>XML User</Name>
    <Gender>Female</Gender>
    <DateOfBirth>1997-02-20</DateOfBirth>
    <Email>xml@example.com</Email>
    <Phone>+962790000002</Phone>
    <PhotoBase64></PhotoBase64>
    <Address>Irbid</Address>
  </BusinessCard>
</BusinessCards>
```

`Photo` is also accepted as an alternative element to `PhotoBase64`.

## QR Import Payloads

QR import accepts an image file containing business card data. The backend supports JSON, XML, vCard, or key-value payloads. Photo data is ignored for QR imports.

JSON payload example:

```json
{
  "name": "QR User",
  "gender": "Male",
  "dateOfBirth": "1996-03-25",
  "email": "qr@example.com",
  "phone": "+962790000003",
  "address": "Zarqa"
}
```

Key-value payload example:

```text
Name=QR User;Gender=Male;DateOfBirth=1996-03-25;Email=qr@example.com;Phone=+962790000003;Address=Zarqa
```

## Export

Filtered result-set export:

```http
GET /api/BusinessCards/export/csv
GET /api/BusinessCards/export/xml
```

Individual card export:

```http
GET /api/BusinessCards/1/export/csv
GET /api/BusinessCards/1/export/xml
```

## Database Script

The database script for testing is available at:

```text
database/schema.sql
```

## Notes

- The Angular app expects the backend at `http://localhost:5222`.
- CORS is configured for `http://localhost:4200`.
- Import files are limited to 5MB.
- Optional photos are limited to 1MB after Base64 decoding.
