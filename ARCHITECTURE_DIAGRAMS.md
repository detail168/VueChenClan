# BulkyBook Architecture Diagrams

## 1. Current MVC Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                        Client Browser                        │
│  User Interface (Server-Side Rendered HTML)                 │
└────────────────────────┬────────────────────────────────────┘
                         │
                    HTTP(S) Request/Response
                         │
                         ▼
┌─────────────────────────────────────────────────────────────┐
│                    ASP.NET Core 8 MVC                        │
│                                                               │
│  ┌──────────────────────────────────────────────────────┐   │
│  │           MVC Controllers                             │   │
│  │  ProductController, OrderController, UserController  │   │
│  └──────────────┬───────────────────────────────────────┘   │
│                 │                                             │
│  ┌──────────────▼───────────────────────────────────────┐   │
│  │        Razor Views & Templates                        │   │
│  │  Generate HTML with embedded data from Model         │   │
│  └──────────────┬───────────────────────────────────────┘   │
│                 │                                             │
│  ┌──────────────▼───────────────────────────────────────┐   │
│  │     Business Logic & Service Layer                    │   │
│  └──────────────┬───────────────────────────────────────┘   │
└─────────────────┼──────────────────────────────────────────┘
                  │
         ┌────────┴────────┐
         │                 │
         ▼                 ▼
    ┌─────────┐      ┌────────────┐
    │  SQLite │      │ SQL Server │
    │   DB    │      │  Azure SQL │
    └─────────┘      └────────────┘

Session-Based Authentication (Cookie)
```

---

## 2. Target Vue.js + API Architecture

```
┌──────────────────────────────────────────────────────────────┐
│                    Vue.js SPA (Browser)                      │
│                                                               │
│  ┌────────────────────────────────────────────────────────┐  │
│  │          Vue 3 Components                               │  │
│  │  ProductList, ProductForm, ShoppingCart, etc.          │  │
│  └────────────────────┬─────────────────────────────────┘  │
│                       │                                      │
│  ┌────────────────────▼─────────────────────────────────┐  │
│  │     Vue Router (Client-side Routing)                  │  │
│  └────────────────────┬─────────────────────────────────┘  │
│                       │                                      │
│  ┌────────────────────▼─────────────────────────────────┐  │
│  │     Pinia State Management Store                      │  │
│  │  (User, Cart, Products, Orders)                       │  │
│  └────────────────────┬─────────────────────────────────┘  │
│                       │                                      │
│  ┌────────────────────▼─────────────────────────────────┐  │
│  │      API Client (Axios/Fetch)                        │  │
│  └────────────────────┬─────────────────────────────────┘  │
└───────────────────────┼──────────────────────────────────┘
                        │
                 JSON/REST over HTTPS
                 JWT Bearer Token (LocalStorage)
                        │
┌───────────────────────▼──────────────────────────────────────┐
│               ASP.NET Core 8 Web API                          │
│                                                               │
│  ┌────────────────────────────────────────────────────────┐  │
│  │       API Controllers (RESTful Endpoints)               │  │
│  │  ProductsController, OrdersController, etc.             │  │
│  │  GET/POST/PUT/DELETE routes                            │  │
│  └────────────────────┬─────────────────────────────────┘  │
│                       │                                      │
│  ┌────────────────────▼─────────────────────────────────┐  │
│  │     Data Transfer Objects (DTOs)                      │  │
│  └────────────────────┬─────────────────────────────────┘  │
│                       │                                      │
│  ┌────────────────────▼─────────────────────────────────┐  │
│  │    Business Logic & Service Layer                     │  │
│  │    UnitOfWork Repository Pattern                      │  │
│  └────────────────────┬─────────────────────────────────┘  │
└───────────────────────┼──────────────────────────────────┘
                        │
         ┌──────────────┴──────────────┐
         │                             │
         ▼                             ▼
    ┌─────────┐                 ┌────────────┐
    │  SQLite │                 │ SQL Server │
    │   DB    │                 │  Azure SQL │
    └─────────┘                 └────────────┘

Token-Based Authentication (JWT in LocalStorage)
```

---

## 3. Permission & Authorization Flow

```
┌──────────────────────────────────────────────────────────────┐
│                   User Login                                  │
└──────────────────────┬───────────────────────────────────────┘
                       │
                       ▼
            ┌────────────────────┐
            │  Verify Credentials │
            │  (Database Check)   │
            └────────────┬────────┘
                         │
           ┌─────────────┴──────────────┐
           │                            │
      Valid                         Invalid
           │                            │
           ▼                            ▼
    ┌────────────────┐        ┌──────────────────┐
    │ Query User Roles│        │ Return 401/403   │
    │ (AspNetUserRole)│        │ Unauthorized     │
    └────────┬────────┘        └──────────────────┘
             │
             ▼
    ┌────────────────────┐
    │ Generate JWT Token │
    │ (Include Role Claims)
    └────────┬────────────┘
             │
             ▼
    ┌────────────────────────────────────────┐
    │ Return Token to Frontend               │
    │ (Stored in LocalStorage)               │
    └────────┬───────────────────────────────┘
             │
   ┌─────────┴──────────────────────────────┐
   │   Each API Request                     │
   │   Header: Authorization: Bearer <token>│
   └─────────┬──────────────────────────────┘
             │
             ▼
    ┌────────────────────────────────────────┐
    │ API Middleware Validates Token        │
    │ [Authorize(Roles="...")]              │
    └─────────┬──────────────────────────────┘
              │
     ┌────────┴────────┐
     │                 │
  Valid              Invalid
     │                 │
     ▼                 ▼
 Process           Return 401
 Request           Unauthorized


RBAC Matrix:
┌───────────────┬──────────┬──────────┬──────────┬──────────┐
│  Feature      │Admin管理員│Employee幹部│Company宗親│Customer親友│
├───────────────┼──────────┼──────────┼──────────┼──────────┤
│ User Mgmt     │    ✓✓    │    ✓     │    ✗     │    ✗     │
│ Product CRUD  │    ✓     │    ✓     │    ✓*    │    ✗     │
│ Order View    │    ✓     │    ✓     │    ✓*    │    ✓     │
│ Browse Events │    ✓     │    ✓     │    ✓     │    ✓     │
│ Register Event│    ✓     │    ✓     │    ✓     │    ✓     │
│ Shopping Cart │    ✓     │    ✓     │    ✓     │    ✓     │
│ Payment       │    ✓     │    ✓     │    ✓     │    ✓     │
│ Heritage Mgmt │    ✓     │    ✓     │    ✓     │    ✗     │
│ Reports       │    ✓     │    ✗     │    ✗     │    ✗     │
└───────────────┴──────────┴──────────┴──────────┴──────────┘
* = Company can only manage own company's products
```

---

## 4. API Request/Response Cycle

```
┌─ Client (Vue.js) ─────────────────────────────────────────┐
│                                                            │
│  ┌──────────────────────────────────────────────────────┐ │
│  │ Vue Component (e.g., ProductForm.vue)               │ │
│  │                                                      │ │
│  │  <form @submit="submitProduct">                    │ │
│  │    <input v-model="form.title" />                 │ │
│  │  </form>                                           │ │
│  └──────────────┬───────────────────────────────────────┘ │
│                 │                                          │
│  ┌──────────────▼───────────────────────────────────────┐ │
│  │ API Call (Pinia Action)                             │ │
│  │                                                      │ │
│  │  productStore.addProduct({                         │ │
│  │    title: form.title,                             │ │
│  │    description: form.description                  │ │
│  │  })                                                │ │
│  └──────────────┬───────────────────────────────────────┘ │
│                 │                                          │
│  ┌──────────────▼───────────────────────────────────────┐ │
│  │ HTTP Request                                        │ │
│  │                                                      │ │
│  │ POST /api/products                                 │ │
│  │ Authorization: Bearer eyJhbGc...                   │ │
│  │ Content-Type: application/json                     │ │
│  │                                                      │ │
│  │ {                                                  │ │
│  │   "title": "Annual Gathering 2025",               │ │
│  │   "description": "..."                            │ │
│  │ }                                                  │ │
│  └──────────────┬───────────────────────────────────────┘ │
└─────────────────┼────────────────────────────────────────┘
                  │
         Network Transmission
                  │
┌─────────────────▼────────────────────────────────────────┐
│ Server (ASP.NET Core API)                               │
│                                                          │
│  ┌────────────────────────────────────────────────────┐ │
│  │ Authentication Middleware                          │ │
│  │ Validates JWT Token                               │ │
│  └────────────┬─────────────────────────────────────┘ │
│               │                                        │
│  ┌────────────▼─────────────────────────────────────┐ │
│  │ Authorization Filter                             │ │
│  │ Checks [Authorize(Roles="...")]                 │ │
│  └────────────┬─────────────────────────────────────┘ │
│               │                                        │
│  ┌────────────▼─────────────────────────────────────┐ │
│  │ ProductsController.Post()                        │ │
│  │ [HttpPost]                                       │ │
│  │ public IActionResult Post(CreateProductDTO dto) │ │
│  │ {                                                │ │
│  │   // Validate input                             │ │
│  │   // Save to database                           │ │
│  │   // Return response                            │ │
│  │ }                                                │ │
│  └────────────┬─────────────────────────────────────┘ │
│               │                                        │
│  ┌────────────▼─────────────────────────────────────┐ │
│  │ Business Logic Layer                            │ │
│  │ (Services, UnitOfWork)                          │ │
│  └────────────┬─────────────────────────────────────┘ │
│               │                                        │
│  ┌────────────▼─────────────────────────────────────┐ │
│  │ Database Operation                              │ │
│  └────────────┬─────────────────────────────────────┘ │
│               │                                        │
│  ┌────────────▼─────────────────────────────────────┐ │
│  │ HTTP Response                                   │ │
│  │                                                 │ │
│  │ 201 Created                                    │ │
│  │ Content-Type: application/json                │ │
│  │                                                 │ │
│  │ {                                              │ │
│  │   "success": true,                            │ │
│  │   "data": {                                   │ │
│  │     "id": 42,                                │ │
│  │     "title": "Annual Gathering 2025",        │ │
│  │     "createdDate": "2025-11-25T10:30:00Z"   │ │
│  │   },                                          │ │
│  │   "message": "Product created successfully"  │ │
│  │ }                                              │ │
│  └────────────┬─────────────────────────────────┘ │
└─────────────────┼────────────────────────────────────┘
                  │
         Network Transmission
                  │
┌─────────────────▼────────────────────────────────────┐
│ Client (Vue.js) - Handle Response               │
│                                                  │
│  ┌────────────────────────────────────────────┐ │
│  │ Parse JSON response                        │ │
│  │ Update Pinia store                         │ │
│  │ Show success notification                 │ │
│  │ Redirect or refresh component              │ │
│  └────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────┘
```

---

## 5. File Organization - Vue Project Structure

```
vue-bulkybook/
├── public/                              # Static files
│   ├── favicon.ico
│   └── index.html
│
├── src/
│   ├── assets/                          # Images, fonts, styles
│   │   ├── images/
│   │   ├── styles/
│   │   └── fonts/
│   │
│   ├── components/                      # Reusable Vue components
│   │   ├── common/
│   │   │   ├── Header.vue
│   │   │   ├── Sidebar.vue
│   │   │   ├── Footer.vue
│   │   │   └── Loading.vue
│   │   │
│   │   ├── admin/
│   │   │   ├── ProductList.vue
│   │   │   ├── ProductForm.vue
│   │   │   ├── UserManagement.vue
│   │   │   ├── OrderManagement.vue
│   │   │   ├── CategoryManagement.vue
│   │   │   ├── KindnessLayout.vue
│   │   │   └── RoleManagement.vue
│   │   │
│   │   └── customer/
│   │       ├── ProductCatalog.vue
│   │       ├── ProductDetails.vue
│   │       ├── ShoppingCart.vue
│   │       ├── Checkout.vue
│   │       ├── OrderConfirmation.vue
│   │       ├── EventRegistration.vue
│   │       └── MyOrders.vue
│   │
│   ├── stores/                          # Pinia state management
│   │   ├── authStore.ts                 # User auth state
│   │   ├── cartStore.ts                 # Shopping cart state
│   │   ├── productStore.ts              # Products state
│   │   ├── orderStore.ts                # Orders state
│   │   └── notificationStore.ts         # UI notifications
│   │
│   ├── services/                        # API services
│   │   ├── api.ts                       # Axios instance & config
│   │   ├── authService.ts
│   │   ├── productService.ts
│   │   ├── cartService.ts
│   │   ├── orderService.ts
│   │   └── userService.ts
│   │
│   ├── router/                          # Vue Router config
│   │   ├── index.ts
│   │   ├── adminRoutes.ts
│   │   ├── customerRoutes.ts
│   │   └── authRoutes.ts
│   │
│   ├── types/                           # TypeScript types/interfaces
│   │   ├── Product.ts
│   │   ├── Order.ts
│   │   ├── User.ts
│   │   ├── Cart.ts
│   │   └── common.ts
│   │
│   ├── utils/                           # Utility functions
│   │   ├── validators.ts
│   │   ├── formatters.ts
│   │   ├── constants.ts
│   │   └── helpers.ts
│   │
│   ├── middleware/                      # Route guards, interceptors
│   │   ├── authGuard.ts
│   │   ├── roleGuard.ts
│   │   └── tokenRefresh.ts
│   │
│   ├── App.vue                          # Root component
│   └── main.ts                          # Entry point
│
├── tests/                               # Unit & integration tests
│   ├── unit/
│   ├── integration/
│   └── e2e/
│
├── .env.development                     # Dev environment vars
├── .env.production                      # Prod environment vars
├── vite.config.ts                       # Vite bundler config
├── tsconfig.json                        # TypeScript config
├── package.json                         # Dependencies
├── README.md
└── docker/
    └── Dockerfile                       # Docker build config
```

---

## 6. Database Connection Options

```
┌────────────────────────────────────────────────────────────┐
│           Database Connection Strategy                      │
├────────────────────────────────────────────────────────────┤
│                                                             │
│  Development Environment                                   │
│  ├─ SQLite (Local)                                         │
│  │   └─ "Data Source=sqlite.db"                           │
│  │                                                          │
│  └─ appsettings.Development.json                           │
│      └─ ConnectionStrings:DefaultConnection               │
│                                                             │
│  Staging/Testing Environment                              │
│  ├─ SQL Server (LocalDB)                                  │
│  │   └─ "Server=(LocalDb)\\MSSQLLocalDB;Database=Bulky"   │
│  │                                                          │
│  └─ appsettings.Staging.json                              │
│      └─ ConnectionStrings:DefaultConnection_MSSQLLocalDB  │
│                                                             │
│  Production Environment (Azure)                           │
│  ├─ Azure SQL Database                                    │
│  │   └─ "Server=tcp:pocsrv.database.windows.net"         │
│  │      "Authentication=Active Directory Default"        │
│  │      "Database=pocdb"                                  │
│  │                                                          │
│  └─ appsettings.Production.json                           │
│      └─ ConnectionStrings:DefaultConnection_Azure         │
│         (Injected via Environment Variables/Secrets)      │
│                                                             │
│  Environment-Specific Configuration                        │
│  ├─ Connection String Selection                            │
│  │   if (connectionString.Contains("Data Source="))       │
│  │     → Use SQLite provider                              │
│  │   else                                                  │
│  │     → Use SQL Server provider                          │
│  │                                                          │
│  └─ Logging & Warnings                                     │
│      ├─ Development: Verbose logging                       │
│      ├─ Staging: Standard logging                          │
│      └─ Production: Minimal logging → Application Insights│
│                                                             │
└────────────────────────────────────────────────────────────┘
```

---

## 7. Transformation Timeline

```
Week 1-2: Analysis & Architecture
├── Analyze current codebase
├── Design new architecture
├── Create API specifications
└── Set up development environment

Week 3-5: Backend API Development
├── Create API controllers
├── Implement DTOs
├── Add JWT authentication
├── API validation & error handling
└── Swagger documentation

Week 6-9: Frontend Components (Vue)
├── Week 6: Setup Vue project + common components
├── Week 7: Admin pages
├── Week 8: Customer pages
├── Week 9: Integration & polishing

Week 10: Testing & Optimization
├── Functional testing
├── Security testing
├── Performance optimization
└── Cross-browser testing

Week 11-12: Docker & Deployment
├── Create Dockerfile
├── Azure configuration
├── CI/CD setup
├── Launch & monitoring

Current: Week of Nov 25, 2025 → Analysis Phase
```

---

## 8. Authentication Flow Comparison

```
Current (Session-Based):
┌─────────────┐
│   Browser   │
└──────┬──────┘
       │
  1. POST /login with credentials
       │
       ▼
┌──────────────────────┐
│  ASP.NET Server      │
│  ↓ Verify Password   │
│  ↓ Set Session       │
│  ↓ Set HttpOnly Cookie
└──────┬───────────────┘
       │
  2. Return Cookie (SessionId)
       │
       ▼
┌─────────────┐
│   Browser   │
│   Stores    │
│   Cookie    │
└─────────────┘


Target (Token-Based JWT):
┌─────────────┐
│   Browser   │
└──────┬──────┘
       │
  1. POST /api/auth/login with credentials
       │
       ▼
┌──────────────────────┐
│  ASP.NET API         │
│  ↓ Verify Password   │
│  ↓ Generate JWT      │
│  ↓ Include Claims    │
│    (userId, roles)   │
└──────┬───────────────┘
       │
  2. Return JWT token
       │
       ▼
┌──────────────────────┐
│   Browser (Vue.js)   │
│   Stores in          │
│   LocalStorage       │
│   or SessionStorage  │
└──────────┬───────────┘
           │
    3. Each API Request:
       Authorization: Bearer <JWT>
           │
           ▼
┌──────────────────────┐
│  ASP.NET API         │
│  ↓ Validate JWT      │
│  ↓ Check Expiration  │
│  ↓ Verify Signature  │
│  ↓ Extract Claims    │
│  ↓ Process Request   │
└──────────────────────┘
```

---

End of Architecture Diagrams
