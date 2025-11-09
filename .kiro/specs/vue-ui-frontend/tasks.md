# Implementation Plan

- [x] 1. Set up project structure and core configuration





  - Initialize Vite project with Vue 3 and TypeScript
  - Configure TypeScript with strict mode and path aliases
  - Set up project directory structure (components, views, composables, services, types, router)
  - Install core dependencies (vue, vue-router, axios)
  - Create environment configuration files (.env.development, .env.production)
  - _Requirements: 1.1, 1.2, 1.3, 1.4, 1.5_

- [x] 2. Implement layout components and routing foundation





  - [x] 2.1 Create App.vue root component with global styles


    - Implement root component using script setup syntax
    - Add global CSS reset and base styles
    - _Requirements: 1.2, 2.5_

  - [x] 2.2 Create AppLayout component with flex structure


    - Implement layout wrapper with header, main content, and footer slots
    - Configure flexbox layout to keep footer at bottom
    - _Requirements: 2.1, 2.2, 2.3, 2.4, 2.5_

  - [x] 2.3 Create AppHeader component with placeholder auth UI


    - Implement header with app title
    - Add placeholder for login/logout button in top right
    - Style header with dark background (#2c3e50)
    - _Requirements: 2.1, 2.4_

  - [x] 2.4 Create AppFooter component with black background


    - Implement footer with black background (#000000)
    - Add placeholder for navigation links
    - _Requirements: 2.2, 2.3, 2.4_

  - [x] 2.5 Configure Vue Router with nested routes


    - Set up router with AppLayout as parent route
    - Create routes for all 6 entity pages (customers, products, skus, licenses, rsa-keys, api-keys)
    - Configure lazy loading for view components
    - Set default redirect to /customers
    - _Requirements: 4.3_

  - [x] 2.6 Create placeholder view components for all entities


    - Create CustomersView, ProductsView, SkusView, LicensesView, RsaKeysView, ApiKeysView
    - Add basic page title and placeholder text to each view
    - _Requirements: 4.3_

- [x] 3. Implement authentication state management




  - [x] 3.1 Create useAuth composable with shared state


    - Implement shared authentication state (isLoggedIn, apiKey)
    - Create login, logout, and toggleAuth functions
    - Use readonly wrappers to prevent external mutation
    - _Requirements: 3.2, 3.3, 3.5_

  - [x] 3.2 Update AppHeader to use useAuth composable


    - Integrate useAuth composable in header
    - Display current auth status (Logged In / Logged Out)
    - Implement login/logout button with toggle functionality
    - Position button in top right corner
    - _Requirements: 3.1, 3.2, 3.3, 3.4_

- [x] 4. Implement navigation system




  - [x] 4.1 Update AppFooter with navigation links


    - Add router-link components for all 6 entity pages
    - Implement horizontal layout for navigation links
    - Add active link highlighting using route matching
    - Style active links with background color and underline indicator
    - _Requirements: 4.1, 4.2, 4.3, 4.4, 4.5_

  - [x] 4.2 Add mobile responsive navigation

    - Implement vertical stacking for mobile screens (< 768px)
    - Make navigation links full-width on mobile
    - _Requirements: 15.5_

- [x] 5. Create shared components and composables





  - [x] 5.1 Create Pagination component


    - Implement pagination UI with previous/next buttons
    - Display current page and total pages
    - Emit events for page changes
    - Style with disabled states for boundary conditions
    - _Requirements: 11.1, 11.3, 11.4, 11.5_

  - [x] 5.2 Create usePagination composable


    - Implement pagination logic for client-side data
    - Set items per page to 20
    - Calculate total pages and paginated items
    - Provide navigation functions (goToPage, nextPage, previousPage)
    - Auto-hide pagination when 20 or fewer items
    - _Requirements: 11.2, 11.3, 11.6_

  - [x] 5.3 Create NotificationContainer component


    - Implement notification display with color coding (success, error, info)
    - Position notifications in top-right corner
    - Add close button for manual dismissal
    - Implement slide-in animation
    - _Requirements: 14.2, 14.3, 14.5_

  - [x] 5.4 Create useNotification composable


    - Implement shared notification state
    - Create success, error, and info notification functions
    - Auto-dismiss success notifications after 3 seconds
    - Keep error notifications until manually dismissed
    - _Requirements: 14.2, 14.3, 14.4, 14.5_

  - [x] 5.5 Create LoadingSpinner component


    - Implement simple spinner animation
    - Style with consistent colors
    - _Requirements: 14.1_

  - [x] 5.6 Create useFormValidation composable


    - Implement validation rules (required, email, minLength, maxLength, pattern, custom)
    - Create validateField and validateForm functions
    - Provide error clearing functions
    - _Requirements: 13.1, 13.2, 13.3, 13.4, 13.5_

- [x] 6. Implement API integration infrastructure





  - [x] 6.1 Create axios instance with interceptors


    - Configure axios with base URL from environment variables
    - Add request interceptor to include Auth_Key header
    - Add response interceptor for global error handling
    - Dispatch custom events for API errors
    - _Requirements: 12.1, 12.2, 12.3, 12.5_

  - [x] 6.2 Integrate NotificationContainer in App.vue


    - Add NotificationContainer to root component
    - Listen for API error events and display error notifications
    - _Requirements: 12.3, 12.4, 14.3_

- [x] 7. Implement Customer management interface




  - [x] 7.1 Create Customer TypeScript interfaces


    - Define Customer, CreateCustomerDto, UpdateCustomerDto interfaces
    - _Requirements: 5.1_

  - [x] 7.2 Create customerService with CRUD operations


    - Implement getAll, getById, create, update, delete methods
    - Use configured axios instance
    - _Requirements: 5.3, 5.5, 5.7, 12.1_

  - [x] 7.3 Create CustomerTable component


    - Display customers in table with columns: Name, Email, Organization, IsVisible
    - Show loading state while fetching data
    - Display empty state when no customers exist
    - Add Edit and Delete buttons for each row
    - Emit edit and delete events
    - _Requirements: 5.1, 5.4, 5.6_

  - [x] 7.4 Create CustomerForm component


    - Create modal form for create/edit modes
    - Add form fields: name, email, organization, isVisible
    - Implement form validation (required fields, email format)
    - Display validation errors inline
    - Clear errors on field input
    - Emit submit and cancel events
    - _Requirements: 5.2, 5.3, 5.4, 5.5, 13.1, 13.2, 13.3, 13.4, 13.5_

  - [x] 7.5 Implement CustomersView with CRUD orchestration


    - Load customers on component mount
    - Implement create customer flow with form modal
    - Implement edit customer flow with form modal
    - Implement delete customer with confirmation dialog
    - Refresh table after successful operations
    - Display success/error notifications
    - Show loading indicator during operations
    - _Requirements: 5.2, 5.3, 5.4, 5.5, 5.6, 5.7, 5.8, 12.3, 12.4, 14.1, 14.2, 14.3_

  - [x] 7.6 Integrate pagination in CustomerTable


    - Use usePagination composable with customer data
    - Display Pagination component when more than 20 customers
    - _Requirements: 11.1, 11.2, 11.3, 11.4, 11.5, 11.6_

- [x] 8. Implement Product management interface





  - [x] 8.1 Create Product TypeScript interfaces


    - Define Product, CreateProductDto, UpdateProductDto interfaces
    - _Requirements: 6.1_

  - [x] 8.2 Create productService with CRUD operations


    - Implement getAll, getById, create, update, delete methods
    - _Requirements: 6.3, 6.5, 6.7, 12.1_

  - [x] 8.3 Create ProductTable component


    - Display products in table with columns: Name, ProductCode, Version, Description
    - Include loading, empty states, and action buttons
    - _Requirements: 6.1, 6.4, 6.6_

  - [x] 8.4 Create ProductForm component


    - Create modal form with fields: name, productCode, version, description
    - Implement validation for required fields
    - _Requirements: 6.2, 6.3, 6.4, 6.5, 13.1, 13.3, 13.4, 13.5_

  - [x] 8.5 Implement ProductsView with CRUD orchestration


    - Implement full CRUD flow following customer pattern
    - Add pagination support
    - _Requirements: 6.2, 6.3, 6.4, 6.5, 6.6, 6.7, 6.8, 11.1, 11.2, 11.3, 14.1, 14.2, 14.3_

- [x] 9. Implement SKU management interface





  - [x] 9.1 Create SKU TypeScript interfaces


    - Define Sku, CreateSkuDto, UpdateSkuDto interfaces
    - _Requirements: 7.1_

  - [x] 9.2 Create skuService with CRUD operations


    - Implement getAll, getById, create, update, delete methods
    - _Requirements: 7.3, 7.5, 7.7, 12.1_

  - [x] 9.3 Create SkuTable component


    - Display SKUs in table with columns: Name, SkuCode, Product Name, Description
    - Include loading, empty states, and action buttons
    - _Requirements: 7.1, 7.4, 7.6_

  - [x] 9.4 Create SkuForm component


    - Create modal form with fields: name, skuCode, productId (dropdown), description
    - Load products for dropdown selection
    - Implement validation for required fields
    - _Requirements: 7.2, 7.3, 7.4, 7.5, 13.1, 13.3, 13.4, 13.5_

  - [x] 9.5 Implement SkusView with CRUD orchestration


    - Load products for form dropdown
    - Implement full CRUD flow with pagination
    - _Requirements: 7.2, 7.3, 7.4, 7.5, 7.6, 7.7, 7.8, 11.1, 11.2, 11.3, 14.1, 14.2, 14.3_

- [x] 10. Implement License management interface





  - [x] 10.1 Create License TypeScript interfaces


    - Define License, CreateLicenseDto, UpdateLicenseDto interfaces
    - _Requirements: 8.1_

  - [x] 10.2 Create licenseService with CRUD operations


    - Implement getAll, getById, create, update, delete methods
    - _Requirements: 8.3, 8.5, 8.7, 12.1_

  - [x] 10.3 Create LicenseTable component


    - Display licenses in table with columns: Customer Name, Product Name, LicenseType, Status, ExpirationDate, CurrentActivations
    - Include loading, empty states, and action buttons
    - _Requirements: 8.1, 8.4, 8.6_

  - [x] 10.4 Create LicenseForm component


    - Create modal form with dropdowns for: customerId, productId, skuId, rsaKeyId
    - Add fields for: licenseType, expirationDate, maxActivations
    - Load all related entities (customers, products, SKUs, RSA keys) for dropdowns
    - Implement validation for required fields
    - _Requirements: 8.2, 8.3, 8.4, 8.5, 13.1, 13.3, 13.4, 13.5_


  - [x] 10.5 Implement LicensesView with CRUD orchestration

    - Load all related entities for form dropdowns
    - Implement full CRUD flow with pagination
    - _Requirements: 8.2, 8.3, 8.4, 8.5, 8.6, 8.7, 8.8, 11.1, 11.2, 11.3, 14.1, 14.2, 14.3_

- [x] 11. Implement RSA Key management interface





  - [x] 11.1 Create RSA Key TypeScript interfaces


    - Define RsaKey, CreateRsaKeyDto interfaces (no update DTO)
    - _Requirements: 9.1_

  - [x] 11.2 Create rsaKeyService with create and delete operations


    - Implement getAll, getById, create, delete methods (no update method)
    - _Requirements: 9.3, 9.5, 12.1_


  - [x] 11.3 Create RsaKeyTable component

    - Display RSA keys in table with columns: Name, KeySize, CreatedBy, CreatedAt
    - Include only Delete button (no Edit button)
    - _Requirements: 9.1, 9.4_


  - [x] 11.4 Create RsaKeyForm component

    - Create modal form with fields: name, keySize (dropdown: 2048, 3072, 4096), createdBy
    - Implement validation for required fields
    - _Requirements: 9.2, 9.3, 13.1, 13.3, 13.4, 13.5_


  - [x] 11.5 Implement RsaKeysView with create and delete operations

    - Implement create and delete flows only (no edit)
    - Add pagination support
    - _Requirements: 9.2, 9.3, 9.4, 9.5, 9.6, 9.7, 11.1, 11.2, 11.3, 14.1, 14.2, 14.3_

- [x] 12. Implement API Key management interface





  - [x] 12.1 Create API Key TypeScript interfaces


    - Define ApiKey, CreateApiKeyDto, UpdateApiKeyDto, ApiKeyCreationResponse interfaces
    - _Requirements: 10.1_

  - [x] 12.2 Create apiKeyService with CRUD operations


    - Implement getAll, getById, create, update, delete methods
    - _Requirements: 10.3, 10.5, 10.7, 12.1_

  - [x] 12.3 Create ApiKeyTable component


    - Display API keys in table with columns: Name, Role, IsActive, CreatedBy, LastUsedAt
    - Include loading, empty states, and action buttons
    - _Requirements: 10.1, 10.4, 10.6_

  - [x] 12.4 Create ApiKeyForm component


    - Create modal form with fields: name, role (dropdown: Admin, User), isActive (checkbox, edit mode only)
    - Implement validation for required fields
    - _Requirements: 10.2, 10.3, 10.4, 10.5, 13.1, 13.3, 13.4, 13.5_

  - [x] 12.5 Create ApiKeyDisplayModal component


    - Create special modal to display newly created API key
    - Add copy-to-clipboard button
    - Display warning that key won't be shown again
    - _Requirements: 10.3_

  - [x] 12.6 Implement ApiKeysView with CRUD orchestration


    - Implement full CRUD flow with pagination
    - Show ApiKeyDisplayModal after successful creation
    - _Requirements: 10.2, 10.3, 10.4, 10.5, 10.6, 10.7, 10.8, 11.1, 11.2, 11.3, 14.1, 14.2, 14.3_

- [x] 13. Implement responsive design enhancements






  - [x] 13.1 Add responsive styles for header

    - Stack header content vertically on mobile (< 768px)
    - Adjust padding and spacing for smaller screens
    - _Requirements: 15.3_



  - [x] 13.2 Add responsive styles for tables

    - Enable horizontal scrolling for tables on mobile
    - Adjust font sizes and padding for tablet and mobile
    - Stack action buttons vertically on mobile
    - _Requirements: 15.2, 15.3, 15.4_


  - [x] 13.3 Add responsive styles for forms

    - Make form modals full-width on mobile (95%)
    - Adjust padding and spacing for smaller screens
    - _Requirements: 15.3_

  - [x] 13.4 Add responsive styles for notifications


    - Make notifications full-width on mobile
    - Adjust positioning for smaller screens
    - _Requirements: 15.3_


  - [x] 13.5 Test responsive behavior across breakpoints

    - Verify desktop layout (> 1024px)
    - Verify tablet layout (768px - 1024px)
    - Verify mobile layout (< 768px)
    - _Requirements: 15.1, 15.2, 15.3_

- [ ]* 14. Polish and optimization
  - [x] 14.1 Add loading states to all async operations






    - Ensure all API calls show loading indicators
    - Disable buttons during operations to prevent double-submission
    - _Requirements: 14.1_

  - [x] 14.2 Enhance error handling









    - Add specific error messages for common API errors (404, 401, 500)
    - Improve error message clarity for users
    - _Requirements: 12.3, 14.3_

  - [ ]* 14.3 Add keyboard navigation support
    - Ensure forms can be navigated with Tab key
    - Add Enter key submission for forms
    - Add Escape key to close modals
    - _Requirements: 13.5_

  - [x] 14.4 Optimize performance






    - Verify lazy loading is working for route components
    - Check for unnecessary re-renders
    - Optimize large lists if needed
    - _Requirements: 1.1_

  - [ ]* 14.5 Add accessibility improvements
    - Add ARIA labels to interactive elements
    - Ensure proper focus management in modals
    - Add screen reader support for notifications
    - _Requirements: 15.1, 15.2, 15.3_
