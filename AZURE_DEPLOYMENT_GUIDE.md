# Azure Deployment Configuration Guide

## Issue: Web Deployment Connection String Error

### Problem
The error message indicates an invalid connection string during Azure deployment:
```
Server=tcp:database1234.database.windows.net;Authentication=Active Directory Default;Initial Catalog=database-project;
```

### Solutions

There are **THREE** main ways to configure your Azure SQL connection:

---

## Option 1: SQL Authentication (Recommended for simplicity)

### 1. In Azure Portal - SQL Database
1. Go to your Azure SQL Server ? `database1234.database.windows.net`
2. Navigate to **Settings** ? **Firewalls and virtual networks**
3. Add your Azure App Service's outbound IP addresses
4. Make sure "Allow Azure services and resources to access this server" is ON

### 2. Connection String Format
```
Server=tcp:database1234.database.windows.net,1433;Initial Catalog=database-project;User ID=your_admin_username;Password=your_secure_password;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;MultipleActiveResultSets=true;
```

### 3. Configure in Azure App Service
1. Go to your **App Service** in Azure Portal
2. Navigate to **Settings** ? **Configuration**
3. Under **Application settings**, add a **Connection String**:
   - **Name**: `DefaultConnection`
   - **Value**: (paste the connection string above with your actual values)
   - **Type**: `SQLAzure`
4. Click **Save**

---

## Option 2: Managed Identity (Most Secure - Recommended for Production)

### 1. Enable Managed Identity on App Service
1. Go to your **App Service** ? **Identity**
2. Under **System assigned**, toggle **Status** to **On**
3. Click **Save** and copy the **Object (principal) ID**

### 2. Grant Database Access
Connect to your Azure SQL Database using Azure Data Studio or SSMS and run:
```sql
CREATE USER [your-app-service-name] FROM EXTERNAL PROVIDER;
ALTER ROLE db_datareader ADD MEMBER [your-app-service-name];
ALTER ROLE db_datawriter ADD MEMBER [your-app-service-name];
ALTER ROLE db_ddladmin ADD MEMBER [your-app-service-name];
```

### 3. Connection String Format
```
Server=tcp:database1234.database.windows.net,1433;Initial Catalog=database-project;Authentication=Active Directory Default;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;MultipleActiveResultSets=true;
```

### 4. Add NuGet Package
Add to your project:
```bash
dotnet add "Inventory & Asset Management System" package Microsoft.Data.SqlClient
```

### 5. Update Program.cs
```csharp
using Azure.Identity;
using Microsoft.Data.SqlClient;

// Add this BEFORE builder.Services.AddDbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    
    if (builder.Environment.IsProduction())
    {
        var sqlConnection = new SqlConnection(connectionString);
        sqlConnection.AccessToken = new DefaultAzureCredential()
            .GetToken(new Azure.Core.TokenRequestContext(
                new[] { "https://database.windows.net/.default" }))
            .Token;
        options.UseSqlServer(sqlConnection);
    }
    else
    {
        options.UseSqlServer(connectionString);
    }
});
```

---

## Option 3: Connection String in Azure Configuration (Quickest Fix)

### 1. Don't put credentials in appsettings.Production.json
Keep it empty as it is now.

### 2. Set Connection String in Azure Portal
1. Go to **App Service** ? **Configuration** ? **Connection strings**
2. Click **New connection string**
3. **Name**: `DefaultConnection`
4. **Value**: 
   ```
   Server=tcp:database1234.database.windows.net,1433;Initial Catalog=database-project;User ID=sqladmin;Password=YourPassword123!;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;MultipleActiveResultSets=true;Persist Security Info=False;
   ```
5. **Type**: `SQLAzure`
6. Click **OK** then **Save**

---

## Current Configuration Files

### Local Development (`appsettings.json`)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=InventoryAssetManagementSystemDB;Trusted_Connection=true;TrustServerCertificate=true;MultipleActiveResultSets=true"
  }
}
```
? Keep this for local SQL Server Express

### Production (`appsettings.Production.json`)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": ""
  }
}
```
? Empty - Will be overridden by Azure App Service Configuration

---

## Steps to Deploy

### 1. Set Connection String in Azure (Choose one option above)

### 2. Apply Database Migrations to Azure SQL
Before deploying, create the database schema:

**Option A: Using Connection String**
```bash
dotnet ef database update --connection "Server=tcp:database1234.database.windows.net,1433;Initial Catalog=database-project;User ID=admin;Password=YourPass;Encrypt=True;"
```

**Option B: Create SQL Script**
```bash
dotnet ef migrations script --output azure-migration.sql
```
Then run the script in Azure Data Studio or Azure Portal Query Editor

### 3. Publish Your Application
```bash
dotnet publish -c Release
```

### 4. Deploy to Azure
Use one of these methods:
- Visual Studio: Right-click project ? Publish
- Azure CLI
- GitHub Actions
- Azure DevOps

---

## Recommended Approach for You

Based on your error, I recommend **Option 3** (Connection String in Azure Configuration):

1. ? Keep `appsettings.Production.json` empty (already done)
2. ? Go to Azure Portal ? Your App Service ? Configuration
3. ? Add Connection String with SQL Authentication
4. ? Apply migrations to Azure SQL Database
5. ? Redeploy your application

---

## Fixing "Dereference of a possibly null reference" Warnings

These are just warnings in your test files. To fix them, you can:

### Option 1: Suppress warnings (Quick)
Add to your test project's `.csproj`:
```xml
<PropertyGroup>
  <Nullable>enable</Nullable>
  <NoWarn>CS8600;CS8602;CS8605</NoWarn>
</PropertyGroup>
```

### Option 2: Use null-forgiving operator (Better)
I can update your test files to use `!` operator where appropriate.

Would you like me to fix the test warnings as well?

---

## Next Steps

1. **Choose an authentication method** (I recommend Option 3 for quick fix)
2. **Configure connection string in Azure Portal**
3. **Run database migrations on Azure SQL**
4. **Redeploy your application**

Let me know which option you'd like to use, and I can help you implement it!
