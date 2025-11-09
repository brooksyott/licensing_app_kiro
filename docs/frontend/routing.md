# Frontend Routing

The application uses Vue Router 4 for client-side routing with a clean URL structure.

## Route Configuration

### Main Routes

| Path | Component | Description |
|------|-----------|-------------|
| `/` | HomeView | Landing page with welcome message |
| `/customers` | CustomersView | Customer management |
| `/products` | ProductsView | Product management |
| `/skus` | SkusView | SKU management |
| `/licenses` | LicensesView | License management |
| `/rsa-keys` | RsaKeysView | RSA key management |
| `/api-keys` | ApiKeysView | API key management |

## Router Setup

```typescript
import { createRouter, createWebHistory } from 'vue-router'
import AppLayout from '@/components/layout/AppLayout.vue'

const routes = [
  {
    path: '/',
    component: AppLayout,
    children: [
      {
        path: '',
        name: 'home',
        component: () => import('@/views/HomeView.vue')
      },
      {
        path: '/customers',
        name: 'customers',
        component: () => import('@/views/CustomersView.vue')
      },
      // ... other routes
    ]
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})
```

## Navigation

### Header Navigation
The main navigation menu in the header provides links to:
- Customers
- Products
- SKUs
- Licenses
- Settings (dropdown)
  - RSA Keys
  - API Keys

### Programmatic Navigation
Components can navigate programmatically:

```typescript
import { useRouter } from 'vue-router'

const router = useRouter()

// Navigate to a route
router.push('/customers')

// Navigate with params
router.push({ name: 'customer-detail', params: { id: '123' } })
```

### Active Route Highlighting
The current route is highlighted in the navigation menu using the `active` class.

## Route Guards

Currently, the application does not implement route guards, but they can be added for:
- Authentication checks
- Authorization based on roles
- Unsaved changes warnings

## Lazy Loading

Routes use lazy loading for better performance:

```typescript
component: () => import('@/views/CustomersView.vue')
```

This splits each view into separate chunks that are loaded on demand.

## History Mode

The router uses HTML5 History mode (`createWebHistory()`), which:
- Provides clean URLs without hash (#)
- Requires server configuration for production
- Falls back gracefully on older browsers
