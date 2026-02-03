# Dash DayTrip API

ASP.NET Core Web API for managing day trip bookings, forms, and tour packages for Tenggol Island.

## 🚀 Technologies

- **.NET 10.0**
- **Entity Framework Core** (SQL Server)
- **Swagger/OpenAPI** for API documentation
- **SQL Server** for database

## 📋 Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- SQL Server 2016 or later
- Visual Studio 2026 or VS Code (optional)

The API will be available at:
- **Local**: `http://localhost:5000`
- **Swagger UI**: `http://localhost:5000/swagger`

## 🌐 Production Deployment

### API Base URLs:
- **Production**: `https://apilive.dstpos.com` or `https://apilivetenggol.dstpos.com`
- **Local Development**: `http://localhost:5000`

### Frontend Integration:
Update your frontend's API base URL to point to: const API_BASE_URL = 'https://apilive.dstpos.com'; // or const API_BASE_URL = 'https://apilivetenggol.dstpos.com';


## 📡 API Endpoints

### **Forms API** (`/api/Forms`)
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/Forms` | Get all forms (supports `?status=active`) |
| GET | `/api/Forms/{id}` | Get form by ID |
| POST | `/api/Forms` | Create new form |
| PUT | `/api/Forms/{id}` | Update form |
| PATCH | `/api/Forms/{id}/status` | Update form status |
| DELETE | `/api/Forms/{id}` | Delete form |

### **Bookings API** (`/api/Bookings`)
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/Bookings` | Get all bookings |
| GET | `/api/Bookings/{id}` | Get booking by ID |
| POST | `/api/Bookings` | Create new booking |
| PUT | `/api/Bookings/{id}` | Update booking (including packages) |
| DELETE | `/api/Bookings/{id}` | Delete booking |
| GET | `/api/Bookings/statistics` | Get booking statistics |
| GET | `/api/Bookings/form/{formId}` | Get bookings by form |
| PATCH | `/api/Bookings/{id}/status` | Update booking status |

### **Packages API** (`/api/Packages`)
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/Packages` | Get all packages |
| GET | `/api/Packages/{id}` | Get package by ID |
| POST | `/api/Packages` | Create new package |
| PUT | `/api/Packages/{id}` | Update package |
| DELETE | `/api/Packages/{id}` | Delete package |

## 📊 Database Schema

- **Forms**: Form configurations and branding
- **Bookings**: Customer bookings and trip details
- **Packages**: Tour package details and pricing
- **BookingPackages**: Many-to-many relationship between bookings and packages
- **FormSettings**: Additional form configuration

