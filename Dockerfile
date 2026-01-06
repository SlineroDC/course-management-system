# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY backend/Domain/Domain.csproj backend/Domain/
COPY backend/Application/Application.csproj backend/Application/
COPY backend/Infrastructure/Infrastructure.csproj backend/Infrastructure/
COPY backend/API/API.csproj backend/API/
RUN dotnet restore backend/API/API.csproj

# Copy everything else and build
COPY backend/ backend/
WORKDIR /src/backend/API
RUN dotnet build -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create health check script
RUN echo "#!/bin/sh" > /app/healthcheck.sh && \
    echo "curl -f http://localhost:5070/api/courses || exit 1" >> /app/healthcheck.sh && \
    chmod +x /app/healthcheck.sh

# Expose port
EXPOSE 5070

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=40s --retries=3 \
    CMD /app/healthcheck.sh || exit 1

ENTRYPOINT ["dotnet", "API.dll"]
