FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

COPY *.csproj ./
RUN dotnet restore

COPY . .

# Install EF tools
#RUN dotnet tool install --global dotnet-ef
#ENV PATH="$PATH:/root/.dotnet/tools"

COPY app.db ./app.db

RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# EXPOSE port and start the app
# Set environment variables for Dynatrace here or in docker-compose
#ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "Ascendion.Products.Dashboard.Api.dll","--urls", "http://0.0.0.0:80"]
