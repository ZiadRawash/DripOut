# DripOut - Online Clothing Store API

## Project Overview

DripOut is a comprehensive backend API for an online clothing store built with **.NET 8** using **Clean Architecture** principles.

## Features

### Authentication
- **JWT Authentication System**
  - Secure access tokens with configurable expiration
  - Refresh token rotation for enhanced security
  - Complete token lifecycle management
- **Role-Based Authorization**
  - Predefined roles: Admin and User
  - Role-specific access to endpoints and resources
- **User Management**
  - Registration with validation
  - Secure login process
  - Account management capabilities

### Database
- SQL Server with Entity Framework Core
- Automatic database seeding with initial users and roles
- Migration support for database versioning

## Project Structure

The solution follows Clean Architecture principles with distinct layers:

| Project | Description |
|---------|-------------|
| DripOut.Domain | Core domain models, constants, enums, and shared contracts |
| DripOut.Application | Application services, DTOs, interfaces, and business logic |
| DripOut.Infrastructure | Implementation of services like authentication, logging, and external integrations |
| DripOut.Persistence | Database context, migrations, repositories, and Entity Framework configuration |
| DripOut.API | Controllers, middleware, and API endpoints for frontend communication |

## Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server (local or remote instance)
- Git

### Installation

1. **Clone the Repository**
   
bash
   git clone https://github.com/ZiadRawash/DripOut.git
   cd DripOut


2. **Configure Database Connection**
   
   Update the connection string in appsettings.json:
   
json
   "ConnectionStrings": {
     "DefaultConnection": "Server=.;Database=DripOut;Trusted_Connection=True;TrustServerCertificate=true;MultipleActiveResultSets=true;"
   }


3. **Apply Migrations and Update Database**
   
bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update


4. **Run the Application**
   
bash
   dotnet run

   
   The API will be accessible at https://localhost:5001

## Authentication Flow

1. **User Registration/Login:**
   - User provides credentials
   - Server validates and returns JWT access token + refresh token

2. **Authenticated Requests:**
   - Include JWT access token in Authorization header
   - Server validates token for each protected endpoint

3. **Token Refresh:**
   - When access token expires, use refresh token to obtain a new one
   - No need to log in again as long as refresh token is valid

4. **Logout:**
   - Invalidates refresh token on the server side
   - Ensures secure session termination

## Development

### Adding New Features
1. Start by defining entities in the Domain layer
2. Create corresponding DTOs and interfaces in the Application layer
3. Implement services in the Infrastructure or Persistence layer
4. Expose functionality through API controllers

## Contact

Project Link: [https://github.com/ZiadRawash/DripOut](https://github.com/ZiadRawash/DripOut)