# License System .Net API 

## Hakkında ( Türkçe )
Key lisans sistemi ürünlerinizi lisanlamanız için oluşturduğum bir projedir. Bu projenin .Net ile geliştirilmiş backend kısmıdır. İstediğiniz admin panelini oluşturup bu API üzerinden istek atarak kullanabilirsiniz. Aplikasyonlar üzerinden müşterilerinize alt paneller oluşturma, son kullanıcı için lisans anahtarları üretme gibi fonksiyonları bulunmaktadır.

<br>



<img src="https://i.hizliresim.com/bhlh36g.png"></img> 

`Bu proje hala geliştirme aşamasındadır gördüğünüz eksikleri giderebilir ve projeye katkıda bulunabilirsiniz.`

`You can find EN README end of content.`

## Kullanım
 - Yeni bir veritabanı oluşturup tabloları MSSQL üzerinde oluşturun. 
 - Terminali açın ( CMD ya da GitBash )
 ```bash
 cd c:\
 git clone https://github.com/farukkardas/LicenseSystem.git
 cd C:\LicenseSystem\WebAPI\bin\Release\net5.0
 WebAPI.exe
 ```
 veya projeyi klasöre çektikten sonra release klasöründen çalıştırabilir, projeyi IDE üzerinden çalıştırıp DEBUG modunda da kullanabilirsiniz.

## MsSQL Tables

<img src="https://i.hizliresim.com/flpypwg.png"></img> 

- Bu proje 7 adet tablo barındırıyor ve SQL içerisinde relationship bulundurmuyor onun yerine DTO kullanıyor. -> [DTO Nedir?](https://docs.microsoft.com/tr-tr/aspnet/web-api/overview/data/using-web-api-with-entity-framework/part-5)

| Applications  | KeyLicenses | Logs | OperationClaims | Panels | UserOperationClaims | Users |
| ------------- | ------------- | ------------- | ------------- | ------------- | ------------- | ------------- |
| Id  (int) | Id  (int)  |  Id  (int) | Id  (int) | Id  (int)   | Id  (int)  | Id  (int) 
| ApplicationName (varchar(50)) | AuthKey (nvarchar(50))  | Message (nvarchar(200)) | Name  (varchar(20)) | PanelOwnerId  (int) | UserId (int) | Email  (nvarchar(50))
| OwnerId (int)  | Hwid (nvarchar(50)) | OwnerId  (int) | | PanelSellerId (int) | OperationClaimId (int) | PasswordHash (varbinary(500))
| CreationTime (datetime2(7))  | OwnerId (int)  | Date (datetime2(7)) | | IsActive (bit) | | PasswordSalt (varbinary(500))
| Status (bit) | ExpirationDate (datetime2(7)) | Success (bit) | | Balance (float) | | Status (bit)
| DailyPrice (float)  | IsOwned (bit) | | | CreatedLicense (int) | | Balance (float)
| WeeklyPrice (float) | ApplicationId (int) | | | ApplicationId (int) | | SecurityKey (nvarchar(100))
| MonthlyPrice  (float) | | | | | | SecurityKeyExpiration (datetime2(7))

## Connection String Ayarlama
- Projeyi herhangi bir editör üzerinde açıktıktan sonra DataAccess->Concrete->LicenseSystemContext.cs yolunu izleyip ilgili DB connection stringi kendinize göre değiştirin.

<img src="https://i.hizliresim.com/jw9le4c.png"></img> 

## Kurulum Video ( TÜRKÇE )


 [![ALT](https://youtube-md.vercel.app/iWlXEg5RAwA)](https://https://www.youtube.com/watch?v=ZDPJe7Pe5Nw)
 
 
 ## About ( English )
Key license system is a project I created for you to license your products. This is the backend part of the project developed with .Net. You can create the admin panel  and use it by making a request through this API. It has functions such as creating sub-panels for your customers, generating license keys for the end user through the applications.

<br>

`This still under development can fix the deficiencies you see and you can support the project.`


## Usage
 - Create a new database on MsSQL and import all tables. 
 - Open the terminal ( CMD  or GitBash )
 ```bash
 cd c:\
 git clone https://github.com/farukkardas/LicenseSystem.git
 cd C:\LicenseSystem\WebAPI\bin\Release\net5.0
 WebAPI.exe
 ```
 instead of terminal after you can use manually. First download and extract this project after that  you can run it WebAPI.exe in  Release folder or run this API on IDE and use it in DEBUG mode.

## MsSQL Tables

<img src="https://i.hizliresim.com/flpypwg.png"></img> 

- This project contains 7 tables and don't have relationship  SQL instead it uses DTO. -> [What is DTO?](https://docs.microsoft.com/en-us/aspnet/web-api/overview/data/using-web-api-with-entity-framework/part-5)

| Applications  | KeyLicenses | Logs | OperationClaims | Panels | UserOperationClaims | Users |
| ------------- | ------------- | ------------- | ------------- | ------------- | ------------- | ------------- |
| Id  (int) | Id  (int)  |  Id  (int) | Id  (int) | Id  (int)   | Id  (int)  | Id  (int) 
| ApplicationName (varchar(50)) | AuthKey (nvarchar(50))  | Message (nvarchar(200)) | Name  (varchar(20)) | PanelOwnerId  (int) | UserId (int) | Email  (nvarchar(50))
| OwnerId (int)  | Hwid (nvarchar(50)) | OwnerId  (int) | | PanelSellerId (int) | OperationClaimId (int) | PasswordHash (varbinary(500))
| CreationTime (datetime2(7))  | OwnerId (int)  | Date (datetime2(7)) | | IsActive (bit) | | PasswordSalt (varbinary(500))
| Status (bit) | ExpirationDate (datetime2(7)) | Success (bit) | | Balance (float) | | Status (bit)
| DailyPrice (float)  | IsOwned (bit) | | | CreatedLicense (int) | | Balance (float)
| WeeklyPrice (float) | ApplicationId (int) | | | ApplicationId (int) | | SecurityKey (nvarchar(100))
| MonthlyPrice  (float) | | | | | | SecurityKeyExpiration (datetime2(7))

## Setup Connection String 
- After opening the project on any editor, follow the path DataAccess->Concrete->LicenseSystemContext.cs and change the relevant DB connection string according to you.

<img src="https://i.hizliresim.com/jw9le4c.png"></img> 

## Installation Video ( EN )

 [![ALT](https://youtube-md.vercel.app/iWlXEg5RAwA)](https://www.youtube.com/watch?v=iWlXEg5RAwA)





