# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY WikiArea.sln ./
COPY src/WikiArea.Core/WikiArea.Core.csproj src/WikiArea.Core/
COPY src/WikiArea.Application/WikiArea.Application.csproj src/WikiArea.Application/
COPY src/WikiArea.Infrastructure/WikiArea.Infrastructure.csproj src/WikiArea.Infrastructure/
COPY src/WikiArea.Web/WikiArea.Web.csproj src/WikiArea.Web/

# Restore dependencies
RUN dotnet restore

# Copy source code
COPY src/ src/

# Build the application
WORKDIR /src/src/WikiArea.Web
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published application
COPY --from=build /app/publish .

# Create logs directory
RUN mkdir -p /app/logs

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80

EXPOSE 80

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
  CMD curl -f http://localhost:80/health || exit 1

ENTRYPOINT ["dotnet", "WikiArea.Web.dll"] 