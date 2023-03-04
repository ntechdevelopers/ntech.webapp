# READ ME

# New solution and add project
dotnet new sln --name Ntech.Windows.Monitoring
dotnet sln add Ntech.Windows.Monitoring.Core/Ntech.Windows.Monitoring.Core.csproj
dotnet sln add Ntech.Windows.Monitoring.Service/Ntech.Windows.Monitoring.Service.csproj
dotnet sln add Ntech.Windows.Monitoring.WPF/Ntech.Windows.Monitoring.WPF.csproj

# Build solution
dotnet build Ntech.WebApp.sln
dotnet build Ntech.Windows.Monitoring.sln

# Migration database
dotnet run --project .\Ntech.Migration\Ntech.Migration.csproj ntech-database

# Run service
dotnet run --project .\Ntech.Windows.Monitoring.Service\Ntech.Windows.Monitoring.Service.csproj

# Run UI APP
dotnet run --project .\Ntech.Windows.Monitoring.WPF\Ntech.Windows.Monitoring.WPF.csproj

# Run Web APP
dotnet run --project .\Ntech.WebApp\Ntech.WebApp.csproj

# Docker
docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d --build