# Requirements Document

## Introduction

This specification defines the changes needed to support multiple SKUs per license in the License Management System. Currently, a license can only contain a single SKU. This enhancement will allow a license to include multiple SKUs, enabling more flexible licensing scenarios where customers can purchase bundles or multiple products under a single license.

## Glossary

- **License Management System**: The application that manages software licenses, products, SKUs, customers, and RSA keys
- **License**: A digital authorization that grants a customer access to specific software products
- **SKU (Stock Keeping Unit)**: A unique identifier for a specific product variant or offering
- **Product**: A software product that can have multiple SKUs associated with it
- **Customer**: An entity that purchases and uses licenses
- **License-SKU Relationship**: A many-to-many relationship between licenses and SKUs

## Requirements

### Requirement 1: Multiple SKUs per License

**User Story:** As a license administrator, I want to create licenses that contain multiple SKUs, so that I can provide customers with bundled product offerings under a single license.

#### Acceptance Criteria

1. WHEN creating a new license, THE License Management System SHALL allow the administrator to select multiple SKUs from a list
2. WHEN a license is created with multiple SKUs, THE License Management System SHALL store all selected SKU associations in the database
3. WHEN viewing a license, THE License Management System SHALL display all associated SKUs in the license details
4. WHEN updating a license, THE License Management System SHALL allow the administrator to add or remove SKUs from the license
5. WHEN deleting a license, THE License Management System SHALL remove all SKU associations for that license

### Requirement 2: SKU Selection Interface

**User Story:** As a license administrator, I want an intuitive interface for selecting multiple SKUs when creating or editing a license, so that I can easily manage which products are included in the license.

#### Acceptance Criteria

1. WHEN the license creation form is displayed, THE License Management System SHALL show a multi-select interface for SKUs
2. WHEN SKUs are displayed in the selection interface, THE License Management System SHALL show both the SKU name and its associated product name
3. WHEN a SKU is selected, THE License Management System SHALL provide visual feedback indicating the selection
4. WHEN multiple SKUs from different products are selected, THE License Management System SHALL allow the selection without restriction
5. WHERE at least one SKU is selected, THE License Management System SHALL enable the form submission

### Requirement 3: License Display with Multiple SKUs

**User Story:** As a license administrator, I want to see all SKUs associated with a license in the license table and details view, so that I can quickly understand what products are included in each license.

#### Acceptance Criteria

1. WHEN viewing the licenses table, THE License Management System SHALL display all associated SKU names for each license
2. WHEN a license has multiple SKUs, THE License Management System SHALL format the SKU list in a readable manner
3. WHEN viewing license details, THE License Management System SHALL show each SKU with its associated product information
4. WHEN a license has no SKUs, THE License Management System SHALL display an appropriate message indicating no SKUs are associated
5. WHEN the SKU list is too long to display in the table, THE License Management System SHALL truncate the list with an indicator showing additional SKUs exist

### Requirement 4: Database Schema Changes

**User Story:** As a system architect, I want the database schema to support many-to-many relationships between licenses and SKUs, so that the data model accurately represents the business requirements.

#### Acceptance Criteria

1. THE License Management System SHALL create a junction table to store license-SKU relationships
2. THE License Management System SHALL maintain referential integrity between licenses and SKUs
3. WHEN a SKU is deleted, THE License Management System SHALL remove all associations with licenses but preserve the license records
4. WHEN a license is deleted, THE License Management System SHALL remove all SKU associations for that license
5. THE License Management System SHALL support efficient querying of licenses with their associated SKUs

### Requirement 5: API Endpoint Updates

**User Story:** As a frontend developer, I want updated API endpoints that support multiple SKUs per license, so that I can build the user interface to manage multi-SKU licenses.

#### Acceptance Criteria

1. WHEN creating a license via the API, THE License Management System SHALL accept an array of SKU IDs in the request body
2. WHEN retrieving a license via the API, THE License Management System SHALL return all associated SKU information in the response
3. WHEN updating a license via the API, THE License Management System SHALL accept an array of SKU IDs to replace existing associations
4. WHEN listing licenses via the API, THE License Management System SHALL include SKU information for each license in the response
5. WHERE invalid SKU IDs are provided, THE License Management System SHALL return a validation error with details about which SKU IDs are invalid

### Requirement 6: Validation Rules

**User Story:** As a license administrator, I want the system to validate my SKU selections, so that I don't create invalid license configurations.

#### Acceptance Criteria

1. WHEN creating or updating a license, THE License Management System SHALL require at least one SKU to be selected
2. WHEN duplicate SKU IDs are provided, THE License Management System SHALL remove duplicates and process unique SKUs only
3. WHEN a non-existent SKU ID is provided, THE License Management System SHALL return a validation error
4. WHEN all selected SKUs are from the same product, THE License Management System SHALL allow the license creation
5. WHEN selected SKUs are from different products, THE License Management System SHALL allow the license creation

### Requirement 7: UI Form Updates

**User Story:** As a license administrator, I want the license creation and edit forms to support multiple SKU selection, so that I can easily manage which products are included in each license.

#### Acceptance Criteria

1. WHEN the license form is displayed, THE License Management System SHALL show a multi-select dropdown or checkbox list for SKUs
2. WHEN SKUs are grouped by product, THE License Management System SHALL display the product name as a group header
3. WHEN a SKU is selected or deselected, THE License Management System SHALL update the form state immediately
4. WHEN the form is submitted with no SKUs selected, THE License Management System SHALL display a validation error
5. WHEN the form is submitted with valid SKUs, THE License Management System SHALL create or update the license with all selected SKUs

### Requirement 8: License Key JWT Structure

**User Story:** As a software application, I want the license key JWT to contain all product and SKU information, so that I can validate which products and features the license grants access to.

#### Acceptance Criteria

1. WHEN a license key is generated, THE License Management System SHALL include the license name in the JWT body
2. WHEN a license key is generated, THE License Management System SHALL include all associated SKU information in the JWT body
3. WHEN a license has multiple SKUs, THE License Management System SHALL include the product name for each SKU in the JWT
4. WHEN the JWT is decoded, THE License Management System SHALL provide a structured format showing each product-SKU pair
5. WHEN a license has SKUs from multiple products, THE License Management System SHALL include all product-SKU combinations in the JWT
