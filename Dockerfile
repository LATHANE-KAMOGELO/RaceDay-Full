FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG PROJECT=RaceDay.API
WORKDIR /src
COPY . .
RUN dotnet publish "${PROJECT}/${PROJECT}.csproj" --configuration Release --output /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ARG APP_DLL=RaceDay.API.dll
ENV APP_DLL=${APP_DLL}
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
USER $APP_UID
ENTRYPOINT ["sh", "-c", "dotnet \"$APP_DLL\""]
