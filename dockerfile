# Etapa de construcci�n
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar solo el archivo de soluci�n y restaurar las dependencias
COPY sgcv-backend.sln .
COPY sgcv-backend.csproj .
RUN dotnet restore

# Copiar el resto de los archivos y compilar la aplicaci�n
COPY . .
RUN dotnet publish -c Release -o out

# Etapa de ejecuci�n
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

EXPOSE 80

ENV ASPNETCORE_URLS=http://+:80
ENV APP_NAME=sgcv-backend

ENTRYPOINT ["dotnet", "sgcv-backend.dll"]
