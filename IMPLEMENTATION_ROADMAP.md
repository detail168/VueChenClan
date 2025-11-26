# BulkyBook Vue.js Transformation - Implementation Roadmap

        }
    }

}

### Ancestral (Heritage) — Views → Vue components & API mapping

The admin Ancestral screens are heavily client-driven (image-backed layouts, dynamic position arrays and Excel import/export). Add the following backend endpoints and frontend components to keep parity:

- Recommended API endpoints

  - `GET /api/ancestral/config` — returns the `Ancestral` section from configuration (Layout_L/Layout_R/Layout, Side, Section, Level, Position, and per-section row/col metadata)
  - `GET /api/ancestral/positions` — returns the current positions (occupied/available) as normalized JSON for rendering
  - `POST /api/ancestral/import` — accepts array of positions for bulk import (mapped from frontend SheetJS parsing)
  - `GET /api/ancestral/export` — returns Excel/CSV or JSON export of ancestral positions
  - `GET /api/ancestral/{id}` — single ancestral record
  - `POST /api/ancestral` / `PUT /api/ancestral/{id}` / `DELETE /api/ancestral/{id}` — standard CRUD

- Example: simple controller method to expose config

```csharp
[ApiController]
[Route("api/[controller]")]
public class AncestralApiController : ControllerBase
{
    private readonly IConfiguration _config;
    public AncestralApiController(IConfiguration config)
    {
        _config = config;
    }

    [HttpGet("config")]
    public IActionResult GetConfig()
    {
        var section = _config.GetSection("Ancestral").GetChildren()
            .ToDictionary(x => x.Key, x => x.Value);
        // better: bind to a typed POCO and return full structure
        var configObj = _config.GetSection("Ancestral").Get<object>();
        return Ok(new { success = true, data = configObj });
    }
}
```

- Frontend component mapping (Vue)

  - `AncestralList.vue` ← `Index.cshtml` (table, search, bulk actions)
  - `AncestralForm.vue` ← `Upsert.cshtml` (create/edit)
  - `AncestralDisplay.vue` ← `DisplayPosition.cshtml` / `Application.cshtml` (layout rendering)
  - `AncestralQuery.vue` ← `PositionQuery.cshtml`

- Notes for migration
  - Replace server `ViewBag` injections with a single `GET /api/ancestral/config` call that returns layout arrays and numeric limits.
  - Port SheetJS logic to the SPA (use `xlsx` client-side library) — frontend will parse and POST normalized JSON to `POST /api/ancestral/import`.
  - DataTables usage in `Index.cshtml` can be replaced with a Vue table component (PrimeVue DataTable or simple custom table with pagination) and axios calls to `GET /api/ancestral/positions`.
  - Ensure images under `wwwroot/images/Ancestral` are accessible (static file middleware) or serve them from cloud storage.

### Step 2: Implement JWT Authentication

---

## Quick Start Guide

### Prerequisites

```powershell
# Check versions
dotnet --version          # Should be 8.0+
node --version           # Should be 18.0+
npm --version            # Should be 9.0+
docker --version         # For deployment
```

### Directory Setup

```powershell
# 1. Create Vue project structure
cd d:\Git\Vue20251125
mkdir vue-frontend
cd vue-frontend

# 2. Initialize new Vue 3 + TypeScript project
npm create vite@latest . -- --template vue-ts

# 3. Install dependencies
npm install
npm install axios pinia vue-router
npm install --save-dev typescript
```

---

## Phase 2: Backend API Development (Week 3-5)

### Step 1: Create API Infrastructure

#### 1.1 Create DTOs (Data Transfer Objects)

```csharp
// BulkyBook.Models/DTOs/ProductDTO.cs
namespace BulkyBook.Models.DTOs
{
    public class CreateProductDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ISBN { get; set; }
        public int CompanyId { get; set; }
        public int CategoryId { get; set; }
        public double ListPrice { get; set; }
        public string HDate { get; set; }
        public char HeldYN { get; set; }
    }

    public class ProductDTO : CreateProductDTO
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
```

#### 1.2 Create API Response Wrapper

```csharp
// BulkyBook.Models/DTOs/ApiResponse.cs
namespace BulkyBook.Models.DTOs
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
```

#### 1.3 Create API Controllers

```csharp
// BulkyWeb/Areas/Admin/Controllers/Api/ProductsApiController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.DTOs;
using BulkyBook.Utility;

namespace BulkyWebWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsApiController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductsApiController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/products
        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAll()
        {
            try
            {
                var products = _unitOfWork.Product.GetAll(includeProperties: "Category,Company,ProductImages");
                var dtos = products.Select(p => new ProductDTO
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    // Map other properties...
                }).ToList();

                return Ok(new ApiResponse<List<ProductDTO>>
                {
                    Success = true,
                    Data = dtos,
                    Message = "Products retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Errors = new() { ex.Message }
                });
            }
        }

        // GET: api/products/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public IActionResult GetById(int id)
        {
            try
            {
                var product = _unitOfWork.Product.Get(p => p.Id == id,
                    includeProperties: "Category,Company,ProductImages");

                if (product == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Product not found"
                    });
                }

                var dto = new ProductDTO
                {
                    Id = product.Id,
                    Title = product.Title,
                    // Map other properties...
                };

                return Ok(new ApiResponse<ProductDTO>
                {
                    Success = true,
                    Data = dto
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Errors = new() { ex.Message }
                });
            }
        }

        // POST: api/products
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult Create([FromBody] CreateProductDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Errors = errors
                    });
                }

                var product = new Product
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    ISBN = dto.ISBN,
                    CompanyId = dto.CompanyId,
                    CategoryId = dto.CategoryId,
                    ListPrice = dto.ListPrice,
                    HDate = dto.HDate,
                    HeldYN = dto.HeldYN
                };

                _unitOfWork.Product.Add(product);
                _unitOfWork.Save();

                var resultDto = new ProductDTO
                {
                    Id = product.Id,
                    Title = product.Title,
                    // Map other properties...
                };

                return CreatedAtAction("GetById", new { id = product.Id },
                    new ApiResponse<ProductDTO>
                    {
                        Success = true,
                        Data = resultDto,
                        Message = "Product created successfully"
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Errors = new() { ex.Message }
                });
            }
        }

        // PUT: api/products/5
        [HttpPut("{id}")]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult Update(int id, [FromBody] CreateProductDTO dto)
        {
            try
            {
                var product = _unitOfWork.Product.Get(p => p.Id == id);
                if (product == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Product not found"
                    });
                }

                product.Title = dto.Title;
                product.Description = dto.Description;
                // Update other properties...

                _unitOfWork.Product.Update(product);
                _unitOfWork.Save();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Product updated successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Errors = new() { ex.Message }
                });
            }
        }

        // DELETE: api/products/5
        [HttpDelete("{id}")]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult Delete(int id)
        {
            try
            {
                var product = _unitOfWork.Product.Get(p => p.Id == id);
                if (product == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Product not found"
                    });
                }

                _unitOfWork.Product.Remove(product);
                _unitOfWork.Save();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Product deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Errors = new() { ex.Message }
                });
            }
        }
    }
}
```

### Step 2: Implement JWT Authentication

#### 2.1 Add JWT Configuration to appsettings.json

```json
{
  "Jwt": {
    "SecretKey": "your-super-secret-key-min-32-characters-long!",
    "Issuer": "BulkyBookAPI",
    "Audience": "BulkyBookClient",
    "ExpirationMinutes": 60
  }
}
```

#### 2.2 Create JWT Service

```csharp
// BulkyBook.Utility/JwtService.cs
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BulkyBook.Utility
{
    public interface IJwtService
    {
        string GenerateToken(string userId, string email, List<string> roles);
    }

    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(string userId, string email, List<string> roles)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, email)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    int.Parse(_configuration["Jwt:ExpirationMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
```

#### 2.3 Update Program.cs for JWT

```csharp
// In Program.cs, after builder.Services...

// Add JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]))
        };
    });

builder.Services.AddScoped<IJwtService, JwtService>();

// Enable CORS for Vue frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("VueFrontend", builder =>
    {
        builder.WithOrigins("http://localhost:5173", "https://yourdomain.com")
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
```

### Step 3: Create Authentication API

```csharp
// BulkyWeb/Controllers/Api/AuthApiController.cs
[ApiController]
[Route("api/[controller]")]
public class AuthApiController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IJwtService _jwtService;
    private readonly IUnitOfWork _unitOfWork;

    public AuthApiController(UserManager<IdentityUser> userManager,
        IJwtService jwtService, IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _unitOfWork = unitOfWork;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDTO dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
        {
            return Unauthorized(new ApiResponse<object>
            {
                Success = false,
                Message = "Invalid email or password"
            });
        }

        var roles = (await _userManager.GetRolesAsync(user)).ToList();
        var token = _jwtService.GenerateToken(user.Id, user.Email, roles);

        return Ok(new ApiResponse<LoginResponseDTO>
        {
            Success = true,
            Data = new LoginResponseDTO
            {
                Token = token,
                User = new UserDTO
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = ((ApplicationUser)user).Name,
                    Roles = roles
                }
            },
            Message = "Login successful"
        });
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
    {
        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            Name = dto.Name,
            PhoneNumber = dto.PhoneNumber,
            StreetAddress = dto.StreetAddress,
            City = dto.City,
            State = dto.State,
            PostalCode = dto.PostalCode
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Errors = result.Errors.Select(e => e.Description).ToList()
            });
        }

        // Assign default role
        await _userManager.AddToRoleAsync(user, SD.Role_Customer);

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "User registered successfully"
        });
    }

    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout()
    {
        // JWT tokens are stateless; logout is handled on client
        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Logout successful"
        });
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return NotFound();
        }

        var roles = await _userManager.GetRolesAsync(user);
        var appUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

        return Ok(new ApiResponse<UserDTO>
        {
            Success = true,
            Data = new UserDTO
            {
                Id = user.Id,
                Email = user.Email,
                Name = appUser.Name,
                PhoneNumber = user.PhoneNumber,
                Roles = roles.ToList()
            }
        });
    }
}

public class LoginDTO
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class LoginResponseDTO
{
    public string Token { get; set; }
    public UserDTO User { get; set; }
}

public class RegisterDTO
{
    public string Email { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public string PhoneNumber { get; set; }
    public string StreetAddress { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string PostalCode { get; set; }
}

public class UserDTO
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public List<string> Roles { get; set; }
}
```

### Step 4: Enable CORS

```csharp
// In Program.cs, before app.Build()
app.UseCors("VueFrontend");
```

---

## Phase 3: Frontend Components (Vue.js) (Week 6-9)

### Step 1: Project Setup

```bash
# Create Vue project with Vite
npm create vite@latest vue-bulkybook -- --template vue-ts

cd vue-bulkybook
npm install
npm install axios pinia vue-router
npm install -D typescript @types/node
npm install --save-dev @vitejs/plugin-vue
```

### Step 2: Create API Services

#### 2.1 Create axios instance

```typescript
// src/services/api.ts
import axios from "axios";

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || "https://localhost:7001/api",
  headers: {
    "Content-Type": "application/json",
  },
});

// Add JWT token to requests
api.interceptors.request.use((config) => {
  const token = localStorage.getItem("auth_token");
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Handle 401 responses
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem("auth_token");
      localStorage.removeItem("user");
      window.location.href = "/login";
    }
    return Promise.reject(error);
  }
);

export default api;
```

#### 2.2 Create Product Service

```typescript
// src/services/productService.ts
import api from "./api";

export interface Product {
  id: number;
  title: string;
  description: string;
  listPrice: number;
  categoryId: number;
  companyId: number;
  heldYN: string;
}

export interface ApiResponse<T> {
  success: boolean;
  data: T;
  message: string;
  errors: string[];
}

export const productService = {
  async getAll(): Promise<Product[]> {
    const response = await api.get<ApiResponse<Product[]>>("/products");
    return response.data.data || [];
  },

  async getById(id: number): Promise<Product> {
    const response = await api.get<ApiResponse<Product>>(`/products/${id}`);
    return response.data.data;
  },

  async create(product: Omit<Product, "id">): Promise<Product> {
    const response = await api.post<ApiResponse<Product>>("/products", product);
    return response.data.data;
  },

  async update(id: number, product: Omit<Product, "id">): Promise<void> {
    await api.put(`/products/${id}`, product);
  },

  async delete(id: number): Promise<void> {
    await api.delete(`/products/${id}`);
  },
};
```

### Step 3: Create Pinia Stores

#### 3.1 Auth Store

```typescript
// src/stores/authStore.ts
import { defineStore } from "pinia";
import { ref, computed } from "vue";
import api from "@/services/api";

export interface User {
  id: string;
  email: string;
  name: string;
  roles: string[];
}

export const useAuthStore = defineStore("auth", () => {
  const user = ref<User | null>(null);
  const token = ref<string | null>(localStorage.getItem("auth_token"));
  const loading = ref(false);

  const isAuthenticated = computed(() => !!token.value);

  async function login(email: string, password: string) {
    loading.value = true;
    try {
      const response = await api.post("/auth/login", { email, password });
      token.value = response.data.data.token;
      user.value = response.data.data.user;
      localStorage.setItem("auth_token", token.value);
      localStorage.setItem("user", JSON.stringify(user.value));
    } finally {
      loading.value = false;
    }
  }

  function logout() {
    token.value = null;
    user.value = null;
    localStorage.removeItem("auth_token");
    localStorage.removeItem("user");
  }

  function hasRole(role: string): boolean {
    return user.value?.roles.includes(role) ?? false;
  }

  return {
    user,
    token,
    loading,
    isAuthenticated,
    login,
    logout,
    hasRole,
  };
});
```

#### 3.2 Product Store

```typescript
// src/stores/productStore.ts
import { defineStore } from "pinia";
import { ref } from "vue";
import { productService, type Product } from "@/services/productService";

export const useProductStore = defineStore("product", () => {
  const products = ref<Product[]>([]);
  const loading = ref(false);
  const error = ref<string | null>(null);

  async function fetchProducts() {
    loading.value = true;
    error.value = null;
    try {
      products.value = await productService.getAll();
    } catch (err) {
      error.value =
        err instanceof Error ? err.message : "Failed to fetch products";
    } finally {
      loading.value = false;
    }
  }

  async function addProduct(product: Omit<Product, "id">) {
    try {
      const newProduct = await productService.create(product);
      products.value.push(newProduct);
      return newProduct;
    } catch (err) {
      throw err instanceof Error ? err : new Error("Failed to create product");
    }
  }

  return {
    products,
    loading,
    error,
    fetchProducts,
    addProduct,
  };
});
```

### Step 4: Create Vue Components

#### 4.1 Login Component

```vue
<!-- src/components/auth/Login.vue -->
<template>
  <div class="login-container">
    <form @submit.prevent="handleLogin">
      <h2>Login</h2>

      <div class="form-group">
        <label>Email:</label>
        <input v-model="form.email" type="email" required />
      </div>

      <div class="form-group">
        <label>Password:</label>
        <input v-model="form.password" type="password" required />
      </div>

      <button type="submit" :disabled="loading">
        {{ loading ? "Logging in..." : "Login" }}
      </button>

      <p v-if="error" class="error">{{ error }}</p>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref } from "vue";
import { useAuthStore } from "@/stores/authStore";
import { useRouter } from "vue-router";

const authStore = useAuthStore();
const router = useRouter();

const form = ref({
  email: "",
  password: "",
});

const loading = ref(false);
const error = ref("");

async function handleLogin() {
  loading.value = true;
  error.value = "";

  try {
    await authStore.login(form.email, form.password);
    router.push("/dashboard");
  } catch (err) {
    error.value = err instanceof Error ? err.message : "Login failed";
  } finally {
    loading.value = false;
  }
}
</script>

<style scoped>
.login-container {
  max-width: 400px;
  margin: 50px auto;
}

form {
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.form-group {
  display: flex;
  flex-direction: column;
}

input {
  padding: 8px;
  border: 1px solid #ccc;
  border-radius: 4px;
}

button {
  padding: 10px;
  background-color: #007bff;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

.error {
  color: red;
}
</style>
```

#### 4.2 Product List Component

```vue
<!-- src/components/admin/ProductList.vue -->
<template>
  <div class="product-list">
    <h2>Products</h2>

    <button @click="navigateToCreate" class="btn-create">Add Product</button>

    <div v-if="loading" class="loading">Loading...</div>

    <table v-else v-if="products.length > 0">
      <thead>
        <tr>
          <th>Title</th>
          <th>Price</th>
          <th>Category</th>
          <th>Actions</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="product in products" :key="product.id">
          <td>{{ product.title }}</td>
          <td>${{ product.listPrice }}</td>
          <td>{{ product.categoryId }}</td>
          <td>
            <button @click="editProduct(product.id)">Edit</button>
            <button @click="deleteProduct(product.id)">Delete</button>
          </td>
        </tr>
      </tbody>
    </table>

    <p v-else>No products found.</p>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from "vue";
import { useRouter } from "vue-router";
import { useProductStore } from "@/stores/productStore";

const router = useRouter();
const productStore = useProductStore();

onMounted(() => {
  productStore.fetchProducts();
});

function navigateToCreate() {
  router.push("/admin/products/create");
}

function editProduct(id: number) {
  router.push(`/admin/products/${id}/edit`);
}

async function deleteProduct(id: number) {
  if (confirm("Are you sure?")) {
    // Call delete endpoint
  }
}
</script>

<style scoped>
table {
  width: 100%;
  border-collapse: collapse;
}

th,
td {
  padding: 12px;
  text-align: left;
  border-bottom: 1px solid #ddd;
}

button {
  padding: 5px 10px;
  margin: 0 5px;
}
</style>
```

---

## Phase 4: Testing & QA (Week 10)

### Test Plan

#### 4.1 Unit Tests

```typescript
// tests/unit/authStore.test.ts
import { describe, it, expect, vi } from "vitest";
import { setActivePinia, createPinia } from "pinia";
import { useAuthStore } from "@/stores/authStore";

describe("Auth Store", () => {
  beforeEach(() => {
    setActivePinia(createPinia());
  });

  it("should set user and token on login", async () => {
    const store = useAuthStore();
    // Mock API call
    // await store.login('test@test.com', 'password')
    // expect(store.isAuthenticated).toBe(true)
  });
});
```

#### 4.2 Integration Tests

- API endpoint testing
- Authentication flow testing
- Authorization testing

#### 4.3 E2E Tests

- User workflows (login → browse → cart → checkout)
- Admin workflows (create product → publish)
- Role-based access verification

---

## Phase 5: Docker & Deployment (Week 11-12)

### Step 1: Create Multi-stage Dockerfile

```dockerfile
# Dockerfile
# Stage 1: Build .NET backend
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS dotnet-build

WORKDIR /app
COPY Bulky.DataAccess/. Bulky.DataAccess/
COPY Bulky.Models/. Bulky.Models/
COPY Bulky.Utility/. Bulky.Utility/
COPY BulkyWeb/. BulkyWeb/

RUN dotnet restore BulkyWeb/BulkyBookWeb.csproj
RUN dotnet publish BulkyWeb/BulkyBookWeb.csproj -c Release -o /app/publish

# Stage 2: Build Vue frontend
FROM node:20 AS vue-build

WORKDIR /vue
COPY vue-frontend/package*.json ./
RUN npm ci

COPY vue-frontend/. .
RUN npm run build

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app
COPY --from=dotnet-build /app/publish .
COPY --from=vue-build /vue/dist ./wwwroot

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

ENTRYPOINT ["dotnet", "BulkyBookWeb.dll"]
```

### Step 2: Azure Deployment Configuration

#### 2.1 Create Azure Resources

```powershell
# PowerShell Script: deploy-azure.ps1

# Variables
$resourceGroup = "bulkybook-rg"
$appServicePlan = "bulkybook-plan"
$appService = "bulkybook-app"
$location = "eastus"
$registry = "bulkybookacr"
$imageName = "bulkybook:latest"

# Create Resource Group
az group create --name $resourceGroup --location $location

# Create App Service Plan
az appservice plan create `
  --name $appServicePlan `
  --resource-group $resourceGroup `
  --sku B1 `
  --is-linux

# Create Container Registry
az acr create `
  --resource-group $resourceGroup `
  --name $registry `
  --sku Basic

# Build and push image
az acr build `
  --registry $registry `
  --image $imageName `
  --file Dockerfile .

# Create App Service from Docker image
az webapp create `
  --resource-group $resourceGroup `
  --plan $appServicePlan `
  --name $appService `
  --deployment-container-image-name "$registry.azurecr.io/$imageName"

# Create SQL Database
az sql server create `
  --name bulkybook-sql `
  --resource-group $resourceGroup `
  --admin-user sqladmin `
  --admin-password 'YourSecurePassword123!'

az sql db create `
  --server bulkybook-sql `
  --name bulkybook-db `
  --resource-group $resourceGroup
```

#### 2.2 Environment Configuration

```json
{
  "ASPNETCORE_ENVIRONMENT": "Production",
  "ConnectionStrings:DefaultConnection": "Server=tcp:bulkybook-sql.database.windows.net;Database=bulkybook-db;User ID=sqladmin;Password=YourSecurePassword123!;",
  "Jwt:SecretKey": "your-production-secret-key-min-32-chars",
  "AllowedHosts": "bulkybook.azurewebsites.net"
}
```

---

## Checklist for Implementation

### Phase 2: Backend API

- [ ] Create DTOs for all models
- [ ] Implement API Response wrapper
- [ ] Create API controllers (Products, Orders, Users, Cart, Auth)
- [ ] Implement JWT authentication
- [ ] Add CORS configuration
- [ ] Write API documentation (Swagger)
- [ ] Unit test API endpoints

### Phase 3: Frontend (Vue)

- [ ] Set up Vue 3 + TypeScript project
- [ ] Create API services
- [ ] Create Pinia stores (auth, products, cart, orders)
- [ ] Implement Vue Router
- [ ] Create authentication components
- [ ] Create admin components
- [ ] Create customer components
- [ ] Implement form validation

### Phase 4: Testing

- [ ] Unit tests (80%+ coverage)
- [ ] Integration tests
- [ ] E2E tests
- [ ] Security testing
- [ ] Performance testing
- [ ] Cross-browser compatibility

### Phase 5: Deployment

- [ ] Create Dockerfile
- [ ] Build and test Docker image locally
- [ ] Push to Azure Container Registry
- [ ] Configure Azure App Service
- [ ] Set up CI/CD pipeline (GitHub Actions)
- [ ] Configure Application Insights
- [ ] Test in staging environment
- [ ] Deploy to production

---

## Troubleshooting Guide

### Common Issues

#### CORS Error

```
Access to XMLHttpRequest blocked by CORS policy
```

**Solution:** Ensure CORS is configured in Program.cs

#### JWT Token Invalid

```
401 Unauthorized
```

**Solution:**

1. Check token expiration
2. Verify secret key matches
3. Check token format in Authorization header

#### Database Connection Error

```
Cannot connect to database
```

**Solution:**

1. Verify connection string
2. Check credentials
3. Ensure database server is running

---

## Next Steps

1. **Review Analysis Documents**

   - ANALYSIS_ARCHITECTURE.md
   - ARCHITECTURE_DIAGRAMS.md

2. **Start Phase 2: Backend API Development**

   - Create API infrastructure
   - Implement JWT authentication
   - Create API controllers

3. **Parallel Phase 3: Frontend Setup**

   - Initialize Vue project
   - Set up API services
   - Create stores

4. **Integration Testing**

   - Test API endpoints
   - Test authentication flow
   - Test authorization

5. **Docker & Azure Deployment**
   - Build Docker image
   - Deploy to Azure

---

**Document Version:** 1.0  
**Last Updated:** November 25, 2025  
**Status:** Ready for Implementation
