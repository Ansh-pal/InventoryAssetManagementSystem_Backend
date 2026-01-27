# API Testing Guide

## ? Your API is Deployed Successfully!

The 404 error at the root URL is **NORMAL** for a Web API. This guide shows you how to properly test your deployed API.

---

## ?? Your API Base URL
```
https://inventorysystem88-e5baekb2dtegbqeu.centralindia-01.azurewebsites.net
```

---

## ?? Available Endpoints

### 1. **Root Endpoint (API Information)**
**URL:** `/`
```
https://inventorysystem88-e5baekb2dtegbqeu.centralindia-01.azurewebsites.net/
```
**Method:** GET  
**Description:** Shows API information and available endpoints  
**Expected Response:**
```json
{
  "name": "Inventory & Asset Management System API",
  "version": "1.0",
  "status": "Running",
  "documentation": "/scalar/v1",
  "endpoints": {
    "inventory": "/api/Inventory",
    "lowStock": "/api/Inventory/low-stock",
    "stock": "/api/Stock"
  },
  "description": "Backend API for Inventory and Asset Management System"
}
```

---

### 2. **API Documentation (Scalar UI)**
**URL:** `/scalar/v1`
```
https://inventorysystem88-e5baekb2dtegbqeu.centralindia-01.azurewebsites.net/scalar/v1
```
**Description:** Interactive API documentation with built-in testing tools

---

### 3. **Inventory Endpoints**

#### Get All Inventory Items
**URL:** `GET /api/Inventory`
```
https://inventorysystem88-e5baekb2dtegbqeu.centralindia-01.azurewebsites.net/api/Inventory
```
**Response:** Array of inventory items

#### Get Single Item
**URL:** `GET /api/Inventory/{id}`
```
https://inventorysystem88-e5baekb2dtegbqeu.centralindia-01.azurewebsites.net/api/Inventory/1
```
**Response:** Single inventory item

#### Create New Item
**URL:** `POST /api/Inventory`
```
https://inventorysystem88-e5baekb2dtegbqeu.centralindia-01.azurewebsites.net/api/Inventory
```
**Body:**
```json
{
  "name": "Test Item",
  "description": "Test Description",
  "quantity": 100,
  "minStockThreshold": 10,
  "price": 50.00
}
```

#### Update Item
**URL:** `PUT /api/Inventory/{id}`
```
https://inventorysystem88-e5baekb2dtegbqeu.centralindia-01.azurewebsites.net/api/Inventory/1
```
**Body:** (Same as create, with updated values)

#### Delete Item
**URL:** `DELETE /api/Inventory/{id}`
```
https://inventorysystem88-e5baekb2dtegbqeu.centralindia-01.azurewebsites.net/api/Inventory/1
```

#### Get Low Stock Items
**URL:** `GET /api/Inventory/low-stock`
```
https://inventorysystem88-e5baekb2dtegbqeu.centralindia-01.azurewebsites.net/api/Inventory/low-stock
```
**Response:** Array of items below minimum threshold

---

### 4. **Stock Management Endpoints**

#### Stock In (Add Stock)
**URL:** `POST /api/Stock/stock-in/{itemId}`
```
https://inventorysystem88-e5baekb2dtegbqeu.centralindia-01.azurewebsites.net/api/Stock/stock-in/1
```
**Body:**
```json
{
  "quantity": 50,
  "notes": "Restocking from supplier"
}
```

#### Stock Out (Remove Stock)
**URL:** `POST /api/Stock/stock-out/{itemId}`
```
https://inventorysystem88-e5baekb2dtegbqeu.centralindia-01.azurewebsites.net/api/Stock/stock-out/1
```
**Body:**
```json
{
  "quantity": 10,
  "notes": "Sold to customer"
}
```

#### Get Transaction History
**URL:** `GET /api/Stock/transactions/{itemId}`
```
https://inventorysystem88-e5baekb2dtegbqeu.centralindia-01.azurewebsites.net/api/Stock/transactions/1
```
**Response:** Array of stock transactions for the item

---

## ?? Testing Methods

### Method 1: Using Your Browser
1. Open any GET endpoint URL in your browser
2. You should see JSON response data

### Method 2: Using Scalar Documentation (Recommended)
1. Navigate to: `https://inventorysystem88-e5baekb2dtegbqeu.centralindia-01.azurewebsites.net/scalar/v1`
2. You can test all endpoints interactively with a built-in UI

### Method 3: Using PowerShell
```powershell
# Test root endpoint
Invoke-RestMethod -Uri "https://inventorysystem88-e5baekb2dtegbqeu.centralindia-01.azurewebsites.net/" -Method GET

# Test get all inventory
Invoke-RestMethod -Uri "https://inventorysystem88-e5baekb2dtegbqeu.centralindia-01.azurewebsites.net/api/Inventory" -Method GET

# Create a new item
$body = @{
    name = "Test Item"
    description = "Created via PowerShell"
    quantity = 50
    minStockThreshold = 10
    price = 25.99
} | ConvertTo-Json

Invoke-RestMethod -Uri "https://inventorysystem88-e5baekb2dtegbqeu.centralindia-01.azurewebsites.net/api/Inventory" `
    -Method POST `
    -Body $body `
    -ContentType "application/json"
```

### Method 4: Using cURL
```bash
# Test root endpoint
curl https://inventorysystem88-e5baekb2dtegbqeu.centralindia-01.azurewebsites.net/

# Test get all inventory
curl https://inventorysystem88-e5baekb2dtegbqeu.centralindia-01.azurewebsites.net/api/Inventory

# Create a new item
curl -X POST https://inventorysystem88-e5baekb2dtegbqeu.centralindia-01.azurewebsites.net/api/Inventory \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Test Item",
    "description": "Created via cURL",
    "quantity": 50,
    "minStockThreshold": 10,
    "price": 25.99
  }'
```

### Method 5: Using Postman
1. Import the OpenAPI specification from `/scalar/v1`
2. Or manually create requests for each endpoint above

---

## ?? Troubleshooting

### Issue: "Cannot connect to the database"
**Solution:** Make sure you've configured the connection string in Azure App Service Configuration (see DEPLOYMENT_CHECKLIST.md)

### Issue: "Empty response from GET /api/Inventory"
**Solution:** This is normal if you haven't created any items yet. Use POST to create your first item.

### Issue: "CORS error" from Angular frontend
**Solution:** Update the CORS policy in Program.cs to include your Angular app's production URL

### Issue: "404 Not Found" on specific endpoints
**Solution:** 
1. Check the URL spelling
2. Ensure you're using the correct HTTP method (GET, POST, PUT, DELETE)
3. Verify the endpoint exists in the controller

---

## ? Quick Verification Checklist

Test these in order to verify your deployment:

1. **[ ]** Access root endpoint: `https://inventorysystem88-e5baekb2dtegbqeu.centralindia-01.azurewebsites.net/`
   - Should return JSON with API information

2. **[ ]** Access Scalar docs: `https://inventorysystem88-e5baekb2dtegbqeu.centralindia-01.azurewebsites.net/scalar/v1`
   - Should show interactive API documentation

3. **[ ]** GET all inventory: `https://inventorysystem88-e5baekb2dtegbqeu.centralindia-01.azurewebsites.net/api/Inventory`
   - Should return empty array `[]` or list of items

4. **[ ]** POST create item using Scalar UI
   - Should return created item with ID

5. **[ ]** GET the created item by ID
   - Should return the item details

6. **[ ]** POST stock-in operation
   - Should increase quantity

7. **[ ]** GET transaction history
   - Should show the stock-in transaction

---

## ?? Connect Your Angular Frontend

Update your Angular service to use this base URL:

```typescript
// inventory.service.ts
private apiUrl = 'https://inventorysystem88-e5baekb2dtegbqeu.centralindia-01.azurewebsites.net/api/Inventory';

// stock.service.ts
private apiUrl = 'https://inventorysystem88-e5baekb2dtegbqeu.centralindia-01.azurewebsites.net/api/Stock';
```

Don't forget to update CORS in Program.cs with your Angular app's deployed URL!

---

## ?? Success Indicators

Your API is working correctly if:
- ? Root URL returns API information JSON
- ? `/scalar/v1` shows documentation
- ? `/api/Inventory` returns data (or empty array)
- ? You can create, read, update, and delete items
- ? Stock operations work and create transactions

---

**Your API is deployed and ready to use!** ??

The 404 you saw at the root was expected behavior for a Web API. Now you have a proper root endpoint that provides API information.
