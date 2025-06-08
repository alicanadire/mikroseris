# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/Services/Inventory/ToyStore.InventoryService/ToyStore.InventoryService.csproj", "src/Services/Inventory/ToyStore.InventoryService/"]
COPY ["src/Shared/ToyStore.Shared/ToyStore.Shared.csproj", "src/Shared/ToyStore.Shared/"]
COPY ["src/Shared/ToyStore.EventBus/ToyStore.EventBus.csproj", "src/Shared/ToyStore.EventBus/"]

# Restore dependencies
RUN dotnet restore "src/Services/Inventory/ToyStore.InventoryService/ToyStore.InventoryService.csproj"

# Copy source code
COPY . .

# Build the application
WORKDIR "/src/src/Services/Inventory/ToyStore.InventoryService"
RUN dotnet build "ToyStore.InventoryService.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "ToyStore.InventoryService.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 80
ENTRYPOINT ["dotnet", "ToyStore.InventoryService.dll"]
