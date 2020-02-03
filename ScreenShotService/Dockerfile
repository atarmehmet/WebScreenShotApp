FROM mcr.microsoft.com/windows/servercore:1803 AS base
# Install .NET Core Runtime
USER ContainerAdministrator
RUN powershell -Command \
        $ProgressPreference = 'SilentlyContinue'; \
        [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::Tls12; \ 
        Invoke-WebRequest \
            -UseBasicParsing \
            -Uri https://dot.net/v1/dotnet-install.ps1 \
            -OutFile dotnet-install.ps1; \
        ./dotnet-install.ps1 \
            -InstallDir '/Program Files/dotnet' \
            -Channel 3.1 \
            -Runtime dotnet; \
        Remove-Item -Force dotnet-install.ps1 \
    && setx /M PATH "%PATH%;C:\Program Files\dotnet"

WORKDIR /app
RUN powershell -Command \
        $ProgressPreference = 'SilentlyContinue'; \
        Invoke-WebRequest \
            -UseBasicParsing \
            -Uri http://dl.google.com/chrome/install/latest/chrome_installer.exe \
            -OutFile chrome_installer.exe 
RUN ["c:/app/chrome_installer.exe", "/silent", "/install"]
USER ContainerUser

FROM mcr.microsoft.com/windows/servercore:1803 AS build

# Install .NET Core SDK
RUN powershell -Command \
        $ProgressPreference = 'SilentlyContinue'; \
        [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::Tls12; \ 
        Invoke-WebRequest \
            -UseBasicParsing \
            -Uri https://dot.net/v1/dotnet-install.ps1 \
            -OutFile dotnet-install.ps1; \
        ./dotnet-install.ps1 \
            -InstallDir '/Program Files/dotnet' \
            -Channel 3.1; \
        Remove-Item -Force dotnet-install.ps1 \
    && setx /M PATH "%PATH%;C:\Program Files\dotnet"

WORKDIR /src
COPY ["ScreenShotService/ScreenShotService.csproj", "ScreenShotService/"]
COPY ["ScreenShotDAL/ScreenShotDAL.csproj", "ScreenShotDAL/"]
RUN dotnet restore "ScreenShotService/ScreenShotService.csproj"
COPY . .
WORKDIR "/src/ScreenShotService"
RUN dotnet build "ScreenShotService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ScreenShotService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ScreenShotService.dll"]