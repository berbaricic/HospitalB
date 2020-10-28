#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["HangfireClientService/HangfireClientService.csproj", "HangfireClientService/"]
RUN dotnet restore "HangfireClientService/HangfireClientService.csproj"
COPY . .
WORKDIR "/src/HangfireClientService"
RUN dotnet build "HangfireClientService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HangfireClientService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HangfireClientService.dll"]