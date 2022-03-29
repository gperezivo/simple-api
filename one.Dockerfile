FROM mcr.microsoft.com/dotnet/sdk:6.0 AS base
EXPOSE 80
EXPOSE 443
EXPOSE 5000

COPY . /app
WORKDIR /app
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish
WORKDIR /app/publish
#COPY --from=publish /app/publish .
ENTRYPOINT dotnet simpleApi.dll