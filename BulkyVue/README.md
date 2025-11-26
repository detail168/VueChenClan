# BulkyVue - Vue.js Frontend Project

# This file documents the Vue.js transformation of the BulkyBook platform

## Project Overview

BulkyVue is a Vue 3 + Vite single-page application (SPA) that replaces the ASP.NET Razor Views. It provides a modern, responsive frontend for the BulkyBook position management system.

## Features

- **Authentication & Authorization** - JWT-based authentication with role-based access control
- **Ancestral Position Management** - CRUD operations, Excel import/export, grid visualization
- **Kindness Position Management** - Floor/section filtering, bulk operations, visual grid
- **Responsive Design** - Bootstrap 5 framework for all devices
- **Real-time Notifications** - Toast notifications and SweetAlert2 dialogs
- **State Management** - Pinia for global app state
- **API Integration** - Axios with interceptors for JWT token injection

## Project Structure

```
BulkyVue/
├── src/
│   ├── main.js                      # Entry point
│   ├── App.vue                      # Root component
│   ├── components/
│   │   ├── Layout/
│   │   │   └── AppLayout.vue        # Main layout with navbar & sidebar
│   │   ├── Ancestral/               # Ancestral-specific components
│   │   └── Kindness/                # Kindness-specific components
│   ├── views/
│   │   ├── LoginView.vue            # Authentication page
│   │   ├── DashboardView.vue        # Dashboard/home page
│   │   ├── AncestralListView.vue    # Position list (table)
│   │   ├── AncestralFormView.vue    # Create/Edit form
│   │   ├── AncestralGridView.vue    # Position grid visualization
│   │   ├── KindnessListView.vue     # Tower position list
│   │   ├── KindnessFormView.vue     # Create/Edit form
│   │   ├── KindnessGridView.vue     # Tower grid visualization
│   │   ├── NotFoundView.vue         # 404 page
│   │   └── UnauthorizedView.vue     # 403 page
│   ├── router/
│   │   └── index.js                 # Vue Router configuration
│   ├── stores/
│   │   ├── authStore.js             # Authentication state (Pinia)
│   │   ├── ancestralStore.js        # Ancestral positions state
│   │   └── kindnessStore.js         # Kindness positions state
│   ├── services/
│   │   ├── axiosInstance.js         # Configured Axios client
│   │   ├── authService.js           # Auth API calls & JWT handling
│   │   └── apiService.js            # CRUD API services
│   └── utils/                       # Utility functions
├── public/                          # Static assets
├── index.html                       # HTML entry point
├── vite.config.js                   # Vite build configuration
├── package.json                     # Dependencies & scripts
├── .env.local                       # Local development environment
└── .env.production                  # Production environment

```

## Installation & Setup

### Prerequisites

- Node.js 18.x LTS or higher
- npm or pnpm

### Quick Start

```bash
# Install dependencies
npm install

# Development server (with hot reload)
npm run dev

# Build for production
npm run build

# Preview production build locally
npm run preview

# Run unit tests
npm run test

# Lint & fix code
npm run lint
```

### Development Environment Variables

Create `.env.local` file:

```env
VITE_API_URL=https://localhost:7001
VITE_APP_TITLE=BulkyBook Position Management
```

### Production Environment Variables

`.env.production`:

```env
VITE_API_URL=https://bulkybook.azurewebsites.net
VITE_APP_TITLE=BulkyBook Position Management
```

## API Integration

### Axios Configuration

Requests are automatically configured with:

- JWT token injection in `Authorization` header
- 10-second timeout
- Automatic error handling
- Response interceptors for 401/403 errors

```javascript
// Example API call
import { AncestralService } from "@/services/apiService";

const positions = await AncestralService.getAll();
const newPosition = await AncestralService.create({
  positionId: "L側-甲區-1:001",
  name: "陳先生",
  status: "Available",
});
```

### Available API Endpoints

**Authentication**

- `POST /api/auth/login` - Login with credentials
- `POST /api/auth/refresh` - Refresh JWT token
- `POST /api/auth/logout` - Invalidate token
- `GET /api/auth/user` - Get current user info
- `GET /api/auth/roles` - Get user roles

**Ancestral Positions**

- `GET /api/admin/ancestral` - List all positions
- `GET /api/admin/ancestral/{id}` - Get position details
- `POST /api/admin/ancestral` - Create position
- `PUT /api/admin/ancestral/{id}` - Update position
- `DELETE /api/admin/ancestral/{id}` - Delete position
- `POST /api/admin/ancestral/import` - Import from Excel
- `GET /api/admin/ancestral/export` - Export to Excel
- `GET /api/config/ancestral` - Get configuration

**Kindness Positions**

- `GET /api/admin/kindness` - List positions with filters
- `GET /api/admin/kindness/{id}` - Get position details
- `POST /api/admin/kindness` - Create position
- `PUT /api/admin/kindness/{id}` - Update position
- `DELETE /api/admin/kindness/{id}` - Delete position
- `POST /api/admin/kindness/import` - Bulk import
- `GET /api/admin/kindness/export` - Export to Excel
- `GET /api/config/kindness` - Get configuration

**Configuration**

- `GET /api/config/ancestral` - Ancestral layout config
- `GET /api/config/kindness` - Kindness layout config
- `GET /api/config/app-settings` - General app settings

## Authentication Flow

1. User navigates to `/login`
2. User enters credentials (email & password)
3. API returns JWT token + refresh token
4. Tokens stored in localStorage
5. Axios interceptor injects token in all requests
6. Protected routes use Vue Router guards to check `authStore.isAuthenticated`
7. On 401 response, user redirected to login
8. Automatic logout warning before token expiration

## State Management (Pinia)

### authStore

```javascript
import { useAuthStore } from "@/stores/authStore";

const authStore = useAuthStore();

// Access state
authStore.user; // Current user object
authStore.token; // JWT token
authStore.isAuthenticated; // Boolean
authStore.userRoles; // Array of role names
authStore.isAdmin; // Computed boolean
authStore.isCustomer; // Computed boolean

// Actions
await authStore.login(email, password);
authStore.logout();
authStore.hasRole("Admin");
await authStore.refreshUser();
authStore.clearError();
```

### ancestralStore

```javascript
import { useAncestralStore } from "@/stores/ancestralStore";

const ancestralStore = useAncestralStore();

// Access state & computed
ancestralStore.positions; // Array of positions
ancestralStore.config; // Layout configuration
ancestralStore.selectedPosition; // Currently selected position
ancestralStore.totalCount; // Total positions
ancestralStore.availablePositions; // Filtered available only

// Actions
await ancestralStore.fetchAll();
await ancestralStore.fetchConfig();
await ancestralStore.getById(id);
await ancestralStore.create(position);
await ancestralStore.update(id, position);
await ancestralStore.deletePosition(id);
await ancestralStore.importExcel(file);
await ancestralStore.exportExcel();
ancestralStore.clearError();
```

### kindnessStore

Similar to ancestralStore, with additional filter management:

```javascript
// Filtering
kindnessStore.setFilter("floor", "1");
kindnessStore.setFilter("section", "A");
kindnessStore.setFilter("status", "Available");
kindnessStore.clearFilters();

// Computed filtered results
kindnessStore.filteredPositions; // Based on active filters
```

## Component Architecture

### Layout Components

**AppLayout.vue**

- Responsive navbar with logo and user menu
- Sidebar navigation based on user roles
- Admin menu for privileged users
- Footer with copyright info
- Router outlet for page content

### View Components

**LoginView.vue**

- Email/password form
- Error message display
- Loading state during authentication
- Redirect to original URL after login

**DashboardView.vue**

- Summary cards showing statistics
- Quick action buttons
- User info display
- System status information

**List Views (Ancestral/Kindness)**

- DataTable with sorting/filtering
- Excel import/export buttons
- Create new button
- Edit/Delete actions per row
- Loading spinner during fetch
- Error message display
- Empty state handling

**Form Views (Ancestral/Kindness)**

- Form validation
- Create vs. Edit mode detection
- Auto-fill on edit
- Loading state during submit
- Error handling & display
- Cancel/Submit buttons

**Grid Views (Ancestral/Kindness)**

- Visual grid layout
- Position status color coding
- Interactive cell selection
- Responsive grid layout
- Configuration-driven sizing

## Styling & Theme

- **Framework:** Bootstrap 5.3
- **Primary Color:** #007bff (Blue)
- **Status Colors:**
  - Green (#28a745) - Available
  - Red (#dc3545) - Occupied
  - Yellow (#ffc107) - Reserved

### Custom Styling

Global styles defined in `App.vue`:

```css
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

body {
  font-family: "Segoe UI", Tahoma, Geneva, Verdana, sans-serif;
  background-color: #f8f9fa;
}
```

## User Roles & Permissions

### Admin Role

- Full access to all modules
- Create/Edit/Delete positions
- Import/Export Excel files
- View all positions
- Manage users (future)

### Customer Role

- View available positions
- Apply for position (future)
- View personal bookings (future)

## Error Handling

### HTTP Errors

| Status | Handling                      |
| ------ | ----------------------------- |
| 400    | Show error message from API   |
| 401    | Redirect to login             |
| 403    | Redirect to unauthorized page |
| 404    | Show 404 not found page       |
| 500    | Show generic error message    |

### Form Validation

- HTML5 validation attributes
- Vue v-model two-way binding
- Backend validation errors displayed in form

## Performance Optimization

- **Code Splitting:** Lazy-loaded route components
- **Tree Shaking:** Unused code removed in production build
- **Minification:** Terser configured in Vite
- **Vendor Chunking:** Separate bundles for vendor code

### Build Output

```
dist/
├── index.html          # ~2KB
├── assets/
│   ├── index-xxx.js    # Main app bundle ~150KB
│   ├── vendor-xxx.js   # Vue, Router, Pinia ~200KB
│   ├── bootstrap-xxx.js # Bootstrap ~100KB
│   └── style-xxx.css   # Compiled CSS ~50KB
```

## Testing

### Unit Tests (Vitest)

```bash
# Run tests
npm run test

# Run with coverage
npm run test -- --coverage

# Watch mode
npm run test -- --watch
```

### E2E Tests (Cypress - optional)

```bash
# Install Cypress
npm install -D cypress

# Run tests
npx cypress open
```

## Deployment

### Production Build

```bash
# Build for production
npm run build

# Output in dist/ folder
# Ready for Docker or Azure deployment
```

### Docker Integration

The `Dockerfile` includes this as Stage 2:

```dockerfile
FROM node:18-alpine AS frontend-builder
WORKDIR /app
COPY BulkyVue/package*.json ./
RUN npm ci
COPY BulkyVue .
RUN npm run build
```

Output is copied to `wwwroot/app` in the final stage.

## Browser Support

- Chrome/Edge latest
- Firefox latest
- Safari latest
- Mobile browsers (iOS Safari, Chrome Mobile)

Requires ES2020+ JavaScript support.

## Known Limitations

1. **Excel Import** - Large files (>10K rows) may timeout. Implement pagination for bulk operations.
2. **Grid Visualization** - Very large grids (500+ cells) may have performance issues. Consider virtualization.
3. **Offline Mode** - Currently requires active internet. Service Worker could provide offline capability.

## Future Enhancements

- [ ] Real-time updates via WebSocket
- [ ] Dark mode toggle
- [ ] Multi-language support (i18n)
- [ ] Advanced search with filters
- [ ] Analytics dashboard
- [ ] Mobile app (React Native)
- [ ] Payment integration
- [ ] Email notifications
- [ ] SMS alerts
- [ ] QR code generation for positions

## Contributing

1. Create feature branch from `develop`
2. Follow Vue 3 Composition API conventions
3. Add unit tests for new features
4. Ensure ESLint passes
5. Create pull request with description

## Troubleshooting

### Dev Server Won't Start

```bash
# Clear node_modules
rm -rf node_modules
npm install

# Check Node version
node --version  # Should be 18.x or higher
```

### API Requests Fail

```bash
# Check backend is running
curl https://localhost:7001/api/health

# Check CORS settings in backend
# Check token is present in localStorage
# Check token hasn't expired
```

### Styling Issues

```bash
# Rebuild CSS
npm run build

# Clear browser cache
# Hard refresh: Ctrl+Shift+R (Windows) or Cmd+Shift+R (Mac)
```

---

**Project Created:** November 26, 2025  
**Vue Version:** 3.4.0  
**Vite Version:** 5.0.0  
**Bootstrap Version:** 5.3.0

For detailed API documentation, see the [DEPLOYMENT_GUIDE.md](./DEPLOYMENT_GUIDE.md) and [MVC_to_Vue_Analysis_Report.md](./MVC_to_Vue_Analysis_Report.md).
