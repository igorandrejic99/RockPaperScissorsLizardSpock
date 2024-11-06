# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy everything and restore dependencies
COPY . ./
RUN dotnet restore "src/RockPaperScissors.csproj"

# Publish the application
RUN dotnet publish "src/RockPaperScissors.csproj" -c Release -o /app/out

# Stage 2: Serve the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

# Expose port 5081 and set the entry point
EXPOSE 5081
ENTRYPOINT ["dotnet", "RockPaperScissors.dll"]
