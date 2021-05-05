FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /source

COPY *.sln .
COPY Vehicles/*.csproj ./Vehicles/
RUN dotnet restore "Vehicles/vehicles.csproj"

COPY Vehicles/. ./Vehicles/
WORKDIR /source/Vehicles
RUN dotnet publish -c release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "vehicles.dll"]

