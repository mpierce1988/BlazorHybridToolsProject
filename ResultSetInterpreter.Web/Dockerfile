FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ResultSetInterpreter.Web/ResultSetInterpreter.Web.csproj", "ResultSetInterpreter.Web/"]
RUN dotnet restore "ResultSetInterpreter.Web/ResultSetInterpreter.Web.csproj"
COPY . .
WORKDIR "/src/ResultSetInterpreter.Web"
RUN dotnet build "ResultSetInterpreter.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ResultSetInterpreter.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ResultSetInterpreter.Web.dll"]
