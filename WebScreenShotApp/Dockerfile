FROM mcr.microsoft.com/dotnet/core/runtime:3.1-nanoserver-1903 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-nanoserver-1903 AS build
WORKDIR /src
COPY ["WebScreenShotApp/ScreenShotConsole.csproj", "WebScreenShotApp/"]
RUN dotnet restore "WebScreenShotApp/ScreenShotConsole.csproj"
COPY . .
WORKDIR "/src/WebScreenShotApp"
RUN dotnet build "ScreenShotConsole.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ScreenShotConsole.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ScreenShotConsole.dll"]