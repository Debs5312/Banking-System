FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["AccountManagementSystem/AccountManagementSystem.csproj", "AccountManagementSystem/"]
COPY ["Persistance/Persistance.csproj", "Persistance/"]
RUN dotnet restore "AccountManagementSystem/AccountManagementSystem.csproj"
COPY . .
WORKDIR "/src/AccountManagementSystem"
RUN dotnet publish "AccountManagementSystem.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Add a non-root user and switch to it for security
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

ENTRYPOINT ["dotnet", "AccountManagementSystem.dll"]
