FROM mcr.microsoft.com/dotnet/sdk:6.0 AS builder

ENV GLIBC_REPO=https://github.com/sgerrand/alpine-pkg-glibc
ENV GLIBC_VERSION=2.30-r0

ARG feed='--source "https://api.nuget.org/v3/index.json"'

WORKDIR /src
COPY ["Ntech.Migration/*.csproj", "Ntech.Migration/"]
RUN dotnet restore Ntech.Migration/ /property:Configuration=Release ${feed} -nowarn:msb3202,nu1503

COPY ["Ntech.Migration/.", "Ntech.Migration/"]
RUN dotnet publish Ntech.Migration/ -c Release -o /app

FROM mcr.microsoft.com/dotnet/sdk:6.0
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT docker

COPY --from=builder /app .

