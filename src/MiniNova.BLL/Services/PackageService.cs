using Microsoft.EntityFrameworkCore;
using MiniNova.BLL.DTO.Package;
using MiniNova.BLL.DTO.People;
using MiniNova.BLL.DTO.Tracking;
using MiniNova.BLL.Interfaces;
using MiniNova.BLL.Pagination;
using MiniNova.DAL.Context;
using MiniNova.DAL.Models;

namespace MiniNova.BLL.Services;

public class PackageService : IPackageService
{
    
    private readonly NovaDbContext _dbContext;

    public PackageService(NovaDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<PagedResponse<PackageAllDTO>> GetAllAsync(int page, int pageSize)
    {
        var query = _dbContext.Shipments.AsQueryable();

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(p => p.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new PackageAllDTO 
            { 
                Id = p.Id,
                Description = p.Description,
                Sender = new PersonAllPackagesDTO()
                {
                    FullName = $"{p.Shipper.FirstName} {p.Shipper.LastName}",
                    Email = $"{p.Shipper.Email}",
                    Phone = $"{p.Shipper.Phone}",
                },
                Receiver = new PersonAllPackagesDTO()
                {
                    FullName = $"{p.Consignee.FirstName} {p.Consignee.LastName}",
                    Email = $"{p.Consignee.Email}",
                    Phone = $"{p.Consignee.Phone}",
                },
                DestinationAddress = p.Destination != null 
                    ? $"{p.Destination.Address}, {p.Destination.City}" 
                    : "Unknown",
                Status = p.Trackings
                    .OrderByDescending(t => t.UpdateTime)
                    .Select(t => t.Status.Name)
                    .FirstOrDefault() ?? "Registered",
                
            })
            .ToListAsync();

        return new PagedResponse<PackageAllDTO>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<PackageByIdDTO> GetPackageByIdAsync(int packageId)
    {
        var package = await _dbContext.Shipments
            .Include(d => d.Destination)
            .Include(d => d.Origin)
            .Include(s => s.Shipper)
            .Include(r => r.Consignee)
            .Include(p => p.Trackings).ThenInclude(t => t.Operator).ThenInclude(o => o.Person)
            .Include(p => p.Trackings).ThenInclude(t => t.Operator).ThenInclude(o => o.Occupation)
            .Include(p => p.Size)
            .Include(p => p.Trackings).ThenInclude(t => t.Status)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == packageId);

        if (package == null) throw new KeyNotFoundException($"Package with id {packageId} not found");
        
        var sortedHistory = package.Trackings.OrderByDescending(t => t.UpdateTime).ToList();

        var lastStatus = package.Trackings
            .OrderByDescending(t => t.UpdateTime)
            .Select(t => t.Status.Name)
            .FirstOrDefault() ??  "Unknown";
        
        return new PackageByIdDTO()
        {
            Id = package.Id,
            Shipper = new PersonResponseDTO()
            {
                Id = package.Shipper.Id,
                FullName = $"{package.Shipper.FirstName} {package.Shipper.LastName}",
                Email = package.Shipper.Email,
                Phone = package.Shipper.Phone,
            },
            Consignee = new PersonResponseDTO()
            {
                Id = package.Consignee.Id,
                FullName = $"{package.Consignee.FirstName} {package.Consignee.LastName}",
                Email = package.Consignee.Email,
                Phone = package.Consignee.Phone,
            },
            Description = package.Description,
            Size = package.Size.Name,
            Weight = package.Weight,

            DestinationAddress = $"{package.Destination.Address}, {package.Destination.City}",
            
            Status = lastStatus,
            History = sortedHistory.Select(t => new TrackingResponseDTO
            {
                Id = t.Id,
                Status = t.Status.Name,
                UpdateTime = t.UpdateTime.ToString("yyyy-MM-dd HH:mm"),
                OperatorName = t.Operator != null 
                    ? $"{t.Operator.Person.FirstName} {t.Operator.Person.LastName}" 
                    : "System",
                OperatorRole = t.Operator?.Occupation.Name ?? "Auto"
            })
        };
    }

    public async Task<PackageByIdDTO> CreatePackageAsync(CreatePackageDTO packageDto, int? senderId = null)
    {
        Person? sender = null;

        if (senderId.HasValue)
        {
            sender = await _dbContext.People.FindAsync(senderId.Value);
        }
    
        if (sender == null && !string.IsNullOrEmpty(packageDto.ShipperEmail))
        {
            sender = await _dbContext.People.FirstOrDefaultAsync(p => p.Email == packageDto.ShipperEmail);
        }

        if (sender == null) 
            throw new KeyNotFoundException("Sender not found. Please login or provide a valid Sender Email.");
    
        var receiver = await _dbContext.People
            .FirstOrDefaultAsync(p => p.Email == packageDto.ConsigneeEmail);
        if (receiver == null) throw new KeyNotFoundException($"Receiver with email {packageDto.ConsigneeEmail} not found");
    
        var destination = await _dbContext.Locations
            .FirstOrDefaultAsync(d => d.Id == packageDto.DestinationId);
        if (destination == null)
            throw new KeyNotFoundException($"Destination with id {packageDto.DestinationId} not found");
    
        if (sender.Id == receiver.Id)
        {
            throw new ArgumentException("You cannot send a package to yourself via our service.");
        }
        
        var package = new Shipment()
        {
            TrackId = "temp",
            ShipperId = sender.Id,
            ConsigneeId = receiver.Id,
            DestinationId = packageDto.DestinationId,
            OriginId =  packageDto.OriginId,
            Description = packageDto.Description,
            Weight = packageDto.Weight,
            SizeId = packageDto.SizeId,
        };
    
        _dbContext.Shipments.Add(package);
        await _dbContext.SaveChangesAsync();
    
        return await GetPackageByIdAsync(package.Id)!;
    }

    public async Task UpdatePackageAsync(UpdatePackageDTO packageDto, int packageId)
    {
        
        var package = await _dbContext.Shipments
            .FirstOrDefaultAsync(p => p.Id == packageId);
        if (package == null) 
            throw new KeyNotFoundException($"Package with id {packageId} not found");

        var destination = await _dbContext.Locations
            .FirstOrDefaultAsync(d => d.Id == packageDto.DestinationId);
        if (destination == null)
            throw new KeyNotFoundException($"Destination with id {packageDto.DestinationId} not found");
        
        package.Description = packageDto.Description;
        package.SizeId = packageDto.SizeId;
        package.Weight = packageDto.Weight;
        package.DestinationId = packageDto.DestinationId;
        
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeletePackageAsync(int packageId)
    {
        var  package = await _dbContext.Shipments
            .FirstOrDefaultAsync(p => p.Id == packageId);
        if (package == null)
            throw new KeyNotFoundException($"Package with id {packageId} not found");
        
        _dbContext.Shipments.Remove(package);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<PagedResponse<PackageByIdDTO>> GetUserPackagesAsync(int userId, int page, int pageSize)
    {
        var query = _dbContext.Shipments
            .Include(p => p.Shipper)
            .Include(p => p.Consignee)     
            .Include(p => p.Destination)
            .Include(p => p.Size)
            .Include(p => p.Trackings)
            .Where(p => p.ShipperId == userId || p.ConsigneeId == userId);

        var totalCount = await query.CountAsync();

        var packages = await query
            .OrderByDescending(p => p.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var packageDtos = packages.Select(p =>
        {
            var lastStatus = p.Trackings
                .OrderByDescending(t => t.UpdateTime)
                .Select(t => t.Status.Name)
                .FirstOrDefault() ??  "Unknown";


            return new PackageByIdDTO
            {
                Id = p.Id,
                Description = p.Description,
                Size = p.Size.Name,
                Weight = p.Weight,

                DestinationAddress = $"{p.Destination.Address}, {p.Destination.City}",

                Shipper = new PersonResponseDTO
                {
                    Id = p.Shipper.Id,
                    FullName = $"{p.Shipper.FirstName} {p.Shipper.LastName}",
                    Email = p.Shipper.Email,
                    Phone = p.Shipper.Phone
                },

                Consignee = new PersonResponseDTO
                {
                    Id = p.Consignee.Id,
                    FullName = $"{p.Consignee.FirstName} {p.Consignee.LastName}",
                    Email = p.Consignee.Email,
                    Phone = p.Consignee.Phone
                },
                
                Status = lastStatus
            };
        });
        
        
        return new PagedResponse<PackageByIdDTO>
        {
            Items = packageDtos,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }
}