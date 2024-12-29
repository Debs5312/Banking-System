FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /src
COPY ./AdharManagementSystem/AdharManagementSystem.csproj AdharManagementSystem/
COPY ./Persistance/Persistance.csproj Persistance/
RUN dotnet restore AdharManagementSystem/AdharManagementSystem.csproj
COPY . .
WORKDIR /src/AdharManagementSystem
RUN dotnet build "AdharManagementSystem.csproj" -c Release -o /app/build

FROM build-env AS publish
RUN dotnet publish "AdharManagementSystem.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AdharManagementSystem.dll"]
