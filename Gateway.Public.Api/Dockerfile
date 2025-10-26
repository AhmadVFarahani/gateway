# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.
#docker build . -t 10.180.5.12:5000/dev/mehr-sepand-apigateway-api:v0.0.1 -f .\Gateway.Public.Api\Dockerfile

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Gateway.Public.Api/Gateway.Public.Api.csproj", "Gateway.Public.Api/"]
COPY ["Gateway.Application/Gateway.Application.csproj", "Gateway.Application/"]
COPY ["Gateway.Domain/Gateway.Domain.csproj", "Gateway.Domain/"]
COPY ["Gateway.Infrastructure/Gateway.Infrastructure.csproj", "Gateway.Infrastructure/"]
COPY ["Gateway.Persistence/Gateway.Persistence.csproj", "Gateway.Persistence/"]
RUN dotnet restore "./Gateway.Public.Api/Gateway.Public.Api.csproj"
COPY . .
WORKDIR "/src/Gateway.Public.Api"
RUN dotnet build "./Gateway.Public.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Gateway.Public.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Gateway.Public.Api.dll"]