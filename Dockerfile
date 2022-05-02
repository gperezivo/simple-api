FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
COPY . /app
WORKDIR /app
RUN dotnet restore


FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV DATABASE__NAME="DockerDatabase"
ENV APIURL="https://pokeapi.co/api/v2/"
ENTRYPOINT dotnet simpleApi.dll