FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копираме проекта от подпапката CGWork (коригирай CGWork, ако папката се казва различно)
COPY ["CGWork/CGWork.csproj", "CGWork/"]
RUN dotnet restore "CGWork/CGWork.csproj"

# Копираме всички останали файлове
COPY . .
WORKDIR "/src/CGWork"
RUN dotnet build "CGWork.csproj" -c Release -o /app/build
RUN dotnet publish "CGWork.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "CGWork.dll"]