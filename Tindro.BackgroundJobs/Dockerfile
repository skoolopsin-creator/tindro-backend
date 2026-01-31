FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore Tindro.Backend.sln
RUN dotnet publish Tindro.BackgroundJobs/Tindro.BackgroundJobs.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
RUN [ -f Tindro.BackgroundJobs.runtimeconfig.json ] || printf '{"runtimeOptions":{"tfm":"net8.0","framework":{"name":"Microsoft.NETCore.App","version":"8.0.0"}}}' > Tindro.BackgroundJobs.runtimeconfig.json

ENTRYPOINT ["dotnet", "Tindro.BackgroundJobs.dll"]
