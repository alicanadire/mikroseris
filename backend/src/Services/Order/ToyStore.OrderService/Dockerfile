# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/Services/Order/ToyStore.OrderService/ToyStore.OrderService.csproj", "src/Services/Order/ToyStore.OrderService/"]
COPY ["src/Shared/ToyStore.Shared/ToyStore.Shared.csproj", "src/Shared/ToyStore.Shared/"]
COPY ["src/Shared/ToyStore.EventBus/ToyStore.EventBus.csproj", "src/Shared/ToyStore.EventBus/"]

# Restore dependencies
RUN dotnet restore "src/Services/Order/ToyStore.OrderService/ToyStore.OrderService.csproj"

# Copy source code
COPY . .

# Build the application
WORKDIR "/src/src/Services/Order/ToyStore.OrderService"
RUN dotnet build "ToyStore.OrderService.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "ToyStore.OrderService.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 80
ENTRYPOINT ["dotnet", "ToyStore.OrderService.dll"]
