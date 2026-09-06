# SportsBites - Game Day Community Platform

A modern, full-stack sports fan community platform built with **Angular** and **ASP.NET Core**. Connect with fellow sports enthusiasts, share real-time commentary, and engage during live events.

![Language Composition](https://img.shields.io/badge/C%23-30.7%25-blue) ![Language Composition](https://img.shields.io/badge/HTML-25.5%25-orange) ![Language Composition](https://img.shields.io/badge/CSS-23.3%25-pink) ![Language Composition](https://img.shields.io/badge/TypeScript-19.6%25-blue) ![Language Composition](https://img.shields.io/badge/Dockerfile-0.9%25-gray)

## 🚀 Quick Start

### Prerequisites

- **Node.js** 20+ (for Angular frontend)
- **.NET 8.0 SDK** (for ASP.NET Core backend)
- **Docker** & **Docker Compose** (optional, for containerized deployment)
- **PostgreSQL** database

### Local Development

#### 1. Clone the Repository
```bash
git clone https://github.com/jaslone1/SportsBites.git
cd SportsBites
```

#### 2. Backend Setup (ASP.NET Core)

```bash
# Restore dependencies
dotnet restore

# Set up environment variables
# Create a .env file or set in your IDE:
export JWT_SECRET_KEY="your-secret-key-here"
export JWT_ISSUER="SportsBites"
export JWT_AUDIENCE="SportsBites"
export ConnectionStrings__PostgresConnection="Server=localhost;Port=5432;Database=sportsbites;User Id=postgres;Password=password;"

# Run migrations
cd GameDayParty
dotnet ef database update

# Start the backend server
dotnet run
```

The API will be available at `http://localhost:5000` with Swagger UI at `http://localhost:5000/swagger`

#### 3. Frontend Setup (Angular)

```bash
cd SportsBitesUI

# Install dependencies
npm install

# Start the development server
npm start
```

The frontend will be available at `http://localhost:4200`

### Docker Deployment

Build and run the entire application in Docker:

```bash
# Build the image
docker build -t sportsbites:latest .

# Run the container
docker run -p 8080:8080 \
  -e JWT_SECRET_KEY="your-secret-key" \
  -e ConnectionStrings__PostgresConnection="your-db-connection" \
  sportsbites:latest
```

Access the application at `http://localhost:8080`

---

## 📋 Project Structure

```
SportsBites/
├── GameDayParty/                    # ASP.NET Core Backend
│   ├── Controllers/                # API endpoints
│   ├── Data/                       # Database context & migrations
│   ├── Models/                     # Domain models
│   ├── Services/                   # Business logic
│   ├── Program.cs                  # Application startup
│   └── GameDayParty.csproj        # Project configuration
│
├── GameDayParty.Client/             # Client integration layer
├── GameDayParty.Shared/             # Shared models & utilities
│
├── SportsBitesUI/                   # Angular Frontend
│   ├── src/
│   │   ├── app/                    # Angular components & services
│   │   ├── assets/                 # Static assets
│   │   └── main.ts                 # Entry point
│   ├── package.json                # Dependencies
│   └── angular.json                # Angular CLI config
│
├── Dockerfile                       # Multi-stage Docker build
├── .dockerignore                   # Docker build exclusions
├── GameDayParty.sln               # Visual Studio Solution
└── README.md                       # This file
```

---

## 🔑 Key Features

### Authentication & Security
- **JWT Bearer Authentication** - Secure token-based authentication
- **Identity Framework** - ASP.NET Core Identity for user management
- **Role-Based Access Control** - Support for different user roles
- **Password Hashing** - Secure password storage

### Database
- **PostgreSQL** - Reliable relational database
- **Entity Framework Core** - Object-relational mapping
- **Automatic Migrations** - Database schema management at startup
- **JSON Serialization** - Proper handling of circular references

### API
- **RESTful API** - Clean, standard HTTP endpoints
- **Swagger/OpenAPI** - Interactive API documentation
- **CORS Support** - Cross-origin resource sharing
- **Camel Case JSON** - Standard naming convention for responses

### Frontend
- **Angular 21** - Modern component-based framework
- **Bootstrap 5** - Professional UI styling
- **Reactive Forms** - Form validation and handling
- **HTTP Client** - Built-in HTTP communication

---

## 🔧 Configuration

### Environment Variables

Create a `.env` file in the root directory or set these in your deployment environment:

```env
JWT_SECRET_KEY=your-very-secure-secret-key-here
JWT_ISSUER=SportsBites
JWT_AUDIENCE=SportsBites
ConnectionStrings__PostgresConnection=Server=localhost;Port=5432;Database=sportsbites;User Id=postgres;Password=password;
ASPNETCORE_URLS=http://+:5000
ASPNETCORE_ENVIRONMENT=Development
```

### Database Connection String

Update `appsettings.json` or set via environment variables:

```json
{
  "ConnectionStrings": {
    "PostgresConnection": "Server=localhost;Port=5432;Database=sportsbites;User Id=postgres;Password=password;"
  }
}
```

---

## 📚 API Documentation

Once the backend is running, access Swagger UI:

```
http://localhost:5000/swagger/index.html
```

This provides interactive documentation for all API endpoints including:
- Authentication endpoints
- User management
- Game/Event data
- Comments and reactions
- Community features

---

## 🧪 Testing

### Backend Tests
```bash
cd GameDayParty
dotnet test
```

### Frontend Tests
```bash
cd SportsBitesUI
npm test
```

### Watch Mode
```bash
cd SportsBitesUI
npm run watch
```

---

## 🐳 Docker Build Process

The Dockerfile uses a **three-stage build** for optimal image size and performance:

1. **Stage 1: Angular Build**
   - Uses `node:20` image
   - Builds Angular production bundle
   - Outputs to `/app/dist/SportsBitesUI/`

2. **Stage 2: .NET Build**
   - Uses `mcr.microsoft.com/dotnet/sdk:8.0`
   - Restores NuGet packages
   - Publishes .NET application in Release mode

3. **Stage 3: Runtime**
   - Uses `mcr.microsoft.com/dotnet/aspnet:8.0`
   - Copies .NET published files
   - Copies Angular dist folder to wwwroot
   - Runs on port 8080

---

## 🚢 Deployment

### Render.com (Recommended)

1. Connect your GitHub repository to Render
2. Set environment variables in Render dashboard:
   - `JWT_SECRET_KEY`
   - `JWT_ISSUER`
   - `JWT_AUDIENCE`
   - `ConnectionStrings__PostgresConnection`

3. Set build command: `docker build -t sportsbites .`
4. Set start command: `dotnet GameDayParty.dll`

### Azure Container Instances
```bash
az container create \
  --resource-group myResourceGroup \
  --name sportsbites \
  --image sportsbites:latest \
  --ports 8080 \
  --environment-variables \
    JWT_SECRET_KEY="your-key" \
    ConnectionStrings__PostgresConnection="your-connection"
```

---

## 🔐 Security Considerations

- Never commit `.env` files to version control
- Use strong JWT secrets in production (minimum 32 characters)
- Enable HTTPS in production
- Set proper CORS origins (not `*` in production)
- Regularly update dependencies: `npm update` and `dotnet restore`
- Use environment-specific configuration files
- Implement rate limiting on API endpoints

---

## 🐛 Troubleshooting

### "Cannot GET /" in browser
- Ensure both frontend and backend are running
- Check backend is serving static files at `wwwroot`
- Verify frontend build succeeded

### CORS errors
- Check `Program.cs` CORS configuration
- Update `AllowOrigins` with correct frontend URL
- Ensure backend is not in production mode

### Database connection errors
- Verify PostgreSQL is running
- Check connection string syntax
- Ensure database exists and user has permissions
- Run migrations: `dotnet ef database update`

### Port already in use
```bash
# Find process using port 5000 (or 4200 for frontend)
lsof -i :5000
kill -9 <PID>
```

---

## 📝 Development Guidelines

### Code Style
- C# (.NET): Follow Microsoft C# Coding Conventions
- TypeScript/Angular: Follow Google TypeScript Style Guide
- Formatting: Use `prettier` for code formatting

Format code:
```bash
# Frontend
npm run format

# Backend
dotnet format
```

### Branching Strategy
- `master` - Production ready code
- `develop` - Development branch
- `feature/*` - Feature branches
- `bugfix/*` - Bug fix branches

### Commit Messages
Use conventional commits:
```
feat: add new feature
fix: resolve bug
docs: update documentation
test: add unit tests
chore: update dependencies
```

---

## 📦 Dependencies

### Backend Key Packages
- `Microsoft.AspNetCore.Authentication.JwtBearer` - JWT authentication
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore` - User management
- `Microsoft.EntityFrameworkCore` - ORM
- `Npgsql.EntityFrameworkCore.PostgreSQL` - PostgreSQL driver
- `Swashbuckle.AspNetCore` - Swagger/OpenAPI

### Frontend Key Packages
- `@angular/core` - Core framework
- `@angular/router` - Routing
- `@angular/forms` - Form handling
- `bootstrap` - UI framework
- `rxjs` - Reactive programming

---

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/amazing-feature`
3. Commit your changes: `git commit -m 'feat: add amazing feature'`
4. Push to the branch: `git push origin feature/amazing-feature`
5. Open a Pull Request

---

## 📄 License

Not currently specified. See repository for license details.

---

## 👤 Author

**jaslone1** - Project Creator and Maintainer

- GitHub: [@jaslone1](https://github.com/jaslone1)
- Repository: [SportsBites](https://github.com/jaslone1/SportsBites)

---

## 📞 Support

For issues, questions, or suggestions:
1. Check existing [GitHub Issues](https://github.com/jaslone1/SportsBites/issues)
2. Review [GitHub Discussions](https://github.com/jaslone1/SportsBites/discussions)
3. Create a new issue if needed

---

## 🗺️ Roadmap

Potential future enhancements:
- [ ] Real-time notifications (SignalR)
- [ ] User profile customization
- [ ] Advanced search and filtering
- [ ] Mobile app (React Native)
- [ ] Analytics dashboard
- [ ] Payment integration
- [ ] Multi-language support
- [ ] Dark mode UI

---

## 📊 Project Stats

- **Language**: C#, TypeScript, HTML, CSS
- **Framework**: ASP.NET Core 8.0, Angular 21
- **Database**: PostgreSQL
- **Created**: December 23, 2025
- **Last Updated**: January 6, 2026
- **Repository**: Public

---

**SportsBites** - *Connecting sports fans worldwide, one game at a time!* ⚽🏀🏈⚾

*Made with ❤️ by sports enthusiasts, for sports enthusiasts.*
