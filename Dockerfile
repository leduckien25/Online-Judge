FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

COPY ["OnlineJudgeSolution.slnx", "."]
COPY ["src/OnlineJudge.Api/OnlineJudge.Api.csproj", "src/OnlineJudge.Api/"]
COPY ["src/OnlineJudge.Core/OnlineJudge.Core.csproj", "src/OnlineJudge.Core/"]
COPY ["src/OnlineJudge.Sandbox/OnlineJudge.Sandbox.csproj", "src/OnlineJudge.Sandbox/"]

RUN dotnet restore "/src/OnlineJudge.Api/OnlineJudge.Api.csproj"

COPY ["src/", "/src/"]

RUN dotnet publish "/src/OnlineJudge.Api/OnlineJudge.Api.csproj" -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN apt-get update && \
    apt-get install -y docker.io && \
    rm -rf /var/lib/apt/lists/*


COPY --from=build /app/out ./

EXPOSE 8080

ENTRYPOINT ["dotnet", "OnlineJudge.Api.dll"]