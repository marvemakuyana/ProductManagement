# Product Management System

## Overview
A full-stack Product Management System built using ASP.NET Core MVC, Web API, Entity Framework Core, and SQL Server.

The application allows authenticated users to manage their own categories and products securely.

---

## Features

### Authentication
- User registration
- User login/logout
- ASP.NET Core Identity

### Category Management
- Create category
- Edit category
- Unique category codes
- Per-user data isolation

### Product Management
- Create product
- Edit product
- Delete product
- Auto-generated product codes
- Product image upload

### Excel Features
- Import products from Excel
- Export products to Excel

### Other Features
- Pagination
- Audit fields
- Swagger API
- Clean Architecture layers

---

## Technology Stack

### Backend
- C#
- ASP.NET Core (.NET 8)
- ASP.NET Core Web API
- Entity Framework Core

### Frontend
- ASP.NET Core MVC
- Razor Views
- Bootstrap

### Database
- SQL Server

### Libraries
- EPPlus (Excel)

---

## Architecture

- Controllers
- Services
- Repositories
- Data (DbContext)
- Models

---

## Security

Each authenticated user can only access their own categories and products.

---

## Setup Instructions

1. Clone project
2. Open solution in Visual Studio
3. Update connection string in `appsettings.json`
4. Run migrations:

```powershell
Update-Database
```
5. Run project
6. Register account
7. Login

---
## Swagger API

Available at:
/swagger

---
## Future Improvements

- JWT Authentication
- Dashboard Reports
- DTO + AutoMapper
- Soft Deletes
- Docker Deployment


## Author

Marvellous Makuyana