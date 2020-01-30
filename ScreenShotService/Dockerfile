FROM mcr.microsoft.com/dotnet/core/runtime:3.1-nanoserver-1903 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/framework/aspnet:4.8    
RUN powershell -Command Add-WindowsFeature Web-WebSockets    
RUN powershell -Command Set-ExecutionPolicy Bypass -Scope Process -Force; iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))    
RUN powershell -Command choco install googlechrome -y
RUN powershell -Command choco install firefox -y

# choco chrome takes a bit
RUN echo 'Downloading chocolatey...'
SHELL ["powershell" , "-Command Install-PackageProvider -name chocolatey -Force"]
SHELL ["powershell" , "-Command Set-PackageSource -Name chocolatey -Trusted"]
SHELL ["powershell" , "-Command Get-PackageSource"]
SHELL ["powershell" , "-Command Install-Package GoogleChrome -MinimumVersion 74"]
# choco chrome takes a bit

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-nanoserver-1903 AS build
WORKDIR /src
COPY ["ScreenShotService/ScreenShotService.csproj", "ScreenShotService/"]
COPY ["ScreenShotDAL/ScreenShotDAL.csproj", "ScreenShotDAL/"]
RUN dotnet restore "ScreenShotService/ScreenShotService.csproj"
COPY . .
WORKDIR "/src/ScreenShotService"
RUN dotnet build "ScreenShotService.csproj" -c Release -o /app/build

FROM base AS final
WORKDIR /app
COPY --from=build /app/build .

ENTRYPOINT ["dotnet", "ScreenShotService.dll"]