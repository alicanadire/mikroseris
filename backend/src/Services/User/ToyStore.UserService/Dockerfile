# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/Services/User/ToyStore.UserService/ToyStore.UserService.csproj", "src/Services/User/ToyStore.UserService/"]
COPY ["src/Shared/ToyStore.Shared/ToyStore.Shared.csproj", "src/Shared/ToyStore.Shared/"]
COPY ["src/Shared/ToyStore.EventBus/ToyStore.EventBus.csproj", "src/Shared/ToyStore.EventBus/"]

# Restore dependencies
RUN dotnet restore "src/Services/User/ToyStore.UserService/ToyStore.UserService.csproj"

# Copy source code
COPY . .

# Build the application
WORKDIR "/src/src/Services/User/ToyStore.UserService"
RUN dotnet build "ToyStore.UserService.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "ToyStore.UserService.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 80
ENTRYPOINT ["dotnet", "ToyStore.UserService.dll"]
