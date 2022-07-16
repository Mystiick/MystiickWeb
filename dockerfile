FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore
RUN dotnet build -c release -o /bin/ --no-restore

RUN dotnet publish -c release -o /app/

FROM base as final

WORKDIR /app
RUN groupadd -r dotnet && useradd -r -g dotnet dotnet
RUN chmod -R a+rwx /app/

COPY --from=build /app .

ENV ASPNETCORE_URLS=http://*:80
# ENV ASPNETCORE_URLS=https://*:443
EXPOSE 80
EXPOSE 443

USER dotnet
ENTRYPOINT [ "dotnet", "MystiickWeb.Server.dll" ]
