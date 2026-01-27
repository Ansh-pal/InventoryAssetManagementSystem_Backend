# Inventory & Asset Management System

A full-stack inventory and asset management system built with .NET 10 and Angular. This application provides a simple and efficient way to manage inventory items, track stock levels, and monitor stock transactions.

## 🚀 Features

- **Inventory Management**
  - View all inventory items in a sortable list
  - Create, read, update, and delete inventory items
  - Track item details including name, description, quantity, price, and minimum stock threshold

- **Stock Management**
  - Stock-in operations to add inventory
  - Stock-out operations to remove inventory
  - Automatic validation to prevent overselling
  - Transaction history tracking

- **Low Stock Alerts**
  - Automatic monitoring of stock levels
  - Low stock item dashboard
  - Configurable minimum stock thresholds

- **Transaction History**
  - Complete audit trail of all stock movements
  - Timestamped transactions with notes
  - Type-based filtering (In/Out)

## 🛠️ Technology Stack

### Backend
- **.NET 10** - Modern web API framework
- **ASP.NET Core** - Web framework
- **Entity Framework Core** - ORM for database operations
- **SQL Server** - Database
- **Scalar** - API documentation and testing

### Frontend
- **Angular** - Frontend framework
- **TypeScript** - Type-safe JavaScript
- **RxJS** - Reactive programming
- **HTML/CSS** - UI markup and styling

## 📋 Prerequisites

Before you begin, ensure you have the following installed:
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js](https://nodejs.org/) (v18 or higher)
- [Angular CLI](https://angular.io/cli) (`npm install -g @angular/cli`)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or SQL Server Express

## 🔧 Installation & Setup

### Backend Setup

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd "Inventory & Asset Management System"
   ```

2. **Configure Database Connection**
   
   Update the connection string in `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=.\\SQLEXPRESS;Database=InventoryAssetManagementSystemDB;Trusted_Connection=true;TrustServerCertificate=true;MultipleActiveResultSets=true"
     }
   }
   ```

3. **Install Dependencies**
   ```bash
   cd "Inventory & Asset Management System"
   dotnet restore
   ```

4. **Apply Database Migrations**
   ```bash
   dotnet ef database update
   ```
   
   If you don't have migrations yet, create them:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

5. **Run the Backend**
   ```bash
   dotnet run
   ```
   
   The API will be available at `https://localhost:5001` or `http://localhost:5000`

6. **Access API Documentation**
   
   Navigate to `https://localhost:5001/scalar/v1` to view and test the API endpoints

### Frontend Setup

1. **Navigate to Frontend Directory**
   ```bash
   cd inventory-frontend
   ```

2. **Install Dependencies**
   ```bash
   npm install
   ```

3. **Run the Frontend**
   ```bash
   ng serve
   ```
   
   The application will be available at `http://localhost:4200`

4. **Build for Production**
   ```bash
   ng build --configuration production
   ```

## 📁 Project Structure

```
Inventory & Asset Management System/
├── Inventory & Asset Management System/
│   ├── Controllers/
│   │   ├── InventoryController.cs    # Inventory CRUD operations
│   │   └── StockController.cs        # Stock management operations
│   ├── Models/
│   │   ├── InventoryItem.cs          # Inventory item entity
│   │   └── StockTransaction.cs       # Transaction entity
│   ├── Data/
│   │   └── ApplicationDbContext.cs   # EF Core DbContext
│   ├── Program.cs                    # Application entry point
│   ├── appsettings.json              # Configuration settings
│   └── Inventory & Asset Management System.csproj
├── inventory-frontend/
│   ├── src/
│   │   ├── app/
│   │   │   ├── components/
│   │   │   │   ├── inventory-list/
│   │   │   │   ├── inventory-form/
│   │   │   │   └── stock-management/
│   │   │   ├── models/
│   │   │   │   ├── inventory-item.model.ts
│   │   │   │   └── stock-transaction.model.ts
│   │   │   ├── services/
│   │   │   │   ├── inventory.service.ts
│   │   │   │   └── stock.service.ts
│   │   │   ├── app.component.ts
│   │   │   ├── app.config.ts
│   │   │   └── app.routes.ts
│   │   └── index.html
│   ├── angular.json
│   └── package.json
└── README.md
```

## 🔌 API Endpoints

### Inventory Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/Inventory` | Get all inventory items |
| GET | `/api/Inventory/{id}` | Get specific inventory item |
| POST | `/api/Inventory` | Create new inventory item |
| PUT | `/api/Inventory/{id}` | Update inventory item |
| DELETE | `/api/Inventory/{id}` | Delete inventory item |
| GET | `/api/Inventory/low-stock` | Get items below minimum threshold |

### Stock Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/Stock/stock-in/{itemId}` | Add stock to item |
| POST | `/api/Stock/stock-out/{itemId}` | Remove stock from item |
| GET | `/api/Stock/transactions/{itemId}` | Get transaction history |

## 📊 Database Schema

### InventoryItems Table
- `Id` (int, PK)
- `Name` (string, required, max 100 chars)
- `Description` (string, nullable)
- `Quantity` (int, required)
- `MinStockThreshold` (int, required)
- `Price` (decimal, required)
- `CreatedDate` (datetime, default UTC now)

### StockTransactions Table
- `Id` (int, PK)
- `InventoryItemId` (int, FK)
- `Type` (string, "In" or "Out")
- `Quantity` (int, required)
- `TransactionDate` (datetime, default UTC now)
- `Notes` (string, nullable)

## 🎯 Usage

1. **Adding New Items**
   - Navigate to the Inventory page
   - Click "Add New Item"
   - Fill in the item details
   - Click "Save"

2. **Managing Stock**
   - Select an item from the inventory list
   - Use "Stock In" to add inventory
   - Use "Stock Out" to remove inventory
   - Add notes to transactions for better tracking

3. **Monitoring Low Stock**
   - Check the "Low Stock Items" section on the dashboard
   - Items with quantity below minimum threshold are automatically displayed

4. **Viewing Transaction History**
   - Select an item
   - View all stock-in and stock-out operations
   - Filter by date or transaction type

## 🔒 CORS Configuration

The backend is configured to accept requests from `http://localhost:4200` during development. To modify CORS settings for production, update the `Program.cs` file:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("https://your-production-domain.com")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

## Testing

### Backend Testing
```bash
cd "Inventory & Asset Management System"
dotnet test
```

### Frontend Testing
```bash
cd inventory-frontend
npm test
```

## 🚀 Deployment
>>>>>>> 36b989e1dfea5d1f100e669f2d41c83e81480ab6

### Backend Deployment
1. Publish the application:
   ```bash
   dotnet publish -c Release -o ./publish
   ```

2. Update connection string for production database

3. Deploy to your hosting service (Azure, IIS, etc.)

### Frontend Deployment
1. Build for production:
   ```bash
   ng build --configuration production
   ```

2. Deploy the `dist/` folder to your hosting service

3. Update API URL in environment configuration

## ?? Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the project
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## ?? License

This project is licensed under the MIT License - see the LICENSE file for details.

## 👥 Authors
Ansh Pal
## 🙏 Acknowledgments

- Built with .NET 10 and Angular
- Database powered by SQL Server
- API documentation with Scalar

## 📧 Contact

Project Link: [https://github.com/yourusername/inventory-asset-management](https://github.com/yourusername/inventory-asset-management)

---

**Note:** This is a development version. Please ensure proper security measures, authentication, and authorization are implemented before deploying to production.

# Run all tests
dotnet test

# Run with detailed output
dotnet test --verbosity normal

# Run specific test project
dotnet test "InventoryAssetManagementSystem.Tests\InventoryAssetManagementSystem.Tests.csproj"
