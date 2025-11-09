# License Management System

A comprehensive full-stack application for managing software licenses, featuring a secure ASP.NET Core 8.0 backend API with PostgreSQL and a modern Vue.js 3 frontend.

## Project Statistics

- **Total Code**: 14,772 lines across 130 files
- **Backend (C#)**: 8,517 lines in 80 files
- **Frontend (Vue)**: 5,411 lines in 29 files
- **Frontend (TypeScript)**: 844 lines in 21 files

## Overview

The License Management System enables software vendors to:

- **Manage customers** and their license entitlements
- **Define products and SKUs** for different software offerings
- **Generate and manage RSA key pairs** for license signing
- **Issue digitally signed licenses** with activation controls
- **Validate and activate license keys**
- **Control API access** with role-based authentication

## Key Features

### Customer Management
Create and manage customer accounts with organization details

### Product & SKU Management
Organize software products and their variants

### RSA Key Management
Generate and securely store RSA key pairs for license signing

### License Generation
Issue digitally signed licenses with expiration and activation limits

### License Validation
Validate license keys with cryptographic security

### API Key Authentication
Role-based access control with cached authentication

### Health Monitoring
Built-in health checks for system status

### Structured Logging
Comprehensive logging with Serilog

### Database Migrations
Automatic schema evolution with Entity Framework Core

## Technology Stack

### Backend
- **Framework**: ASP.NET Core 8.0
- **Database**: PostgreSQL with Npgsql provider
- **ORM**: Entity Framework Core 9.0
- **Logging**: Serilog with console and file sinks
- **Authentication**: Custom API key-based authentication with role-based authorization
- **Cryptography**: RSA digital signatures for license signing (JWT)
- **Health Checks**: ASP.NET Core Health Checks with PostgreSQL connectivity verification

### Frontend
- **Framework**: Vue.js 3 with Composition API
- **Language**: TypeScript
- **Build Tool**: Vite
- **Routing**: Vue Router
- **HTTP Client**: Axios
- **Styling**: Scoped CSS with responsive design

## Quick Links

- [Quick Start Guide](getting-started/quick-start.md) - Get up and running in minutes
- [API Reference](api/overview.md) - Complete API documentation
- [Authentication Guide](api/authentication.md) - Learn about API key authentication
- [Database Schema](architecture/database.md) - Understand the data model
- [Requirements](development/requirements.md) - Feature requirements and specifications

## Prerequisites

### Backend
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL 12+](https://www.postgresql.org/download/)
- A PostgreSQL database instance running and accessible

### Frontend
- [Node.js 18+](https://nodejs.org/)
- npm or yarn package manager

## Next Steps

1. Follow the [Quick Start Guide](getting-started/quick-start.md) to set up your development environment
2. Review the [Configuration Guide](getting-started/configuration.md) for production setup
3. Explore the [API Reference](api/overview.md) to understand available endpoints
4. Check out the [Postman Collection](api/postman.md) for interactive API testing
