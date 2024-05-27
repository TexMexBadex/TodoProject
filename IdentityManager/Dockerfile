# Use the official ASP.NET Core runtime as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Use the SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["IdentityManager/IdentityManager.csproj", "IdentityManager/"]
RUN dotnet restore "IdentityManager/IdentityManager.csproj"
COPY . .
WORKDIR "/src/IdentityManager"
RUN dotnet build "IdentityManager.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "IdentityManager.csproj" -c Release -o /app/publish

# Use the runtime image again and copy the published files
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IdentityManager.dll"]
