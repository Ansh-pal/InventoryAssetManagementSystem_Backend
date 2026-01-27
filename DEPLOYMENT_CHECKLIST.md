# Azure Deployment Checklist

## ? Pre-Deployment Steps Completed

1. ? **CORS Configuration Added** - Program.cs configured for Angular frontend
2. ? **Production Settings Created** - appsettings.Production.json (empty - will use Azure config)
3. ? **All Tests Passing** - 22/22 tests successful
4. ? **No Build Warnings** - Clean build
5. ? **Null Reference Warnings Fixed** - All code warnings resolved

---

## ?? Deployment Steps

### Step 1: Configure Azure SQL Database Connection

Choose ONE of these methods:

#### **Method A: SQL Authentication (Easiest - Recommended)**

1. Open **Azure Portal** ? Your **App Service**
2. Go to **Configuration** ? **Connection strings**
3. Click **+ New connection string**
4. Enter:
   - **Name**: `DefaultConnection`
   - **Value**: 
     ```
     Server=tcp:database1234.database.windows.net,1433;Initial Catalog=database-project;User ID=YOUR_ADMIN_USERNAME;Password=YOUR_SECURE_PASSWORD;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;MultipleActiveResultSets=true;
     ```
   - **Type**: Select `SQLAzure`
5. Click **OK**, then **Save**
6. Restart your App Service

#### **Method B: Managed Identity (Most Secure)**
See detailed steps in `AZURE_DEPLOYMENT_GUIDE.md`

---

### Step 2: Apply Database Migrations to Azure SQL

Before deploying your app, create the database schema:

```bash
dotnet ef database update --connection "Server=tcp:database1234.database.windows.net,1433;Initial Catalog=database-project;User ID=admin;Password=YourPassword;Encrypt=True;TrustServerCertificate=False;"
```

Or create a migration script:
```bash
dotnet ef migrations script --output azure-migration.sql
```
Then run it in Azure Portal Query Editor or Azure Data Studio.

---

### Step 3: Update CORS for Production

1. Open your App Service in Azure Portal
2. Go to **API** ? **CORS**
3. Add your Angular app's production URL (e.g., `https://yourangularapp.azurewebsites.net`)
4. Click **Save**

Or update `Program.cs` to allow your production frontend URL:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins(
            "http://localhost:4200",  // Development
            "https://yourangularapp.azurewebsites.net"  // Production
        )
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});
```

---

### Step 4: Publish Your Application

#### Using Visual Studio:
1. Right-click on "Inventory & Asset Management System" project
2. Select **Publish**
3. Choose **Azure** ? **Azure App Service (Windows/Linux)**
4. Select your App Service
5. Click **Publish**

#### Using Command Line:
```bash
# Navigate to your project directory
cd "Inventory & Asset Management System"

# Publish the app
dotnet publish -c Release -o ./publish

# Deploy using Azure CLI (if configured)
az webapp deploy --resource-group YOUR_RESOURCE_GROUP --name YOUR_APP_NAME --src-path ./publish --type zip
```

#### Using GitHub Actions (Automated):
See the GitHub Actions workflow in your repository settings.

---

### Step 5: Verify Deployment

1. **Check App Service Logs**
   - Azure Portal ? App Service ? **Log stream**
   - Look for startup messages and errors

2. **Test API Endpoints**
   ```
   https://your-app-name.azurewebsites.net/api/Inventory
   https://your-app-name.azurewebsites.net/scalar/v1
   ```

3. **Check Database Connection**
   - Try creating an inventory item via Scalar UI
   - Verify it appears in Azure SQL Database

---

## ?? Troubleshooting

### Issue: "Connection string is invalid"
**Solution**: Make sure you set the connection string in Azure App Service Configuration, not in appsettings.Production.json

### Issue: "Cannot connect to database"
**Solutions**:
1. Check SQL Server firewall rules
2. Enable "Allow Azure services and resources to access this server"
3. Verify connection string credentials

### Issue: "CORS error from Angular app"
**Solution**: Add your Angular app's URL to CORS policy in Program.cs or Azure Portal

### Issue: "Database schema not found"
**Solution**: Run migrations on Azure SQL Database before deploying

### Issue: "500 Internal Server Error"
**Solutions**:
1. Check App Service logs (Log stream)
2. Verify connection string in Configuration
3. Ensure database exists and has correct schema

---

## ?? Post-Deployment Checklist

- [ ] Connection string configured in Azure
- [ ] Database migrations applied
- [ ] CORS configured for production frontend
- [ ] App successfully published to Azure
- [ ] API endpoints responding correctly
- [ ] Scalar API documentation accessible
- [ ] Test creating an inventory item
- [ ] Test stock-in/stock-out operations
- [ ] Verify low stock alerts work
- [ ] Angular frontend can communicate with API

---

## ?? Important URLs

- **API Base URL**: `https://your-app-name.azurewebsites.net`
- **API Documentation**: `https://your-app-name.azurewebsites.net/scalar/v1`
- **Azure Portal**: https://portal.azure.com
- **GitHub Repository**: https://github.com/Ansh-pal/InventoryAssetManagementSystem_Backend

---

## ?? Need Help?

Refer to these files:
- `AZURE_DEPLOYMENT_GUIDE.md` - Detailed deployment instructions
- `README.md` - Project overview and local setup
- `TEST_SUMMARY.md` - Test coverage details

---

**Next Step**: Configure your connection string in Azure Portal (Step 1), then proceed with deployment!
