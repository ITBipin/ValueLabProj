# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy csproj and restore
COPY ["CleanArchitecture.Domain/CleanArchitecture.Domain.csproj", "CleanArchitecture.Domain/"]
COPY ["CleanArchitecture.Application/CleanArchitecture.Application.csproj", "CleanArchitecture.Application/"]
COPY ["CleanArchitecture.Infrastructure/CleanArchitecture.Infrastructure.csproj", "CleanArchitecture.Infrastructure/"]
COPY ["ValueLabProj.Api/ValueLabProj.Api.csproj", "ValueLabProj.Api/"]
COPY ["CodeChallenge.Tests/CodeChallenge.Tests.csproj", "CodeChallenge.Tests/"]

RUN dotnet restore "ValueLabProj.Api/ValueLabProj.Api.csproj"

# copy everything else and build
COPY . ./
RUN dotnet publish "ValueLabProj.Api/ValueLabProj.Api.csproj" -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "ValueLabProj.Api.dll"]
