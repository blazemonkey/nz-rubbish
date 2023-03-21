#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["NZRubbishCollection.Tests/NZRubbishCollection.Tests.csproj", "NZRubbishCollection.Tests/"]
COPY ["NZRubbishCollection.Shared/NZRubbishCollection.Shared.csproj", "NZRubbishCollection.Shared/"]
RUN dotnet restore "NZRubbishCollection.Tests/NZRubbishCollection.Tests.csproj"
COPY . .
WORKDIR "/src/NZRubbishCollection.Tests"
RUN dotnet build "NZRubbishCollection.Tests.csproj" -c Release -o /app/build

FROM build AS test
RUN dotnet test --filter FullyQualifiedName~NZRubbishCollection.Tests.Tests.MockTests