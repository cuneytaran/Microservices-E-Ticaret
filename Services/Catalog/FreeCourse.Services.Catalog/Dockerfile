FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim as build # build i�lemini yapt�k
WORKDIR /app # �al��ma dizinini belirledik
EXPOSE 80 # port numaras�n� belirledik
COPY Shared/FreeCourse.Shared/*.csproj Shared/FreeCourse.Shared/ # projeyi kopyalad�k ��nk� di�er microservislerin ula�abilmesi i�in
COPY Services/Catalog/FreeCourse.Services.Catalog/*.csproj Services/Catalog/FreeCourse.Services.Catalog/ # projeyi kopyalad�k. dotnet restore nudget package lar� y�klemek i�in
RUN dotnet restore Services/Catalog/FreeCourse.Services.Catalog/*.csproj # projeyi restore ettik
COPY . . # t�m dosyalar� kopyalad�k
RUN dotnet publish Services/Catalog/FreeCourse.Services.Catalog/*.csproj -c Release -o out # projeyi publish ettik relase modunda ve out klas�r�ne publish ettik
FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim as runtime # runtime i�lemini yapt�k
WORKDIR /app # �al��ma dizinini belirledik
COPY --from=build /app/out . # projeyi kopyalad�k. . app klas�r�ne kopyalad�k
ENTRYPOINT [ "dotnet","FreeCourse.Services.Catalog.dll" ] # projeyi �al��t�rd�k
```




