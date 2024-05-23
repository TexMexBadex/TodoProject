# Use the official .NET 8 SDK image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the csproj and restore as distinct layers
COPY ReminderMicroservice/*.csproj .
RUN dotnet restore

# Copy the remaining source code and build the project
COPY ReminderMicroservice/. .
RUN dotnet publish -c Release -o /app

# Use the official ASP.NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "ReminderMicroservice.dll"]
