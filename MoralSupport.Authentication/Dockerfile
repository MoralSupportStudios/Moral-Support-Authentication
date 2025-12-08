FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["MoralSupport.Authentication.sln", "."]
COPY ["MoralSupport.Authentication.Web/MoralSupport.Authentication.Web.csproj", "MoralSupport.Authentication.Web/"]
COPY ["MoralSupport.Authentication.Application/MoralSupport.Authentication.Application.csproj", "MoralSupport.Authentication.Application/"]
COPY ["MoralSupport.Authentication.Domain/MoralSupport.Authentication.Domain.csproj", "MoralSupport.Authentication.Domain/"]
COPY ["MoralSupport.Authentication.Infrastructure/MoralSupport.Authentication.Infrastructure.csproj", "MoralSupport.Authentication.Infrastructure/"]
COPY ["MoralSupport.Authentication.Tests/MoralSupport.Authentication.Tests.csproj", "MoralSupport.Authentication.Tests/"]

RUN dotnet restore

COPY . .
WORKDIR "/src/MoralSupport.Authentication.Web"
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MoralSupport.Authentication.Web.dll"]
