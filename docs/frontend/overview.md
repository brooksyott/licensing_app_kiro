# Frontend Overview

The License Management System frontend is a modern single-page application (SPA) built with Vue.js 3, providing an intuitive interface for managing licenses, customers, products, and system settings.

## Technology Stack

- **Framework**: Vue.js 3 with Composition API
- **Language**: TypeScript
- **Build Tool**: Vite
- **Routing**: Vue Router 4
- **HTTP Client**: Axios
- **Styling**: Scoped CSS with responsive design

## Project Structure

```
vue-ui-frontend/
├── src/
│   ├── assets/          # Static assets (images, logos)
│   ├── components/      # Reusable Vue components
│   │   ├── common/      # Shared components (notifications, pagination, etc.)
│   │   ├── entities/    # Entity-specific components
│   │   │   ├── customers/
│   │   │   ├── products/
│   │   │   ├── skus/
│   │   │   ├── licenses/
│   │   │   ├── rsa-keys/
│   │   │   └── api-keys/
│   │   └── layout/      # Layout components (header, footer)
│   ├── composables/     # Vue composables for shared logic
│   ├── router/          # Vue Router configuration
│   ├── services/        # API service layer
│   ├── types/           # TypeScript type definitions
│   ├── utils/           # Utility functions
│   ├── views/           # Page-level components
│   ├── App.vue          # Root component
│   └── main.ts          # Application entry point
├── public/              # Public static files
├── index.html           # HTML template
├── vite.config.ts       # Vite configuration
├── tsconfig.json        # TypeScript configuration
└── package.json         # Dependencies and scripts
```

## Key Features

### Entity Management
- **Customers**: Create, view, edit, and delete customer records
- **Products**: Manage software products
- **SKUs**: Define product variants and features
- **Licenses**: Issue and manage licenses with multiple SKUs
- **RSA Keys**: Generate and manage encryption keys
- **API Keys**: Control API access

### User Interface
- **Responsive Design**: Works on desktop, tablet, and mobile devices
- **Navigation**: Clean header with dropdown menus
- **Tables**: Sortable, paginated data tables
- **Forms**: Validated forms with error handling
- **Modals**: Overlay dialogs for viewing details
- **Notifications**: Toast notifications for user feedback

### License Viewing
- View complete license details
- Copy JWT license tokens
- Display associated SKUs and products
- Show activation status

## Development Server

Start the development server:

```bash
cd vue-ui-frontend
npm install
npm run dev
```

The application will be available at `http://localhost:5173`

## Build for Production

Build the application for production:

```bash
npm run build
```

The built files will be in the `dist/` directory.

## Environment Configuration

The frontend connects to the backend API. Configure the API base URL in your environment or service files.

Default API URL: `http://localhost:5000`

## Code Statistics

- **Vue Components**: 29 files, 5,411 lines
- **TypeScript**: 21 files, 844 lines
- **Total Frontend Code**: 6,255 lines
