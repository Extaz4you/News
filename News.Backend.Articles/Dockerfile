# Этап сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 as build
WORKDIR /src
# Копируем и восстанавливаем зависимости
COPY *.csproj .
RUN dotnet restore
# Копируем и публикуем
COPY . .
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
EXPOSE 80

COPY --from=build /src/out .
ENTRYPOINT ["dotnet", "News.Backend.Articles.dll"]