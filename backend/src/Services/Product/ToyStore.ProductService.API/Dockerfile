# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/Services/Product/ToyStore.ProductService.API/ToyStore.ProductService.API.csproj", "src/Services/Product/ToyStore.ProductService.API/"]
COPY ["src/Services/Product/ToyStore.ProductService.Application/ToyStore.ProductService.Application.csproj", "src/Services/Product/ToyStore.ProductService.Application/"]
COPY ["src/Services/Product/ToyStore.ProductService.Domain/ToyStore.ProductService.Domain.csproj", "src/Services/Product/ToyStore.ProductService.Domain/"]
COPY ["src/Services/Product/ToyStore.ProductService.Infrastructure/ToyStore.ProductService.Infrastructure.csproj", "src/Services/Product/ToyStore.ProductService.Infrastructure/"]
COPY ["src/Shared/ToyStore.Shared/ToyStore.Shared.csproj", "src/Shared/ToyStore.Shared/"]
COPY ["src/Shared/ToyStore.EventBus/ToyStore.EventBus.csproj", "src/Shared/ToyStore.EventBus/"]

# Restore dependencies
RUN dotnet restore "src/Services/Product/ToyStore.ProductService.API/ToyStore.ProductService.API.csproj"

# Copy source code
COPY . .

# Build the application
WORKDIR "/src/src/Services/Product/ToyStore.ProductService.API"
RUN dotnet build "ToyStore.ProductService.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "ToyStore.ProductService.API.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 80
ENTRYPOINT ["dotnet", "ToyStore.ProductService.API.dll"]
