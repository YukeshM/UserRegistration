# Use the official .NET runtime image for running the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Use the official .NET SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["Services/DatabaseService/DatabaseService/DatabaseService.Api.csproj", "DatabaseService.Api/"]
COPY ["Services/DatabaseService/DatabaseService.Model/DatabaseService.Core.csproj", "DatabaseService.Core/"]
RUN dotnet restore "DatabaseService.Api/DatabaseService.Api.csproj"

# Copy the rest of the code and publish the app
COPY . .
WORKDIR /src/Services/DatabaseService/DatabaseService
RUN dotnet build "DatabaseService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build
RUN dotnet publish "DatabaseService.Api.csproj" -c Release -o /out


FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "DatabaseService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish


# Copy the published files and run the app
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DatabaseService.Api.dll"]