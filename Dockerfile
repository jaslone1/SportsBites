# STAGE 1: Build Angular
FROM node:20 AS angular-build
WORKDIR /app
COPY GameDayParty.Client/package*.json ./
RUN npm install
COPY GameDayParty.Client/ ./
RUN npm run build -- --configuration production

# STAGE 2: Build .NET
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS dotnet-build
WORKDIR /src
COPY ["GameDayParty/GameDayParty.csproj", "GameDayParty/"]
COPY ["GameDayParty.Client/GameDayParty.Client.csproj", "GameDayParty.Client/"]
RUN dotnet restore "GameDayParty/GameDayParty.csproj"
COPY . .
RUN dotnet publish "GameDayParty/GameDayParty.csproj" -c Release -o /app/publish

# STAGE 3: Final Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=dotnet-build /app/publish .
COPY --from=angular-build /app/dist/game-day-party/browser ./wwwroot

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "GameDayParty.dll"]