using MiniNova.BLL.DTO.Package;
using MiniNova.BLL.Pagination;

namespace MiniNova.BLL.Interfaces;

public interface IPackageService
{
    Task<PagedResponse<PackageAllDTO>> GetAllAsync(int page = 1, int pageSize = 10);
    Task<PackageByIdDTO?> GetPackageByIdAsync(int packageId);
    //
    Task<PackageByIdDTO> CreatePackageAsync(CreatePackageDTO packageDto);
    Task UpdatePackageAsync(UpdatePackageDTO package, int packageId);
    Task DeletePackageAsync(int packageId);
    
}