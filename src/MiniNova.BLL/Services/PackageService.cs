using Microsoft.EntityFrameworkCore;
using MiniNova.BLL.DTO.Package;
using MiniNova.BLL.DTO.People;
using MiniNova.BLL.Interfaces;
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
    
    public async Task<IEnumerable<PackageAllDTO>> GetAllAsync()
    {
        return await _dbContext.Packages
            .Select(pkg => new PackageAllDTO
            {
                Id = pkg.Id,
                Sender = new PersonAllPackagesDTO
                {
                    FullName = pkg.Sender.FirstName + " " + pkg.Sender.LastName,
                    Email = pkg.Sender.Email,
                    Phone = pkg.Sender.Phone
                },
                Receiver = new PersonAllPackagesDTO
                {
                    FullName = pkg.Receiver.FirstName + " " + pkg.Receiver.LastName,
                    Email = pkg.Receiver.Email,
                    Phone = pkg.Receiver.Phone
                },
                Description = pkg.Description,
            }).ToListAsync();
    }

    public async Task<PackageByIdDTO?> GetPackageByIdAsync(int packageId)
    {
        var package = await _dbContext.Packages
            .Include(d => d.Destination)
            .Include(s => s.Sender)
            .Include(r => r.Receiver)
            .FirstOrDefaultAsync(p => p.Id == packageId);

        if (package == null) return null;
        
        return new PackageByIdDTO()
        {
            Id = package.Id,
            Sender = new PersonByIdDTO()
            {
                Id = package.Sender.Id,
                FullName = $"{package.Sender.FirstName} {package.Sender.LastName}",
                Email = package.Sender.Email,
                Phone = package.Sender.Phone,
            },
            Receiver = new PersonByIdDTO()
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

    public async Task<PackageByIdDTO> CreatePackageAsync(CreatePackageDTO packageDto)
    {
        var sender = await _dbContext.People
            .FirstOrDefaultAsync(p => p.Email == packageDto.SenderEmail);
        if (sender == null) throw new KeyNotFoundException($"Sender with email {packageDto.SenderEmail} not found");
        
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
}