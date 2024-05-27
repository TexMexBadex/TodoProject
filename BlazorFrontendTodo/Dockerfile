# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
EXPOSE 80

# Set the working directory inside the container
WORKDIR /src

# Copy the project file and restore any dependencies
COPY ["BlazorFrontendTodo/BlazorFrontendTodo.csproj", "BlazorFrontendTodo/"]
RUN dotnet restore "BlazorFrontendTodo/BlazorFrontendTodo.csproj"

# Copy the rest of the application source code
COPY ["BlazorFrontendTodo/", "BlazorFrontendTodo/"]

# Build the Blazor WebAssembly project
WORKDIR "/src/BlazorFrontendTodo"
RUN dotnet build "BlazorFrontendTodo.csproj" -c Release -o /app/build
RUN dotnet publish "BlazorFrontendTodo.csproj" -c Release -o /app/publish

# Use a lightweight web server to serve the Blazor WebAssembly app
FROM nginx:alpine AS runtime

# Copy the published output to the nginx html folder
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html

# Expose the default port
EXPOSE 80

# Start nginx
CMD ["nginx", "-g", "daemon off;"]
