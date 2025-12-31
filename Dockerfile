# STAGE 1: Build Angular Frontend (SportsBitesUI)
FROM node:20 AS angular-build
WORKDIR /app

# Copy the package files from the correct folder
COPY SportsBitesUI/package*.json ./
RUN npm install

# Copy the rest of the Angular source and build
COPY SportsBitesUI/ ./
RUN npm run build -- --configuration production

# STAGE 2: Build .NET
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS dotnet-build
WORKDIR /src
COPY ["GameDayParty/GameDayParty.csproj", "GameDayParty/"]
COPY ["GameDayParty.Client/GameDayParty.Client.csproj", "GameDayParty.Client/"]
RUN dotnet restore "GameDayParty/GameDayParty.csproj"
COPY . .
RUN dotnet publish "GameDayParty/GameDayParty.csproj" -c Release -o /app/publish

# remove after this
RUN find ./wwwroot -name "index.html"

# STAGE 3: Final Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=dotnet-build /app/publish .
COPY --from=angular-build /app/dist/SportsBitesUI/browser ./wwwroot

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "GameDayParty.dll"]