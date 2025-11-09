using Microsoft.EntityFrameworkCore;
using LicenseManagementApi.Data;
using LicenseManagementApi.Models;
using LicenseManagementApi.Models.DTOs;
using LicenseManagementApi.Models.Entities;
using LicenseManagementApi.Models.Requests;

namespace LicenseManagementApi.Services;

public class ProductService : IProductService
{
    private readonly LicenseManagementDbContext _context;
    private readonly ILogger<ProductService> _logger;

    public ProductService(LicenseManagementDbContext context, ILogger<ProductService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ServiceResult<ProductDto>> CreateProductAsync(CreateProductRequest request)
    {
        try
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                ProductCode = request.ProductCode,
                Description = request.Description
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Product created with ID: {ProductId}", product.Id);

            return ServiceResult<ProductDto>.Success(MapToDto(product));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            return ServiceResult<ProductDto>.Failure("An error occurred while creating the product", "INTERNAL_ERROR");
        }
    }

    public async Task<ServiceResult<ProductDto>> GetProductByIdAsync(Guid id)
    {
        try
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return ServiceResult<ProductDto>.Failure($"Product with ID {id} not found", "NOT_FOUND");
            }

            return ServiceResult<ProductDto>.Success(MapToDto(product));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving product with ID: {ProductId}", id);
            return ServiceResult<ProductDto>.Failure("An error occurred while retrieving the product", "INTERNAL_ERROR");
        }
    }

    public async Task<ServiceResult<ProductDto>> UpdateProductAsync(Guid id, UpdateProductRequest request)
    {
        try
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return ServiceResult<ProductDto>.Failure($"Product with ID {id} not found", "NOT_FOUND");
            }

            product.Name = request.Name;
            product.ProductCode = request.ProductCode;
            product.Description = request.Description;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Product updated with ID: {ProductId}", product.Id);

            return ServiceResult<ProductDto>.Success(MapToDto(product));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product with ID: {ProductId}", id);
            return ServiceResult<ProductDto>.Failure("An error occurred while updating the product", "INTERNAL_ERROR");
        }
    }

    public async Task<ServiceResult<bool>> DeleteProductAsync(Guid id)
    {
        try
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return ServiceResult<bool>.Failure($"Product with ID {id} not found", "NOT_FOUND");
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Product deleted with ID: {ProductId}", id);

            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product with ID: {ProductId}", id);
            return ServiceResult<bool>.Failure("An error occurred while deleting the product", "INTERNAL_ERROR");
        }
    }

    public async Task<ServiceResult<PagedResult<ProductDto>>> ListProductsAsync(int page, int pageSize)
    {
        try
        {
            // Enforce maximum page size
            pageSize = Math.Min(pageSize, 100);
            page = Math.Max(page, 1);

            var totalCount = await _context.Products.CountAsync();

            var products = await _context.Products
                .OrderBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var productDtos = products.Select(MapToDto);

            var pagedResult = new PagedResult<ProductDto>(productDtos, page, pageSize, totalCount);

            return ServiceResult<PagedResult<ProductDto>>.Success(pagedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing products");
            return ServiceResult<PagedResult<ProductDto>>.Failure("An error occurred while listing products", "INTERNAL_ERROR");
        }
    }

    public async Task<ServiceResult<IEnumerable<ProductDto>>> SearchProductsAsync(string searchTerm, int page, int pageSize)
    {
        try
        {
            // Enforce maximum page size
            pageSize = Math.Min(pageSize, 100);
            page = Math.Max(page, 1);

            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => 
                    p.Name.Contains(searchTerm) || 
                    p.ProductCode.Contains(searchTerm) ||
                    (p.Description != null && p.Description.Contains(searchTerm)));
            }

            var products = await query
                .OrderBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var productDtos = products.Select(MapToDto);

            return ServiceResult<IEnumerable<ProductDto>>.Success(productDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching products with term: {SearchTerm}", searchTerm);
            return ServiceResult<IEnumerable<ProductDto>>.Failure("An error occurred while searching products", "INTERNAL_ERROR");
        }
    }

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            ProductCode = product.ProductCode,
            Description = product.Description,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }
}
