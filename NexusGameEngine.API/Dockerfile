# STADIO BASE
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base

USER $APP_UID

WORKDIR /app

EXPOSE 8080
EXPOSE 8081


# STADIO BUILD
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

ARG BUILD_CONFIGURATION=Release

WORKDIR /src

COPY ["NexusGameEngine.API/NexusGameEngine.API.csproj", "NexusGameEngine.API/"]
COPY ["NexusGameEngine.Application/NexusGameEngine.Application.csproj", "NexusGameEngine.Application/"]
COPY ["NexusGameEngine.Domain/NexusGameEngine.Domain.csproj", "NexusGameEngine.Domain/"]
COPY ["NexusGameEngine.Infrastructure/NexusGameEngine.Infrastructure.csproj", "NexusGameEngine.Infrastructure/"]

RUN dotnet restore "./NexusGameEngine.API/NexusGameEngine.API.csproj"

COPY . .

WORKDIR "/src/NexusGameEngine.API"

RUN dotnet build "./NexusGameEngine.API.csproj" -c ${BUILD_CONFIGURATION} -o /app/build


# STADIO PUBLISH
FROM build AS publish

ARG BUILD_CONFIGURATION=Release

RUN dotnet publish "./NexusGameEngine.API.csproj" -c ${BUILD_CONFIGURATION} -o /app/publish /p:UseAppHost=false



# STADIO FINAL
FROM base AS final

WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT [ "dotnet", "NexusGameEngine.API.dll" ]