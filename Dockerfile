FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj files and restore
COPY Flower.Backend/Flower.Backend.csproj ./Flower.Backend/
COPY Flower.Data/Flower.Data.csproj ./Flower.Data/
RUN dotnet restore ./Flower.Backend/Flower.Backend.csproj

# Copy all source and publish
COPY Flower.Backend/ ./Flower.Backend/
COPY Flower.Data/ ./Flower.Data/
RUN dotnet publish ./Flower.Backend/Flower.Backend.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out ./

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_USE_POLLING_FILE_WATCHER=true
EXPOSE 8080

ENTRYPOINT ["dotnet", "Flower.Backend.dll"]
