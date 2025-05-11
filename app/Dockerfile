FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY app.csproj ./
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:9.0 AS final
WORKDIR /app
COPY --from=build /publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "app.dll"]