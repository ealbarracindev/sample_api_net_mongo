# Sample api .net con Mongo DB
Una muestra de una api construida en Net 5 conectada a mongo DB.

### Alternatively you can also clone the Repository.

1. Clone this Repository and Extract it to a Folder.
3. Change the Connection Strings for the Application and Identity in the web_api/appsettings.dev.json - (WebApi Project)
2. Run the following commands on Powershell in the WebApi Projecct's Directory.
- dotnet restore 
- dotnet run (OR) Run the Solution using Visual Studio 2022

# Dependencies
  Net 5  

## Technologies
- ASP.NET Core 5 WebApi
- REST Standards

## Features
- [x] Entity Framework Core - Code First
- [ ] Repository Pattern - Generic
- [ ] Serilog
- [x] Swagger UI
- [ ] Response Wrappers
- [ ] Healthchecks
- [ ] Pagination
- [ ] In-Memory Caching
- [ ] Redis Caching
- [x] Microsoft Identity with JWT Authentication
- [ ] Role based Authorization
- [ ] Identity Seeding
- [ ] Database Seeding
- [x] Custom Exception Handling Middlewares
- [ ] Global Exception Filter
- [ ] API Versioning
- [ ] Fluent Validation
- [ ] Automapper
- [ ] SMTP / Mailkit / Sendgrid Email Service
- [ ] Complete User Management Module (Register / Generate Token / Forgot Password / Confirmation Mail)
- [ ] User Auditing
- [ ] Soft Delete
