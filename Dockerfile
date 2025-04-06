FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
COPY . /app
WORKDIR /app

FROM build AS publish
RUN dotnet publish simple-api.csproj -c Release -o /app/publish --self-contained true  --runtime linux-x64 

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV DATABASE__NAME="DockerDatabase"
ENV APIURL="https://pokeapi.co/api/v2/"
ENTRYPOINT ["./simple-api"]