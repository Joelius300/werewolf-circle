# Auto-generated via Add -> Docker Support
# See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
# EXPOSE 80 Exposing this port in the docker compose file instead

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["WerewolfCircle/WerewolfCircle.csproj", "WerewolfCircle/"]
COPY ["WerewolfCircle.Data/WerewolfCircle.Data.csproj", "WerewolfCircle.Data/"]
RUN dotnet restore "WerewolfCircle/WerewolfCircle.csproj"
COPY . .
WORKDIR "/src/WerewolfCircle"
RUN dotnet build "WerewolfCircle.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WerewolfCircle.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WerewolfCircle.dll"]
