# BulkyBook MVC to Vue.js Transformation - Architecture Analysis

**Generated:** November 25, 2025  
**Project:** BulkyBook - Event Management & E-commerce Platform  
**Technology Stack:** ASP.NET Core 8 MVC → Vue.js 3 + .NET Backend

---

## Executive Summary

This document outlines the comprehensive analysis of the BulkyBook MVC project structure, permission system, and technical architecture in preparation for transformation to a Vue.js-based Single Page Application (SPA) with REST API backend.

---

## 1. Project Overview

### 1.1 Current Architecture

- **Backend:** ASP.NET Core 8 MVC (Server-Side Rendering)
- **Database:** SQLite (Development) / SQL Server (Production) / Azure SQL (Cloud)
- **Authentication:** ASP.NET Core Identity
- **Frontend:** Razor Views (C# Template Engine)
- **Key Features:**
  - Event management and registration
  - Product/Activity catalog
  - E-commerce shopping cart
  - Kindness/Heritage management
  - User role-based access control
  - Stripe payment integration
  - Email notifications (SendGrid)

### 1.2 Project Structure

```
BulkyWeb/
├── Areas/
│   ├── Admin/          (Administrative functions)
│   ├── Customer/       (Customer-facing functions)
│   └── Identity/       (Authentication & Authorization)
├── Views/              (Razor templates)
├── Controllers/        (MVC Controllers)
└── wwwroot/            (Static assets)

Bulky.Models/           (Data models)
Bulky.DataAccess/       (Repository pattern, UnitOfWork)
Bulky.Utility/          (Constants, helpers, email sender)
```

---

## 2. Permission & Authorization System

### 2.1 Role-Based Access Control (RBAC)

The system implements 4 primary roles defined in `SD.cs`:

| Role                 | Constant        | Permissions                                             | Purpose               |
| -------------------- | --------------- | ------------------------------------------------------- | --------------------- |
| **Admin 管理員**     | `Role_Admin`    | Full system access, user management, role assignment    | System administration |
| **Company 宗親會員** | `Role_Company`  | Event/product management for own company                | Organization member   |
| **Employee 幹部**    | `Role_Employee` | Limited admin functions, event oversight                | Staff/manager         |
| **Customer 親友**    | `Role_Customer` | Browse events, register, shopping cart, order placement | General users         |

### 2.2 Authentication Flow

1. **Login/Register** → Areas/Identity/Pages/Account/
2. **ASP.NET Core Identity** validates credentials
3. **Role Assignment** → UserController.RoleManagment()
4. **Authorization Attributes** → `[Authorize(Roles="...")]` on controllers
5. **Session Management** → 100-minute timeout, HttpOnly cookies

### 2.3 Key Authorization Points

```csharp
// Admin Area - Full Access
[Area("admin")]
[Authorize]
public class OrderController : Controller

// Role-Specific Access
[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Customer)]
public class UserController : Controller

// Customer Area
[Area("customer")]
[Authorize(Roles = SD.Role_Admin)]  // Note: Restricted to Admin only
public class CartController : Controller
```

### 2.4 Permission Propagation to Frontend

- Roles passed via `TempData["Role"]` to views
- Frontend renders UI elements conditionally based on role
- No sensitive logic in frontend; backend enforces authorization

---

## 3. Frontend Architecture (Current - Razor)

### 3.1 View Structure

```
BulkyWeb/Areas/
├── Admin/Views/
│   ├── Product/         (Upsert, Index - CRUD)
│   ├── Category/        (CRUD operations)
│   ├── Order/           (View, update status)
│   ├── User/            (User list, role management)
│   ├── Kindness/        (Heritage management)
│   └── Company/         (Company CRUD)
│
├── Customer/Views/
│   ├── Home/            (Product listing, details)
│   ├── Cart/            (Shopping cart management)
│   ├── EventRegistration/ (Event registration)
│   └── Order/           (Order confirmation)
│
└── Identity/Pages/Account/
    ├── Register.cshtml
    ├── Login.cshtml
    ├── Logout.cshtml
    └── Manage/ (Profile management)
```

### 3.2 View Rendering Technologies

- **Razor Templates** - Server-side HTML generation
- **Form Validation** - Model state validation in server
- **AJAX** - Minimal; mostly full-page postbacks
- **Data Binding** - Server-side model binding to views

### 3.3 Ancestral Views & Layout Settings (Admin)

- Location: `BulkyWeb/Areas/Admin/Views/Ancestral/` — files found:

  - `Index.cshtml` (list, DataTables, import/export, search)
  - `Upsert.cshtml` (create/edit form)
  - `DisplayPosition.cshtml` (visual layout rendering)
  - `Application.cshtml` (detailed position application and save)
  - `PositionQuery.cshtml` (query UI)
  - Static assets used: `BulkyWeb/wwwroot/images/Ancestral/*` and `~/css/position_*.css`.

- Purpose / behavior summary:

  - Provides CRUD for `AncestralPosition` entities (Admin area).
  - `Index.cshtml` uses DataTables for listing, supports Excel import/export (SheetJS/XLSX) and bulk actions.
  - `Upsert.cshtml` is a standard form using `asp-for` bindings for `AncestralPosition` model properties.
  - `DisplayPosition.cshtml` and `Application.cshtml` render a dynamic, image-backed layout of ancestral positions (left/right/middle) and build client-side arrays from `ViewBag` variables such as `alarow1`, `ararow1`, etc.
  - Client-side JS is extensive: layout arrays are serialized into page via `@Html.Raw(JsonSerializer.Serialize(ViewBag.*))`, then DOM is built dynamically. The views use jQuery, SheetJS (`xlsx`), SweetAlert (`swal`), and TinyMCE in some forms.

- Configuration keys used (from `appsettings*.json` under `BulkyWeb`):

  - `Ancestral.Layout_L` / `Ancestral.Layout_R` / `Ancestral.Layout` — comma-separated layout names
  - `Ancestral.Side`, `Ancestral.Section`, `Ancestral.Level`, `Ancestral.Position` — numeric limits
  - `Ancestral.la`, `Ancestral.lb`, ... `Ancestral.rm` — per-section `row`/`col` metadata used to generate `alarowN`, `ararowN`, etc.
  - `Logout_Duration:AUTO_LOGOUT_MINUTE`, `Logout_Duration:WARNING_BEFORE_LOGOUT_SECOND` — used in multiple Ancestral views for auto-logout UI
  - `Work_Duration`, `WORK_WARNING_SECONDS` — also surfaced to `ViewBag` for timers shown in the UI

- Migration implications (Razor → Vue):
  - The layout configuration in `appsettings` should be exposed via an API endpoint (e.g., `GET /api/ancestral/config`) so the SPA can retrieve layout metadata at runtime instead of server-side `ViewBag` injection.
  - The dynamic arrays (`alarow1`, `ararow1`, etc.) should be generated by the backend API (or computed client-side from canonical layout data) and returned as normalized JSON.
  - Excel import/export flows (SheetJS) can remain in the SPA; server endpoints should accept bulk import JSON (`POST /api/ancestral/import`) and provide export (`GET /api/ancestral/export`).
  - Visual rendering (DisplayPosition/Application) should translate to a Vue component that consumes the layout JSON and renders position tiles; images in `wwwroot/images/Ancestral` can be served as static assets or moved to blob storage.

---

---

## 4. Backend APIs (To Be Created)

### 4.1 API Controllers Architecture

The existing MVC controllers will be converted to RESTful API endpoints:

#### Product Management

```
GET    /api/products              - List all products
GET    /api/products/{id}         - Get product details
POST   /api/products              - Create product
PUT    /api/products/{id}         - Update product
DELETE /api/products/{id}         - Delete product
GET    /api/products/{id}/images  - Get product images
POST   /api/products/{id}/images  - Upload product images
```

#### Order Management

```
GET    /api/orders                - List user's orders
GET    /api/orders/{id}           - Get order details
POST   /api/orders                - Create order
PUT    /api/orders/{id}/status    - Update order status
GET    /api/orders/{id}/details   - Get order line items
```

#### User & Authentication

```
POST   /api/auth/register         - User registration
POST   /api/auth/login            - User login (JWT)
POST   /api/auth/logout           - User logout
GET    /api/auth/profile          - Get current user profile
PUT    /api/auth/profile          - Update user profile
POST   /api/auth/refresh          - Refresh JWT token
```

#### Shopping Cart

```
GET    /api/cart                  - Get cart items
POST   /api/cart/items            - Add item to cart
PUT    /api/cart/items/{id}       - Update cart item quantity
DELETE /api/cart/items/{id}       - Remove item from cart
DELETE /api/cart                  - Clear cart
```

#### Events & Registrations

```
GET    /api/events                - List events
POST   /api/events/{id}/register  - Register for event
GET    /api/registrations         - Get user registrations
DELETE /api/registrations/{id}    - Cancel registration
```

#### Kindness/Heritage Management

```
GET    /api/kindness/layout       - Get layout configuration
GET    /api/kindness/positions    - Get available positions
POST   /api/kindness/claim        - Claim a position
GET    /api/kindness/ancestral   - Get ancestral records
```

### 4.2 Response Format (JSON)

```json
{
  "success": true,
  "data": { ... },
  "message": "Operation successful",
  "errors": []
}
```

---

## 5. Database Schema

### 5.1 Core Entities

#### Users & Authentication

- **AspNetUsers** - Identity users
- **AspNetRoles** - Role definitions
- **AspNetUserRoles** - User-role mappings
- **ApplicationUser** - Extended user properties (Name, Address, CompanyId)

#### E-Commerce

- **Products** - Events/activities
- **ProductImages** - Product attachments
- **Categories** - Product categories
- **Companies** - Organizing companies
- **ShoppingCart** - User cart items
- **OrderHeader** - Order master records
- **OrderDetail** - Order line items

#### Events & Management

- **EventRegistration** - Event registrations
- **KindnessPosition** - Heritage position assignments
- **AncestralPosition** - Ancestral records
- **SurveyResponse** - User surveys

### 5.2 Key Foreign Keys

- ApplicationUser → Company (Optional)
- Product → Company, Category
- ShoppingCart → ApplicationUser, Product
- OrderHeader → ApplicationUser
- OrderDetail → OrderHeader, Product

---

## 6. Transformation Strategy

### 6.1 Frontend Transformation: Razor → Vue.js

#### Phase 1: Architecture Setup

1. Create Vue 3 project structure
2. Implement API client (axios/fetch)
3. Create state management (Pinia)
4. Set up routing (Vue Router)

#### Phase 2: Component Conversion

1. **Authentication Components**

   - `Login.vue` ← Register.cshtml
   - `Register.vue` ← Register.cshtml
   - `UserProfile.vue` ← Account/Manage
   - `RoleManagement.vue` ← RoleManagment.cshtml

2. **Admin Components**

   - `ProductList.vue` ← Product/Index.cshtml
   - `ProductForm.vue` ← Product/Upsert.cshtml
   - `CategoryList.vue` ← Category/Index.cshtml
   - `UserList.vue` ← User/Index.cshtml
   - `OrderList.vue` ← Order/Index.cshtml
   - `KindnessLayout.vue` ← Kindness/DisplayPosition.cshtml

3. **Customer Components**
   - `ProductCatalog.vue` ← Home/Index.cshtml
   - `ProductDetails.vue` ← Home/Details.cshtml
   - `ShoppingCart.vue` ← Cart/Index.cshtml
   - `Checkout.vue` ← Cart/Summary.cshtml
   - `OrderConfirmation.vue` ← Cart/OrderConfirmation.cshtml
   - `EventRegistration.vue` ← EventRegistration/Upsert.cshtml

#### Phase 3: State Management

- **User Store** - Authentication state, current user
- **Cart Store** - Shopping cart items, totals
- **Product Store** - Product listings, filters
- **Order Store** - Order history, details

#### Phase 4: API Integration

- Replace form submissions with API calls
- Implement error handling & validation
- Add loading states & notifications
- Implement JWT token refresh mechanism

### 6.2 Backend Transformation: MVC → API

1. Create ApiController base class
2. Convert MVC controllers to API endpoints
3. Implement DTO (Data Transfer Objects)
4. Add API validation & error handling
5. Implement JWT authentication
6. Create API documentation (Swagger)

### 6.3 Authentication Migration

**Current (Session-based):**

```
Razor Form → Controller → Session → Cookie
```

**Target (Token-based):**

```
Vue Form → API Login → JWT Token → LocalStorage
```

### 6.4 Communication Pattern

**Current (Form-based):**

```
User Action → Razor Form POST/GET → MVC Controller → Return View
```

**Target (API-based):**

```
User Action → Vue Component → API Call → JSON Response → Update UI
```

---

## 7. Key Technical Considerations

### 7.1 Data Transfer

**Current:**

- Server renders complete HTML with data embedded
- ViewBag, ViewData for passing data
- Model binding from form submissions

**Target:**

- RESTful JSON APIs
- Request/Response DTOs
- Content negotiation

### 7.2 Validation

**Current:**

- Server-side model validation
- Display ModelState errors in view

**Target:**

- Client-side validation (Vue)
- Server-side validation (API)
- Consistent error messages

### 7.3 Session Management

**Current:**

- ASP.NET Session with 100-minute timeout
- Auto-logout on inactivity

**Target:**

- JWT tokens with expiration
- Refresh token mechanism
- LocalStorage for persistence

### 7.4 File Uploads

**Current:**

- Form-based file upload to ~/pics/ directory
- Physical file storage

**Target:**

- Multipart form data to API
- Optional: Azure Blob Storage for scalability

### 7.5 Real-time Features

**Current:**

- None (traditional polling)

**Target:**

- Consider SignalR for real-time notifications
- WebSocket for live updates

---

## 8. Deployment Architecture

### 8.1 Docker Container Structure

```dockerfile
# Multi-stage build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# Build .NET backend

FROM node:20 AS vuebuild
# Build Vue.js frontend

FROM mcr.microsoft.com/dotnet/aspnet:8.0
# Runtime: .NET Core + Vue SPA
```

### 8.2 Azure Deployment

- **App Service** - Web hosting
- **SQL Database** - Data persistence
- **App Service Plan** - Compute resources
- **Container Registry** - Docker image storage
- **Application Insights** - Monitoring & logging
- **Key Vault** - Secrets management

### 8.3 Environment Configuration

```
Development:  SQLite, Local debugging
Staging:      SQL Server, Pre-production testing
Production:   Azure SQL, Azure App Service, Docker
```

---

## 9. Migration Checklist

### 9.1 Pre-Migration

- [ ] Backup database
- [ ] Document all existing APIs
- [ ] Create test data sets
- [ ] Establish monitoring baseline

### 9.2 Development

- [ ] Set up Vue project
- [ ] Create API endpoints
- [ ] Implement authentication
- [ ] Convert first 3 views to Vue components
- [ ] Unit tests (>80% coverage)
- [ ] Integration tests

### 9.3 Testing

- [ ] Functional testing (all user workflows)
- [ ] Permission/Authorization testing
- [ ] Performance testing
- [ ] Security testing (OWASP Top 10)
- [ ] Browser compatibility
- [ ] Mobile responsiveness

### 9.4 Deployment

- [ ] Create Docker image
- [ ] Push to Azure Container Registry
- [ ] Deploy to App Service
- [ ] Configure CI/CD pipeline
- [ ] Health check monitoring
- [ ] Rollback procedure

---

## 10. Technology Stack Comparison

| Aspect           | Current (MVC)     | Target (Vue + API)   |
| ---------------- | ----------------- | -------------------- |
| Frontend         | Razor Templates   | Vue.js 3             |
| Backend          | ASP.NET Core MVC  | ASP.NET Core Web API |
| State Management | Server Session    | Pinia (Client-side)  |
| Routing          | ASP.NET Routing   | Vue Router           |
| HTTP             | Traditional Forms | REST/JSON            |
| Authentication   | Cookies/Session   | JWT Tokens           |
| Real-time        | Polling           | SignalR (optional)   |
| Deployment       | IIS / App Service | Docker / Container   |
| Build Tool       | Visual Studio     | Vite / npm           |
| Package Manager  | NuGet             | npm                  |

---

## 11. Risk Assessment & Mitigation

### 11.1 Risks

| Risk                   | Impact   | Probability | Mitigation                               |
| ---------------------- | -------- | ----------- | ---------------------------------------- |
| Data Loss              | Critical | Low         | Database backups, transaction management |
| Permission Bypass      | High     | Low         | Comprehensive authorization testing      |
| Performance Regression | High     | Medium      | Load testing, caching strategy           |
| Incompatibility        | Medium   | Medium      | Phased rollout, feature flags            |
| Breaking Changes       | High     | Low         | Backward-compatible API versions         |

### 11.2 Mitigation Strategies

1. **Dual-run period** - MVC and Vue running in parallel
2. **Feature flags** - Gradual feature rollout
3. **Automated testing** - 80%+ code coverage
4. **Monitoring** - Real-time performance metrics
5. **Rollback plan** - Quick revert procedure

---

## 12. Timeline Estimate

| Phase               | Duration        | Tasks                                         |
| ------------------- | --------------- | --------------------------------------------- |
| Analysis & Design   | 1 week          | Architecture, API design, schema mapping      |
| Backend API         | 2-3 weeks       | Controllers, DTOs, validation, authentication |
| Frontend Components | 3-4 weeks       | Vue components, routing, state management     |
| Integration Testing | 2 weeks         | End-to-end testing, permission verification   |
| Docker & Deployment | 1 week          | Dockerfile, CI/CD, Azure configuration        |
| **Total**           | **10-12 weeks** | Complete transformation & deployment          |

---

## 13. Conclusion

The transformation from ASP.NET Core MVC to Vue.js + API architecture will provide:

✅ **Better UX** - Single Page Application with instant feedback  
✅ **Scalability** - Decoupled frontend and backend  
✅ **Modern Stack** - Industry-standard Vue.js framework  
✅ **Cloud Native** - Docker containerization for Azure  
✅ **Maintainability** - Clear separation of concerns  
✅ **Performance** - Reduced server load, client-side rendering

The existing ASP.NET Core backend remains largely intact; we're primarily replacing the Razor view layer with Vue.js components and converting controllers to RESTful APIs.

---

## Appendix A: File Mapping Reference

### Views to Convert

- Admin/Product/Index.cshtml → ProductList.vue
- Admin/Product/Upsert.cshtml → ProductForm.vue
- Customer/Home/Index.cshtml → ProductCatalog.vue
- Customer/Cart/Index.cshtml → ShoppingCart.vue
- Admin/User/Index.cshtml → UserManagement.vue
- Admin/Order/Index.cshtml → OrderManagement.vue

### Controllers to Modify

- ProductController → api/products
- CategoryController → api/categories
- OrderController → api/orders
- UserController → api/users
- CartController → api/cart
- EventRegistrationController → api/events

### Models (Remain Unchanged)

- All models in Bulky.Models/ remain as-is
- Create DTOs for API serialization

---

**Document Version:** 1.0  
**Last Updated:** November 25, 2025  
**Status:** Draft - Ready for Development Phase
