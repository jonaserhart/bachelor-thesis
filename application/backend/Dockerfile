FROM mcr.microsoft.com/dotnet/sdk:7.0.401-bullseye-slim AS build
WORKDIR /source

COPY ./src .
RUN dotnet publish -c Release -o /app .

FROM mcr.microsoft.com/dotnet/aspnet:7.0.11-bullseye-slim
WORKDIR /app

ENV ASPNETCORE_URLS="http://+"

COPY --from=build /app .
ENTRYPOINT ["dotnet", "backend.dll"]