FROM mcr.microsoft.com/dotnet/sdk:6.0 

LABEL author="Arnór Moncada"

COPY . /app

WORKDIR /app

RUN dotnet restore

RUN dotnet build

ENTRYPOINT ["dotnet", "run", "--project", "Cryptocop.Software.API/Cryptocop.Software.API.csproj"]