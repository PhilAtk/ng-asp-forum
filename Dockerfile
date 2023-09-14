FROM alpine AS build

RUN apk add dotnet7-sdk
RUN apk add npm

WORKDIR /source

COPY *.sln .
COPY *.csproj .
RUN dotnet restore

COPY . .
WORKDIR /source
RUN dotnet publish -c release --property:PublishDir=/app

FROM alpine

RUN apk add aspnetcore7-runtime

WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "ng-asp-forum.dll"]