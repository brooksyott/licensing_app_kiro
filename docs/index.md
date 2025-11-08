# License Management API

A secure, scalable backend system built on ASP.NET Core 8.0 with PostgreSQL for managing software licenses, license keys, customers, and access control.

## Overview

The License Management API enables software vendors to:

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

- **Framework**: ASP.NET Core 8.0
- **Database**: PostgreSQL with Npgsql provider
- **ORM**: Entity Framework Core 9.0
- **Logging**: Serilog with console and file sinks
- **Authentication**: Custom API key-based authentication with role-based authorization
- **Cryptography**: RSA digital signatures for license signing
- **Health Checks**: ASP.NET Core Health Checks with PostgreSQL connectivity verification

## Quick Links

- [Quick Start Guide](getting-started/quick-start.md) - Get up and running in minutes
- [API Reference](api/overview.md) - Complete API documentation
- [Authentication Guide](api/authentication.md) - Learn about API key authentication
- [Database Schema](architecture/database.md) - Understand the data model
- [Requirements](development/requirements.md) - Feature requirements and specifications

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL 12+](https://www.postgresql.org/download/)
- A PostgreSQL database instance running and accessible

## Next Steps

1. Follow the [Quick Start Guide](getting-started/quick-start.md) to set up your development environment
2. Review the [Configuration Guide](getting-started/configuration.md) for production setup
3. Explore the [API Reference](api/overview.md) to understand available endpoints
4. Check out the [Postman Collection](api/postman.md) for interactive API testing
