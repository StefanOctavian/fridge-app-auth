# Use the official .NET SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Set the working directory inside the container
WORKDIR /app

# Copy the project files
COPY . ./

# Install the required dependencies from .csproj
RUN dotnet restore

# Build the application for release
RUN dotnet publish -c Release -o /out

# Use the official .NET runtime image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

# Set the working directory inside the container
WORKDIR /app

# Copy the published files from the build stage
COPY --from=build /out .

# Expose the port the application runs on
EXPOSE 80

# Set the entry point for the container
ENTRYPOINT ["dotnet", "Auth.dll"]