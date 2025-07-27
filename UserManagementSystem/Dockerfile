# Stage 1: Base runtime image. Using a specific version is recommended over 'latest'.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
# .NET 8+ apps listen on port 8080 by default inside the container.
EXPOSE 8080
# Set the listening URL to match the exposed port.
ENV ASPNETCORE_URLS=http://+:8080

# Stage 2: Build environment
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files first to leverage Docker layer caching.
# This step only re-runs if the project dependencies change.
COPY ./UserManagementSystem/UserManagementSystem.csproj UserManagementSystem/
COPY ./Persistance/Persistance.csproj Persistance/
RUN dotnet restore UserManagementSystem/UserManagementSystem.csproj

# Copy the rest of the source code. The .dockerignore file prevents
# unnecessary files from being copied.
COPY . .
WORKDIR "/src/UserManagementSystem"

# UseAppHost=false is an optimization for framework-dependent deployments.
RUN dotnet publish "UserManagementSystem.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Final, lean image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
# Switch to the non-root 'app' user for enhanced security
USER app
ENTRYPOINT ["dotnet", "UserManagementSystem.dll"]
