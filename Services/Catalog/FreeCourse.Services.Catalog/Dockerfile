FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim as build # build iþlemini yaptýk
WORKDIR /app # çalýþma dizinini belirledik
EXPOSE 80 # port numarasýný belirledik
COPY Shared/FreeCourse.Shared/*.csproj Shared/FreeCourse.Shared/ # projeyi kopyaladýk çünkü diðer microservislerin ulaþabilmesi için
COPY Services/Catalog/FreeCourse.Services.Catalog/*.csproj Services/Catalog/FreeCourse.Services.Catalog/ # projeyi kopyaladýk. dotnet restore nudget package larý yüklemek için
RUN dotnet restore Services/Catalog/FreeCourse.Services.Catalog/*.csproj # projeyi restore ettik
COPY . . # tüm dosyalarý kopyaladýk
RUN dotnet publish Services/Catalog/FreeCourse.Services.Catalog/*.csproj -c Release -o out # projeyi publish ettik relase modunda ve out klasörüne publish ettik
FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim as runtime # runtime iþlemini yaptýk
WORKDIR /app # çalýþma dizinini belirledik
COPY --from=build /app/out . # projeyi kopyaladýk. . app klasörüne kopyaladýk
ENTRYPOINT [ "dotnet","FreeCourse.Services.Catalog.dll" ] # projeyi çalýþtýrdýk
```




