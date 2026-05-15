# Consulte https://aka.ms/customizecontainer para aprender a personalizar su contenedor de depuración y cómo Visual Studio usa este Dockerfile para compilar sus imágenes para una depuración más rápida.

# Esta fase se usa cuando se ejecuta desde VS en modo rápido (valor predeterminado para la configuración de depuración)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# 1. Instalamos las fuentes como ROOT antes de cambiar de usuario
USER root
RUN apt-get update && \
    echo "ttf-mscorefonts-installer msttcorefontconfig/accepted-mscorefonts-eula select true" | debconf-set-selections && \
    apt-get install -y --no-install-recommends fontconfig ttf-mscorefonts-installer && \
    apt-get clean && \
    rm -rf /var/lib/lists/*
RUN fc-cache -f -v

# 2. Volvemos al usuario seguro de .NET para la ejecución
USER $APP_UID
# Esta fase se usa para compilar el proyecto de servicio
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["PadelApp/PadelApp.csproj", "PadelApp/"]
RUN dotnet restore "./PadelApp/PadelApp.csproj"
COPY . .
WORKDIR "/src/PadelApp"
RUN dotnet build "./PadelApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Esta fase se usa para publicar el proyecto de servicio que se copiará en la fase final.
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./PadelApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Esta fase se usa en producción o cuando se ejecuta desde VS en modo normal (valor predeterminado cuando no se usa la configuración de depuración)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PadelApp.dll"]