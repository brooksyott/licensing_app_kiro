# Design Document - Vue UI Frontend

## Overview

This document outlines the design for a Vue 3 frontend application that provides a user interface for the License Management API. The application will be built using modern Vue 3 patterns with the Composition API and TypeScript, featuring a consistent layout structure with header and footer navigation.

## Architecture

### Technology Stack

- **Framework**: Vue 3 with Composition API
- **Language**: TypeScript
- **Build Tool**: Vite
- **Styling**: CSS3 with scoped styles
- **HTTP Client**: Axios
- **Router**: Vue Router 4
- **State Management**: Vue 3 Composition API (ref, reactive, computed)

### Project Structure

```
vue-ui-frontend/
├── src/
│   ├── components/
│   │   ├── layout/
│   │   │   ├── AppHeader.vue
│   │   │   ├── AppFooter.vue
│   │   │   └── AppLayout.vue
│   │   ├── common/
│   │   └── entities/
│   ├── views/
│   ├── composables/
│   ├── services/
│   ├── types/
│   ├── router/
│   ├── App.vue
│   └── main.ts
├── public/
└── package.json
```

**Rationale**: This structure separates concerns clearly - layout components are isolated, reusable common components are centralized, and entity-specific components are grouped together. The composables directory will house reusable composition functions for state and logic.

## Components and Interfaces

### 1. Application Framework (Requirement 1)

#### Main Application Setup

**File**: `src/main.ts`

```typescript
import { createApp } from 'vue'
import App from './App.vue'
import router from './router'

const app = createApp(App)
app.use(router)
app.mount('#app')
```

**Design Decision**: Use Vite as the build tool for fast development experience and optimal production builds. Vite provides native ES modules support and is the recommended tooling for Vue 3 projects.

#### Root Component

**File**: `src/App.vue`

```vue
<script setup lang="ts">
// Root component using script setup syntax
</script>

<template>
  <div id="app">
    <router-view />
  </div>
</template>

<style>
/* Global styles */
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

body {
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
}

#app {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}
</style>
```

**Design Decision**: Use `<script setup>` syntax throughout the application for cleaner, more concise component code. This is the recommended approach for Vue 3 Composition API and reduces boilerplate.

#### TypeScript Configuration

**File**: `tsconfig.json`

```json
{
  "compilerOptions": {
    "target": "ES2020",
    "useDefineForClassFields": true,
    "module": "ESNext",
    "lib": ["ES2020", "DOM", "DOM.Iterable"],
    "skipLibCheck": true,
    "moduleResolution": "bundler",
    "allowImportingTsExtensions": true,
    "resolveJsonModule": true,
    "isolatedModules": true,
    "noEmit": true,
    "jsx": "preserve",
    "strict": true,
    "noUnusedLocals": true,
    "noUnusedParameters": true,
    "noFallthroughCasesInSwitch": true,
    "paths": {
      "@/*": ["./src/*"]
    }
  },
  "include": ["src/**/*.ts", "src/**/*.d.ts", "src/**/*.tsx", "src/**/*.vue"],
  "references": [{ "path": "./tsconfig.node.json" }]
}
```

**Design Decision**: Enable strict TypeScript checking to catch errors early and improve code quality. Path aliases (@/*) simplify imports and make refactoring easier.

### 2. Layout Structure (Requirement 2)

#### App Layout Component

**File**: `src/components/layout/AppLayout.vue`

```vue
<script setup lang="ts">
// Layout wrapper component
</script>

<template>
  <div class="app-layout">
    <AppHeader />
    <main class="main-content">
      <slot />
    </main>
    <AppFooter />
  </div>
</template>

<style scoped>
.app-layout {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.main-content {
  flex: 1;
  padding: 2rem;
  max-width: 1400px;
  width: 100%;
  margin: 0 auto;
}
</style>
```

**Design Decision**: Use a layout wrapper component that enforces consistent structure across all pages. The flex layout ensures the footer stays at the bottom even with minimal content. The main content area has a max-width for readability on large screens.

#### Header Component

**File**: `src/components/layout/AppHeader.vue`

```vue
<script setup lang="ts">
import { ref } from 'vue'

// Authentication state will be managed here
const isLoggedIn = ref(false)

const toggleAuth = () => {
  isLoggedIn.value = !isLoggedIn.value
}
</script>

<template>
  <header class="app-header">
    <div class="header-content">
      <div class="header-left">
        <h1 class="app-title">License Management</h1>
      </div>
      <div class="header-right">
        <span class="auth-status">
          {{ isLoggedIn ? 'Logged In' : 'Logged Out' }}
        </span>
        <button class="auth-button" @click="toggleAuth">
          {{ isLoggedIn ? 'Logout' : 'Login' }}
        </button>
      </div>
    </div>
  </header>
</template>

<style scoped>
.app-header {
  background-color: #2c3e50;
  color: white;
  padding: 1rem 2rem;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.header-content {
  max-width: 1400px;
  margin: 0 auto;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.header-left {
  display: flex;
  align-items: center;
}

.app-title {
  font-size: 1.5rem;
  font-weight: 600;
  margin: 0;
}

.header-right {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.auth-status {
  font-size: 0.9rem;
  color: #ecf0f1;
}

.auth-button {
  background-color: #3498db;
  color: white;
  border: none;
  padding: 0.5rem 1rem;
  border-radius: 4px;
  cursor: pointer;
  font-size: 0.9rem;
  font-weight: 500;
  transition: background-color 0.2s;
}

.auth-button:hover {
  background-color: #2980b9;
}

.auth-button:active {
  transform: translateY(1px);
}
</style>
```

**Design Decision**: The header uses a dark color scheme (#2c3e50) for visual hierarchy and contrast. The login/logout button is positioned in the top right corner as specified. The authentication state is displayed next to the button for clarity. The component uses local state (ref) for now, which will be refactored to use a composable for shared state management in later requirements.

#### Footer Component

**File**: `src/components/layout/AppFooter.vue`

```vue
<script setup lang="ts">
// Navigation will be added in later requirements
</script>

<template>
  <footer class="app-footer">
    <div class="footer-content">
      <nav class="footer-nav">
        <!-- Navigation links will be added here -->
        <p class="footer-placeholder">Navigation links will appear here</p>
      </nav>
    </div>
  </footer>
</template>

<style scoped>
.app-footer {
  background-color: #000000;
  color: white;
  padding: 1.5rem 2rem;
  margin-top: auto;
}

.footer-content {
  max-width: 1400px;
  margin: 0 auto;
}

.footer-nav {
  display: flex;
  justify-content: center;
  align-items: center;
}

.footer-placeholder {
  color: #95a5a6;
  font-size: 0.9rem;
  margin: 0;
}
</style>
```

**Design Decision**: The footer uses a black background (#000000) as specified in the requirements. The navigation will be implemented in subsequent requirements. The footer uses `margin-top: auto` in combination with the flex layout to ensure it stays at the bottom of the page.

### 3. Authentication State Management (Requirement 3)

#### Authentication Composable

**File**: `src/composables/useAuth.ts`

```typescript
import { ref, computed, readonly } from 'vue'

// Shared authentication state
const isLoggedIn = ref(false)
const apiKey = ref<string | null>(null)

export function useAuth() {
  const login = () => {
    isLoggedIn.value = true
    // In a real implementation, this would set the actual API key
    apiKey.value = 'LMA_sample_key'
  }

  const logout = () => {
    isLoggedIn.value = false
    apiKey.value = null
  }

  const toggleAuth = () => {
    if (isLoggedIn.value) {
      logout()
    } else {
      login()
    }
  }

  return {
    isLoggedIn: readonly(isLoggedIn),
    apiKey: readonly(apiKey),
    login,
    logout,
    toggleAuth
  }
}
```

**Design Decision**: Use a composable pattern to share authentication state across components. The state is defined outside the composable function to ensure it's shared across all component instances. The `readonly` wrapper prevents external mutation of the state, enforcing that changes only happen through the provided methods.

#### Updated Header Component

**File**: `src/components/layout/AppHeader.vue`

```vue
<script setup lang="ts">
import { useAuth } from '@/composables/useAuth'

const { isLoggedIn, toggleAuth } = useAuth()
</script>

<template>
  <header class="app-header">
    <div class="header-content">
      <div class="header-left">
        <h1 class="app-title">License Management</h1>
      </div>
      <div class="header-right">
        <span class="auth-status">
          Status: {{ isLoggedIn ? 'Logged In' : 'Logged Out' }}
        </span>
        <button class="auth-button" @click="toggleAuth">
          {{ isLoggedIn ? 'Logout' : 'Login' }}
        </button>
      </div>
    </div>
  </header>
</template>

<style scoped>
.app-header {
  background-color: #2c3e50;
  color: white;
  padding: 1rem 2rem;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.header-content {
  max-width: 1400px;
  margin: 0 auto;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.header-left {
  display: flex;
  align-items: center;
}

.app-title {
  font-size: 1.5rem;
  font-weight: 600;
  margin: 0;
}

.header-right {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.auth-status {
  font-size: 0.9rem;
  color: #ecf0f1;
}

.auth-button {
  background-color: #3498db;
  color: white;
  border: none;
  padding: 0.5rem 1rem;
  border-radius: 4px;
  cursor: pointer;
  font-size: 0.9rem;
  font-weight: 500;
  transition: background-color 0.2s;
}

.auth-button:hover {
  background-color: #2980b9;
}

.auth-button:active {
  transform: translateY(1px);
}
</style>
```

**Design Decision**: The header now uses the shared authentication composable instead of local state. This ensures the authentication state persists across page navigations and can be accessed by other components that need it (like API service for adding auth headers).

### 4. Navigation System (Requirement 4)

#### Router Configuration

**File**: `src/router/index.ts`

```typescript
import { createRouter, createWebHistory, RouteRecordRaw } from 'vue-router'
import AppLayout from '@/components/layout/AppLayout.vue'

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    component: AppLayout,
    children: [
      {
        path: '',
        redirect: '/customers'
      },
      {
        path: '/customers',
        name: 'customers',
        component: () => import('@/views/CustomersView.vue')
      },
      {
        path: '/products',
        name: 'products',
        component: () => import('@/views/ProductsView.vue')
      },
      {
        path: '/skus',
        name: 'skus',
        component: () => import('@/views/SkusView.vue')
      },
      {
        path: '/licenses',
        name: 'licenses',
        component: () => import('@/views/LicensesView.vue')
      },
      {
        path: '/rsa-keys',
        name: 'rsa-keys',
        component: () => import('@/views/RsaKeysView.vue')
      },
      {
        path: '/api-keys',
        name: 'api-keys',
        component: () => import('@/views/ApiKeysView.vue')
      }
    ]
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router
```

**Design Decision**: Use Vue Router 4 with nested routes. The AppLayout is the parent route, ensuring all child views are wrapped with the header and footer. Lazy loading is used for view components to improve initial load time. The default route redirects to customers page.

#### Updated Footer Component with Navigation

**File**: `src/components/layout/AppFooter.vue`

```vue
<script setup lang="ts">
import { useRoute } from 'vue-router'
import { computed } from 'vue'

const route = useRoute()

interface NavLink {
  name: string
  path: string
  label: string
}

const navLinks: NavLink[] = [
  { name: 'customers', path: '/customers', label: 'Customers' },
  { name: 'products', path: '/products', label: 'Products' },
  { name: 'skus', path: '/skus', label: 'SKUs' },
  { name: 'licenses', path: '/licenses', label: 'Licenses' },
  { name: 'rsa-keys', path: '/rsa-keys', label: 'RSA Keys' },
  { name: 'api-keys', path: '/api-keys', label: 'API Keys' }
]

const isActive = (linkName: string) => {
  return computed(() => route.name === linkName)
}
</script>

<template>
  <footer class="app-footer">
    <div class="footer-content">
      <nav class="footer-nav">
        <router-link
          v-for="link in navLinks"
          :key="link.name"
          :to="link.path"
          class="nav-link"
          :class="{ active: isActive(link.name).value }"
        >
          {{ link.label }}
        </router-link>
      </nav>
    </div>
  </footer>
</template>

<style scoped>
.app-footer {
  background-color: #000000;
  color: white;
  padding: 1.5rem 2rem;
  margin-top: auto;
}

.footer-content {
  max-width: 1400px;
  margin: 0 auto;
}

.footer-nav {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 2rem;
  flex-wrap: wrap;
}

.nav-link {
  color: #ecf0f1;
  text-decoration: none;
  font-size: 0.95rem;
  font-weight: 500;
  padding: 0.5rem 1rem;
  border-radius: 4px;
  transition: all 0.2s;
  position: relative;
}

.nav-link:hover {
  color: #ffffff;
  background-color: rgba(255, 255, 255, 0.1);
}

.nav-link.active {
  color: #3498db;
  background-color: rgba(52, 152, 219, 0.1);
}

.nav-link.active::after {
  content: '';
  position: absolute;
  bottom: 0;
  left: 50%;
  transform: translateX(-50%);
  width: 60%;
  height: 2px;
  background-color: #3498db;
}

@media (max-width: 768px) {
  .footer-nav {
    flex-direction: column;
    gap: 0.5rem;
  }
  
  .nav-link {
    width: 100%;
    text-align: center;
  }
}
</style>
```

**Design Decision**: The footer navigation uses Vue Router's `<router-link>` component for declarative navigation. The active link is highlighted using route matching, with both a background color change and an underline indicator. The navigation is horizontally arranged on desktop and stacks vertically on mobile (addressing part of Requirement 15). The black background is maintained as specified.

#### Placeholder View Components

**File**: `src/views/SkusView.vue`, `src/views/LicensesView.vue`, `src/views/RsaKeysView.vue`, `src/views/ApiKeysView.vue`

```vue
<script setup lang="ts">
// Entity management will be implemented in later requirements
</script>

<template>
  <div class="view-container">
    <h2>[Entity Name]</h2>
    <p>[Entity] management interface will be implemented here.</p>
  </div>
</template>

<style scoped>
.view-container {
  padding: 1rem;
}

h2 {
  color: #2c3e50;
  margin-bottom: 1rem;
}
</style>
```

**Design Decision**: Create placeholder view components for entity types not yet implemented. This allows the navigation to work immediately and provides a foundation for incremental development.

### 5. Customer Management Interface (Requirement 5)

#### Customer Data Model

**File**: `src/types/customer.ts`

```typescript
export interface Customer {
  id: number
  name: string
  email: string
  organization: string
  isVisible: boolean
  createdAt: string
  updatedAt: string
}

export interface CreateCustomerDto {
  name: string
  email: string
  organization: string
  isVisible: boolean
}

export interface UpdateCustomerDto {
  name: string
  email: string
  organization: string
  isVisible: boolean
}
```

**Design Decision**: Separate interfaces for entity, create, and update operations. This matches the API contract and provides type safety for different operations.

#### Customer Service

**File**: `src/services/customerService.ts`

```typescript
import axios from 'axios'
import type { Customer, CreateCustomerDto, UpdateCustomerDto } from '@/types/customer'

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000'

export const customerService = {
  async getAll(): Promise<Customer[]> {
    const response = await axios.get(`${API_BASE_URL}/api/customers`)
    return response.data
  },

  async getById(id: number): Promise<Customer> {
    const response = await axios.get(`${API_BASE_URL}/api/customers/${id}`)
    return response.data
  },

  async create(customer: CreateCustomerDto): Promise<Customer> {
    const response = await axios.post(`${API_BASE_URL}/api/customers`, customer)
    return response.data
  },

  async update(id: number, customer: UpdateCustomerDto): Promise<Customer> {
    const response = await axios.put(`${API_BASE_URL}/api/customers/${id}`, customer)
    return response.data
  },

  async delete(id: number): Promise<void> {
    await axios.delete(`${API_BASE_URL}/api/customers/${id}`)
  }
}
```

**Design Decision**: Create a service layer to encapsulate API calls. This separates concerns and makes it easier to mock for testing. The service uses axios for HTTP requests and returns typed responses.

#### Customer Data Table Component

**File**: `src/components/entities/customers/CustomerTable.vue`

```vue
<script setup lang="ts">
import type { Customer } from '@/types/customer'

interface Props {
  customers: Customer[]
  loading: boolean
}

interface Emits {
  (e: 'edit', customer: Customer): void
  (e: 'delete', customer: Customer): void
}

defineProps<Props>()
const emit = defineEmits<Emits>()

const handleEdit = (customer: Customer) => {
  emit('edit', customer)
}

const handleDelete = (customer: Customer) => {
  if (confirm(`Are you sure you want to delete customer "${customer.name}"?`)) {
    emit('delete', customer)
  }
}
</script>

<template>
  <div class="table-container">
    <div v-if="loading" class="loading">Loading customers...</div>
    <table v-else class="data-table">
      <thead>
        <tr>
          <th>Name</th>
          <th>Email</th>
          <th>Organization</th>
          <th>Visible</th>
          <th>Actions</th>
        </tr>
      </thead>
      <tbody>
        <tr v-if="customers.length === 0">
          <td colspan="5" class="no-data">No customers found</td>
        </tr>
        <tr v-for="customer in customers" :key="customer.id">
          <td>{{ customer.name }}</td>
          <td>{{ customer.email }}</td>
          <td>{{ customer.organization }}</td>
          <td>
            <span :class="['status-badge', customer.isVisible ? 'visible' : 'hidden']">
              {{ customer.isVisible ? 'Yes' : 'No' }}
            </span>
          </td>
          <td class="actions">
            <button class="btn btn-edit" @click="handleEdit(customer)">Edit</button>
            <button class="btn btn-delete" @click="handleDelete(customer)">Delete</button>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<style scoped>
.table-container {
  overflow-x: auto;
  background: white;
  border-radius: 8px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.loading {
  padding: 2rem;
  text-align: center;
  color: #7f8c8d;
}

.data-table {
  width: 100%;
  border-collapse: collapse;
  min-width: 600px;
}

.data-table th {
  background-color: #f8f9fa;
  padding: 1rem;
  text-align: left;
  font-weight: 600;
  color: #2c3e50;
  border-bottom: 2px solid #e9ecef;
}

.data-table td {
  padding: 1rem;
  border-bottom: 1px solid #e9ecef;
  color: #495057;
}

.data-table tbody tr:hover {
  background-color: #f8f9fa;
}

.no-data {
  text-align: center;
  color: #7f8c8d;
  font-style: italic;
}

.status-badge {
  padding: 0.25rem 0.75rem;
  border-radius: 12px;
  font-size: 0.85rem;
  font-weight: 500;
}

.status-badge.visible {
  background-color: #d4edda;
  color: #155724;
}

.status-badge.hidden {
  background-color: #f8d7da;
  color: #721c24;
}

.actions {
  display: flex;
  gap: 0.5rem;
}

.btn {
  padding: 0.5rem 1rem;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 0.875rem;
  font-weight: 500;
  transition: all 0.2s;
}

.btn-edit {
  background-color: #3498db;
  color: white;
}

.btn-edit:hover {
  background-color: #2980b9;
}

.btn-delete {
  background-color: #e74c3c;
  color: white;
}

.btn-delete:hover {
  background-color: #c0392b;
}
</style>
```

**Design Decision**: Create a reusable table component that emits events for edit and delete actions. The parent component handles the actual logic. The table includes loading state, empty state, and confirmation dialog for delete operations. Horizontal scrolling is enabled for smaller screens.

#### Customer Form Component

**File**: `src/components/entities/customers/CustomerForm.vue`

```vue
<script setup lang="ts">
import { ref, watch } from 'vue'
import type { Customer, CreateCustomerDto, UpdateCustomerDto } from '@/types/customer'

interface Props {
  customer?: Customer | null
  mode: 'create' | 'edit'
}

interface Emits {
  (e: 'submit', data: CreateCustomerDto | UpdateCustomerDto): void
  (e: 'cancel'): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

const formData = ref({
  name: '',
  email: '',
  organization: '',
  isVisible: true
})

const errors = ref({
  name: '',
  email: '',
  organization: ''
})

// Populate form when editing
watch(() => props.customer, (customer) => {
  if (customer) {
    formData.value = {
      name: customer.name,
      email: customer.email,
      organization: customer.organization,
      isVisible: customer.isVisible
    }
  }
}, { immediate: true })

const validateEmail = (email: string): boolean => {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
  return emailRegex.test(email)
}

const validateForm = (): boolean => {
  let isValid = true
  errors.value = { name: '', email: '', organization: '' }

  if (!formData.value.name.trim()) {
    errors.value.name = 'Name is required'
    isValid = false
  }

  if (!formData.value.email.trim()) {
    errors.value.email = 'Email is required'
    isValid = false
  } else if (!validateEmail(formData.value.email)) {
    errors.value.email = 'Invalid email format'
    isValid = false
  }

  if (!formData.value.organization.trim()) {
    errors.value.organization = 'Organization is required'
    isValid = false
  }

  return isValid
}

const handleSubmit = () => {
  if (validateForm()) {
    emit('submit', formData.value)
  }
}

const handleCancel = () => {
  emit('cancel')
}
</script>

<template>
  <div class="form-overlay" @click.self="handleCancel">
    <div class="form-modal">
      <h3>{{ mode === 'create' ? 'Create Customer' : 'Edit Customer' }}</h3>
      <form @submit.prevent="handleSubmit">
        <div class="form-group">
          <label for="name">Name *</label>
          <input
            id="name"
            v-model="formData.name"
            type="text"
            :class="{ error: errors.name }"
            @input="errors.name = ''"
          />
          <span v-if="errors.name" class="error-message">{{ errors.name }}</span>
        </div>

        <div class="form-group">
          <label for="email">Email *</label>
          <input
            id="email"
            v-model="formData.email"
            type="email"
            :class="{ error: errors.email }"
            @input="errors.email = ''"
          />
          <span v-if="errors.email" class="error-message">{{ errors.email }}</span>
        </div>

        <div class="form-group">
          <label for="organization">Organization *</label>
          <input
            id="organization"
            v-model="formData.organization"
            type="text"
            :class="{ error: errors.organization }"
            @input="errors.organization = ''"
          />
          <span v-if="errors.organization" class="error-message">{{ errors.organization }}</span>
        </div>

        <div class="form-group checkbox-group">
          <label>
            <input v-model="formData.isVisible" type="checkbox" />
            <span>Visible</span>
          </label>
        </div>

        <div class="form-actions">
          <button type="button" class="btn btn-cancel" @click="handleCancel">Cancel</button>
          <button type="submit" class="btn btn-submit">
            {{ mode === 'create' ? 'Create' : 'Update' }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<style scoped>
.form-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
}

.form-modal {
  background: white;
  padding: 2rem;
  border-radius: 8px;
  width: 90%;
  max-width: 500px;
  max-height: 90vh;
  overflow-y: auto;
}

.form-modal h3 {
  margin: 0 0 1.5rem 0;
  color: #2c3e50;
}

.form-group {
  margin-bottom: 1.5rem;
}

.form-group label {
  display: block;
  margin-bottom: 0.5rem;
  color: #2c3e50;
  font-weight: 500;
}

.form-group input[type="text"],
.form-group input[type="email"] {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 1rem;
  transition: border-color 0.2s;
}

.form-group input[type="text"]:focus,
.form-group input[type="email"]:focus {
  outline: none;
  border-color: #3498db;
}

.form-group input.error {
  border-color: #e74c3c;
}

.error-message {
  display: block;
  color: #e74c3c;
  font-size: 0.875rem;
  margin-top: 0.25rem;
}

.checkbox-group label {
  display: flex;
  align-items: center;
  cursor: pointer;
}

.checkbox-group input[type="checkbox"] {
  margin-right: 0.5rem;
  cursor: pointer;
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 1rem;
  margin-top: 2rem;
}

.btn {
  padding: 0.75rem 1.5rem;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 1rem;
  font-weight: 500;
  transition: all 0.2s;
}

.btn-cancel {
  background-color: #95a5a6;
  color: white;
}

.btn-cancel:hover {
  background-color: #7f8c8d;
}

.btn-submit {
  background-color: #27ae60;
  color: white;
}

.btn-submit:hover {
  background-color: #229954;
}
</style>
```

**Design Decision**: Create a modal form component that handles both create and edit modes. The form includes validation for required fields and email format, with real-time error clearing. The modal overlay can be dismissed by clicking outside the form.

#### Customers View

**File**: `src/views/CustomersView.vue`

```vue
<script setup lang="ts">
import { ref, onMounted } from 'vue'
import CustomerTable from '@/components/entities/customers/CustomerTable.vue'
import CustomerForm from '@/components/entities/customers/CustomerForm.vue'
import { customerService } from '@/services/customerService'
import type { Customer, CreateCustomerDto, UpdateCustomerDto } from '@/types/customer'

const customers = ref<Customer[]>([])
const loading = ref(false)
const showForm = ref(false)
const formMode = ref<'create' | 'edit'>('create')
const selectedCustomer = ref<Customer | null>(null)

const loadCustomers = async () => {
  loading.value = true
  try {
    customers.value = await customerService.getAll()
  } catch (error) {
    console.error('Failed to load customers:', error)
    // Error notification will be handled in later requirements
  } finally {
    loading.value = false
  }
}

const handleCreate = () => {
  formMode.value = 'create'
  selectedCustomer.value = null
  showForm.value = true
}

const handleEdit = (customer: Customer) => {
  formMode.value = 'edit'
  selectedCustomer.value = customer
  showForm.value = true
}

const handleDelete = async (customer: Customer) => {
  try {
    await customerService.delete(customer.id)
    await loadCustomers()
    // Success notification will be handled in later requirements
  } catch (error) {
    console.error('Failed to delete customer:', error)
    // Error notification will be handled in later requirements
  }
}

const handleFormSubmit = async (data: CreateCustomerDto | UpdateCustomerDto) => {
  try {
    if (formMode.value === 'create') {
      await customerService.create(data as CreateCustomerDto)
    } else if (selectedCustomer.value) {
      await customerService.update(selectedCustomer.value.id, data as UpdateCustomerDto)
    }
    showForm.value = false
    await loadCustomers()
    // Success notification will be handled in later requirements
  } catch (error) {
    console.error('Failed to save customer:', error)
    // Error notification will be handled in later requirements
  }
}

const handleFormCancel = () => {
  showForm.value = false
  selectedCustomer.value = null
}

onMounted(() => {
  loadCustomers()
})
</script>

<template>
  <div class="view-container">
    <div class="view-header">
      <h2>Customers</h2>
      <button class="btn btn-create" @click="handleCreate">Create Customer</button>
    </div>
    
    <CustomerTable
      :customers="customers"
      :loading="loading"
      @edit="handleEdit"
      @delete="handleDelete"
    />

    <CustomerForm
      v-if="showForm"
      :customer="selectedCustomer"
      :mode="formMode"
      @submit="handleFormSubmit"
      @cancel="handleFormCancel"
    />
  </div>
</template>

<style scoped>
.view-container {
  padding: 1rem;
}

.view-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}

.view-header h2 {
  color: #2c3e50;
  margin: 0;
}

.btn-create {
  background-color: #27ae60;
  color: white;
  border: none;
  padding: 0.75rem 1.5rem;
  border-radius: 4px;
  cursor: pointer;
  font-size: 1rem;
  font-weight: 500;
  transition: background-color 0.2s;
}

.btn-create:hover {
  background-color: #229954;
}
</style>
```

**Design Decision**: The view component orchestrates the interaction between the table and form components. It manages the state for loading, form visibility, and selected customer. All CRUD operations refresh the table data after completion.

### 6. Product Management Interface (Requirement 6)

#### Product Data Model

**File**: `src/types/product.ts`

```typescript
export interface Product {
  id: number
  name: string
  productCode: string
  version: string
  description: string
  createdAt: string
  updatedAt: string
}

export interface CreateProductDto {
  name: string
  productCode: string
  version: string
  description: string
}

export interface UpdateProductDto {
  name: string
  productCode: string
  version: string
  description: string
}
```

#### Product Service

**File**: `src/services/productService.ts`

```typescript
import axios from 'axios'
import type { Product, CreateProductDto, UpdateProductDto } from '@/types/product'

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000'

export const productService = {
  async getAll(): Promise<Product[]> {
    const response = await axios.get(`${API_BASE_URL}/api/products`)
    return response.data
  },

  async getById(id: number): Promise<Product> {
    const response = await axios.get(`${API_BASE_URL}/api/products/${id}`)
    return response.data
  },

  async create(product: CreateProductDto): Promise<Product> {
    const response = await axios.post(`${API_BASE_URL}/api/products`, product)
    return response.data
  },

  async update(id: number, product: UpdateProductDto): Promise<Product> {
    const response = await axios.put(`${API_BASE_URL}/api/products/${id}`, product)
    return response.data
  },

  async delete(id: number): Promise<void> {
    await axios.delete(`${API_BASE_URL}/api/products/${id}`)
  }
}
```

**Design Decision**: Product service follows the same pattern as customer service for consistency. This makes the codebase predictable and easier to maintain.

#### Product Components

The Product management interface will follow the same component structure as Customers:

- **ProductTable.vue**: Displays products in a table with columns for Name, ProductCode, Version, and Description
- **ProductForm.vue**: Modal form for creating/editing products with validation for required fields
- **ProductsView.vue**: Main view that orchestrates table and form interactions

**Design Decision**: Reuse the same architectural pattern established for customers. This creates consistency across the application and reduces cognitive load for developers. The components will have the same structure but with product-specific fields and validation.

### 7. SKU Management Interface (Requirement 7)

#### SKU Data Model

**File**: `src/types/sku.ts`

```typescript
export interface Sku {
  id: number
  name: string
  skuCode: string
  productId: number
  productName: string
  description: string
  createdAt: string
  updatedAt: string
}

export interface CreateSkuDto {
  name: string
  skuCode: string
  productId: number
  description: string
}

export interface UpdateSkuDto {
  name: string
  skuCode: string
  productId: number
  description: string
}
```

**Design Decision**: SKU includes both productId (for API operations) and productName (for display). The API response includes the related product name through a join or projection.

#### SKU Service

**File**: `src/services/skuService.ts`

```typescript
import axios from 'axios'
import type { Sku, CreateSkuDto, UpdateSkuDto } from '@/types/sku'

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000'

export const skuService = {
  async getAll(): Promise<Sku[]> {
    const response = await axios.get(`${API_BASE_URL}/api/skus`)
    return response.data
  },

  async getById(id: number): Promise<Sku> {
    const response = await axios.get(`${API_BASE_URL}/api/skus/${id}`)
    return response.data
  },

  async create(sku: CreateSkuDto): Promise<Sku> {
    const response = await axios.post(`${API_BASE_URL}/api/skus`, sku)
    return response.data
  },

  async update(id: number, sku: UpdateSkuDto): Promise<Sku> {
    const response = await axios.put(`${API_BASE_URL}/api/skus/${id}`, sku)
    return response.data
  },

  async delete(id: number): Promise<void> {
    await axios.delete(`${API_BASE_URL}/api/skus/${id}`)
  }
}
```

#### SKU Components

The SKU management interface follows the established pattern:

- **SkuTable.vue**: Displays SKUs with columns for Name, SkuCode, Product Name, and Description
- **SkuForm.vue**: Modal form with a dropdown/select for Product selection, plus fields for name, skuCode, and description
- **SkusView.vue**: Orchestrates CRUD operations and loads products for the form dropdown

**Design Decision**: The SKU form requires loading the list of products to populate a dropdown selector. This will be handled in the view component and passed as a prop to the form.

### 8. License Management Interface (Requirement 8)

#### License Data Model

**File**: `src/types/license.ts`

```typescript
export interface License {
  id: number
  customerId: number
  customerName: string
  productId: number
  productName: string
  skuId: number
  skuName: string
  rsaKeyId: number
  rsaKeyName: string
  licenseType: string
  status: string
  expirationDate: string | null
  maxActivations: number
  currentActivations: number
  licenseKey: string
  createdAt: string
  updatedAt: string
}

export interface CreateLicenseDto {
  customerId: number
  productId: number
  skuId: number
  rsaKeyId: number
  licenseType: string
  expirationDate: string | null
  maxActivations: number
}

export interface UpdateLicenseDto {
  customerId: number
  productId: number
  skuId: number
  rsaKeyId: number
  licenseType: string
  expirationDate: string | null
  maxActivations: number
  status: string
}
```

**Design Decision**: License is the most complex entity with multiple foreign key relationships. The interface includes both IDs (for operations) and names (for display). The form will need to load customers, products, SKUs, and RSA keys for selection dropdowns.

#### License Service

**File**: `src/services/licenseService.ts`

```typescript
import axios from 'axios'
import type { License, CreateLicenseDto, UpdateLicenseDto } from '@/types/license'

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000'

export const licenseService = {
  async getAll(): Promise<License[]> {
    const response = await axios.get(`${API_BASE_URL}/api/licenses`)
    return response.data
  },

  async getById(id: number): Promise<License> {
    const response = await axios.get(`${API_BASE_URL}/api/licenses/${id}`)
    return response.data
  },

  async create(license: CreateLicenseDto): Promise<License> {
    const response = await axios.post(`${API_BASE_URL}/api/licenses`, license)
    return response.data
  },

  async update(id: number, license: UpdateLicenseDto): Promise<License> {
    const response = await axios.put(`${API_BASE_URL}/api/licenses/${id}`, license)
    return response.data
  },

  async delete(id: number): Promise<void> {
    await axios.delete(`${API_BASE_URL}/api/licenses/${id}`)
  }
}
```

#### License Components

The License management interface follows the established pattern with additional complexity:

- **LicenseTable.vue**: Displays licenses with columns for Customer Name, Product Name, LicenseType, Status, ExpirationDate, and CurrentActivations
- **LicenseForm.vue**: Complex modal form with multiple dropdowns for Customer, Product, SKU, and RSA Key selection, plus fields for license type, expiration date, and max activations
- **LicensesView.vue**: Orchestrates CRUD operations and loads all related entities (customers, products, SKUs, RSA keys) for form dropdowns

**Design Decision**: The license form is the most complex, requiring multiple entity selections. The view will load all necessary reference data and pass it to the form. Consider implementing cascading dropdowns where SKU selection is filtered by the selected Product.

### 9. RSA Key Management Interface (Requirement 9)

#### RSA Key Data Model

**File**: `src/types/rsaKey.ts`

```typescript
export interface RsaKey {
  id: number
  name: string
  keySize: number
  createdBy: string
  createdAt: string
}

export interface CreateRsaKeyDto {
  name: string
  keySize: number
  createdBy: string
}
```

**Design Decision**: RSA Keys do not support update operations (as specified in requirements), so no UpdateRsaKeyDto is needed. The private key is never exposed to the frontend.

#### RSA Key Service

**File**: `src/services/rsaKeyService.ts`

```typescript
import axios from 'axios'
import type { RsaKey, CreateRsaKeyDto } from '@/types/rsaKey'

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000'

export const rsaKeyService = {
  async getAll(): Promise<RsaKey[]> {
    const response = await axios.get(`${API_BASE_URL}/api/rsakeys`)
    return response.data
  },

  async getById(id: number): Promise<RsaKey> {
    const response = await axios.get(`${API_BASE_URL}/api/rsakeys/${id}`)
    return response.data
  },

  async create(rsaKey: CreateRsaKeyDto): Promise<RsaKey> {
    const response = await axios.post(`${API_BASE_URL}/api/rsakeys`, rsaKey)
    return response.data
  },

  async delete(id: number): Promise<void> {
    await axios.delete(`${API_BASE_URL}/api/rsakeys/${id}`)
  }
}
```

#### RSA Key Components

The RSA Key management interface is simplified (no edit functionality):

- **RsaKeyTable.vue**: Displays RSA keys with columns for Name, KeySize, CreatedBy, and CreatedAt. Only includes Delete button (no Edit button)
- **RsaKeyForm.vue**: Modal form for creating RSA keys with fields for name, key size (dropdown with options: 2048, 3072, 4096), and createdBy
- **RsaKeysView.vue**: Orchestrates create and delete operations only

**Design Decision**: Omit edit functionality as specified. RSA keys are immutable once created for security reasons. The key size field uses a dropdown with standard RSA key sizes.

### 10. API Key Management Interface (Requirement 10)

#### API Key Data Model

**File**: `src/types/apiKey.ts`

```typescript
export interface ApiKey {
  id: number
  name: string
  role: string
  isActive: boolean
  createdBy: string
  createdAt: string
  lastUsedAt: string | null
  // The actual key value is only returned on creation
}

export interface CreateApiKeyDto {
  name: string
  role: string
  createdBy: string
}

export interface UpdateApiKeyDto {
  name: string
  role: string
  isActive: boolean
}

export interface ApiKeyCreationResponse {
  id: number
  name: string
  role: string
  isActive: boolean
  createdBy: string
  createdAt: string
  key: string // Only available on creation
}
```

**Design Decision**: The actual API key value is only returned once upon creation and must be displayed to the user with a warning to save it. Subsequent operations only work with the key ID.

#### API Key Service

**File**: `src/services/apiKeyService.ts`

```typescript
import axios from 'axios'
import type { ApiKey, CreateApiKeyDto, UpdateApiKeyDto, ApiKeyCreationResponse } from '@/types/apiKey'

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000'

export const apiKeyService = {
  async getAll(): Promise<ApiKey[]> {
    const response = await axios.get(`${API_BASE_URL}/api/apikeys`)
    return response.data
  },

  async getById(id: number): Promise<ApiKey> {
    const response = await axios.get(`${API_BASE_URL}/api/apikeys/${id}`)
    return response.data
  },

  async create(apiKey: CreateApiKeyDto): Promise<ApiKeyCreationResponse> {
    const response = await axios.post(`${API_BASE_URL}/api/apikeys`, apiKey)
    return response.data
  },

  async update(id: number, apiKey: UpdateApiKeyDto): Promise<ApiKey> {
    const response = await axios.put(`${API_BASE_URL}/api/apikeys/${id}`, apiKey)
    return response.data
  },

  async delete(id: number): Promise<void> {
    await axios.delete(`${API_BASE_URL}/api/apikeys/${id}`)
  }
}
```

#### API Key Components

The API Key management interface includes special handling for key display:

- **ApiKeyTable.vue**: Displays API keys with columns for Name, Role, IsActive (badge), CreatedBy, and LastUsedAt. Includes Edit and Delete buttons
- **ApiKeyForm.vue**: Modal form with fields for name, role (dropdown: Admin, User), and isActive (checkbox, only shown in edit mode)
- **ApiKeyDisplayModal.vue**: Special modal that displays the newly created API key with a copy button and warning message
- **ApiKeysView.vue**: Orchestrates CRUD operations and shows the key display modal after successful creation

**Design Decision**: Create a separate modal component to display the API key after creation. This modal should be prominent and include a warning that the key won't be shown again. Include a copy-to-clipboard button for user convenience.

## Data Models

### TypeScript Interfaces

**File**: `src/types/index.ts`

```typescript
// Authentication state interface
export interface AuthState {
  isLoggedIn: boolean
  apiKey: string | null
}

// Navigation link interface
export interface NavLink {
  name: string
  path: string
  label: string
}
```

**Design Decision**: Define TypeScript interfaces early to establish type contracts. Entity-specific types are defined in separate files (customer.ts, product.ts, etc.) for better organization.

## Error Handling

The application implements comprehensive error handling:

1. **API Error Interceptor**: Catches all API errors and dispatches custom events for global handling
2. **Try-Catch Blocks**: All async operations in views are wrapped in try-catch blocks
3. **User-Friendly Messages**: Error notifications display user-friendly messages instead of technical details
4. **Validation Errors**: Form validation errors are displayed inline next to the relevant fields
5. **Loading States**: Loading indicators prevent user confusion during async operations
6. **Confirmation Dialogs**: Destructive actions (delete) require confirmation to prevent accidents

## Testing Strategy

Testing approach for the application:

1. **Unit Tests**: Test composables (useAuth, usePagination, useNotification, useFormValidation) and service modules using Vitest
2. **Component Tests**: Test individual components in isolation using Vue Test Utils
3. **Integration Tests**: Test complete user flows (CRUD operations) using Vue Test Utils
4. **E2E Tests**: Optional end-to-end tests for critical paths using Playwright
5. **Type Checking**: TypeScript provides compile-time type checking to catch errors early

**Testing Priority**: Focus on testing business logic in composables and services first, as these are framework-agnostic and easier to test. Component tests should focus on user interactions and prop/event contracts.

## Architecture Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                         App.vue                              │
│                      (Root Component)                        │
└────────────────────────┬────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────┐
│                      Vue Router                              │
└────────────────────────┬────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────┐
│                    AppLayout.vue                             │
│  ┌──────────────────────────────────────────────────────┐  │
│  │              AppHeader.vue                            │  │
│  │  (Login/Logout, Auth Status Display)                 │  │
│  └──────────────────────────────────────────────────────┘  │
│  ┌──────────────────────────────────────────────────────┐  │
│  │              <router-view>                            │  │
│  │  (Entity Views: Customers, Products, SKUs, etc.)     │  │
│  └──────────────────────────────────────────────────────┘  │
│  ┌──────────────────────────────────────────────────────┐  │
│  │              AppFooter.vue                            │  │
│  │  (Navigation Links)                                   │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│                    Entity View Pattern                       │
│  (CustomersView, ProductsView, SkusView, etc.)              │
│                                                              │
│  ┌────────────────┐  ┌────────────────┐  ┌──────────────┐ │
│  │  EntityTable   │  │  EntityForm    │  │ Pagination   │ │
│  │  Component     │  │  Component     │  │ Component    │ │
│  └────────────────┘  └────────────────┘  └──────────────┘ │
│           │                   │                             │
│           └───────────┬───────┘                             │
│                       ▼                                      │
│              ┌─────────────────┐                            │
│              │ Entity Service  │                            │
│              │ (API Calls)     │                            │
│              └────────┬────────┘                            │
│                       │                                      │
│                       ▼                                      │
│              ┌─────────────────┐                            │
│              │  Axios Instance │                            │
│              │  (with Auth     │                            │
│              │   Interceptor)  │                            │
│              └────────┬────────┘                            │
│                       │                                      │
│                       ▼                                      │
│              ┌─────────────────┐                            │
│              │ License Mgmt API│                            │
│              └─────────────────┘                            │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│                  Shared Composables                          │
│                                                              │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────────┐ │
│  │  useAuth()   │  │usePagination │  │useNotification() │ │
│  │              │  │    ()        │  │                  │ │
│  └──────────────┘  └──────────────┘  └──────────────────┘ │
│                                                              │
│  ┌──────────────────────────────────────────────────────┐  │
│  │         useFormValidation()                           │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│                  Global Components                           │
│                                                              │
│  ┌────────────────────────┐  ┌──────────────────────────┐  │
│  │ NotificationContainer  │  │   LoadingSpinner         │  │
│  └────────────────────────┘  └──────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
```

## Implementation Notes

### Development Workflow

1. **Setup Phase**: Initialize Vite project, install dependencies, configure TypeScript
2. **Foundation Phase**: Implement layout components, routing, and authentication
3. **Entity Phase**: Implement entity management following the established pattern (one entity at a time)
4. **Enhancement Phase**: Add pagination, notifications, and responsive improvements
5. **Polish Phase**: Refine styling, add loading states, improve error handling

### Key Dependencies

```json
{
  "dependencies": {
    "vue": "^3.4.0",
    "vue-router": "^4.2.0",
    "axios": "^1.6.0"
  },
  "devDependencies": {
    "@vitejs/plugin-vue": "^5.0.0",
    "typescript": "^5.3.0",
    "vite": "^5.0.0",
    "vue-tsc": "^1.8.0"
  }
}
```

### File Organization Best Practices

- Keep components small and focused (< 300 lines)
- Use composition over inheritance
- Co-locate related files (component, types, tests)
- Use barrel exports (index.ts) for cleaner imports
- Separate business logic into composables
- Keep views thin - delegate to components and composables

### Performance Considerations

- Lazy load route components for code splitting
- Use `v-show` for frequently toggled elements, `v-if` for conditional rendering
- Implement virtual scrolling if tables exceed 1000 rows
- Consider server-side pagination for large datasets
- Optimize images and assets
- Use production builds for deployment

## Design Rationale Summary

1. **Vue 3 with Composition API**: Provides better TypeScript support, improved code organization, and better performance compared to Options API. Enables better code reuse through composables.

2. **Script Setup Syntax**: Reduces boilerplate and makes components more readable while maintaining full TypeScript support. This is the recommended approach for Vue 3.

3. **Component-Based Layout**: Separating header, footer, and layout into distinct components promotes reusability and maintainability. Each component has a single responsibility.

4. **Flexbox Layout**: Using flexbox for the main layout ensures the footer stays at the bottom and provides responsive behavior without complex positioning.

5. **Scoped Styles**: Each component has scoped styles to prevent CSS conflicts and improve maintainability. Global styles are minimal and only used for resets and utilities.

6. **TypeScript**: Provides type safety, better IDE support, and catches errors at compile time. Separate type files for each entity improve organization.

7. **Vite Build Tool**: Offers fast development server with HMR, optimized builds, and is the recommended tooling for Vue 3. Environment variable support simplifies configuration.

8. **Composable Pattern**: Using composables for shared logic (auth, pagination, notifications, validation) promotes code reuse and testability. Composables are framework-agnostic and easy to test.

9. **Vue Router with Nested Routes**: Provides declarative navigation and ensures consistent layout across all pages. Lazy loading improves initial load time by code splitting.

10. **Service Layer Pattern**: Separating API calls into service modules keeps components focused on presentation logic. Services are easy to mock for testing.

11. **Axios Interceptors**: Centralized request/response handling ensures consistent authentication and error handling across all API calls.

12. **Modal Forms**: Using modal overlays for create/edit forms keeps users in context and provides a better UX than navigating to separate pages.

13. **Reusable Table Pattern**: Establishing a consistent pattern for entity tables (table component + form component + view orchestrator) makes the codebase predictable and maintainable.

14. **Client-Side Pagination**: Implementing pagination on the client side simplifies the initial implementation. Can be migrated to server-side pagination later if needed for performance.

15. **Notification System**: Global notification system with auto-dismiss for success messages and manual dismiss for errors provides consistent user feedback.

16. **Responsive Design**: Mobile-first approach with progressive enhancement ensures usability across all device sizes. Horizontal scrolling for tables preserves data integrity on small screens.

17. **Validation Composable**: Declarative validation rules make forms easier to maintain and provide consistent validation behavior across the application.

18. **Environment Configuration**: Using environment variables for API configuration enables different settings for development, staging, and production without code changes.

### 11. Table Pagination (Requirement 11)

#### Pagination Composable

**File**: `src/composables/usePagination.ts`

```typescript
import { ref, computed } from 'vue'

export function usePagination<T>(items: Ref<T[]>, itemsPerPage: number = 20) {
  const currentPage = ref(1)

  const totalPages = computed(() => {
    return Math.ceil(items.value.length / itemsPerPage)
  })

  const paginatedItems = computed(() => {
    const start = (currentPage.value - 1) * itemsPerPage
    const end = start + itemsPerPage
    return items.value.slice(start, end)
  })

  const showPagination = computed(() => {
    return items.value.length > itemsPerPage
  })

  const goToPage = (page: number) => {
    if (page >= 1 && page <= totalPages.value) {
      currentPage.value = page
    }
  }

  const nextPage = () => {
    if (currentPage.value < totalPages.value) {
      currentPage.value++
    }
  }

  const previousPage = () => {
    if (currentPage.value > 1) {
      currentPage.value--
    }
  }

  const resetPagination = () => {
    currentPage.value = 1
  }

  return {
    currentPage,
    totalPages,
    paginatedItems,
    showPagination,
    goToPage,
    nextPage,
    previousPage,
    resetPagination
  }
}
```

**Design Decision**: Create a reusable composable for pagination logic. This keeps the pagination logic separate from components and makes it easy to apply to any table. The composable automatically hides pagination when there are 20 or fewer items.

#### Pagination Component

**File**: `src/components/common/Pagination.vue`

```vue
<script setup lang="ts">
interface Props {
  currentPage: number
  totalPages: number
}

interface Emits {
  (e: 'page-change', page: number): void
  (e: 'next'): void
  (e: 'previous'): void
}

defineProps<Props>()
const emit = defineEmits<Emits>()

const handlePageChange = (page: number) => {
  emit('page-change', page)
}

const handleNext = () => {
  emit('next')
}

const handlePrevious = () => {
  emit('previous')
}
</script>

<template>
  <div class="pagination">
    <button
      class="pagination-btn"
      :disabled="currentPage === 1"
      @click="handlePrevious"
    >
      Previous
    </button>
    
    <span class="pagination-info">
      Page {{ currentPage }} of {{ totalPages }}
    </span>
    
    <button
      class="pagination-btn"
      :disabled="currentPage === totalPages"
      @click="handleNext"
    >
      Next
    </button>
  </div>
</template>

<style scoped>
.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 1rem;
  padding: 1.5rem;
  background: white;
  border-radius: 0 0 8px 8px;
}

.pagination-btn {
  padding: 0.5rem 1rem;
  border: 1px solid #ddd;
  background: white;
  border-radius: 4px;
  cursor: pointer;
  font-size: 0.9rem;
  transition: all 0.2s;
}

.pagination-btn:hover:not(:disabled) {
  background-color: #f8f9fa;
  border-color: #3498db;
}

.pagination-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.pagination-info {
  color: #2c3e50;
  font-size: 0.9rem;
}
</style>
```

**Design Decision**: Create a simple, reusable pagination component with previous/next buttons and page indicator. The component is presentational and emits events for the parent to handle.

#### Integration with Tables

Each table component will be updated to use pagination:

```vue
<script setup lang="ts">
import { usePagination } from '@/composables/usePagination'
import Pagination from '@/components/common/Pagination.vue'

// ... existing code ...

const { 
  paginatedItems, 
  showPagination, 
  currentPage, 
  totalPages,
  nextPage,
  previousPage,
  goToPage 
} = usePagination(customers, 20)
</script>

<template>
  <div class="table-container">
    <table class="data-table">
      <!-- Use paginatedItems instead of customers -->
      <tr v-for="customer in paginatedItems" :key="customer.id">
        <!-- ... -->
      </tr>
    </table>
    
    <Pagination
      v-if="showPagination"
      :current-page="currentPage"
      :total-pages="totalPages"
      @page-change="goToPage"
      @next="nextPage"
      @previous="previousPage"
    />
  </div>
</template>
```

**Design Decision**: Integrate pagination into existing table components by wrapping the data with the pagination composable. The pagination controls only appear when there are more than 20 items.

### 12. API Integration (Requirement 12)

#### Axios Configuration with Interceptors

**File**: `src/services/api.ts`

```typescript
import axios from 'axios'
import { useAuth } from '@/composables/useAuth'

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000'

// Create axios instance
export const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json'
  }
})

// Request interceptor to add auth header
api.interceptors.request.use(
  (config) => {
    const { apiKey } = useAuth()
    if (apiKey.value) {
      config.headers['Auth_Key'] = apiKey.value
    }
    return config
  },
  (error) => {
    return Promise.reject(error)
  }
)

// Response interceptor for error handling
api.interceptors.response.use(
  (response) => {
    return response
  },
  (error) => {
    const errorMessage = error.response?.data?.message || error.message || 'An error occurred'
    
    // Emit error event for global error handling
    window.dispatchEvent(new CustomEvent('api-error', { 
      detail: { 
        message: errorMessage,
        status: error.response?.status 
      } 
    }))
    
    return Promise.reject(error)
  }
)

export default api
```

**Design Decision**: Create a configured axios instance with interceptors. The request interceptor automatically adds the Auth_Key header from the auth composable. The response interceptor handles errors globally by dispatching custom events that can be caught by a notification system.

#### Update Services to Use Configured Axios

All service files will be updated to import the configured axios instance:

```typescript
import api from './api'

export const customerService = {
  async getAll(): Promise<Customer[]> {
    const response = await api.get('/api/customers')
    return response.data
  },
  // ... other methods
}
```

**Design Decision**: Centralize API configuration and use the configured instance across all services. This ensures consistent behavior for authentication and error handling.

#### Environment Configuration

**File**: `.env.development`

```
VITE_API_BASE_URL=http://localhost:5000
```

**File**: `.env.production`

```
VITE_API_BASE_URL=https://api.production.com
```

**Design Decision**: Use Vite's environment variables to configure the API base URL. This allows different configurations for development and production without code changes.

### 13. Form Validation (Requirement 13)

Form validation has been partially implemented in the Customer and Product forms. The validation strategy includes:

#### Validation Composable

**File**: `src/composables/useFormValidation.ts`

```typescript
import { ref } from 'vue'

export interface ValidationRule {
  required?: boolean
  email?: boolean
  minLength?: number
  maxLength?: number
  pattern?: RegExp
  custom?: (value: any) => boolean
  message: string
}

export interface ValidationRules {
  [key: string]: ValidationRule[]
}

export function useFormValidation() {
  const errors = ref<Record<string, string>>({})

  const validateField = (fieldName: string, value: any, rules: ValidationRule[]): boolean => {
    for (const rule of rules) {
      if (rule.required && !value) {
        errors.value[fieldName] = rule.message
        return false
      }

      if (rule.email && value) {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
        if (!emailRegex.test(value)) {
          errors.value[fieldName] = rule.message
          return false
        }
      }

      if (rule.minLength && value && value.length < rule.minLength) {
        errors.value[fieldName] = rule.message
        return false
      }

      if (rule.maxLength && value && value.length > rule.maxLength) {
        errors.value[fieldName] = rule.message
        return false
      }

      if (rule.pattern && value && !rule.pattern.test(value)) {
        errors.value[fieldName] = rule.message
        return false
      }

      if (rule.custom && !rule.custom(value)) {
        errors.value[fieldName] = rule.message
        return false
      }
    }

    errors.value[fieldName] = ''
    return true
  }

  const validateForm = (formData: Record<string, any>, rules: ValidationRules): boolean => {
    let isValid = true
    errors.value = {}

    for (const [fieldName, fieldRules] of Object.entries(rules)) {
      if (!validateField(fieldName, formData[fieldName], fieldRules)) {
        isValid = false
      }
    }

    return isValid
  }

  const clearError = (fieldName: string) => {
    errors.value[fieldName] = ''
  }

  const clearAllErrors = () => {
    errors.value = {}
  }

  return {
    errors,
    validateField,
    validateForm,
    clearError,
    clearAllErrors
  }
}
```

**Design Decision**: Create a reusable validation composable that supports common validation rules (required, email, length, pattern, custom). This provides a consistent validation approach across all forms.

#### Enhanced Form Components

Forms will use the validation composable:

```vue
<script setup lang="ts">
import { useFormValidation } from '@/composables/useFormValidation'

const { errors, validateForm, clearError } = useFormValidation()

const validationRules = {
  name: [
    { required: true, message: 'Name is required' },
    { minLength: 2, message: 'Name must be at least 2 characters' }
  ],
  email: [
    { required: true, message: 'Email is required' },
    { email: true, message: 'Invalid email format' }
  ]
}

const handleSubmit = () => {
  if (validateForm(formData.value, validationRules)) {
    emit('submit', formData.value)
  }
}
</script>
```

**Design Decision**: Use declarative validation rules that are easy to read and maintain. Real-time error clearing improves user experience.

### 14. User Feedback (Requirement 14)

#### Notification System

**File**: `src/composables/useNotification.ts`

```typescript
import { ref } from 'vue'

export interface Notification {
  id: number
  type: 'success' | 'error' | 'info'
  message: string
  duration?: number
}

const notifications = ref<Notification[]>([])
let notificationId = 0

export function useNotification() {
  const addNotification = (type: Notification['type'], message: string, duration: number = 3000) => {
    const id = ++notificationId
    const notification: Notification = { id, type, message, duration }
    
    notifications.value.push(notification)

    if (type === 'success' && duration > 0) {
      setTimeout(() => {
        removeNotification(id)
      }, duration)
    }
  }

  const removeNotification = (id: number) => {
    const index = notifications.value.findIndex(n => n.id === id)
    if (index > -1) {
      notifications.value.splice(index, 1)
    }
  }

  const success = (message: string) => {
    addNotification('success', message, 3000)
  }

  const error = (message: string) => {
    addNotification('error', message, 0) // Errors don't auto-dismiss
  }

  const info = (message: string) => {
    addNotification('info', message, 3000)
  }

  return {
    notifications,
    success,
    error,
    info,
    removeNotification
  }
}
```

**Design Decision**: Create a notification system using a composable with shared state. Success notifications auto-dismiss after 3 seconds, while error notifications require manual dismissal.

#### Notification Component

**File**: `src/components/common/NotificationContainer.vue`

```vue
<script setup lang="ts">
import { useNotification } from '@/composables/useNotification'

const { notifications, removeNotification } = useNotification()
</script>

<template>
  <div class="notification-container">
    <transition-group name="notification">
      <div
        v-for="notification in notifications"
        :key="notification.id"
        :class="['notification', `notification-${notification.type}`]"
      >
        <span class="notification-message">{{ notification.message }}</span>
        <button
          class="notification-close"
          @click="removeNotification(notification.id)"
        >
          ×
        </button>
      </div>
    </transition-group>
  </div>
</template>

<style scoped>
.notification-container {
  position: fixed;
  top: 1rem;
  right: 1rem;
  z-index: 9999;
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  max-width: 400px;
}

.notification {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 1rem 1.5rem;
  border-radius: 4px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  animation: slideIn 0.3s ease-out;
}

.notification-success {
  background-color: #d4edda;
  color: #155724;
  border-left: 4px solid #28a745;
}

.notification-error {
  background-color: #f8d7da;
  color: #721c24;
  border-left: 4px solid #dc3545;
}

.notification-info {
  background-color: #d1ecf1;
  color: #0c5460;
  border-left: 4px solid #17a2b8;
}

.notification-message {
  flex: 1;
  margin-right: 1rem;
}

.notification-close {
  background: none;
  border: none;
  font-size: 1.5rem;
  cursor: pointer;
  color: inherit;
  opacity: 0.7;
  transition: opacity 0.2s;
}

.notification-close:hover {
  opacity: 1;
}

.notification-enter-active,
.notification-leave-active {
  transition: all 0.3s ease;
}

.notification-enter-from {
  opacity: 0;
  transform: translateX(100%);
}

.notification-leave-to {
  opacity: 0;
  transform: translateX(100%);
}

@keyframes slideIn {
  from {
    opacity: 0;
    transform: translateX(100%);
  }
  to {
    opacity: 1;
    transform: translateX(0);
  }
}
</style>
```

**Design Decision**: Position notifications in the top-right corner with smooth animations. Use color coding for different notification types. Include a close button for manual dismissal.

#### Integration with Views

Views will use the notification system:

```vue
<script setup lang="ts">
import { useNotification } from '@/composables/useNotification'

const { success, error } = useNotification()

const handleDelete = async (customer: Customer) => {
  try {
    await customerService.delete(customer.id)
    success('Customer deleted successfully')
    await loadCustomers()
  } catch (err) {
    error('Failed to delete customer')
  }
}
</script>
```

#### Loading Indicator Component

**File**: `src/components/common/LoadingSpinner.vue`

```vue
<template>
  <div class="loading-spinner">
    <div class="spinner"></div>
  </div>
</template>

<style scoped>
.loading-spinner {
  display: flex;
  justify-content: center;
  align-items: center;
  padding: 2rem;
}

.spinner {
  width: 40px;
  height: 40px;
  border: 4px solid #f3f3f3;
  border-top: 4px solid #3498db;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}
</style>
```

**Design Decision**: Create a simple, reusable loading spinner component that can be used throughout the application.

### 15. Responsive Design (Requirement 15)

#### Responsive Breakpoints

The application will use the following breakpoints:

- **Desktop**: > 1024px
- **Tablet**: 768px - 1024px
- **Mobile**: < 768px

#### Global Responsive Styles

**File**: `src/assets/styles/responsive.css`

```css
/* Desktop (default) - > 1024px */
.container {
  max-width: 1400px;
  margin: 0 auto;
  padding: 2rem;
}

/* Tablet - 768px to 1024px */
@media (max-width: 1024px) {
  .container {
    padding: 1.5rem;
  }

  .data-table {
    font-size: 0.9rem;
  }

  .data-table th,
  .data-table td {
    padding: 0.75rem;
  }
}

/* Mobile - < 768px */
@media (max-width: 768px) {
  .container {
    padding: 1rem;
  }

  .view-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 1rem;
  }

  .view-header .btn-create {
    width: 100%;
  }

  .table-container {
    overflow-x: auto;
    -webkit-overflow-scrolling: touch;
  }

  .data-table {
    font-size: 0.85rem;
  }

  .data-table th,
  .data-table td {
    padding: 0.5rem;
    white-space: nowrap;
  }

  .actions {
    flex-direction: column;
    gap: 0.25rem;
  }

  .actions .btn {
    width: 100%;
    padding: 0.4rem 0.75rem;
  }

  .form-modal {
    width: 95%;
    padding: 1.5rem;
  }

  .notification-container {
    left: 1rem;
    right: 1rem;
    max-width: none;
  }
}
```

**Design Decision**: Use CSS media queries for responsive behavior. Tables use horizontal scrolling on mobile to maintain data integrity. Forms and buttons adapt to full-width on mobile for better touch targets.

#### Responsive Header

The header component already includes responsive behavior:

```css
@media (max-width: 768px) {
  .app-header {
    padding: 1rem;
  }

  .header-content {
    flex-direction: column;
    align-items: flex-start;
    gap: 1rem;
  }

  .header-right {
    width: 100%;
    justify-content: space-between;
  }
}
```

#### Responsive Footer Navigation

The footer navigation (already implemented in Requirement 4) stacks vertically on mobile:

```css
@media (max-width: 768px) {
  .footer-nav {
    flex-direction: column;
    gap: 0.5rem;
  }
  
  .nav-link {
    width: 100%;
    text-align: center;
  }
}
```

**Design Decision**: Prioritize usability on mobile devices by making interactive elements larger and easier to tap. Use horizontal scrolling for tables rather than hiding columns to preserve data visibility.
