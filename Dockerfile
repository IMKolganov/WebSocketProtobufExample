# Укажите базовый образ
FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
# Установите рабочую директорию
WORKDIR /app

# Скопируйте файлы проекта в рабочую директорию
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["WebSocketProtobufExample.csproj", "./"]
RUN dotnet restore "WebSocketProtobufExample.csproj"
COPY . .
WORKDIR "/src/"

# Выполните сборку проекта
RUN dotnet build "WebSocketProtobufExample.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebSocketProtobufExample.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Укажите запуск приложения
ENTRYPOINT ["dotnet", "WebSocketProtobufExample.dll"]
