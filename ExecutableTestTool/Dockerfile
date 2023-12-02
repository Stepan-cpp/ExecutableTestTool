FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ExecutableTestTool/ExecutableTestTool.csproj", "ExecutableTestTool/"]
RUN dotnet restore "ExecutableTestTool/ExecutableTestTool.csproj"
COPY . .
WORKDIR "/src/ExecutableTestTool"
RUN dotnet build "ExecutableTestTool.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ExecutableTestTool.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ExecutableTestTool.dll"]
