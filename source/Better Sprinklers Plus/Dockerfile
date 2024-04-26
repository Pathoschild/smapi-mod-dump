FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine as base

RUN apk add --no-cache p7zip jq

FROM base as zipper
ARG PASSWORD="unset"
WORKDIR /app

COPY ./stardew/ ./stardew/

RUN 7z a stardew.7z ./stardew/* -p"${PASSWORD}"
RUN rm -rf ./stardew

FROM base as builder
COPY --from=zipper /app/stardew.7z /app/stardew.7z
