# Build from the repository root:
#   docker build -t hotel-manager-booking .

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Restore the web host and its Infrastructure project separately so Docker can
# reuse the restore layer when application source files change.
COPY hotel-management-platform/HotelManagement.Infrastructure/HotelManagement.Infrastructure.csproj hotel-management-platform/HotelManagement.Infrastructure/
COPY hotel-management-platform/HotelManagement.Web/HotelManagement.Web.csproj hotel-management-platform/HotelManagement.Web/
RUN dotnet restore hotel-management-platform/HotelManagement.Web/HotelManagement.Web.csproj

COPY hotel-management-platform/ hotel-management-platform/
WORKDIR /src/hotel-management-platform/HotelManagement.Web
RUN dotnet publish HotelManagement.Web.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_HTTP_PORTS=8080 \
    ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "HotelManagement.Web.dll"]
