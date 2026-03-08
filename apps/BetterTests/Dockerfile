FROM mcr.microsoft.com/dotnet/sdk:10.0.103 AS build

WORKDIR /src

COPY Directory.Build.props .
COPY Directory.Packages.props .
COPY global.json .

COPY apps/BetterTests/BetterTests.csproj apps/BetterTests/
COPY apps/BetterTests/Domain/BetterTests.Domain.csproj apps/BetterTests/Domain/
COPY apps/BetterTests/Application/BetterTests.Application.csproj apps/BetterTests/Application/
COPY apps/BetterTests/Infrastructure/BetterTests.Infrastructure.csproj apps/BetterTests/Infrastructure/

RUN dotnet restore apps/BetterTests/BetterTests.csproj

COPY . .

RUN dotnet publish apps/BetterTests/BetterTests.csproj \
  -c Release \
  -o /app/publish \
  --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app

COPY --from=build /app/publish .

COPY apps/BetterTests/certs ./certs

RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

RUN useradd -m -u 1001 appuser && chown -R appuser:appuser /app
USER appuser

EXPOSE 3000

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_HTTP_PORTS=3000

HEALTHCHECK --interval=30s --timeout=3s --start-period=40s --retries=3 \
  CMD curl -f http://localhost:3000/healthz || exit 1

ENTRYPOINT ["dotnet", "BetterTests.dll"]
