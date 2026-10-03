FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ServiceRemote.Test/ServiceRemote.Test.csproj", "ServiceRemote.Test/"]
RUN dotnet restore "ServiceRemote.Test/ServiceRemote.Test.csproj"
COPY . .
WORKDIR "/src/ServiceRemote.Test"
RUN dotnet build "./ServiceRemote.Test.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ServiceRemote.Test.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ServiceRemote.Test.dll"]
