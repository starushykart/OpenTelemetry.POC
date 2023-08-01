FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["OpenTelemetry.POC.Api/OpenTelemetry.POC.Api.csproj", "OpenTelemetry.POC.Api/"]
RUN dotnet restore "OpenTelemetry.POC.Api/OpenTelemetry.POC.Api.csproj"
COPY . .
WORKDIR "/src/OpenTelemetry.POC.Api"
RUN dotnet build "OpenTelemetry.POC.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OpenTelemetry.POC.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OpenTelemetry.POC.Api.dll"]
