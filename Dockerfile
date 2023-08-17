FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app
COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o bin
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app/bin .
EXPOSE 80
ENTRYPOINT ["dotnet", "t_g_Minesweeper.WebApi.dll"]