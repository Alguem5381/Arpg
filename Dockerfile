FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
#USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Server/Arpg.Api/Arpg.Api.csproj", "src/Server/Arpg.Api/"]
COPY ["src/Server/Arpg.Infrastructure/Arpg.Infrastructure.csproj", "src/Server/Arpg.Infrastructure/"]
COPY ["src/Shared/Arpg.Primitives/Arpg.Primitives.csproj", "src/Shared/Arpg.Primitives/"]
COPY ["src/Server/Arpg.Core/Arpg.Core.csproj", "src/Server/Arpg.Core/"]
COPY ["src/Shared/Arpg.Contracts/Arpg.Contracts.csproj", "src/Shared/Arpg.Contracts/"]
COPY ["src/Server/Arpg.Application/Arpg.Application.csproj", "src/Server/Arpg.Application/"]
RUN dotnet restore "src/Server/Arpg.Api/Arpg.Api.csproj"
COPY . .
WORKDIR "/src/src/Server/Arpg.Api"
RUN dotnet build "./Arpg.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Arpg.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Arpg.Api.dll"]
