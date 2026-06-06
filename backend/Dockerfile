# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

# Copy solution and project files and restore dependencies
COPY Garimpo.Backend.sln .
COPY src/Api/*.csproj src/Api/
COPY src/Application/*.csproj src/Application/
COPY src/Domain/*.csproj src/Domain/
COPY src/Infrastructure/*.csproj src/Infrastructure/
RUN dotnet restore Garimpo.Backend.sln

# Copy the rest of the source code
COPY src/. ./src/

# Publish the application
WORKDIR /source/src/Api
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Garimpo.Api.dll"]
