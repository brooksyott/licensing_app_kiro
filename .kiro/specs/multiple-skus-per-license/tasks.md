# Implementation Plan

- [x] 1. Backend Database Schema Changes





  - Create LicenseSku junction entity model
  - Update License entity to remove single SkuId and add LicenseSkus collection
  - Create EF Core migration for database schema changes
  - Update DbContext to include LicenseSkus DbSet
  - _Requirements: 1.2, 4.1, 4.2_

- [x] 2. Backend DTOs and Models





  - [x] 2.1 Create LicenseSkuDto class


    - Add SkuId, SkuName, and SkuCode properties
    - _Requirements: 5.2_

  - [x] 2.2 Update LicenseDto


    - Replace single SKU properties with Skus list property
    - _Requirements: 5.2_

  - [x] 2.3 Update CreateLicenseDto


    - Replace SkuId with SkuIds list property
    - Add validation attributes for required list
    - _Requirements: 5.1_

  - [x] 2.4 Update UpdateLicenseDto


    - Replace SkuId with SkuIds list property
    - Add validation attributes for required list
    - _Requirements: 5.3_

- [x] 3. Backend Service Layer





  - [x] 3.1 Update ILicenseService interface


    - Update method signatures if needed for new DTOs
    - _Requirements: 5.1, 5.3_

  - [x] 3.2 Update LicenseService.CreateAsync


    - Add validation for SKU IDs (at least one required)
    - Remove duplicate SKU IDs
    - Verify all SKU IDs exist in database
    - Create LicenseSku associations for each SKU
    - Return appropriate error messages for invalid SKUs
    - _Requirements: 1.1, 1.2, 6.1, 6.2, 6.3, 6.5_


  - [x] 3.3 Update LicenseService.UpdateAsync

    - Add validation for SKU IDs (at least one required)
    - Remove duplicate SKU IDs
    - Verify all SKU IDs exist in database
    - Remove existing LicenseSku associations
    - Create new LicenseSku associations
    - _Requirements: 1.4, 6.1, 6.2, 6.3, 6.5_

  - [x] 3.4 Update LicenseService.GetAllAsync


    - Include LicenseSkus with Sku navigation property
    - Map SKU data to LicenseSkuDto objects
    - _Requirements: 1.3, 5.4_


  - [x] 3.5 Update LicenseService.GetByIdAsync

    - Include LicenseSkus with Sku navigation property
    - Map SKU data to LicenseSkuDto objects
    - _Requirements: 1.3, 5.2_

  - [x] 3.6 Update LicenseService.DeleteAsync


    - Ensure cascade delete removes LicenseSku associations
    - _Requirements: 1.5, 4.4_

- [x] 4. Backend License Key Generator





  - [x] 4.1 Update ILicenseKeyGenerator interface


    - Update GenerateAsync to accept List<Sku> parameter
    - _Requirements: 8.2, 8.3_


  - [x] 4.2 Update LicenseKeyGenerator.GenerateAsync

    - Group SKUs by product
    - Build license name from license type and customer name
    - Create products array with product and SKU details
    - Build JWT payload with license name and products array
    - Include product name for each SKU in JWT
    - Sign and encode JWT with RSA key
    - _Requirements: 8.1, 8.2, 8.3, 8.4, 8.5_

- [x] 5. Backend Controller Updates





  - [x] 5.1 Update LicensesController.Create


    - Accept updated CreateLicenseDto with SkuIds list
    - Return appropriate error responses for validation failures
    - _Requirements: 5.1, 5.5_

  - [x] 5.2 Update LicensesController.Update


    - Accept updated UpdateLicenseDto with SkuIds list
    - Return appropriate error responses for validation failures
    - _Requirements: 5.3, 5.5_

  - [x] 5.3 Update LicensesController.GetAll


    - Return licenses with SKU arrays in response
    - _Requirements: 5.4_

  - [x] 5.4 Update LicensesController.GetById


    - Return license with SKU array in response
    - _Requirements: 5.2_

- [x] 6. Frontend TypeScript Types




  - [x] 6.1 Create LicenseSku interface


    - Add skuId, skuName, and skuCode properties
    - _Requirements: 3.3_

  - [x] 6.2 Update License interface


    - Replace single SKU properties with skus array property
    - _Requirements: 3.1, 3.2, 3.3_

  - [x] 6.3 Update CreateLicenseDto interface


    - Replace skuId with skuIds array property
    - _Requirements: 2.1_

  - [x] 6.4 Update UpdateLicenseDto interface


    - Replace skuId with skuIds array property
    - _Requirements: 2.1_

- [x] 7. Frontend License Form Component





  - [x] 7.1 Update LicenseForm template


    - Replace single SKU dropdown with multi-select interface
    - Add checkbox for each available SKU
    - Display SKU name, code, and product name for each option
    - Group SKUs by product with product name headers
    - _Requirements: 2.1, 2.2, 7.1, 7.2_

  - [x] 7.2 Update LicenseForm script

    - Change formData.skuId to formData.skuIds array
    - Initialize skuIds as empty array
    - Handle checkbox selection/deselection
    - Add validation for at least one SKU selected
    - Update form submission to send skuIds array
    - _Requirements: 2.1, 7.3, 7.4, 7.5_


  - [x] 7.3 Update LicenseForm validation


    - Add error message for no SKUs selected
    - Display validation error in UI
    - _Requirements: 7.4_

- [x] 8. Frontend License Table Component





  - [x] 8.1 Update LicenseTable template


    - Display multiple SKUs in table cell
    - Show first 2 SKUs with badges
    - Add "+X more" indicator for additional SKUs
    - Format SKU list for readability
    - _Requirements: 3.1, 3.2, 3.5_

  - [x] 8.2 Add SKU display styling


    - Style SKU badges
    - Style SKU count indicator
    - Ensure responsive layout for SKU lists
    - _Requirements: 3.2_

- [x] 9. Frontend License Service





  - [x] 9.1 Update licenseService.create


    - Send skuIds array in request body
    - Handle validation errors from API
    - _Requirements: 2.1_

  - [x] 9.2 Update licenseService.update


    - Send skuIds array in request body
    - Handle validation errors from API
    - _Requirements: 2.1_

  - [x] 9.3 Verify licenseService.getAll


    - Ensure it handles SKU arrays in response
    - _Requirements: 3.1_

  - [x] 9.4 Verify licenseService.getById


    - Ensure it handles SKU array in response
    - _Requirements: 3.3_

- [x] 10. Database Migration Execution





  - [x] 10.1 Review generated migration

    - Verify LicenseSkus table creation
    - Verify foreign key constraints
    - Verify indexes on LicenseId and SkuId
    - Verify SkuId column removal from Licenses table
    - _Requirements: 4.1, 4.2, 4.3_

  - [x] 10.2 Apply migration to database


    - Run migration in development environment
    - Verify schema changes applied correctly
    - _Requirements: 4.1_

- [x] 11. Integration Testing





  - [x] 11.1 Test license creation with multiple SKUs


    - Create license with 2+ SKUs from same product
    - Create license with SKUs from different products
    - Verify all SKU associations created in database
    - Verify JWT contains all product/SKU information
    - _Requirements: 1.1, 1.2, 8.2, 8.3, 8.5_

  - [x] 11.2 Test license update with SKU changes

    - Update license to add SKUs
    - Update license to remove SKUs
    - Update license to replace all SKUs
    - Verify old associations removed and new ones created
    - _Requirements: 1.4_

  - [x] 11.3 Test license retrieval with SKUs

    - Retrieve single license and verify SKU data
    - Retrieve all licenses and verify SKU data
    - Verify SKU display in UI table
    - _Requirements: 1.3, 3.1, 3.2_

  - [x] 11.4 Test validation scenarios

    - Attempt to create license with no SKUs (should fail)
    - Attempt to create license with invalid SKU IDs (should fail)
    - Create license with duplicate SKU IDs (should succeed with deduplication)
    - _Requirements: 6.1, 6.2, 6.3_

  - [x] 11.5 Test license deletion

    - Delete license and verify SKU associations removed
    - Verify license record deleted
    - _Requirements: 1.5, 4.4_

  - [x] 11.6 Test JWT structure

    - Decode generated license key JWT
    - Verify license name is present
    - Verify products array structure
    - Verify each product has correct SKU array
    - Verify product names are included for all SKUs
    - _Requirements: 8.1, 8.2, 8.3, 8.4, 8.5_
