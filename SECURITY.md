# Security Configuration Guide

## Environment Variables

This application requires the following environment variables to be set. **Never commit these values to version control.**

### Database Configuration

```bash
export ConnectionStrings__PostgresConnection="Host=your-host;Port=5432;Database=postgres;Username=postgres;Password=YOUR_PASSWORD;SSL Mode=Require;Trust Server Certificate=true"
```

### JWT Authentication

```bash
export JWT_SECRET_KEY="your-very-secure-secret-key-minimum-32-characters"
export JWT_ISSUER="SportsBites"
export JWT_AUDIENCE="SportsBites"
```

### Development Setup

1. **Create a `.env` file in the root directory** (never commit this):
   ```
   ConnectionStrings__PostgresConnection=your-connection-string
   JWT_SECRET_KEY=your-secret-key
   JWT_ISSUER=SportsBites
   JWT_AUDIENCE=SportsBites
   ASPNETCORE_ENVIRONMENT=Development
   ```

2. **Load environment variables before running:**
   ```bash
   # Linux/Mac
   set -a && source .env && set +a
   dotnet run
   
   # Windows PowerShell
   Get-Content .env | ForEach-Object {
       if ($_ -notmatch '^#' -and $_ -match '^(.+)=(.+)$') {
           $name = $matches[1]
           $value = $matches[2]
           [Environment]::SetEnvironmentVariable($name, $value, 'Process')
       }
   }
   dotnet run
   ```

3. **Using .NET User Secrets (Recommended for Development):**
   ```bash
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:PostgresConnection" "your-connection-string"
   dotnet user-secrets set "JWT_SECRET_KEY" "your-secret-key"
   dotnet user-secrets set "JWT_ISSUER" "SportsBites"
   dotnet user-secrets set "JWT_AUDIENCE" "SportsBites"
   ```
   
   User secrets are stored locally and never committed to version control.

### Production Deployment

**On Render.com:**
1. Go to your service dashboard
2. Navigate to Environment → Environment Variables
3. Add each variable:
   - `ConnectionStrings__PostgresConnection`
   - `JWT_SECRET_KEY`
   - `JWT_ISSUER`
   - `JWT_AUDIENCE`

**On Azure Container Instances:**
```bash
az container create \
  --resource-group myResourceGroup \
  --name sportsbites \
  --image sportsbites:latest \
  --environment-variables \
    ConnectionStrings__PostgresConnection="your-connection" \
    JWT_SECRET_KEY="your-secret-key"
```

## Credential Rotation

If credentials have been exposed:

1. **Immediately change the password** in your database provider (Supabase, AWS RDS, etc.)
2. **Update all deployment environments** with the new credentials
3. **Review access logs** for unauthorized activity
4. **Enable database access logging** for future monitoring

## Best Practices

✅ **DO:**
- Store all secrets in environment variables
- Use .NET User Secrets for development
- Use secrets management systems in production (AWS Secrets Manager, Azure Key Vault, Render environment variables)
- Rotate credentials regularly
- Use strong, randomly generated passwords
- Enable secret scanning in GitHub repository settings

❌ **DON'T:**
- Commit `.env` files
- Store passwords in `appsettings.json`
- Use the same credentials across environments
- Share credentials via email or chat
- Use simple/predictable passwords

## GitHub Security Settings

1. Enable "Secret scanning" in repository Settings → Code security and analysis
2. Configure branch protection rules requiring status checks
3. Enable "Require status checks to pass before merging"
