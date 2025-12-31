# STAGE 1: Build Angular Frontend
FROM node:20 AS angular-build
WORKDIR /app
COPY SportsBitesUI/package*.json ./
RUN npm install
COPY SportsBitesUI/ ./
RUN npm run build -- --configuration production

# STAGE 2: Build .NET Backend
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS dotnet-build
WORKDIR /src
COPY ["GameDayParty/GameDayParty.csproj", "GameDayParty/"]
RUN dotnet restore "GameDayParty/GameDayParty.csproj"
COPY . .
RUN dotnet publish "GameDayParty/GameDayParty.csproj" -c Release -o /app/publish

# STAGE 3: Final Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# 1. Copy the .NET API files first
COPY --from=dotnet-build /app/publish .

# 2. Copy the Angular files into the wwwroot folder
# Using the path confirmed by your logs: /app/dist/SportsBitesUI
COPY --from=angular-build /app/dist/SportsBitesUI ./wwwroot

# 3. DEBUG: This will now work because it's in the final stage
RUN ls -R ./wwwroot

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "GameDayParty.dll"]