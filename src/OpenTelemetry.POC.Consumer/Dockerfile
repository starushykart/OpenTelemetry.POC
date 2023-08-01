FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/OpenTelemetry.POC.Consumer/OpenTelemetry.POC.Consumer.csproj", "src/OpenTelemetry.POC.Consumer/"]
RUN dotnet restore "src/OpenTelemetry.POC.Consumer/OpenTelemetry.POC.Consumer.csproj"
COPY . .
WORKDIR "/src/src/OpenTelemetry.POC.Consumer"
RUN dotnet build "OpenTelemetry.POC.Consumer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OpenTelemetry.POC.Consumer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OpenTelemetry.POC.Consumer.dll"]
