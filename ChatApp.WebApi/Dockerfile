FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["ChatApp.WebApi/ChatApp.WebApi.csproj", "ChatApp.WebApi/"]
COPY ["ChatApp.Application/ChatApp.Application.csproj", "ChatApp.Application/"]
COPY ["ChatApp.Domain/ChatApp.Domain.csproj", "ChatApp.Domain/"]
COPY ["ChatApp.Infrastructure/ChatApp.Infrastructure.csproj", "ChatApp.Infrastructure/"]
RUN dotnet restore "ChatApp.WebApi/ChatApp.WebApi.csproj"
COPY . .
WORKDIR "/src/ChatApp.WebApi"
RUN dotnet build "ChatApp.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ChatApp.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChatApp.WebApi.dll"] 