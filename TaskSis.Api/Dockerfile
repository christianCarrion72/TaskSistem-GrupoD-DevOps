FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["TaskSis.Api.csproj", "./"]
RUN dotnet restore "TaskSis.Api.csproj"

COPY . .
RUN dotnet publish "TaskSis.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5255
ENV ASPNETCORE_URLS=http://+:5255
ENV ASPNETCORE_ENVIRONMENT=Development

ENTRYPOINT ["dotnet", "TaskSis.Api.dll"]
