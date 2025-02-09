FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY *.csproj .
RUN dotnet restore

COPY . .

RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:9.0

WORKDIR /app
RUN chown nobody /app

COPY --from=build --chown=nobody:root /app .

ENV ASPNETCORE_URLS=http://+:8080
ENV UseWebSocketHandler=true
ENV PROTECTION_DIR="/data"
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV COMPlus_EnableDiagnostics=0

USER nobody

EXPOSE 8080

ENTRYPOINT ["dotnet", "/app/Match.dll"]

HEALTHCHECK --interval=30s --timeout=3s \
    CMD curl -f http://localhost:8080/ || exit 1
