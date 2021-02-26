#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["LMS.IdP/LMS.IdP.csproj", "LMS.IdP/"]
RUN dotnet restore "LMS.IdP/LMS.IdP.csproj"
COPY . .
WORKDIR "/src/LMS.IdP"
RUN dotnet build "LMS.IdP.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LMS.IdP.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LMS.IdP.dll"]