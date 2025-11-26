# Frontend-Backend Communication Architecture

## Overview

The application uses an **ASP.NET Core MVC + Separate API Architecture** with two parallel communication paths:

1. **Traditional MVC Path** - Server-rendered views via Razor (for traditional web interactions)
2. **API Path** - RESTful JSON endpoints (for client-side data loading and AJAX operations)

---

## Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                        FRONTEND (Browser)                        │
├─────────────────────────────────────────────────────────────────┤
│  HTML Views (Razor .cshtml)                                     │
│  ├── Product/Index.cshtml                                       │
│  ├── Product/Upsert.cshtml                                      │
│  ├── Order/Index.cshtml                                         │
│  ├── Category/Index.cshtml                                      │
│  └── ... other views                                            │
│                                                                  │
│  JavaScript Files (jQuery + AJAX)                               │
│  ├── product.js (DataTables + AJAX calls)                       │
│  ├── order.js (DataTables + dynamic status filtering)           │
│  ├── user.js (Lock/Unlock, permission management)               │
│  ├── ancestral.js                                               │
│  ├── kindness.js                                                │
│  └── EventRegistration.js                                       │
│                                                                  │
│  CSS & Libraries                                                │
│  ├── Bootstrap 5                                                │
│  ├── DataTables (jQuery plugin)                                 │
│  ├── SweetAlert2 (confirmation dialogs)                         │
│  ├── Toastr (notifications)                                     │
│  └── Icons (Bootstrap Icons, Font Awesome)                      │
└─────────────────────────────────────────────────────────────────┘
                              ↕
                    HTTP Requests/Responses
                              ↕
┌─────────────────────────────────────────────────────────────────┐
│                  BACKEND (ASP.NET Core 8.0)                     │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │  MVC CONTROLLERS (Traditional Views)                     │  │
│  │  └── Areas/Admin/Controllers/                           │  │
│  │      ├── ProductController.cs                           │  │
│  │      │   └── Index()       → renders Razor view         │  │
│  │      │   └── Upsert()      → form handling              │  │
│  │      ├── OrderController.cs                             │  │
│  │      ├── CategoryController.cs                          │  │
│  │      ├── UserController.cs                              │  │
│  │      └── ...                                            │  │
│  └──────────────────────────────────────────────────────────┘  │
│                                                                  │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │  API CONTROLLERS (JSON Endpoints)                        │  │
│  │  └── Areas/Admin/Controllers/                           │  │
│  │      ├── ProductApiController.cs                        │  │
│  │      │   └── GET /api/admin/product                     │  │
│  │      │   └── GET /api/admin/product/{id}                │  │
│  │      │   └── POST /api/admin/product                    │  │
│  │      │   └── PUT /api/admin/product/{id}                │  │
│  │      │   └── DELETE /api/admin/product/{id}             │  │
│  │      ├── OrderApiController.cs                          │  │
│  │      ├── CategoryApiController.cs                       │  │
│  │      └── ...                                            │  │
│  └──────────────────────────────────────────────────────────┘  │
│                                                                  │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │  BUSINESS LOGIC (Repository Pattern + UnitOfWork)       │  │
│  │  └── Bulky.DataAccess/Repository/                       │  │
│  │      ├── UnitOfWork.cs                                  │  │
│  │      ├── ProductRepository.cs                           │  │
│  │      ├── OrderRepository.cs                             │  │
│  │      ├── CategoryRepository.cs                          │  │
│  │      └── ...                                            │  │
│  └──────────────────────────────────────────────────────────┘  │
│                                                                  │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │  DATA ACCESS (Entity Framework Core + SQLite/SQLServer) │  │
│  │  └── ApplicationDbContext                               │  │
│  │      └── SQLite Database (sqlite.db)                    │  │
│  └──────────────────────────────────────────────────────────┘  │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

---

## Communication Paths

### Path 1: Traditional MVC Rendering

**Flow:**

```
User clicks link (e.g., /admin/product)
    ↓
MVC Controller receives request (ProductController)
    ↓
Controller method (e.g., Index()) executes
    ↓
Controller retrieves data via UnitOfWork/Repository
    ↓
Data passed to Razor view (View model)
    ↓
View renders HTML server-side
    ↓
Complete HTML page sent to browser
```

**Example: Product Index Page**

```csharp
// ProductController.cs (MVC)
[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class ProductController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public ProductController(IUnitOfWork unitOfWork, ...)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        // Fetch data from database
        List<Product> objProductList = _unitOfWork.Product.GetAll(
            includeProperties: "Category,Company"
        ).ToList();

        // Set role in TempData
        if (User.IsInRole(SD.Role_Admin))
            TempData["Role"] = SD.Role_Admin;

        // Return view with data
        return View(objProductList);
    }
}
```

**Corresponding View (Razor):**

```html
<!-- Areas/Admin/Views/Product/Index.cshtml -->
@model List<Product>
  <div class="card shadow border-0 my-4">
    <div class="card-header bg-secondary bg-gradient ml-0 py-3">
      <div class="row">
        <div class="col-12 text-center">
          <h2 class="text-white py-2">活動清單</h2>
        </div>
      </div>
    </div>
    <div class="card-body p-4">
      <table id="tblData" class="table table-bordered table-striped">
        <thead>
          <tr>
            <th>名稱</th>
            <th>舉辦(是Y/否N)</th>
            <th>日期</th>
            <th></th>
          </tr>
        </thead>
      </table>
    </div>
  </div>

  @section Scripts {
  <script src="~/js/product.js"></script>
  }</Product
>
```

---

### Path 2: AJAX + API Data Loading

**Flow:**

```
User views page with DataTable
    ↓
JavaScript (product.js) initializes on page load
    ↓
jQuery DataTables plugin makes AJAX call
    ↓
Request: GET /admin/product/getall
    ↓
Routing directs to API Controller method
    ↓
API Controller (ProductApiController) processes request
    ↓
Controller queries database via UnitOfWork
    ↓
Controller returns JSON response
    ↓
jQuery/DataTables receives JSON
    ↓
JavaScript renders table rows dynamically
    ↓
User sees populated data table
```

**Example: Product Data Loading**

```javascript
// wwwroot/js/product.js
var dataTable;

$(document).ready(function () {
  loadDataTable();
});

function loadDataTable() {
  dataTable = $("#tblData").DataTable({
    // AJAX Configuration - calls /admin/product/getall
    ajax: { url: "/admin/product/getall" },

    columns: [
      { data: "title", width: "20%" },
      {
        data: "heldYN",
        render: function (data) {
          // Custom rendering logic
          let color = data == "Y" ? "green" : "red";
          return '<span style="color:' + color + '">' + data + "</span>";
        },
        width: "10%",
      },
      { data: "hDate", width: "10%" },
      {
        data: "id",
        render: function (data) {
          // Render action buttons
          return `<div class="w-75 btn-group fw-bolder" role="group">
                        <a href="/admin/product/upsert?id=${data}" class="btn btn-primary mx-2"> 
                            <i class="bi bi-pencil-square"></i> Edit
                        </a>               
                        <a onClick=Delete('/admin/product/delete/${data}') class="btn btn-danger mx-2"> 
                            <i class="bi bi-trash-fill"></i> Delete
                        </a>
                    </div>`;
        },
        width: "20%",
      },
    ],
  });
}

// DELETE via AJAX
function Delete(url) {
  Swal.fire({
    title: "Are you sure?",
    icon: "warning",
    showCancelButton: true,
    confirmButtonText: "Yes, delete it!",
  }).then((result) => {
    if (result.isConfirmed) {
      $.ajax({
        url: url,
        type: "DELETE",
        success: function (data) {
          dataTable.ajax.reload(); // Reload table
          toastr.success(data.message);
        },
      });
    }
  });
}
```

**Corresponding API Endpoint:**

```csharp
// ProductApiController.cs
[ApiController]
[Route("api/admin/product")]
public class ProductApiController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public ProductApiController(IUnitOfWork unitOfWork, ...)
    {
        _unitOfWork = unitOfWork;
    }

    // GET /api/admin/product or /admin/product/getall (routing)
    [HttpGet]
    public IActionResult GetAll([FromQuery] string? search)
    {
        IEnumerable<Product> list;

        if (!string.IsNullOrWhiteSpace(search))
            list = _unitOfWork.Product.GetAll(
                filter: p => p.Title != null && p.Title.Contains(search),
                includeProperties: "Category,Company"
            ).ToList();
        else
            list = _unitOfWork.Product.GetAll(
                includeProperties: "Category,Company"
            ).ToList();

        // Return JSON response
        return Ok(new { data = list });
    }

    // GET /api/admin/product/{id}
    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        var item = _unitOfWork.Product.Get(
            u => u.Id == id,
            includeProperties: "Category,Company,ProductImages"
        );
        if (item == null) return NotFound();
        return Ok(item);
    }

    // POST /api/admin/product
    [HttpPost]
    public IActionResult Create([FromBody] Product obj)
    {
        if (obj == null) return BadRequest();
        _unitOfWork.Product.Add(obj);
        _unitOfWork.Save();
        return Ok(new { success = true, data = obj });
    }

    // PUT /api/admin/product/{id}
    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] Product obj)
    {
        var existing = _unitOfWork.Product.Get(u => u.Id == id);
        if (existing == null) return NotFound();

        _unitOfWork.Product.Update(obj);
        _unitOfWork.Save();
        return Ok(new { success = true, data = obj });
    }

    // DELETE /api/admin/product/{id}
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var item = _unitOfWork.Product.Get(u => u.Id == id);
        if (item == null) return NotFound();

        _unitOfWork.Product.Remove(item);
        _unitOfWork.Save();
        return Ok(new { success = true, message = "Deleted successfully" });
    }
}
```

---

## Key Controllers & API Routes

### Admin Area Controllers

#### **ProductController** (MVC) + **ProductApiController** (API)

| Operation   | MVC Route                        | API Route                        | Purpose                              |
| ----------- | -------------------------------- | -------------------------------- | ------------------------------------ |
| View List   | `GET /admin/product`             | `GET /api/admin/product`         | Display index page / Load table data |
| Get Item    | -                                | `GET /api/admin/product/{id}`    | Fetch single product                 |
| Create Form | `GET /admin/product/upsert`      | -                                | Show create form                     |
| Create Item | `POST /admin/product/upsert`     | `POST /api/admin/product`        | Create new product                   |
| Edit Form   | `GET /admin/product/upsert?id=1` | -                                | Show edit form                       |
| Update Item | `POST /admin/product/upsert`     | `PUT /api/admin/product/{id}`    | Update existing product              |
| Delete Item | `GET /admin/product/delete/{id}` | `DELETE /api/admin/product/{id}` | Delete product                       |

#### **OrderController** (MVC) + **OrderApiController** (API)

```javascript
// order.js - Dynamic status filtering
function loadDataTable(status) {
  dataTable = $("#tblData").DataTable({
    ajax: { url: "/admin/order/getall?status=" + status },
    columns: [
      { data: "id", width: "5%" },
      { data: "name", width: "25%" },
      { data: "phoneNumber", width: "20%" },
      { data: "applicationUser.email", width: "20%" },
      { data: "orderStatus", width: "10%" },
      { data: "orderTotal", width: "10%" },
      // ... action buttons
    ],
  });
}
```

#### **UserController** (MVC) + **UserApiController** (API)

```javascript
// user.js - Lock/Unlock user
function LockUnlock(id) {
  $.ajax({
    type: "POST",
    url: "/Admin/User/LockUnlock",
    data: JSON.stringify(id),
    contentType: "application/json",
    success: function (data) {
      if (data.success) {
        toastr.success(data.message);
        dataTable.ajax.reload();
      }
    },
  });
}
```

#### **CategoryApiController**, **CompanyApiController**, **KindnessApiController**, **AncestralApiController**

All follow the same pattern:

- `GET /api/admin/{resource}` - Get all items
- `GET /api/admin/{resource}/{id}` - Get single item
- `POST /api/admin/{resource}` - Create
- `PUT /api/admin/{resource}/{id}` - Update
- `DELETE /api/admin/{resource}/{id}` - Delete
- `POST /api/admin/{resource}/deleterange` - Batch delete
- `POST /api/admin/{resource}/deleteall` - Delete all

---

## Data Flow Example: Creating a Product

### Step 1: User clicks "Add Product" Button

```html
<a asp-controller="Product" asp-action="Upsert" class="btn btn-primary">
  <i class="bi bi-plus-circle"></i> 新增活動
</a>
```

### Step 2: GET /admin/product/upsert (MVC)

```csharp
public IActionResult Upsert(int? id)
{
    ProductVM productVM = new()
    {
        CategoryList = _unitOfWork.Category.GetAll()
            .Select(u => new SelectListItem { Text = u.Name, Value = u.Id.ToString() }),
        CompanyList = _unitOfWork.Company.GetAll()
            .Select(u => new SelectListItem { Text = u.Name, Value = u.Id.ToString() }),
        Product = new Product()
    };
    return View(productVM);
}
```

### Step 3: User fills form and submits

```html
<!-- Areas/Admin/Views/Product/Upsert.cshtml -->
<form asp-action="Upsert" enctype="multipart/form-data" method="POST">
  <!-- Form fields bound to ProductVM.Product -->
  <input asp-for="Product.Title" class="form-control" />
  <input asp-for="Product.Description" class="form-control" />
  <select asp-for="Product.CategoryId" asp-items="Model.CategoryList"></select>
  <!-- ... more fields ... -->
  <button type="submit" class="btn btn-primary">Save</button>
</form>
```

### Step 4: POST /admin/product/upsert (MVC Form Submission)

```csharp
[HttpPost]
public IActionResult Upsert(ProductVM productVM, List<IFormFile> files)
{
    if (ModelState.IsValid)
    {
        if (productVM.Product.Id == 0) {
            _unitOfWork.Product.Add(productVM.Product);
        } else {
            _unitOfWork.Product.Update(productVM.Product);
        }
        _unitOfWork.Save();
        // ... file handling ...
        return RedirectToAction(nameof(Index));
    }
    return View(productVM);
}
```

### Step 5: Database Updated (Entity Framework Core)

```
UnitOfWork.Product.Add(obj)
    ↓
ProductRepository.Add(obj)
    ↓
DbContext.Products.Add(obj)
    ↓
DbContext.SaveChanges()
    ↓
SQLite/SQLServer Database Updated
```

### Step 6: User redirected to index, data loads via AJAX

```javascript
// product.js triggers on page load
loadDataTable(); // → GET /api/admin/product → returns JSON → DataTables renders
```

---

## Authentication & Authorization

### Identity Setup (Program.cs)

```csharp
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});
```

### Role-Based Access Control

```csharp
// Controllers require admin role
[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class ProductController : Controller { ... }

// API Controllers (similar protection)
[ApiController]
[Route("api/admin/product")]
public class ProductApiController : ControllerBase { ... }
```

### Roles

- `Admin` - Full system access
- `Company` - Company-level management
- `Employee` - Staff member
- `Customer` - End user

---

## Dependency Injection & Service Registration

### Services configured in Program.cs

```csharp
// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// Authentication
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Repository Pattern
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Session & Email
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => options.IdleTimeout = TimeSpan.FromMinutes(100));
builder.Services.AddScoped<IEmailSender, EmailSender>();

// Controllers with Views
builder.Services.AddControllersWithViews();
```

### Constructor Injection Pattern

```csharp
public class ProductController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IConfiguration _configuration;

    // Services injected via constructor
    public ProductController(
        IUnitOfWork unitOfWork,
        IWebHostEnvironment webHostEnvironment,
        IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _webHostEnvironment = webHostEnvironment;
        _configuration = configuration;
    }
}
```

---

## Frontend Technologies & Libraries

### HTML & CSS

- **Bootstrap 5** - Responsive UI framework
- **Bootstrap Icons** - Icon library

### JavaScript Libraries

- **jQuery** - DOM manipulation, AJAX
- **DataTables** - Server-driven/client-driven tables
- **SweetAlert2** - Confirmation dialogs
- **Toastr** - Toast notifications

### Form Handling

- **Razor Tag Helpers** - `asp-for`, `asp-controller`, `asp-action`
- **ASP.NET Core Model Binding** - Automatic form-to-model mapping

---

## Request/Response Patterns

### Successful AJAX Response

```json
{
  "data": [
    {
      "id": 1,
      "title": "Family Reunion 2024",
      "heldYN": "Y",
      "hDate": "2024-08-15",
      "category": { "id": 1, "name": "Event" },
      "company": { "id": 1, "name": "ChenClan" }
    }
  ]
}
```

### API Success Response

```json
{
  "success": true,
  "message": "Deleted successfully",
  "data": {
    /* entity */
  }
}
```

### Error Response

```json
{
  "success": false,
  "message": "Product not found"
}
```

---

## Routing Configuration

```csharp
// Program.cs - Default route pattern
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index1}/{id?}"
);
```

### Route Examples

| URL                       | Area     | Controller | Action | Purpose                 |
| ------------------------- | -------- | ---------- | ------ | ----------------------- |
| `/admin/product`          | Admin    | Product    | Index  | List products           |
| `/admin/product/upsert`   | Admin    | Product    | Upsert | Create/edit product     |
| `/admin/product/delete/1` | Admin    | Product    | Delete | Delete product          |
| `/api/admin/product`      | Admin    | ProductApi | GetAll | API: Get all products   |
| `/api/admin/product/1`    | Admin    | ProductApi | Get    | API: Get single product |
| `/customer/home`          | Customer | Home       | Index1 | Customer home page      |

---

## Integration Test Coverage

The test suite validates the frontend-backend connection:

### GET Endpoints (Read Operations)

- ✅ `GET /api/admin/category` - Retrieve categories
- ✅ `GET /api/admin/product` - Retrieve products with related data
- ✅ `GET /api/admin/kindness` - Retrieve kindness positions
- ✅ `GET /api/admin/ancestral` - Retrieve ancestral positions

### DELETE Endpoints (Delete Operations)

- ✅ `DELETE /api/admin/category/{id}` - Delete single category
- ✅ `POST /api/admin/category/deleterange` - Batch delete with validation
- ✅ `POST /api/admin/ancestral/deleteall` - Delete all records

### Search & Filter

- ✅ `GET /api/admin/category?search=keyword` - Search functionality
- ✅ Query parameter handling with null checks

### Related Data Loading

- ✅ Products with Category and Company relationships
- ✅ Orders with ApplicationUser relationships

**Test Results:** 11/11 passing ✅

---

## Summary

### Two-Path Architecture

1. **Traditional MVC**

   - Server-side rendering
   - Full-page requests
   - Form submissions
   - Best for: Page navigation, form workflows

2. **RESTful API**
   - JSON responses
   - AJAX/client-side rendering
   - DataTables integration
   - Best for: List views, CRUD operations, dynamic updates

### Communication Stack

```
Browser (HTML/CSS/JS)
    ↓
HTTP Requests (GET/POST/PUT/DELETE)
    ↓
ASP.NET Core Routing
    ↓
Controllers (MVC or API)
    ↓
Business Logic (UnitOfWork Pattern)
    ↓
Entity Framework Core
    ↓
SQLite/SQLServer Database
```

### Key Interactions

- **List Page Loads** → MVC Controller returns view with empty table
- **Table Data Loads** → AJAX calls API endpoint, JSON returned
- **User Edits Data** → Form submits to MVC controller or AJAX to API
- **Delete Operation** → SweetAlert confirmation → AJAX DELETE to API
- **Success/Error** → Toastr notification + DataTable reload (if AJAX) or redirect (if MVC)
