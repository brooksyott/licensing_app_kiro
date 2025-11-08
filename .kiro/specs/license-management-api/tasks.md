# Implementation Plan

- [x] 1. Set up project structure and core infrastructure





  - [x] 1.1 Create ASP.NET Core 8.0 Web API project with appropriate folder structure (Controllers, Services, Models, Data)


    - _Requirements: 11.1, 12.1_
  - [x] 1.2 Configure Npgsql and Entity Framework Core packages


    - _Requirements: 11.1_

  - [x] 1.3 Set up Serilog for structured logging with console and file sinks

    - _Requirements: 10.1_

  - [x] 1.4 Configure dependency injection for services and repositories

    - _Requirements: 12.2_
  - [x] 1.5 Create appsettings.json with PostgreSQL connection string and logging configuration


    - _Requirements: 11.1, 10.1_

- [x] 2. Implement ServiceResult pattern and base infrastructure





  - [x] 2.1 Create ServiceResult<T> class with success, failure, and validation failure factory methods


    - _Requirements: 10.3, 12.3_
  - [x] 2.2 Create PagedResult<T> class for pagination support


    - _Requirements: 10.4_
  - [x] 2.3 Create base controller class that maps ServiceResult to HTTP responses


    - _Requirements: 10.4, 10.5, 10.6, 10.7, 12.5_
  - [x] 2.4 Create global exception handling middleware


    - _Requirements: 10.2, 10.8_

- [x] 3. Define data models and Entity Framework Core context





  - [x] 3.1 Create Customer entity with name, email, organization, visibility flag, and timestamps


    - _Requirements: 1.1, 11.2_
  - [x] 3.2 Create Product entity with name, product code, version, description, and timestamps


    - _Requirements: 2.1, 11.2_
  - [x] 3.3 Create SKU entity with product association, name, SKU code, description, and timestamps


    - _Requirements: 3.1, 11.2_
  - [x] 3.4 Create RsaKey entity with name, public key, encrypted private key, key size, audit metadata, and timestamps


    - _Requirements: 4.2, 4.7, 11.2_
  - [x] 3.5 Create License entity with customer, product, SKU, RSA key associations, license key hash, signed payload, type, expiration, activation counts, status, and timestamps


    - _Requirements: 5.2, 5.5, 11.2_
  - [x] 3.6 Create ApiKey entity with key hash, name, role, active status, audit metadata, and timestamps


    - _Requirements: 8.1, 8.6, 11.2_
  - [x] 3.7 Create LicenseManagementDbContext with DbSet properties for all entities


    - _Requirements: 11.1_

  - [x] 3.8 Configure entity relationships, cascade deletes, and unique constraints in OnModelCreating

    - _Requirements: 2.7, 3.7, 11.3, 11.4_
  - [x] 3.9 Configure automatic timestamp management using SaveChanges override


    - _Requirements: 11.2_

  - [x] 3.10 Create initial database migration

    - _Requirements: 11.5_

  - [x] 3.11 Configure automatic migration application on startup

    - _Requirements: 11.6_

- [x] 4. Implement Customer management




  - [x] 4.1 Create CustomerDto and request models (CreateCustomerRequest, UpdateCustomerRequest)


    - _Requirements: 1.1, 1.4_
  - [x] 4.2 Create ICustomerService interface with all CRUD operations


    - _Requirements: 1.1, 1.2, 1.3, 1.4, 1.5, 1.6, 12.1_
  - [x] 4.3 Implement CustomerService with create, get by ID, get by name, search, update, delete, and list operations


    - _Requirements: 1.1, 1.2, 1.3, 1.4, 1.5, 1.6, 12.3, 12.4_
  - [x] 4.4 Add email uniqueness validation in create operation


    - _Requirements: 1.7_
  - [x] 4.5 Create CustomersController with all endpoints


    - _Requirements: 1.1, 1.2, 1.3, 1.4, 1.5, 1.6, 12.2, 12.5_
  - [x] 4.6 Write unit tests for CustomerService core operations


    - _Requirements: 1.1, 1.2, 1.3, 1.4, 1.5, 1.6_

- [x] 5. Implement Product management


  - [x] 5.1 Create ProductDto and request models (CreateProductRequest, UpdateProductRequest)
    - _Requirements: 2.1, 2.3_
  - [x] 5.2 Create IProductService interface with all CRUD operations
    - _Requirements: 2.1, 2.2, 2.3, 2.4, 2.5, 2.6, 12.1_
  - [x] 5.3 Implement ProductService with create, get by ID, update, delete, list, and search operations
    - _Requirements: 2.1, 2.2, 2.3, 2.4, 2.5, 2.6, 12.3, 12.4_
  - [x] 5.4 Ensure cascade delete removes associated SKUs when product is deleted
    - _Requirements: 2.7_
  - [x] 5.5 Create ProductsController with all endpoints
    - _Requirements: 2.1, 2.2, 2.3, 2.4, 2.5, 2.6, 12.2, 12.5_
  - [x] 5.6 Write unit tests for ProductService core operations

    - _Requirements: 2.1, 2.2, 2.3, 2.4, 2.5, 2.6_

- [x] 6. Implement SKU management




  - [x] 6.1 Create SkuDto and request models (CreateSkuRequest, UpdateSkuRequest)


    - _Requirements: 3.1, 3.3_
  - [x] 6.2 Create ISkuService interface with all CRUD operations


    - _Requirements: 3.1, 3.2, 3.3, 3.4, 3.5, 3.6, 12.1_
  - [x] 6.3 Implement SkuService with create, get by ID, update, delete, list, list by product, and search operations


    - _Requirements: 3.1, 3.2, 3.3, 3.4, 3.5, 3.6, 12.3, 12.4_
  - [x] 6.4 Ensure product associations are removed when SKU is deleted


    - _Requirements: 3.7_
  - [x] 6.5 Create SkusController with all endpoints


    - _Requirements: 3.1, 3.2, 3.3, 3.4, 3.5, 3.6, 12.2, 12.5_

  - [x] 6.6 Write unit tests for SkuService core operations

    - _Requirements: 3.1, 3.2, 3.3, 3.4, 3.5, 3.6_

- [x] 7. Implement RSA key management and cryptographic services




  - [x] 7.1 Create RsaKeyDto and request models (GenerateRsaKeyRequest, UpdateRsaKeyRequest)


    - _Requirements: 4.1, 4.5_
  - [x] 7.2 Create ICryptographyService interface for RSA key generation, encryption, and signing operations


    - _Requirements: 4.1, 5.4, 13.1_
  - [x] 7.3 Implement CryptographyService with RSA key pair generation, private key encryption/decryption, and license signing


    - _Requirements: 4.1, 5.4, 13.1_
  - [x] 7.4 Create IRsaKeyService interface with key management operations


    - _Requirements: 4.1, 4.3, 4.4, 4.5, 4.6, 12.1_

  - [x] 7.5 Implement RsaKeyService with generate, get by ID, download private key, update, delete, and list operations

    - _Requirements: 4.1, 4.2, 4.3, 4.4, 4.5, 4.6, 4.7, 12.3, 12.4_
  - [x] 7.6 Create RsaKeysController with all endpoints


    - _Requirements: 4.1, 4.3, 4.4, 4.5, 4.6, 12.2, 12.5_

  - [x] 7.7 Write unit tests for CryptographyService and RsaKeyService

    - _Requirements: 4.1, 4.2, 4.3, 4.4, 4.5, 4.6_

- [x] 8. Implement License key generation and management




  - [x] 8.1 Create LicenseDto and request models (CreateLicenseRequest, UpdateLicenseRequest)


    - _Requirements: 5.1, 5.8_
  - [x] 8.2 Create ILicenseKeyGenerator interface for secure key generation


    - _Requirements: 5.3, 13.1, 13.2, 13.5_
  - [x] 8.3 Implement LicenseKeyGenerator using cryptographically secure random number generator with minimum 20 character length


    - _Requirements: 5.3, 13.1, 13.2, 13.5_

  - [x] 8.4 Implement license key hashing with salt for secure storage







    - _Requirements: 13.3_
  - [x] 8.5 Create ILicenseService interface with all license management operations

    - _Requirements: 5.1, 5.6, 5.8, 5.9, 6.1, 6.2, 6.3, 6.4, 6.5, 7.1, 7.5, 12.1_
  - [x] 8.6 Implement LicenseService create operation with key generation, hashing, and digital signing

    - _Requirements: 5.1, 5.2, 5.3, 5.4, 5.5, 12.3, 12.4_
  - [x] 8.7 Implement LicenseService get by ID, update, delete, and revoke operations

    - _Requirements: 5.6, 5.8, 5.9, 6.5, 12.3, 12.4_
  - [x] 8.8 Implement LicenseService list operations with filtering by customer, product, and status

    - _Requirements: 6.1, 6.2, 6.3, 6.4, 12.3, 12.4_
  - [x] 8.9 Create LicensesController with all endpoints


    - _Requirements: 5.1, 5.6, 5.8, 5.9, 6.1, 6.2, 6.3, 6.4, 6.5, 12.2, 12.5_
  - [x] 8.10 Write unit tests for LicenseKeyGenerator and LicenseService


    - _Requirements: 5.1, 5.2, 5.3, 5.4, 5.5, 5.6, 5.8, 5.9_

- [x] 9. Implement License validation and activation





  - [x] 9.1 Create LicenseValidationResult and LicenseActivationResult DTOs

    - _Requirements: 7.1, 7.5_
  - [x] 9.2 Implement ValidateLicenseKeyAsync in LicenseService with key existence, revocation, activation count, and expiration checks

    - _Requirements: 7.1, 7.2, 7.3, 7.4, 12.3, 12.4_
  - [x] 9.3 Implement constant-time comparison for license key validation to prevent timing attacks

    - _Requirements: 13.4_
  - [x] 9.4 Implement ActivateLicenseKeyAsync in LicenseService with activation count increment

    - _Requirements: 7.5, 12.3, 12.4_

  - [x] 9.5 Add validation and activation endpoints to LicensesController

    - _Requirements: 7.1, 7.5, 12.2, 12.5_
  - [x] 9.6 Write unit tests for license validation and activation logic


    - _Requirements: 7.1, 7.2, 7.3, 7.4, 7.5_

- [x] 10. Implement API key management




  - [x] 10.1 Create ApiKeyDto and request models (CreateApiKeyRequest, UpdateApiKeyRequest)


    - _Requirements: 8.1, 8.3_
  - [x] 10.2 Create IApiKeyService interface with all API key operations


    - _Requirements: 8.1, 8.2, 8.3, 8.4, 8.5, 12.1_
  - [x] 10.3 Implement ApiKeyService with create, get by ID, update, delete, list, and validate operations


    - _Requirements: 8.1, 8.2, 8.3, 8.4, 8.5, 8.6, 12.3, 12.4_
  - [x] 10.4 Implement secure API key hashing for storage


    - _Requirements: 8.1_
  - [x] 10.5 Create ApiKeysController with all endpoints


    - _Requirements: 8.1, 8.2, 8.3, 8.4, 8.5, 12.2, 12.5_
  - [x] 10.6 Write unit tests for ApiKeyService


    - _Requirements: 8.1, 8.2, 8.3, 8.4, 8.5_


- [x] 11. Implement authentication and authorization



  - [x] 11.1 Create IAuthenticationService interface with authenticate and cache invalidation methods


    - _Requirements: 9.1, 9.6, 12.1_
  - [x] 11.2 Implement AuthenticationService with API key validation and 5-minute caching


    - _Requirements: 9.1, 9.3, 9.6, 12.3, 12.4_
  - [x] 11.3 Create authentication middleware that validates Auth_Key header


    - _Requirements: 9.1, 9.6_
  - [x] 11.4 Implement role-based authorization with defined roles


    - _Requirements: 9.2_
  - [x] 11.5 Configure middleware to return 401 for unauthenticated requests and 403 for unauthorized requests

    - _Requirements: 9.4, 9.5_
  - [x] 11.6 Update ApiKeyService delete operation to invalidate cache


    - _Requirements: 8.7_
  - [x] 11.7 Register authentication middleware in Program.cs with health check endpoint exemption


    - _Requirements: 9.1, 14.5_
  - [x] 11.8 Write unit tests for AuthenticationService and authorization logic


    - _Requirements: 9.1, 9.2, 9.3, 9.4, 9.5, 9.6_

- [x] 12. Implement health check and monitoring





  - [x] 12.1 Create health check endpoint that verifies PostgreSQL connectivity


    - _Requirements: 14.1, 14.2_
  - [x] 12.2 Configure health check to return unhealthy status when database is unreachable

    - _Requirements: 14.3_

  - [x] 12.3 Add version information to health check response

    - _Requirements: 14.4_

  - [x] 12.4 Ensure health check endpoint allows unauthenticated access

    - _Requirements: 14.5_


  - [x] 12.5 Configure health check response time target of 200 milliseconds


    - _Requirements: 10.5_

- [x] 13. Configure database optimization and scalability





  - [x] 13.1 Configure connection pooling for PostgreSQL connections


    - _Requirements: 10.1_
  - [x] 13.2 Add database indexes on frequently queried fields (email, product code, SKU code, license key hash, API key hash)


    - _Requirements: 10.2, 11.3_
  - [x] 13.3 Configure maximum page size of 100 items for all list endpoints


    - _Requirements: 10.4_
  - [x] 13.4 Ensure stateless design for horizontal scaling


    - _Requirements: 10.3_

- [x] 14. Create Postman collection for API testing





  - [x] 14.1 Create Postman collection file with all API endpoints organized by resource type

    - _Requirements: 15.1, 15.2_

  - [x] 14.2 Add environment variables for base URL and Auth_Key configuration

    - _Requirements: 15.3_

  - [x] 14.3 Add example request bodies with valid sample data for all POST and PUT operations

    - _Requirements: 15.4_


  - [x] 14.4 Add response validation tests for status codes and response structure

    - _Requirements: 15.5_

- [x] 15. Create comprehensive documentation








  - [x] 15.1 Create README.md with project overview, setup instructions, and running instructions


    - _Requirements: All_
  - [x] 15.2 Document API endpoints with request/response examples


    - _Requirements: All_
  - [x] 15.3 Document authentication and authorization flow


    - _Requirements: 9.1, 9.2, 9.6_


  - [x] 15.4 Document database schema and relationships




    - _Requirements: 11.1, 11.4_
