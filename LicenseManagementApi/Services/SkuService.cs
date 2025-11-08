using Microsoft.EntityFrameworkCore;
using LicenseManagementApi.Data;
using LicenseManagementApi.Models;
using LicenseManagementApi.Models.DTOs;
using LicenseManagementApi.Models.Entities;
using LicenseManagementApi.Models.Requests;

namespace LicenseManagementApi.Services;

public class SkuService : ISkuService
{
    private readonly LicenseManagementDbContext _context;
    private readonly ILogger<SkuService> _logger;

    public SkuService(LicenseManagementDbContext context, ILogger<SkuService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ServiceResult<SkuDto>> CreateSkuAsync(CreateSkuRequest request)
    {
        try
        {
            // Verify product exists
            var productExists = await _context.Products.AnyAsync(p => p.Id == request.ProductId);
            if (!productExists)
            {
                return ServiceResult<SkuDto>.Failure($"Product with ID {request.ProductId} not found", "NOT_FOUND");
            }

            var sku = new Sku
            {
                Id = Guid.NewGuid(),
                ProductId = request.ProductId,
                Name = request.Name,
                SkuCode = request.SkuCode,
                Description = request.Description
            };

            _context.Skus.Add(sku);
            await _context.SaveChangesAsync();

            _logger.LogInformation("SKU created with ID: {SkuId}", sku.Id);

            return ServiceResult<SkuDto>.Success(MapToDto(sku));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating SKU");
            return ServiceResult<SkuDto>.Failure("An error occurred while creating the SKU", "INTERNAL_ERROR");
        }
    }

    public async Task<ServiceResult<SkuDto>> GetSkuByIdAsync(Guid id)
    {
        try
        {
            var sku = await _context.Skus.FindAsync(id);

            if (sku == null)
            {
                return ServiceResult<SkuDto>.Failure($"SKU with ID {id} not found", "NOT_FOUND");
            }

            return ServiceResult<SkuDto>.Success(MapToDto(sku));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving SKU with ID: {SkuId}", id);
            return ServiceResult<SkuDto>.Failure("An error occurred while retrieving the SKU", "INTERNAL_ERROR");
        }
    }

    public async Task<ServiceResult<SkuDto>> UpdateSkuAsync(Guid id, UpdateSkuRequest request)
    {
        try
        {
            var sku = await _context.Skus.FindAsync(id);

            if (sku == null)
            {
                return ServiceResult<SkuDto>.Failure($"SKU with ID {id} not found", "NOT_FOUND");
            }

            // Verify product exists
            var productExists = await _context.Products.AnyAsync(p => p.Id == request.ProductId);
            if (!productExists)
            {
                return ServiceResult<SkuDto>.Failure($"Product with ID {request.ProductId} not found", "NOT_FOUND");
            }

            sku.ProductId = request.ProductId;
            sku.Name = request.Name;
            sku.SkuCode = request.SkuCode;
            sku.Description = request.Description;

            await _context.SaveChangesAsync();

            _logger.LogInformation("SKU updated with ID: {SkuId}", sku.Id);

            return ServiceResult<SkuDto>.Success(MapToDto(sku));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating SKU with ID: {SkuId}", id);
            return ServiceResult<SkuDto>.Failure("An error occurred while updating the SKU", "INTERNAL_ERROR");
        }
    }

    public async Task<ServiceResult<bool>> DeleteSkuAsync(Guid id)
    {
        try
        {
            var sku = await _context.Skus.FindAsync(id);

            if (sku == null)
            {
                return ServiceResult<bool>.Failure($"SKU with ID {id} not found", "NOT_FOUND");
            }

            _context.Skus.Remove(sku);
            await _context.SaveChangesAsync();

            _logger.LogInformation("SKU deleted with ID: {SkuId}", id);

            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting SKU with ID: {SkuId}", id);
            return ServiceResult<bool>.Failure("An error occurred while deleting the SKU", "INTERNAL_ERROR");
        }
    }

    public async Task<ServiceResult<PagedResult<SkuDto>>> ListSkusAsync(int page, int pageSize)
    {
        try
        {
            // Enforce maximum page size
            pageSize = Math.Min(pageSize, 100);
            page = Math.Max(page, 1);

            var totalCount = await _context.Skus.CountAsync();

            var skus = await _context.Skus
                .OrderBy(s => s.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var skuDtos = skus.Select(MapToDto);

            var pagedResult = new PagedResult<SkuDto>(skuDtos, page, pageSize, totalCount);

            return ServiceResult<PagedResult<SkuDto>>.Success(pagedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing SKUs");
            return ServiceResult<PagedResult<SkuDto>>.Failure("An error occurred while listing SKUs", "INTERNAL_ERROR");
        }
    }

    public async Task<ServiceResult<PagedResult<SkuDto>>> ListSkusByProductAsync(Guid productId, int page, int pageSize)
    {
        try
        {
            // Enforce maximum page size
            pageSize = Math.Min(pageSize, 100);
            page = Math.Max(page, 1);

            var totalCount = await _context.Skus.CountAsync(s => s.ProductId == productId);

            var skus = await _context.Skus
                .Where(s => s.ProductId == productId)
                .OrderBy(s => s.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var skuDtos = skus.Select(MapToDto);

            var pagedResult = new PagedResult<SkuDto>(skuDtos, page, pageSize, totalCount);

            return ServiceResult<PagedResult<SkuDto>>.Success(pagedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing SKUs for product: {ProductId}", productId);
            return ServiceResult<PagedResult<SkuDto>>.Failure("An error occurred while listing SKUs", "INTERNAL_ERROR");
        }
    }

    public async Task<ServiceResult<IEnumerable<SkuDto>>> SearchSkusAsync(string searchTerm, int page, int pageSize)
    {
        try
        {
            // Enforce maximum page size
            pageSize = Math.Min(pageSize, 100);
            page = Math.Max(page, 1);

            var query = _context.Skus.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(s => 
                    s.Name.Contains(searchTerm) || 
                    s.SkuCode.Contains(searchTerm) ||
                    (s.Description != null && s.Description.Contains(searchTerm)));
            }

            var skus = await query
                .OrderBy(s => s.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var skuDtos = skus.Select(MapToDto);

            return ServiceResult<IEnumerable<SkuDto>>.Success(skuDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching SKUs with term: {SearchTerm}", searchTerm);
            return ServiceResult<IEnumerable<SkuDto>>.Failure("An error occurred while searching SKUs", "INTERNAL_ERROR");
        }
    }

    private static SkuDto MapToDto(Sku sku)
    {
        return new SkuDto
        {
            Id = sku.Id,
            ProductId = sku.ProductId,
            Name = sku.Name,
            SkuCode = sku.SkuCode,
            Description = sku.Description,
            CreatedAt = sku.CreatedAt,
            UpdatedAt = sku.UpdatedAt
        };
    }
}
