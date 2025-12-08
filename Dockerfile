FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and restore dependencies
COPY MoralSupport.Authentication/ ./MoralSupport.Authentication/
WORKDIR /src/MoralSupport.Authentication
RUN dotnet restore MoralSupport.Authentication.sln

# Publish the web project
RUN dotnet publish MoralSupport.Authentication.Web/MoralSupport.Authentication.Web.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

ENTRYPOINT ["dotnet", "MoralSupport.Authentication.Web.dll"]
