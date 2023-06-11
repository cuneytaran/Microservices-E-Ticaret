FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim as build  #build aþamasýnda çalýþtýrýyoruz
WORKDIR /app
EXPOSE 80
COPY Shared/FreeCourse.Shared/*.csproj Shared/FreeCourse.Shared/ #Shared/FreeCourse.Shared/*.csproj bu dosyayý Shared/FreeCourse.Shared/ içine kopyalýyoruz 
COPY Services/Basket/FreeCourse.Services.Basket/*.csproj Services/Basket/FreeCourse.Services.Basket/
RUN dotnet restore Services/Basket/FreeCourse.Services.Basket/*.csproj #dotnet restore ile projeyi restore ediyoruz
COPY . . # tüm dosyalarý kopyalýyoruz
RUN dotnet publish Services/Basket/FreeCourse.Services.Basket/*.csproj -c Release -o out #dotnet publish ile projeyi publish ediyoruz
FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim as runtime #runtime olarak çalýþtýrýyoruz
WORKDIR /app #çalýþma dizini
COPY --from=build /app/out . #build aþamasýnda oluþturduðumuz out klasörünü buraya kopyalýyoruz
ENTRYPOINT [ "dotnet","FreeCourse.Services.Basket.dll" ] #dotnet ile projeyi çalýþtýrýyoruz




