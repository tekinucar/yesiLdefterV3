# Location Entities Documentation

This document describes the province (İl) and district (İlçe) entity models and their relationships with the UstadFirms table.

## Database Tables

### 1. **ILTipi** (Province Table)
**Purpose**: Stores Turkish provinces/cities (İller)

**Columns**:
- `Id` (PK) - Primary key
- `IlKodu` - Province code (1-81 for Turkish provinces)
- `IlAdiBUYUK` - Province name in uppercase (e.g., "ANKARA", "İSTANBUL")
- `IlAdiKucuk` - Province name in lowercase (e.g., "Ankara", "İstanbul")
- `IlGIBKodu` - GIB (Tax Office) code for the province
- `IlKoduSifirli` - Province code with leading zero (e.g., "01", "06", "34")

**Example Data**:
```
Id: 6, IlKodu: 6, IlAdiBUYUK: "ANKARA", IlAdiKucuk: "Ankara"
Id: 34, IlKodu: 34, IlAdiBUYUK: "İSTANBUL", IlAdiKucuk: "İstanbul"
```

---

### 2. **UstadFirmsDistrictType** (District Table)
**Purpose**: Stores Turkish districts (İlçeler) within provinces

**Columns**:
- `Id` (PK) - Primary key
- `IlKodu` (FK) - Province code - Links to ILTipi.IlKodu
- `DistrictKodu` - District code (unique within province)
- `DistrictAdi` - District name (e.g., "SEYHAN", "ÇANKAYA", "KADIKÖY")

**Example Data**:
```
Id: 10101, IlKodu: 1, DistrictKodu: 1104, DistrictAdi: "SEYHAN"
Id: 10606, IlKodu: 6, DistrictKodu: 1231, DistrictAdi: "ÇANKAYA"
Id: 13412, IlKodu: 34, DistrictKodu: 1421, DistrictAdi: "KADİKÖY"
```

---

### 3. **UstadFirms** (Firm Table - Location Relationship)
**Purpose**: Stores firm information including location

**Location-Related Columns**:
- `CityTypeId` (FK) - Foreign key to ILTipi.Id (province)
- `DistrictTypeId` (FK) - Foreign key to UstadFirmsDistrictType.Id (district)

**Relationship**:
```
UstadFirms.CityTypeId → ILTipi.Id
UstadFirms.DistrictTypeId → UstadFirmsDistrictType.Id
UstadFirmsDistrictType.IlKodu → ILTipi.IlKodu
```

---

## Entity Models

### ILTipi.cs
**Location**: `Ustad.API/Models/ILTipi.cs`

**Properties**:
- `Id` - int
- `IlKodu` - int
- `IlAdiBUYUK` - string
- `IlAdiKucuk` - string
- `IlGIBKodu` - string?
- `IlKoduSifirli` - string?

**Response DTO**: `ProvinceResponse`
- Simplified response model for API endpoints
- Includes all province information in a clean format

---

### UstadFirmsDistrictType.cs
**Location**: `Ustad.API/Models/UstadFirmsDistrictType.cs`

**Properties**:
- `Id` - int
- `IlKodu` - int (links to province)
- `DistrictKodu` - int
- `DistrictAdi` - string

**Response DTOs**:
- `DistrictResponse` - Basic district information
- `DistrictWithProvinceResponse` - District with province information and full location string

---

## API Endpoints

### LocationController
**Base Route**: `/api/location`

#### Province Endpoints

**GET /api/location/provinces**
- Returns all provinces
- Response: `List<ProvinceResponse>`
- Example: `GET /api/location/provinces`

**GET /api/location/provinces/{provinceCode}**
- Returns a specific province by code (1-81)
- Response: `ProvinceResponse`
- Example: `GET /api/location/provinces/6` (Ankara)

#### District Endpoints

**GET /api/location/districts**
- Returns all districts
- Optional query parameter: `provinceCode` to filter by province
- Response: `List<DistrictResponse>`
- Examples:
  - `GET /api/location/districts` (all districts)
  - `GET /api/location/districts?provinceCode=6` (Ankara districts only)

**GET /api/location/districts/{districtId}**
- Returns a specific district by ID
- Response: `DistrictResponse`
- Example: `GET /api/location/districts/10606` (Çankaya)

**GET /api/location/provinces/{provinceCode}/districts**
- Returns all districts for a specific province with province information
- Response: `List<DistrictWithProvinceResponse>`
- Includes `FullLocation` property (e.g., "Çankaya, Ankara")
- Example: `GET /api/location/provinces/6/districts`

---

## Database Relationships

```
┌─────────────┐
│   ILTipi    │
│  (Province) │
└──────┬──────┘
       │
       │ IlKodu (1:Many)
       │
       ▼
┌──────────────────────┐
│UstadFirmsDistrictType│
│     (District)       │
└──────┬───────────────┘
       │
       │ Id (Many:1)
       │
       ▼
┌─────────────┐
│ UstadFirms  │
│   (Firm)    │
└─────────────┘
       │
       │ CityTypeId (Many:1)
       │
       └──────────┐
                  │
                  ▼
            ┌─────────────┐
            │   ILTipi    │
            │  (Province) │
            └─────────────┘
```

**Relationship Details**:
1. **Province → Districts**: One province has many districts (`ILTipi.IlKodu` → `UstadFirmsDistrictType.IlKodu`)
2. **District → Firm**: Many districts can be associated with many firms (`UstadFirmsDistrictType.Id` → `UstadFirms.DistrictTypeId`)
3. **Province → Firm**: Many provinces can be associated with many firms (`ILTipi.Id` → `UstadFirms.CityTypeId`)

---

## Usage Examples

### Get All Provinces
```csharp
// API Call
GET /api/location/provinces

// Response
[
  {
    "id": 6,
    "provinceCode": 6,
    "name": "Ankara",
    "nameUppercase": "ANKARA",
    "gibCode": null,
    "provinceCodePadded": "06"
  },
  ...
]
```

### Get Districts for a Province
```csharp
// API Call
GET /api/location/provinces/6/districts

// Response
[
  {
    "id": 10606,
    "districtCode": 1231,
    "districtName": "ÇANKAYA",
    "provinceCode": 6,
    "provinceName": "Ankara",
    "fullLocation": "ÇANKAYA, Ankara"
  },
  ...
]
```

### Get Firm with Location
```csharp
// When querying UstadFirms, join with location tables:
SELECT 
    f.FirmId,
    f.FirmLongName,
    f.CityTypeId,
    f.DistrictTypeId,
    p.IlAdiKucuk AS ProvinceName,
    d.DistrictAdi AS DistrictName
FROM UstadFirms f
LEFT JOIN ILTipi p ON f.CityTypeId = p.Id
LEFT JOIN UstadFirmsDistrictType d ON f.DistrictTypeId = d.Id
WHERE f.FirmId = @FirmId
```

---

## Integration with UstadFirms

The `UstadFirm` model has been updated to include:
- `DistrictTypeId` - Links to district
- `CityTypeId` - Links to province
- `DistrictName` - Populated from UstadFirmsDistrictType
- `ProvinceName` - Populated from ILTipi

When querying firms, you can join with these tables to get location information:

```sql
SELECT 
    f.*,
    p.IlAdiKucuk AS ProvinceName,
    d.DistrictAdi AS DistrictName
FROM UstadFirms f
LEFT JOIN ILTipi p ON f.CityTypeId = p.Id
LEFT JOIN UstadFirmsDistrictType d ON f.DistrictTypeId = d.Id
```

---

## Notes

1. **Province Code Range**: Turkish provinces use codes 1-81
2. **District Relationship**: Districts are linked to provinces via `IlKodu`, not `Id`
3. **Firm Location**: Firms can optionally have both province and district assigned
4. **Data Integrity**: Ensure `UstadFirmsDistrictType.IlKodu` matches `ILTipi.IlKodu` for valid relationships

---

## Future Enhancements

- Add validation to ensure district belongs to province when creating/updating firms
- Add location-based filtering for firm searches
- Add geographic coordinates if needed
- Add postal code integration if available

