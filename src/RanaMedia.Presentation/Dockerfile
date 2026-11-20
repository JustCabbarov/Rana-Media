FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Restore dependencies using project files only for better layer caching
COPY src/RanaMedia.Domain/RanaMedia.Domain.csproj src/RanaMedia.Domain/
COPY src/RanaMedia.Contract/RanaMedia.Contract.csproj src/RanaMedia.Contract/
COPY src/RanaMedia.Persistence/RanaMedia.Persistence.csproj src/RanaMedia.Persistence/
COPY src/RanaMedia.Application/RanaMedia.Application.csproj src/RanaMedia.Application/
COPY src/RanaMedia.Presentation/RanaMedia.Presentation.csproj src/RanaMedia.Presentation/

RUN dotnet restore src/RanaMedia.Presentation/RanaMedia.Presentation.csproj

# Copy source and publish
COPY src/ src/
RUN dotnet publish src/RanaMedia.Presentation/RanaMedia.Presentation.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .
RUN mkdir -p /app/wwwroot/uploads /app/Logs

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "RanaMedia.Presentation.dll"]
