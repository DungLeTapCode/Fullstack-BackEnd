# Sử dụng image cơ bản của .NET SDK
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Sử dụng .NET SDK để build ứng dụng
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["API_BackEnd/API_BackEnd.csproj", "API_BackEnd/"]
RUN dotnet restore "API_BackEnd/API_BackEnd.csproj"

# Copy mã nguồn vào image và build
COPY . .
WORKDIR "/src/API_BackEnd"
RUN dotnet build "API_BackEnd.csproj" -c Release -o /app/build

# Publish ứng dụng
RUN dotnet publish "API_BackEnd.csproj" -c Release -o /app/publish

# Chạy ứng dụng
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "API_BackEnd.dll"]
