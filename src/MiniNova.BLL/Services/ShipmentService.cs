using Microsoft.EntityFrameworkCore;
using MiniNova.BLL.DTO.Package;
using MiniNova.BLL.DTO.People;
using MiniNova.BLL.DTO.Tracking;
using MiniNova.BLL.Generators;
using MiniNova.BLL.Interfaces;
using MiniNova.BLL.Pagination;
using MiniNova.DAL.Context;
using MiniNova.DAL.Models;

namespace MiniNova.BLL.Services;

public class ShipmentService : IShipmentService
{
    
    private readonly NovaDbContext _dbContext;
    private readonly ITrackingNumberGeneratorService _trackingNumberGenerator;

    public ShipmentService(NovaDbContext dbContext,  ITrackingNumberGeneratorService trackingNumberGenerator)
    {
        _dbContext = dbContext;
        _trackingNumberGenerator = trackingNumberGenerator;
    }
    
    public async Task<PagedResponse<ShipmentAllDTO>> GetAllAsync(CancellationToken cancellationToken, int page, int pageSize = 10)
    {
        var query = _dbContext.Shipments.AsNoTracking().AsQueryable();

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(p => p.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ShipmentAllDTO 
            { 
                Id = p.Id,
                TrackingNo = p.TrackId,
                Description = p.Description,
                Sender = new PersonAllShipmentsDTO()
                {
                    FullName = $"{p.Shipper.FirstName} {p.Shipper.LastName}",
                    Email = $"{p.Shipper.Email}",
                    Phone = $"{p.Shipper.Phone}",
                },
                Receiver = new PersonAllShipmentsDTO()
                {
                    FullName = $"{p.Consignee.FirstName} {p.Consignee.LastName}",
                    Email = $"{p.Consignee.Email}",
                    Phone = $"{p.Consignee.Phone}",
                },
                DestinationAddress = $"{p.Destination.Address} [{p.Destination.Postcode}], {p.Destination.City}, {p.Destination.Country}",
                OriginAddress = $"{p.Origin.Address} [{p.Origin.Postcode}], {p.Origin.City}, {p.Origin.Country}",
                Status = p.Trackings
                    .OrderByDescending(t => t.UpdateTime)
                    .Select(t => t.Status.Name)
                    .FirstOrDefault() ?? "null",
                
            })
            .ToListAsync(cancellationToken);

        return new PagedResponse<ShipmentAllDTO>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<ShipmentByIdDTO> GetShipmentByIdAsync(int packageId, CancellationToken cancellationToken)
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
            .FirstOrDefaultAsync(p => p.Id == packageId,  cancellationToken);

        if (package == null) throw new KeyNotFoundException($"Package with id {packageId} not found");
        
        var sortedHistory = package.Trackings.OrderByDescending(t => t.UpdateTime).ToList();

        var lastStatus = package.Trackings
            .OrderByDescending(t => t.UpdateTime)
            .Select(t => t.Status.Name)
            .FirstOrDefault() ?? "null";
        
        return new ShipmentByIdDTO()
        {
            Id = package.Id,
            TrackingNo = package.TrackId,
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

    public async Task<ShipmentByIdDTO> CreateShipmentAsync(CreateShipmentDTO packageDto, CancellationToken cancellationToken, int? senderId = null)
    {
        Person? sender = null;

        if (senderId.HasValue)
        {
            sender = await _dbContext.People.FindAsync([senderId.Value], cancellationToken);
        }
    
        if (sender == null && !string.IsNullOrEmpty(packageDto.ShipperEmail))
        {
            sender = await _dbContext.People.FirstOrDefaultAsync(p => p.Email == packageDto.ShipperEmail, cancellationToken);
        }

        if (sender == null) 
            throw new KeyNotFoundException("Sender not found. Please login or provide a valid Sender Email.");
    
        var receiver = await _dbContext.People
            .FirstOrDefaultAsync(p => p.Email == packageDto.ConsigneeEmail,  cancellationToken);
        if (receiver == null) throw new KeyNotFoundException($"Receiver with email {packageDto.ConsigneeEmail} not found");
    
        var destination = await _dbContext.Locations
            .FirstOrDefaultAsync(d => d.Id == packageDto.DestinationId,  cancellationToken);
        if (destination == null)
            throw new KeyNotFoundException($"Destination with id {packageDto.DestinationId} not found");
        
        if (sender.Id == receiver.Id)
        {
            throw new ArgumentException("You cannot send a package to yourself via our service.");
        }
        
        string generateTrackNo = _trackingNumberGenerator.GenerateTrackingNumber(destination.Country, packageDto.SizeId, packageDto.Weight);
        
        var package = new Shipment()
        {
            TrackId = generateTrackNo,
            ShipperId = sender.Id,
            ConsigneeId = receiver.Id,
            DestinationId = packageDto.DestinationId,
            OriginId =  packageDto.OriginId,
            Description = packageDto.Description,
            Weight = packageDto.Weight,
            SizeId = packageDto.SizeId,
        };
    
        _dbContext.Shipments.Add(package);
        
        var initialTracking = new Tracking()
        {
            Shipment = package,
            StatusId = 1,
            UpdateTime = DateTime.UtcNow,
            OperatorId = 1
        };
        _dbContext.Trackings.Add(initialTracking);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    
        return await GetShipmentByIdAsync(package.Id, cancellationToken);
    }

    public async Task UpdateShipmentAsync(UpdateShipmentDTO packageDto, int packageId, CancellationToken cancellationToken)
    {
        
        var package = await _dbContext.Shipments
            .FirstOrDefaultAsync(p => p.Id == packageId, cancellationToken);
        if (package == null) 
            throw new KeyNotFoundException($"Package with id {packageId} not found");

        var destination = await _dbContext.Locations
            .FirstOrDefaultAsync(d => d.Id == packageDto.DestinationId, cancellationToken);
        if (destination == null)
            throw new KeyNotFoundException($"Destination with id {packageDto.DestinationId} not found");
        
        package.Description = packageDto.Description;
        package.SizeId = packageDto.SizeId;
        package.Weight = packageDto.Weight;
        package.DestinationId = packageDto.DestinationId;
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteShipmentAsync(int packageId, CancellationToken cancellationToken)
    {
        var  package = await _dbContext.Shipments
            .FirstOrDefaultAsync(p => p.Id == packageId, cancellationToken);
        if (package == null)
            throw new KeyNotFoundException($"Package with id {packageId} not found");
        
        _dbContext.Shipments.Remove(package);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<PagedResponse<ShipmentByIdDTO>> GetUserShipmentsAsync(int userId, CancellationToken cancellationToken,  int page, int pageSize)
    {
        var query = _dbContext.Shipments
            .Include(p => p.Shipper)
            .Include(p => p.Consignee)     
            .Include(p => p.Destination)
            .Include(p => p.Size)
            .Include(p => p.Trackings)
            .Where(p => p.ShipperId == userId || p.ConsigneeId == userId);

        var totalCount = await query.CountAsync(cancellationToken);

        var packages = await query
            .OrderByDescending(p => p.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var packageDtos = packages.Select(p =>
        {
            var lastStatus = p.Trackings
                .OrderByDescending(t => t.UpdateTime)
                .Select(t => t.Status.Name)
                .FirstOrDefault() ??  "Unknown";


            return new ShipmentByIdDTO
            {
                Id = p.Id,
                TrackingNo = p.TrackId,
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
        
        
        return new PagedResponse<ShipmentByIdDTO>
        {
            Items = packageDtos,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<ShipmentByIdDTO> GetShipmentByTrackingNumberAsync(string trackingNumber, CancellationToken cancellationToken)
    {
        if (!_trackingNumberGenerator.ValidateTrackingNumber(trackingNumber))
        {
            throw new ArgumentException("Invalid tracking number format or checksum.");
        }
        
        var cleanTrackNo = trackingNumber.Replace(" ", "").Replace("-", "").ToUpper();
        
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
            .FirstOrDefaultAsync(p => p.TrackId == cleanTrackNo, cancellationToken);
        
        if (package == null) 
            throw new KeyNotFoundException($"Shipment with tracking number '{trackingNumber}' not found");

        var sortedHistory = package.Trackings.OrderByDescending(t => t.UpdateTime).ToList();

        var lastStatus = package.Trackings
            .OrderByDescending(t => t.UpdateTime)
            .Select(t => t.Status.Name)
            .FirstOrDefault() ?? "null";
        
        return new ShipmentByIdDTO()
        {
            Id = package.Id,
            TrackingNo = package.TrackId,
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
}