FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Services/Identity/ToyStore.IdentityService/ToyStore.IdentityService.csproj", "src/Services/Identity/ToyStore.IdentityService/"]
COPY ["src/Shared/ToyStore.Shared/ToyStore.Shared.csproj", "src/Shared/ToyStore.Shared/"]
COPY ["src/Shared/ToyStore.EventBus/ToyStore.EventBus.csproj", "src/Shared/ToyStore.EventBus/"]
RUN dotnet restore "src/Services/Identity/ToyStore.IdentityService/ToyStore.IdentityService.csproj"
COPY . .
WORKDIR "/src/src/Services/Identity/ToyStore.IdentityService"
RUN dotnet build "ToyStore.IdentityService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ToyStore.IdentityService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ToyStore.IdentityService.dll"]
