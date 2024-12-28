FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /src
COPY ["AccountManagementSystem/AccountManagementSystem.csproj", "AccountManagementSystem/"]
COPY ["Persistance/Persistance.csproj", "Persistance/"]
# COPY ["Model/Model.csproj", "Model/"]
RUN dotnet restore "AccountManagementSystem/AccountManagementSystem.csproj"
COPY . .
WORKDIR "/src/AccountManagementSystem"
RUN dotnet build "AccountManagementSystem.csproj" -c Release -o /app/build

FROM build-env AS publish
RUN dotnet publish "AccountManagementSystem.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AccountManagementSystem.dll"]
