FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files for layer caching
COPY BookstoreApi.sln ./
COPY src/BookstoreApi/BookstoreApi.csproj src/BookstoreApi/
COPY tests/BookstoreApi.Tests/BookstoreApi.Tests.csproj tests/BookstoreApi.Tests/
RUN dotnet restore

# Copy all source and publish
COPY . .
RUN dotnet publish src/BookstoreApi/BookstoreApi.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "BookstoreApi.dll"]
