FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY BokApi/BokApi.csproj BokApi/
RUN dotnet restore BokApi/BokApi.csproj

COPY BokApi/ BokApi/
RUN dotnet publish BokApi/BokApi.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
# Railway sätter PORT vid start; Program.cs lyssnar på den (fallback 8080).
EXPOSE 8080

ENTRYPOINT ["dotnet", "BokApi.dll"]
