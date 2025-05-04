# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia o arquivo de projeto e restaura as dependências
COPY ContactQueueService/*.csproj ./ContactQueueService/
RUN dotnet restore ./ContactQueueService/ContactQueueService.csproj

# Copia o restante do código e publica a aplicação
COPY . ./
WORKDIR /app/ContactQueueService
RUN dotnet publish -c Release -o /out

# Etapa final: imagem de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .

# Define a porta padrão e o ponto de entrada
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

ENTRYPOINT ["dotnet", "ContactQueueService.dll"]
