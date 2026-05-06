# ============================================================
# Stage 1: Build
# Use the full SDK image to restore, build and publish
# ============================================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy solution and project files first (better layer caching)
# Docker will re-run restore only if .csproj files change
COPY TaskOps.sln .
COPY src/TaskOps.Domain/TaskOps.Domain.csproj          src/TaskOps.Domain/
COPY src/TaskOps.Application/TaskOps.Application.csproj src/TaskOps.Application/
COPY src/TaskOps.Infrastructure/TaskOps.Infrastructure.csproj src/TaskOps.Infrastructure/
COPY src/TaskOps.API/TaskOps.API.csproj                 src/TaskOps.API/

# Restore all NuGet packages
RUN dotnet restore

# Copy the rest of the source code
COPY src/ src/

# Publish in Release mode to /app/publish
RUN dotnet publish src/TaskOps.API/TaskOps.API.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish

# ============================================================
# Stage 2: Runtime
# Use the smaller ASP.NET runtime image (no SDK tools)
# ============================================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Create a non-root user for security
RUN addgroup --system --gid 1001 appgroup && \
    adduser --system --uid 1001 --ingroup appgroup appuser

# Copy published output from build stage
COPY --from=build /app/publish .

# Switch to non-root user
USER appuser

# Expose HTTP port
EXPOSE 8080

# Set ASP.NET Core to listen on port 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "TaskOps.API.dll"]