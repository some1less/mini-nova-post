using MiniNova.BLL.DTO.Package;

namespace MiniNova.BLL.Interfaces;

public interface IPackageService
{
    Task<IEnumerable<PackageAllDTO>> GetAllAsync();
    Task<PackageByIdDTO?> GetPackageByIdAsync(int packageId);
    //
    Task<PackageByIdDTO> CreatePackageAsync(CreatePackageDTO packageDto);
    Task UpdatePackageAsync(UpdatePackageDTO package, int packageId);
    Task DeletePackageAsync(int packageId);
    
}