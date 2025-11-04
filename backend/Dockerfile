# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

# Copy csproj files and restore dependencies
COPY src/GreenSpec.Domain/GreenSpec.Domain.csproj src/GreenSpec.Domain/
COPY src/GreenSpec.Application/GreenSpec.Application.csproj src/GreenSpec.Application/
COPY src/GreenSpec.Infrastructure/GreenSpec.Infrastructure.csproj src/GreenSpec.Infrastructure/
COPY src/GreenSpec.Api/GreenSpec.Api.csproj src/GreenSpec.Api/
RUN dotnet restore src/GreenSpec.Api/GreenSpec.Api.csproj

# Copy everything else and build
COPY src/ src/
WORKDIR /source/src/GreenSpec.Api
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .

# Expose ports
EXPOSE 8080
EXPOSE 8081

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080;https://+:8081
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "GreenSpec.Api.dll"]
