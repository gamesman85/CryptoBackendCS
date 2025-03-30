FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["CryptoBackend/CryptoBackend.csproj", "CryptoBackend/"]
RUN dotnet restore "CryptoBackend/CryptoBackend.csproj"

# Copy all files and build
COPY . .
WORKDIR "/src/CryptoBackend"
RUN dotnet build "CryptoBackend.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "CryptoBackend.csproj" -c Release -o /app/publish

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 5277
ENTRYPOINT ["dotnet", "CryptoBackend.dll"]