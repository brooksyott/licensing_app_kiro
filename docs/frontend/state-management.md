# State Management

The frontend uses Vue 3's Composition API with composables for state management, avoiding the need for a centralized store like Vuex or Pinia for this application's scope.

## Composables

### useAuth
Manages authentication state across the application.

```typescript
// composables/useAuth.ts
import { ref } from 'vue'

const isLoggedIn = ref(false)

export function useAuth() {
  const toggleAuth = () => {
    isLoggedIn.value = !isLoggedIn.value
  }

  return {
    isLoggedIn,
    toggleAuth
  }
}
```

**Usage:**
- Shared authentication state
- Login/logout functionality
- Status display in header

### useNotification
Provides toast notification functionality.

```typescript
// composables/useNotification.ts
export function useNotification() {
  const success = (message: string) => { /* ... */ }
  const error = (message: string) => { /* ... */ }
  const info = (message: string) => { /* ... */ }
  const warning = (message: string) => { /* ... */ }

  return { success, error, info, warning }
}
```

**Usage:**
- Display success messages after operations
- Show error messages from API
- Provide user feedback

### usePagination
Handles pagination logic for data tables.

```typescript
// composables/usePagination.ts
export function usePagination<T>(items: Ref<T[]>, pageSize: number) {
  const currentPage = ref(1)
  const totalPages = computed(() => Math.ceil(items.value.length / pageSize))
  const paginatedItems = computed(() => { /* ... */ })

  const nextPage = () => { /* ... */ }
  const previousPage = () => { /* ... */ }
  const goToPage = (page: number) => { /* ... */ }

  return {
    paginatedItems,
    currentPage,
    totalPages,
    nextPage,
    previousPage,
    goToPage,
    showPagination
  }
}
```

**Usage:**
- Paginate large data sets
- Navigate between pages
- Display page indicators

## Component-Level State

Each view component manages its own local state:

```typescript
const customers = ref<Customer[]>([])
const loading = ref(false)
const showForm = ref(false)
const selectedCustomer = ref<Customer | null>(null)
```

## Service Layer

API calls are centralized in service files:

```typescript
// services/customerService.ts
export const customerService = {
  async getAll(): Promise<Customer[]> {
    const response = await api.get('/api/customers')
    return response.data
  },
  
  async create(customer: CreateCustomerDto): Promise<Customer> {
    const response = await api.post('/api/customers', customer)
    return response.data
  },
  
  // ... other methods
}
```

**Benefits:**
- Centralized API logic
- Type-safe responses
- Easy to test
- Reusable across components

## Error Handling

Global error handling through Axios interceptors:

```typescript
// services/api.ts
api.interceptors.response.use(
  response => response,
  error => {
    // Dispatch custom event for global error handling
    window.dispatchEvent(new CustomEvent('api-error', {
      detail: { message: errorMessage }
    }))
    return Promise.reject(error)
  }
)
```

**Features:**
- Automatic error notifications
- Consistent error messages
- User-friendly error display

## State Patterns

### Loading States
```typescript
const loading = ref(false)

const loadData = async () => {
  loading.value = true
  try {
    const data = await service.getAll()
    items.value = data
  } finally {
    loading.value = false
  }
}
```

### Form States
```typescript
const showForm = ref(false)
const formMode = ref<'create' | 'edit'>('create')
const selectedItem = ref<Item | null>(null)

const handleCreate = () => {
  formMode.value = 'create'
  selectedItem.value = null
  showForm.value = true
}

const handleEdit = (item: Item) => {
  formMode.value = 'edit'
  selectedItem.value = item
  showForm.value = true
}
```

### Operation States
```typescript
const operationLoading = ref(false)

const handleDelete = async (item: Item) => {
  operationLoading.value = true
  try {
    await service.delete(item.id)
    await loadData()
    success('Item deleted successfully')
  } finally {
    operationLoading.value = false
  }
}
```

## Why No Vuex/Pinia?

For this application, composables provide sufficient state management because:
- Limited shared state (mainly authentication)
- Most state is component-specific
- Simple data flow
- Easier to understand and maintain
- Less boilerplate code

If the application grows, consider adding Pinia for:
- Complex shared state
- Multiple user roles
- Real-time updates
- Offline support
- State persistence
