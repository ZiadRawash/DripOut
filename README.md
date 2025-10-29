# DripOut - Online Clothing Store API  

## 🧩 Project Overview  

**DripOut** is a modern and comprehensive backend API for an online clothing store built with .NET 8, following Clean Architecture principles.
It serves as a robust and scalable foundation for e-commerce operations — including authentication, product management, orders, user reviews, image handling, and governorate management — all secured with role-based access and JWT authentication.

---

## ⚙️ Tech Stack  

- **Framework:** .NET 8 (ASP.NET Core)  
- **Database:** SQL Server (via Entity Framework Core 9.0)  
- **ORM:** Entity Framework Core with migrations and seeding  
- **Authentication:** ASP.NET Core Identity + JWT Authentication  
- **Authorization:** Role-based (Admin, User)  
- **Validation:** FluentValidation  
- **Mapping:** AutoMapper  
- **Logging:** Serilog (console, file, Seq)  
- **Background Jobs:** Hangfire  
- **Image Management:** Cloudinary  
- **Email Service:** MailKit / SMTP  
- **API Documentation:** Swagger (Swashbuckle)  
- **External Auth:** Google OAuth integration  

---

## 🔒 Authentication & Authorization  

### **JWT Authentication System**  
- Secure access tokens with configurable expiration (default: 15 minutes)  
- Refresh token rotation and lifecycle management  
- Secure password hashing and user credential storage  

### **Email Verification**  
- Automated email verification codes sent upon registration  
- SMTP integration (Gmail supported)  
- Activation workflow via unique verification code  

### **External Authentication**  
- Google OAuth login for web and Android clients  
- Seamless sign-in/sign-up process  

### **Role-Based Authorization**  
- Roles: **Admin**, **User**  
- Role-specific access to endpoints and data  

### **User Management**  
- Secure registration with validation  
- Login with JWT/Refresh token pair  
- Account management and logout endpoints  

---

## 🗄️ Database  

- **SQL Server** with **Entity Framework Core**  
- Automatic migrations and database seeding  
- Repository Pattern for clean data access  
- **Unit of Work** pattern for atomic transactions  
- Optimized queries and relationships (with cascading behavior)  

---

## 🧱 Architecture  

DripOut adheres to **Clean Architecture** and a **layered structure** to ensure scalability, maintainability, and testability.  

| Project | Description |
|----------|-------------|
| **DripOut.Domain** | Core entities, enums, and business logic (e.g., `Product`, `Order`, `Review`, `AppUser`) |
| **DripOut.Application** | DTOs, interfaces, and service logic for business use cases |
| **DripOut.Infrastructure** | Logging, external services (Cloudinary, MailKit), and integrations |
| **DripOut.Persistence** | Database context, repositories, and migrations |
| **DripOut.API** | Controllers, middleware, startup configuration, and routing |

### 🧩 Key Design Patterns  

- **Repository Pattern:** Abstraction for all CRUD operations  
- **Unit of Work:** Coordinates multiple repositories and transactions  
- **Dependency Injection:** Native ASP.NET Core DI for services  
- **Separation of Concerns:** Keeps domain logic independent of external dependencies  

---

## 🌍 Features Overview  

### 🛍️ Product Management  
- CRUD for products, categories, and variants (size, color, etc.)  
- Image management with Cloudinary (automatic optimization and resizing)  

### 💬 User Interactions  
- Product reviews with upvotes/downvotes  
- Favorites (wishlist) functionality  
- Shopping carts with item tracking  

### 📦 Order Management  
- Order creation and stock reservations  
- Prevents overselling through inventory locking  
- Governorate (region) support for shipping/address management  

### ⚙️ Background Processing  
- **Hangfire** jobs for background tasks like order notifications  

### 🧾 Logging & Monitoring  
- **Serilog** logging to console, file, or **Seq**  
### 🧾 Payment Gateway  
- **Stripe** for handling payments  

---

## 🚀 Getting Started  

### Prerequisites  

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)  
- [SQL Server](https://www.microsoft.com/en-us/sql-server) (local or remote)  
- [Git](https://git-scm.com/)  
- [Cloudinary Account](https://cloudinary.com/)  
- [Google Cloud Console](https://console.cloud.google.com/) (for OAuth setup)  
- [Stripe Dashboard](https://dashboard.stripe.com/) (for payment gateway setup)

---

### Installation  

1. **Clone the Repository**  
   ```bash
   git clone https://github.com/ZiadRawash/DripOut.git
   cd DripOut
   ```

2. **Configure Application Settings**  
   Update your `appsettings.json`:  
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=.;Database=DripOut;Trusted_Connection=True;TrustServerCertificate=true;"
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
     },
     "Stripe": {
      "SecretKey": "your-stripe-secret-key",
      "PublishableKey": "your-stripe-publishable-key",
      "WebhookSecret": "your-stripe-webhook-secret"
      }
     
   }
   ```

3. **Install Dependencies**  
   ```bash
   dotnet restore
   ```

4. **Apply Migrations & Seed Database**  
   ```bash
   dotnet ef database update
   ```

5. **Run the Application**  
   ```bash
   dotnet run --project DripOut.API
   ```

   The API will be accessible at **`https://localhost:5001`**  

---

## 📘 API Documentation  

Access the interactive Swagger UI:  
👉 [https://localhost:5001/swagger](https://localhost:5001/swagger)


## 🤝 Contributing  

1. Fork the repository  
2. Create a feature branch:  
   ```bash
   git checkout -b feature/AmazingFeature
   ```
3. Commit your changes:  
   ```bash
   git commit -m "Add AmazingFeature"
   ```
4. Push to your branch:  
   ```bash
   git push origin feature/AmazingFeature
   ```
5. Open a Pull Request  

---

## 👤 Contact  

- GitHub: [@ZiadRawash](https://github.com/ZiadRawash)  
- Project Link: [https://github.com/ZiadRawash/DripOut](https://github.com/ZiadRawash/DripOut)  

---

