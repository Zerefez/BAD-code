FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["SharedExperiences-MSSQL/SharedExperiences.csproj", "SharedExperiences-MSSQL/"]
RUN dotnet restore "SharedExperiences-MSSQL/SharedExperiences.csproj"
COPY . .
WORKDIR "/src/SharedExperiences-MSSQL"
RUN dotnet build "SharedExperiences.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SharedExperiences.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Create logs directory
RUN mkdir -p logs
# Add curl for healthcheck
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
# Add healthcheck (using root path where Swagger is located)
HEALTHCHECK --interval=30s --timeout=5s --retries=3 CMD curl -f http://localhost:80/ || exit 1
ENTRYPOINT ["dotnet", "SharedExperiences.dll"] 