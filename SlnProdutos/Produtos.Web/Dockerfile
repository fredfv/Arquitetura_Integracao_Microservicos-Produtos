#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Produtos.Web/Produtos.Web.csproj", "Produtos.Web/"]
COPY ["GeekBurguer.Products.Contract/GeekBurguer.Products.Contract.csproj", "GeekBurguer.Products.Contract/"]
COPY ["GeekBurguer.Products.IoC/GeekBurguer.Products.IoC.csproj", "GeekBurguer.Products.IoC/"]
COPY ["GeekBurguer.Products.Infra/GeekBurguer.Products.Infra.csproj", "GeekBurguer.Products.Infra/"]
COPY ["GeekBurguer.Products.Service/GeekBurguer.Products.Service.csproj", "GeekBurguer.Products.Service/"]
RUN dotnet restore "Produtos.Web/Produtos.Web.csproj"
COPY . .
WORKDIR "/src/Produtos.Web"
RUN dotnet build "Produtos.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Produtos.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Produtos.Web.dll"]