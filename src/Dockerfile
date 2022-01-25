ARG USERNAME=filesuser
ARG USERID=1000

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
EXPOSE 80
ARG USERID
ARG USERNAME
ENV Urls http://*:80
ENV Options:BaseFolder /data/files
HEALTHCHECK  --interval=5s --timeout=10s --retries=3 CMD /healthcheck/HealthCheck.Client || exit 1
RUN adduser --disabled-password --home /app --uid $USERID --gecos '' $USERNAME \
    && mkdir -p /data/files \
    && mkdir -p /app \
    && mkdir -p /healthcheck \
    && chown -R $USERNAME /app \
    && chown -R $USERNAME /healthcheck \
    && chown -R $USERNAME /data/files    

# Jcl: add this first so its layers get cached soon. We won't change this client too often
FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
# install npm
RUN apk add --update npm
WORKDIR /src
# install node dev devepencies
COPY ["Jcl.FileBrowser/package.json", "."]
RUN npm install
# restore nuget packages
COPY ["Jcl.FileBrowser/Jcl.FileBrowser.csproj", "."]
RUN dotnet restore "Jcl.FileBrowser.csproj"
COPY ["Jcl.FileBrowser/",  "."]
RUN dotnet build "Jcl.FileBrowser.csproj" -c Release -o /app/build
RUN dotnet publish "Jcl.FileBrowser.csproj" -c Release -o /app/publish

WORKDIR /src-healthcheck
COPY ["HealthCheck.Client/HealthCheck.Client.csproj", "."]
RUN dotnet restore "HealthCheck.Client.csproj"
COPY ["HealthCheck.Client/",  "."]
RUN dotnet build "HealthCheck.Client.csproj" -c Release -o /app-healthcheck/build
RUN dotnet publish "HealthCheck.Client.csproj" -c Release -o /app-healthcheck/publish --no-self-contained -p:PublishReadyToRun=true -p:PublishSingleFile=false -r linux-musl-x64

FROM base AS final
ARG USERNAME
WORKDIR /healthcheck
COPY --from=build /app-healthcheck/publish .
RUN chown -R $USERNAME /healthcheck
WORKDIR /app
COPY --from=build /app/publish .
RUN chown -R $USERNAME /app
USER $USERNAME
ENTRYPOINT ["dotnet", "Jcl.FileBrowser.dll"]
