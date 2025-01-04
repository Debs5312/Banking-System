FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /src
COPY ./UserManagementSystem/UserManagementSystem.csproj UserManagementSystem/
COPY ./Persistance/Persistance.csproj Persistance/
RUN dotnet restore UserManagementSystem/UserManagementSystem.csproj
COPY . .
WORKDIR /src/UserManagementSystem
RUN dotnet build "UserManagementSystem.csproj" -c Release -o /app/build

FROM build-env AS publish
RUN dotnet publish "UserManagementSystem.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserManagementSystem.dll"]
