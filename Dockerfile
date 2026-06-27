FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["ClinicBookingSystem.csproj", "."]
RUN dotnet restore --no-cache
COPY . .
RUN dotnet