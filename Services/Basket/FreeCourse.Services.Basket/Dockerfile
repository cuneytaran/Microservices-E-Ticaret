FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim as build  #build a�amas�nda �al��t�r�yoruz
WORKDIR /app
EXPOSE 80
COPY Shared/FreeCourse.Shared/*.csproj Shared/FreeCourse.Shared/ #Shared/FreeCourse.Shared/*.csproj bu dosyay� Shared/FreeCourse.Shared/ i�ine kopyal�yoruz 
COPY Services/Basket/FreeCourse.Services.Basket/*.csproj Services/Basket/FreeCourse.Services.Basket/
RUN dotnet restore Services/Basket/FreeCourse.Services.Basket/*.csproj #dotnet restore ile projeyi restore ediyoruz
COPY . . # t�m dosyalar� kopyal�yoruz
RUN dotnet publish Services/Basket/FreeCourse.Services.Basket/*.csproj -c Release -o out #dotnet publish ile projeyi publish ediyoruz
FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim as runtime #runtime olarak �al��t�r�yoruz
WORKDIR /app #�al��ma dizini
COPY --from=build /app/out . #build a�amas�nda olu�turdu�umuz out klas�r�n� buraya kopyal�yoruz
ENTRYPOINT [ "dotnet","FreeCourse.Services.Basket.dll" ] #dotnet ile projeyi �al��t�r�yoruz




