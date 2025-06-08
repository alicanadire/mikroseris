FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/ApiGateway/ToyStore.ApiGateway/ToyStore.ApiGateway.csproj", "src/ApiGateway/ToyStore.ApiGateway/"]
COPY ["src/Shared/ToyStore.Shared/ToyStore.Shared.csproj", "src/Shared/ToyStore.Shared/"]
RUN dotnet restore "src/ApiGateway/ToyStore.ApiGateway/ToyStore.ApiGateway.csproj"
COPY . .
WORKDIR "/src/src/ApiGateway/ToyStore.ApiGateway"
RUN dotnet build "ToyStore.ApiGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ToyStore.ApiGateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ToyStore.ApiGateway.dll"]
