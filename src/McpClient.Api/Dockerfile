FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/McpClient.Api/McpClient.Api.csproj", "src/McpClient.Api/"]
COPY ["src/Shared.Contracts/Shared.Contracts.csproj", "src/Shared.Contracts/"]
COPY ["src/Shared.Infrastructure/Shared.Infrastructure.csproj", "src/Shared.Infrastructure/"]
RUN dotnet restore "./src/McpClient.Api/McpClient.Api.csproj"
COPY . .
WORKDIR "/src/src/McpClient.Api"
RUN dotnet build "./McpClient.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./McpClient.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "McpClient.Api.dll"]
