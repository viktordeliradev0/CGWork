FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копираме .csproj файла и възстановяваме пакетите
COPY ["CGWork.csproj", "./"]
RUN dotnet restore "CGWork.csproj"

# Копираме целия проект и го компилираме
COPY . .
RUN dotnet build "CGWork.csproj" -c Release -o /app/build
RUN dotnet publish "CGWork.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "CGWork.dll"]