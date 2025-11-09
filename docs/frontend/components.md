# Frontend Components Reference

Comprehensive guide to all Vue components in the License Management System.

## Layout Components

### AppLayout
**Location**: `src/components/layout/AppLayout.vue`

Main application layout wrapper.

**Structure**:
- AppHeader (navigation)
- Main content area (router-view)
- AppFooter

### AppHeader
**Location**: `src/components/layout/AppHeader.vue`

Navigation header with background image.

**Features**:
- Clickable logo (returns to home)
- Main navigation menu
- Settings dropdown (RSA Keys, API Keys)
- Authentication status
- Responsive mobile menu

**Props**: None (uses composables)

### AppFooter
**Location**: `src/components/layout/AppFooter.vue`

Footer with logo and copyright.

**Features**:
- Logo in bottom left
- Centered copyright text
- Responsive layout

## Common Components

### NotificationContainer
**Location**: `src/components/common/NotificationContainer.vue`

Toast notification system.

**Features**:
- Success, error, info, warning types
- Auto-dismiss with timeout
- Manual dismiss
- Stacked notifications
- Smooth animations

**Usage**:
```typescript
import { useNotification } from '@/composables/useNotification'

const { success, error } = useNotification()

success('Operation completed!')
error('Something went wrong')
```

### Pagination
**Location**: `src/components/common/Pagination.vue`

Reusable pagination component.

**Props**:
```typescript
interface Props {
  currentPage: number
  totalPages: number
}
```

**Events**:
- `@page-change(page: number)` - User selects specific page
- `@next` - Next page clicked
- `@previous` - Previous page clicked

### LoadingSpinner
**Location**: `src/components/common/LoadingSpinner.vue`

Loading indicator overlay.

**Props**: None

## Entity Components

### Customer Components

#### CustomerTable
**Location**: `src/components/entities/customers/CustomerTable.vue`

Displays customer list in a table.

**Props**:
```typescript
interface Props {
  customers: Customer[]
  loading: boolean
  operationLoading?: boolean
}
```

**Events**:
- `@edit(customer: Customer)` - Edit button clicked
- `@delete(customer: Customer)` - Delete button clicked

**Features**:
- Sortable columns
- Visibility status badges
- Action buttons
- Empty state
- Loading state

#### CustomerForm
**Location**: `src/components/entities/customers/CustomerForm.vue`

Create/edit customer form modal.

**Props**:
```typescript
interface Props {
  customer?: Customer | null
  mode: 'create' | 'edit'
  loading?: boolean
}
```

**Events**:
- `@submit(data: CreateCustomerDto | UpdateCustomerDto)` - Form submitted
- `@cancel` - Form cancelled

**Validation**:
- Name required
- Email required and format validated
- Organization required

### Product Components

#### ProductTable
**Location**: `src/components/entities/products/ProductTable.vue`

Displays product list.

**Features**:
- Product name and code
- Description
- Edit and delete actions

#### ProductForm
**Location**: `src/components/entities/products/ProductForm.vue`

Create/edit product form.

**Fields**:
- Name (required)
- Product Code (required)
- Description (required)

### SKU Components

#### SkuTable
**Location**: `src/components/entities/skus/SkuTable.vue`

Displays SKU list with product names.

**Features**:
- SKU name and code
- Associated product name
- Description
- Edit and delete actions

#### SkuForm
**Location**: `src/components/entities/skus/SkuForm.vue`

Create/edit SKU form.

**Fields**:
- Name (required)
- SKU Code (required)
- Product selection (dropdown)
- Description (required)

### License Components

#### LicenseTable
**Location**: `src/components/entities/licenses/LicenseTable.vue`

Displays license list with comprehensive information.

**Features**:
- Customer and product names
- Multiple SKUs display (shows first 2, then "+X more")
- License type and status badges
- Expiration date
- Activation count
- View, Edit, Delete actions

#### LicenseForm
**Location**: `src/components/entities/licenses/LicenseForm.vue`

Create/edit license form with multi-select.

**Fields**:
- Customer selection
- Product selection
- SKU multi-select (filtered by product)
- RSA key selection
- License type
- Expiration date (optional)
- Max activations

**Features**:
- Dynamic SKU filtering based on selected product
- Date picker for expiration
- Validation for all required fields

#### LicenseViewModal
**Location**: `src/components/entities/licenses/LicenseViewModal.vue`

Modal for viewing complete license details.

**Features**:
- All license information
- JWT token display
- Copy to clipboard button
- Associated SKUs list
- Status badges
- Close button

### RSA Key Components

#### RsaKeyTable
**Location**: `src/components/entities/rsa-keys/RsaKeyTable.vue`

Displays RSA key list.

**Features**:
- Key name
- Key size (bits)
- Created by
- Created date
- Delete action

#### RsaKeyForm
**Location**: `src/components/entities/rsa-keys/RsaKeyForm.vue`

Generate new RSA key pair.

**Fields**:
- Name (required)
- Key size selection (2048, 3072, 4096 bits)
- Created by (required)

### API Key Components

#### ApiKeyTable
**Location**: `src/components/entities/api-keys/ApiKeyTable.vue`

Displays API key list.

**Features**:
- Key name
- Role (Admin/User)
- Active/Inactive status
- Created by
- Last used timestamp
- Edit and delete actions

#### ApiKeyForm
**Location**: `src/components/entities/api-keys/ApiKeyForm.vue`

Create/edit API key form.

**Fields**:
- Name (required)
- Role selection (Admin/User)
- Active status toggle
- Created by (required)

#### ApiKeyDisplayModal
**Location**: `src/components/entities/api-keys/ApiKeyDisplayModal.vue`

Modal shown after creating an API key.

**Features**:
- One-time display warning
- API key value display
- Copy to clipboard button
- Security reminder

## Component Patterns

### Table Components Pattern
All table components follow a consistent pattern:

```vue
<script setup lang="ts">
interface Props {
  items: Item[]
  loading: boolean
  operationLoading?: boolean
}

interface Emits {
  (e: 'edit', item: Item): void
  (e: 'delete', item: Item): void
}
</script>

<template>
  <div class="table-container">
    <div v-if="loading">Loading...</div>
    <table v-else>
      <!-- Table content -->
    </table>
  </div>
</template>
```

### Form Components Pattern
All form components follow a consistent pattern:

```vue
<script setup lang="ts">
interface Props {
  item?: Item | null
  mode: 'create' | 'edit'
  loading?: boolean
}

interface Emits {
  (e: 'submit', data: CreateDto | UpdateDto): void
  (e: 'cancel'): void
}

const formData = ref({ /* fields */ })
const errors = ref({ /* field errors */ })

const validateForm = (): boolean => { /* validation */ }
const handleSubmit = () => { /* submit logic */ }
</script>
```

### Modal Components Pattern
Modal components use overlay pattern:

```vue
<template>
  <div class="modal-overlay" @click.self="handleClose">
    <div class="modal-content">
      <!-- Modal content -->
    </div>
  </div>
</template>

<style scoped>
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.6);
  z-index: 2000;
}
</style>
```

## Styling Conventions

### Color Palette
- Primary: `#00A3AD` (teal)
- Success: `#27ae60` (green)
- Danger: `#e74c3c` (red)
- Dark: `#2c3e50`
- Light: `#ecf0f1`

### Button Styles
- Edit: Teal (`#00A3AD`)
- Delete: Red (`#e74c3c`)
- Create: Green (`#27ae60`)
- View: Blue (`#3498db`)
- Cancel: Gray (`#95a5a6`)

### Status Badges
- Active: Green background
- Inactive/Expired: Red background
- Suspended: Yellow background

### Responsive Breakpoints
- Mobile: `max-width: 768px`
- Tablet: `max-width: 1024px`
- Desktop: `> 1024px`
