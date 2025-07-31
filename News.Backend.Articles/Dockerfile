FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["News.Backend.Articles.csproj", "."]
RUN dotnet restore "News.Backend.Articles.csproj"

COPY . .
RUN dotnet build "News.Backend.Articles.csproj" -c Release -o /app/src

FROM build AS publish
RUN dotnet publish "News.Backend.Articles.csproj" -c Release -o /app/publish

FROM base AS final
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "News.Backend.Articles.dll"]