FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY . .
RUN dotnet restore

WORKDIR /app/Deeproxio.Asset.API

RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim
WORKDIR /app
COPY --from=build-env /app/Deeproxio.Asset.API/out .

EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "Deeproxio.Asset.API.dll"]