using Microsoft.EntityFrameworkCore;
using MiniNova.BLL.DTO.Package;
using MiniNova.BLL.DTO.People;
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
        var query = _dbContext.Packages.AsQueryable();

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
                    FullName = $"{p.Sender.FirstName} {p.Sender.LastName}",
                    Email = $"{p.Sender.Email}",
                    Phone = $"{p.Sender.Phone}",
                },
                Receiver = new PersonAllPackagesDTO()
                {
                    FullName = $"{p.Receiver.FirstName} {p.Receiver.LastName}",
                    Email = $"{p.Receiver.Email}",
                    Phone = $"{p.Receiver.Phone}",
                }
                
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

    public async Task<PackageByIdDTO?> GetPackageByIdAsync(int packageId)
    {
        var package = await _dbContext.Packages
            .Include(d => d.Destination)
            .Include(s => s.Sender)
            .Include(r => r.Receiver)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == packageId);

        if (package == null) return null;
        
        return new PackageByIdDTO()
        {
            Id = package.Id,
            Sender = new PersonResponseDTO()
            {
                Id = package.Sender.Id,
                FullName = $"{package.Sender.FirstName} {package.Sender.LastName}",
                Email = package.Sender.Email,
                Phone = package.Sender.Phone,
            },
            Receiver = new PersonResponseDTO()
            {
                Id = package.Receiver.Id,
                FullName = $"{package.Receiver.FirstName} {package.Receiver.LastName}",
                Email = package.Receiver.Email,
                Phone = package.Receiver.Phone,
            },
            Description = package.Description,
            Size = package.Size,
            Weight = package.Weight,

            DestinationAddress = $"{package.Destination.Street}, {package.Destination.City}"
        };
    }

    public async Task<PackageByIdDTO> CreatePackageAsync(CreatePackageDTO packageDto, int? senderId = null)
    {
        Person? sender = null;

        if (senderId.HasValue)
        {
            sender = await _dbContext.People.FindAsync(senderId.Value);
        }
    
        if (sender == null && !string.IsNullOrEmpty(packageDto.SenderEmail))
        {
            sender = await _dbContext.People.FirstOrDefaultAsync(p => p.Email == packageDto.SenderEmail);
        }

        if (sender == null) 
            throw new KeyNotFoundException("Sender not found. Please login or provide a valid Sender Email.");
    
        var receiver = await _dbContext.People
            .FirstOrDefaultAsync(p => p.Email == packageDto.ReceiverEmail);
        if (receiver == null) throw new KeyNotFoundException($"Receiver with email {packageDto.ReceiverEmail} not found");
    
        var destination = await _dbContext.Destinations
            .FirstOrDefaultAsync(d => d.Id == packageDto.DestinationId);
        if (destination == null)
            throw new KeyNotFoundException($"Destination with id {packageDto.DestinationId} not found");
    
        var package = new Package()
        {
            SenderId = sender.Id,
            ReceiverId = receiver.Id,
            DestinationId = packageDto.DestinationId,
            Description = packageDto.Description,
            Weight = packageDto.Weight,
            Size = packageDto.Size
        };
    
        _dbContext.Packages.Add(package);
        await _dbContext.SaveChangesAsync();
    
        return await GetPackageByIdAsync(package.Id)!;
    }

    public async Task UpdatePackageAsync(UpdatePackageDTO packageDto, int packageId)
    {
        
        var package = await _dbContext.Packages
            .FirstOrDefaultAsync(p => p.Id == packageId);
        if (package == null) 
            throw new KeyNotFoundException($"Package with id {packageId} not found");

        var destination = await _dbContext.Destinations
            .FirstOrDefaultAsync(d => d.Id == packageDto.DestinationId);
        if (destination == null)
            throw new KeyNotFoundException($"Destination with id {packageDto.DestinationId} not found");
        
        package.Description = packageDto.Description;
        package.Size = packageDto.Size;
        package.Weight = packageDto.Weight;
        package.DestinationId = packageDto.DestinationId;
        
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeletePackageAsync(int packageId)
    {
        var  package = await _dbContext.Packages
            .FirstOrDefaultAsync(p => p.Id == packageId);
        if (package == null)
            throw new KeyNotFoundException($"Package with id {packageId} not found");
        
        _dbContext.Packages.Remove(package);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<PagedResponse<PackageByIdDTO>> GetUserPackagesAsync(int userId, int page, int pageSize)
    {
        var query = _dbContext.Packages
            .Include(p => p.Sender)
            .Include(p => p.Receiver)     
            .Include(p => p.Destination)  
            .Where(p => p.SenderId == userId || p.ReceiverId == userId);

        var totalCount = await query.CountAsync();

        var packages = await query
            .OrderByDescending(p => p.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var packageDtos = packages.Select(p => new PackageByIdDTO
        {
            Id = p.Id,
            Description = p.Description,
            Size = p.Size,
            Weight = p.Weight,

            DestinationAddress = $"{p.Destination.Street}, {p.Destination.City}",
        
            Sender = new PersonResponseDTO
            {
                Id = p.Sender.Id,
                FullName = $"{p.Sender.FirstName} {p.Sender.LastName}",
                Email = p.Sender.Email,
                Phone = p.Sender.Phone
            },
        
            Receiver = new PersonResponseDTO
            {
                Id = p.Receiver.Id,
                FullName = $"{p.Receiver.FirstName} {p.Receiver.LastName}",
                Email = p.Receiver.Email,
                Phone = p.Receiver.Phone
            }
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