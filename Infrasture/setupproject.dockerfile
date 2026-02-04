FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ./Common/Common.csproj Common/
COPY ./Fia.API/Fia.API.csproj Fia.Api/
COPY ./AspnetCoreRestApi/AspnetCoreRestApi.csproj AspnetCoreRestApi/
RUN dotnet restore "AspnetCoreRestApi"
COPY . .
WORKDIR "/src/AspnetCoreRestApi"
RUN dotnet build "AspnetCoreRestApi.csproj" -c $BUILD_CONFIGURATION -o /app/build
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AspnetCoreRestApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AspnetCoreRestApi.dll"]