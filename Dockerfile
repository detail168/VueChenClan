# =============================================================================
# BulkyBook Multi-Stage Docker Build
# Stage 1: Build .NET Backend
# Stage 2: Build Vue Frontend
# Stage 3: Runtime (Combined)
# =============================================================================

# ============= STAGE 1: Build Backend (.NET 8) =============
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS backend-builder

WORKDIR /src

# Copy project files
COPY ["BulkyWeb/BulkyBookWeb.csproj", "BulkyWeb/"]
COPY ["Bulky.DataAccess/BulkyBook.DataAccess.csproj", "Bulky.DataAccess/"]
COPY ["Bulky.Models/BulkyBook.Models.csproj", "Bulky.Models/"]
COPY ["Bulky.Utility/BulkyBook.Utility.csproj", "Bulky.Utility/"]

# Restore dependencies
RUN dotnet restore "BulkyWeb/BulkyBookWeb.csproj"

# Copy source code
COPY . .

# Build application
WORKDIR "/src/BulkyWeb"
RUN dotnet build "BulkyBookWeb.csproj" -c Release -o /app/build

# Publish application
RUN dotnet publish "BulkyBookWeb.csproj" -c Release -o /app/publish

# ============= STAGE 2: Build Frontend (Vue 3) =============
FROM node:18-alpine AS frontend-builder

WORKDIR /app

# Copy Vue project
COPY BulkyVue/package*.json ./

# Install dependencies
RUN npm ci

# Copy source
COPY BulkyVue .

# Build production bundle
RUN npm run build

# ============= STAGE 3: Runtime (.NET + Vue) =============
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Set working directory
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published .NET application from backend builder
COPY --from=backend-builder /app/publish .

# Copy Vue build output to wwwroot
COPY --from=frontend-builder /app/dist ./wwwroot/app

# Expose ports
EXPOSE 80 443

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=40s --retries=3 \
  CMD curl -f http://localhost/health || exit 1

# Set entrypoint
ENTRYPOINT ["dotnet", "BulkyBookWeb.dll"]
