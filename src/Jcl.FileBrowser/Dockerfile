ARG USERNAME=filesuser
ARG USERID=1000

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
EXPOSE 80
ARG USERID
ARG USERNAME
ENV Urls http://*:80
ENV Options:BaseFolder /data/files
HEALTHCHECK  --interval=5s --timeout=10s --retries=3 CMD curl --fail http://localhost:80/healthcheck || exit 1
RUN apk --no-cache add curl
RUN adduser --disabled-password --home /app --uid $USERID --gecos '' $USERNAME \
    && mkdir -p /data/files \
    && chown -R $USERNAME /app \
    && chown -R $USERNAME /data/files    

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /src
COPY ["Jcl.FileBrowser.csproj", "."]
RUN dotnet restore "Jcl.FileBrowser.csproj"
COPY . .
RUN dotnet build "Jcl.FileBrowser.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Jcl.FileBrowser.csproj" -c Release -o /app/publish

FROM base AS final
ARG USERNAME
WORKDIR /app
COPY --from=publish /app/publish .
RUN chown -R $USERNAME /app
USER $USERNAME
ENTRYPOINT ["dotnet", "Jcl.FileBrowser.dll"]
