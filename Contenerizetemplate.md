# Use official .NET 8.0 SDK to build the app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy project files

COPY _.sln .
COPY {project}/_.csproj ./{project}/
COPY {project}.Tests/\*.csproj ./{project}.Tests/

# Restore dependencies

RUN dotnet restore

# Copy everything and build

COPY . .
WORKDIR /app/{project}
RUN dotnet publish -c Release -o /out

# Runtime image

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .

ENTRYPOINT ["dotnet", "{project}.dll"]
