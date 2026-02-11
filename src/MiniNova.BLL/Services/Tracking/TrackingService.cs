using MiniNova.BLL.DTO.Tracking;
using MiniNova.DAL.Repositories.Interfaces;

namespace MiniNova.BLL.Services.Tracking;

public class TrackingService : ITrackingService
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly ITrackingRepository _trackingRepository;
    private readonly IStatusRepository _statusRepository;
    private readonly IOperatorRepository _operatorRepository;
    private readonly IAccountRepository _accountRepository;

    public TrackingService(IShipmentRepository shipmentRepository,
        ITrackingRepository trackingRepository,  IStatusRepository statusRepository,
        IOperatorRepository operatorRepository,  IAccountRepository accountRepository)
    {

        _shipmentRepository = shipmentRepository;
        _trackingRepository = trackingRepository;
        _statusRepository = statusRepository;
        _operatorRepository = operatorRepository;
        _accountRepository = accountRepository;
    }


    public async Task<IEnumerable<TrackingResponseDTO>> GetHistoryByPackageIdAsync(int shipmentId, CancellationToken cancellationToken)
    {
        var shipment = await _shipmentRepository.ExistsAsync(shipmentId, cancellationToken);
        if (!shipment) throw new KeyNotFoundException($"Shipment with id {shipmentId} not found");

        var history = await _trackingRepository.GetAllAsync(shipmentId, cancellationToken);

        return history.Select(t => new TrackingResponseDTO()
        {
            Id = t.Id,
            Status = t.Status?.Name ?? "Unknown",
            UpdateTime = t.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss"),
            OperatorName = t.Operator != null
                ? $"{t.Operator.Person.FirstName} {t.Operator.Person.LastName}"
                : "System Auto-Update",
            OperatorRole = t.Operator?.Occupation.Name ?? "System"
        });
    }

    public async Task<TrackingResponseDTO> AddTrackingAsync(TrackingDTO trackingDto, string operatorLogin,
        CancellationToken cancellationToken)
    {
        var shipment = await _shipmentRepository.ExistsAsync(trackingDto.PackageId, cancellationToken);
        if (!shipment) throw new KeyNotFoundException($"Shipment with id {trackingDto.PackageId} not found");
        
        var statusEntity = await _statusRepository.GetByIdAsync(trackingDto.StatusId, cancellationToken);
        if (statusEntity == null) throw new KeyNotFoundException($"Status ID {trackingDto.StatusId} not found");

        var account = await _accountRepository.GetByLoginAsync(operatorLogin, cancellationToken);
        if (account == null) throw new KeyNotFoundException($"Account {operatorLogin} not found");

        var oper = await _operatorRepository.GetByPersonIdAsync(account.PersonId, cancellationToken);
        if (oper == null) throw new UnauthorizedAccessException("Current user is not an Operator");
        
        var tracking = new DAL.Models.Tracking
        {
            ShipmentId = trackingDto.PackageId,
            OperatorId = oper.Id,
            StatusId = trackingDto.StatusId,
            UpdateTime = DateTime.UtcNow,
        };
        
        await  _trackingRepository.AddAsync(tracking, cancellationToken);
        await _trackingRepository.SaveChangesAsync(cancellationToken);
        
        return new TrackingResponseDTO
        {
            Id = tracking.Id,
            Status = statusEntity.Name,
            UpdateTime = tracking.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss"),
            OperatorName = $"{oper.Person.FirstName} {oper.Person.LastName}",
            OperatorRole = oper.Occupation.Name
        };
    }

    public async Task UpdateTrackingAsync(int trackingId, UpdateTrackingDTO trackingDto, CancellationToken cancellationToken)
    {
        var tracking = await _trackingRepository.GetByIdAsync(trackingId, cancellationToken);
        if (tracking == null) throw new KeyNotFoundException($"Tracking {trackingId} not found");

        var statusExists = await _statusRepository.IfExistsAsync(trackingDto.StatusId,  cancellationToken);
        if (!statusExists) throw new KeyNotFoundException($"Status ID {trackingDto.StatusId} not found");

        tracking.StatusId = trackingDto.StatusId;
        
        _trackingRepository.Update(tracking);
        await _trackingRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteTrackingAsync(int trackingId, CancellationToken cancellationToken)
    {
        var tracking = await _trackingRepository.GetByIdAsync(trackingId, cancellationToken);
            
        if (tracking == null) 
            throw new KeyNotFoundException($"Tracking record with id {trackingId} not found");

        _trackingRepository.Delete(tracking);
        await _trackingRepository.SaveChangesAsync(cancellationToken); 
    }
}