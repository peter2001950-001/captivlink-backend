#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Captivlink.Api/Captivlink.Api.csproj", "Captivlink.Api/"]
COPY ["Captivlink.Application/Captivlink.Application.csproj", "Captivlink.Application/"]
COPY ["Captivlink.Domain/Captivlink.Infrastructure.csproj", "Captivlink.Domain/"]
RUN dotnet restore "./Captivlink.Api/Captivlink.Api.csproj"
COPY . .
WORKDIR "/src/Captivlink.Api"
RUN dotnet build "./Captivlink.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Captivlink.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Captivlink.Api.dll"]