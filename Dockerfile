FROM microsoft/dotnet:2.1-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY MassTransitDemo/MassTransitDemo.csproj MassTransitDemo/
RUN dotnet restore MassTransitDemo/MassTransitDemo.csproj
COPY . .
WORKDIR /src/MassTransitDemo
RUN dotnet build MassTransitDemo.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish MassTransitDemo.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "MassTransitDemo.dll"]
