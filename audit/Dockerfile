FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["audit/audit.csproj", "audit/"]
RUN dotnet restore "audit/audit.csproj"
COPY . .
WORKDIR "/src/audit"
RUN dotnet build "audit.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "audit.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "audit.dll"]