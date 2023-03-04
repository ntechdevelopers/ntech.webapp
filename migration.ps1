cd Ntech.WebApp

dotnet tool install --global dotnet-ef --version 6.*
dotnet add package Microsoft.EntityFrameworkCore.Design  
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL

#dotnet ef migrations add ReInit --context ApplicationDbContext

dotnet ef database update --context ApplicationDbContext

#----
dotnet ef migrations add ReInit --context ApplicationDbContext
dotnet ef migrations script --context ApplicationDbContext -o ./Scripts/script0001.sql