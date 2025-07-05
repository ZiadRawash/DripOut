# DripOut - Online Clothing Store API

## Project Overview

DripOut is a comprehensive backend API for an online clothing store built with **.NET 8** using **Clean Architecture** principles. The API provides a robust foundation for e-commerce operations with secure authentication.

## Features

### Authentication & Authorization

- **JWT Authentication System**
  - Secure access tokens with configurable expiration (15 minutes)
  - Refresh token rotation for enhanced security
  - Complete token lifecycle management
- **Email Verification**
  - Automated email verification codes sent upon registration
  - SMTP integration with Gmail for reliable email delivery
  - Account activation workflow
- **External Authentication**
  - Google OAuth integration for seamless sign-up/sign-in
  - Support for web and Android client authentication
  - Simplified user onboarding process
- **Role-Based Authorization**
  - Predefined roles: Admin and User
  - Role-specific access to endpoints and resources
- **User Management**
  - Registration with comprehensive validation
  - Secure login process with password hashing
  - Account management capabilities

### Database

- **SQL Server** with Entity Framework Core
- Automatic database seeding with initial users and roles
- Migration support for database versioning
- Optimized queries and indexing

### Image Management

- **Cloudinary Integration**
  - Cloud-based image storage and optimization
  - Automatic image resizing and format conversion
  - CDN delivery for fast image loading
  - Secure image upload and management

## Architecture

The solution follows **Clean Architecture** principles with distinct layers:

| Project | Description |
|---------|-------------|
| **DripOut.Domain** | Core domain models, constants, enums, and shared contracts |
| **DripOut.Application** | Application services, DTOs, interfaces, and business logic |
| **DripOut.Infrastructure** | Implementation of services like authentication, logging, and external integrations |
| **DripOut.Persistence** | Database context, migrations, repositories, and Entity Framework configuration |
| **DripOut.API** | Controllers, middleware, and API endpoints for frontend communication |

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server) (local or remote instance)
- [Git](https://git-scm.com/)
- [Cloudinary Account](https://cloudinary.com/) (for image management)
- [Google Cloud Console](https://console.cloud.google.com/) (for OAuth setup)

### Installation

1. **Clone the Repository**

   ```bash
   git clone https://github.com/ZiadRawash/DripOut.git
   cd DripOut
   ```

2. **Configure Application Settings**

   Update the configuration in `appsettings.json`:

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=.;Database=DripOut;Trusted_Connection=True;TrustServerCertificate=true;MultipleActiveResultSets=true;"
     },
     "JWTSettings": {
       "SignInKey": "your-jwt-secret-key",
       "Audience": "https://localhost:5260",
       "Issuer": "https://localhost:5260",
       "AccessTokenExpiryInMinutes": "15"
     },
     "CloudinarySettings": {
       "CloudName": "your-cloudinary-cloud-name",
       "ApiKey": "your-cloudinary-api-key",
       "ApiSecret": "your-cloudinary-api-secret"
     },
     "MailSettings": {
       "Email": "your-email@gmail.com",
       "DisplayName": "DripOut",
       "Password": "your-app-password",
       "Host": "smtp.gmail.com",
       "Port": 587
     },
     "Authentication": {
       "Google": {
         "Webclient_id": "your-google-web-client-id",
         "Androidclient_id": "your-google-android-client-id",
         "client_secret": "your-google-client-secret"
       }
     }
   }
   ```

3. **Install Dependencies**

   ```bash
   dotnet restore
   ```

4. **Apply Migrations and Update Database**

   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

5. **Run the Application**

   ```bash
   dotnet run --project DripOut.API
   ```

   The API will be accessible at `https://localhost:5001`

### API Documentation

Once running, you can access the interactive API documentation at:

- **Swagger UI**: `https://localhost:5001/swagger`

## Authentication Flow & API Endpoints

### Standard Authentication

1. **User Registration:**
   - `POST /api/Account/Register` - User provides credentials
   - System sends email verification code to user's email
   - Returns success message if email sent

2. **Email Verification:**
   - `POST /api/Account/Verify?email={email}&code={code}` - Verify email with code
   - Returns JWT access token + refresh token upon successful verification

3. **User Login:**
   - `POST /api/Account/Login` - User provides credentials
   - Returns JWT access token + refresh token

4. **Google OAuth:**
   - `POST /api/Account/Google-signin` - Send Google ID token
   - System validates with Google and returns JWT tokens

5. **Token Management:**
   - `POST /api/Account/GenrateAccessToken` - Refresh expired access token
   - `POST /api/Account/LogOut` - Invalidate refresh token

### Authentication Endpoints

| Endpoint | Method | Description | Request Body |
|----------|---------|-------------|--------------|
| `/api/Account/Register` | POST | Register new user | `RegisterDto` |
| `/api/Account/Verify` | POST | Verify email with code | Query params: `email`, `code` |
| `/api/Account/Login` | POST | User login | `LoginDto` |
| `/api/Account/Google-signin` | POST | Google OAuth login | `GoogleSignupTokenDto` |
| `/api/Account/GenrateAccessToken` | POST | Refresh access token | Refresh token string |
| `/api/Account/LogOut` | POST | Logout user | Refresh token string |

## Development

### Adding New Features

1. **Domain Layer**: Define entities and domain logic
2. **Application Layer**: Create DTOs, interfaces, and application services
3. **Infrastructure/Persistence**: Implement concrete services and repositories
4. **API Layer**: Expose functionality through controllers

## Configuration

### Required Configuration in `appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=DripOut;Trusted_Connection=True;TrustServerCertificate=true;MultipleActiveResultSets=true;"
  },
  "JWTSettings": {
    "SignInKey": "your-256-bit-secret-key",
    "Audience": "https://localhost:5260",
    "Issuer": "https://localhost:5260",
    "AccessTokenExpiryInMinutes": "15"
  },
  "CloudinarySettings": {
    "CloudName": "your-cloudinary-cloud-name",
    "ApiKey": "your-cloudinary-api-key",
    "ApiSecret": "your-cloudinary-api-secret"
  },
  "MailSettings": {
    "Email": "your-email@gmail.com",
    "DisplayName": "DripOut",
    "Password": "your-gmail-app-password",
    "Host": "smtp.gmail.com",
    "Port": 587
  },
  "Authentication": {
    "Google": {
      "Webclient_id": "your-google-web-client-id",
      "Androidclient_id": "your-google-android-client-id",
      "client_secret": "your-google-client-secret"
    }
  }
}
```

### Configuration Notes

- **Cloudinary**: Sign up at [Cloudinary](https://cloudinary.com/) for image management
- **Google OAuth**: Set up OAuth 2.0 credentials at [Google Cloud Console](https://console.cloud.google.com/)

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## Contact

- GitHub: [@ZiadRawash](https://github.com/ZiadRawash)
- Project Link: [https://github.com/ZiadRawash/DripOut](https://github.com/ZiadRawash/DripOut)
