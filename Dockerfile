# Build stage: compiles the app and runs the Tailwind CSS build (needs Node)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

RUN curl -fsSL https://deb.nodesource.com/setup_20.x | bash - \
    && apt-get install -y --no-install-recommends nodejs \
    && rm -rf /var/lib/apt/lists/*

COPY . .
RUN dotnet publish StudySpeech.csproj -c Release -o /app/publish

# Runtime stage: just the published output, no SDK/Node
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

ENTRYPOINT ["sh", "-c", "dotnet StudySpeech.dll --urls http://+:${PORT:-8080}"]
