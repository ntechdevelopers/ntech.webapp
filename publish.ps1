dotnet build ..\JMS.sln

iisreset -stop
dotnet publish -o ..\..\publish
iisreset -start