# Estágio 1: Compilação (Build)
# Usamos a imagem oficial do .NET SDK 8.0 para compilar o projeto
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copia os arquivos de projeto e solução e restaura as dependências
COPY *.sln .
COPY CadastroPessoas.Api/*.csproj ./CadastroPessoas.Api/
COPY CadastroPessoas.Api.Tests/*.csproj ./CadastroPessoas.Api.Tests/
RUN dotnet restore

# Copia todo o resto do código fonte e publica a aplicação
COPY . .
WORKDIR /source/CadastroPessoas.Api
RUN dotnet publish -c Release -o /app/out

# Estágio 2: Execução (Runtime)
# Usamos uma imagem menor, apenas com o necessário para rodar a aplicação .NET 8
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Define a porta que a aplicação vai usar
ENV PORT=8080

# Comando para iniciar a aplicação quando o container ligar
ENTRYPOINT ["dotnet", "CadastroPessoas.Api.dll"]