// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

namespace FreeCourse.IdentityServer
{
    public static class Config
    {
        //login olduğunda token içinden hangi bilgilerin geleceğini belirliyoruz.
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.Email(),
                new IdentityResources.OpenId(),//kullanıcıya özgü subkeyword mutlaka dolu olması lazım.OpenId mutlaka dolu olmak zorunda.zorunlu alan
                new IdentityResources.Profile(),//profile bilgilerini erişmesi
                new IdentityResource(){ Name="roles", DisplayName="Roles", Description="Kullanıcı rolleri", UserClaims=new []{ "role"} } //kullanıcı rolünü token içine gömme. kullanıcı bilgisi bir Claim nesnesidir. 
            };


        //postmande Scope lara karşılık geliyor. hangi microservislere ne YETKİ verileceği tanımlanıyor
        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("catalog_fullpermission","Catalog API için full erişim"),//catalog_fullpermission ApiResources deki ile aynı olmalı buna dikkat et.Catalog API için full erişim yazısı postmande istek yapıldığında görüncek
                new ApiScope("photo_stock_fullpermission","Photo Stock API için full erişim"),
                new ApiScope("basket_fullpermission","Basket API için full erişim"),// ************************  1. işlem ************************
                new ApiScope("discount_fullpermission","Discount API için full erişim"),
                new ApiScope("order_fullpermission","Order API için full erişim"),
                new ApiScope("payment_fullpermission","Payment API için full erişim"),
                new ApiScope("gateway_fullpermission","Gateway API için full erişim"),
                new ApiScope(IdentityServerConstants.LocalApi.ScopeName)//identity serverin kendi scope bu.
            };


        //Postmande AUD lara karşılık geliyor. Yetkisiyle birlikte microservisleri ERİŞİM Ve TANIMLAMA yapıyoruz.
        //Dikkat! bunu startup a eklemeyi unutma. .AddInMemoryApiResources(Config.ApiResources) şeklinde.
            public static IEnumerable<ApiResource> ApiResources => new ApiResource[]
            {
                new ApiResource("resource_catalog"){Scopes={"catalog_fullpermission"}},//Scopes=yetkileri tanımlıyoruz.write, read vs...
                new ApiResource("resource_photo_stock"){Scopes={"photo_stock_fullpermission"}},
                new ApiResource("resource_basket"){Scopes={"basket_fullpermission"}},// ************************  2. işlem ************************
                new ApiResource("resource_discount"){Scopes={"discount_fullpermission"}},
                new ApiResource("resource_order"){Scopes={"order_fullpermission"}},
                new ApiResource("resource_payment"){Scopes={"payment_fullpermission"}},
                new ApiResource("resource_gateway"){Scopes={"gateway_fullpermission"}},
                new ApiResource(IdentityServerConstants.LocalApi.ScopeName)//identity serverin kendi Resources tanımlıyoruz
            };


        //Token almak isteyen SİTEYİ tanımlıyoruz.ClientId ve ClientSecrets ihtiyacımız var.
        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client //Refresh token yok.Kullancı adı ve şifre gerek yok. site tanımlanıyor.
                {
                   ClientName="Asp.Net Core MVC",//çok ihtiyaç yok ama hangi siteden geliyoru anlamak için yazıyoruz.
                   ClientId="WebMvcClient",//kullanıcı username gibi düşün site için
                   ClientSecrets= {new Secret("secret".Sha256())},//kullanıcı şifresi gibi düşün site için. şifreyi secret olarak tanımladık.
                   AllowedGrantTypes= GrantTypes.ClientCredentials,//izin verilen izin tipi ClientCredentials
                   AllowedScopes={ "catalog_fullpermission","photo_stock_fullpermission", "gateway_fullpermission", IdentityServerConstants.LocalApi.ScopeName }//WebMvcClient kimlere istek yapacağını tanımlıyoruz.yetkileriyle birlikte.ClientCredential da refresh token olmaz.
                },
                new Client //Refresh token var. Kullanıcı bilgilerini kimler erişebileceğini tanımlıyoruz. Kullancı adı ve şifreli
                {
                   ClientName="Asp.Net Core MVC",
                   ClientId="WebMvcClientForUser",
                   AllowOfflineAccess=true,//refresh token kullanılsın izini verilmesi
                   ClientSecrets= {new Secret("secret".Sha256())},
                   AllowedGrantTypes= GrantTypes.ResourceOwnerPassword,                   
                   AllowedScopes={ //AllowedSopes de yukarıda IdentityResources tanımladığımız bilgilerin hangilerini hangi apinin hangi userbilgilerini ulaşacağını belirliyoruz.
                        "basket_fullpermission", // ************************  3. işlem ************************
                        "order_fullpermission", 
                        "gateway_fullpermission",
                        "discount_fullpermission",
                        "payment_fullpermission",
                        "gateway_fullpermission",
                        "roles", 
                        IdentityServerConstants.StandardScopes.Email, 
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile, 
                        IdentityServerConstants.StandardScopes.OfflineAccess, //refresh token elimizde varsa, kullanıcı login olmasada kullanıcı adına yeni bir refresh token alabiliriz. eğer bunu kaldırırsak süre dolarsa login safyasına yönlendirir.
                        IdentityServerConstants.LocalApi.ScopeName
                    },
                   AccessTokenLifetime=1*60*60,//token ömrünü belirliyoruz. yani şuanki 1 saatlik bir ömür verdik.saat*dakika*saniye
                   RefreshTokenExpiration=TokenExpiration.Absolute,//kesin bir refresh tarih olsun diye tanımlıyoruz.61.gün ömrü dolacak.
                   AbsoluteRefreshTokenLifetime= (int) (DateTime.Now.AddDays(60)- DateTime.Now).TotalSeconds,//mevcut olan güne 60 gün ekliyoruz. ve şuanki günden çıkarıyoruz ve saniye cinsinden ayarlıyoruz.
                   RefreshTokenUsage= TokenUsage.ReUse//ReUse arka arkaya kullanabilirsin. eğer OneTimeOnly dersek sadece bu bir kere kullanılır.
                    //NOT:eğer kullanıcı 60 gün boyunca hiç login olmazsa 61.gün login ekranına gider.Eğer 35.gün girdi diyelim. refresh token tekrar 60 gün olarak uzayacak.
                },

                // ************************  4. işlem ilgili projenin  startubuna ConfigureServices içine ekle ************************
                //ilgili projede Nuget Packages de Microsoft.AspNetCore.Authentication.JwtBearer yükle
                //var requireAuthorizePolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                //JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");//token içindeki bilgiyi clientid yi sub olarak göster.
                //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
                //{
                //    options.Authority = Configuration["IdentityServerURL"];
                //    options.Audience = "resource_basket";
                //    options.RequireHttpsMetadata = false;
                //});
                
                //services.AddControllers(opt =>
                //{
                //    opt.Filters.Add(new AuthorizeFilter(requireAuthorizePolicy));
                //});

                // ************************  5. işlem ilgili projenin  startup Configure içine ekle ************************
                //app.UseAuthentication();
                //app.UseAuthorization();

                // ************************  6. işlem ilgili projenin  appestings.json içine ekle ************************
                //"IdentityServerURL": "http://localhost:5001",


                //TOKEN EXCHANGE CLIENT işlemi
                new Client
                {
                   ClientName="Token Exchange Client",
                    ClientId="TokenExhangeClient",
                    ClientSecrets= {new Secret("secret".Sha256())},
                    AllowedGrantTypes= new []{ "urn:ietf:params:oauth:grant-type:token-exchange" },//urn:ietf:params:oauth:grant-type:token-exchange= akış tipi
                    AllowedScopes={ "discount_fullpermission", "payment_fullpermission", IdentityServerConstants.StandardScopes.OpenId }//token gönderildiğinde kimlere izin verileceğini belirliyoruz.
                //diğer ayarları yapmak için ocelot içindeki TokenExchange delegatelerini kullanıyoruz. occelot içine git.
                //gatewey içindeki delegatehadlers içindeki TokenExhangeDelegateHandler içine git.
                },
            };
    }
}