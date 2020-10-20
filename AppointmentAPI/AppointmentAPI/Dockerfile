#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["AppointmentAPI/AppointmentAPI.csproj", "AppointmentAPI/"]
RUN dotnet restore "AppointmentAPI/AppointmentAPI.csproj"
COPY . .
WORKDIR "/src/AppointmentAPI"
RUN dotnet build "AppointmentAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AppointmentAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AppointmentAPI.dll"]