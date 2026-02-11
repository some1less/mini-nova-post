using MiniNova.BLL.DTO.Tracking;
using MiniNova.BLL.Mappers;
using MiniNova.DAL.Repositories.Account;
using MiniNova.DAL.Repositories.Operator;
using MiniNova.DAL.Repositories.Shipment;
using MiniNova.DAL.Repositories.Status;
using MiniNova.DAL.Repositories.Tracking;

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


    public async Task<IEnumerable<TrackingResponseDTO>> GetHistoryByPackageIdAsync(int shipmentId, CancellationToken ct)
    {
        var shipment = await _shipmentRepository.ExistsAsync(shipmentId, ct);
        if (!shipment) throw new KeyNotFoundException($"Shipment with id {shipmentId} not found");

        var history = await _trackingRepository.GetAllAsync(shipmentId, ct);

        return history.Select(t => t.ToDto());
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
        
        var tracking = trackingDto.ToEntity(oper.Id);
        await  _trackingRepository.AddAsync(tracking, cancellationToken);
        
        tracking.Status = statusEntity;
        tracking.Operator = oper;
        return tracking.ToDto();
    }

    public async Task UpdateTrackingAsync(int trackingId, UpdateTrackingDTO trackingDto, CancellationToken ct)
    {
        var tracking = await _trackingRepository.GetByIdAsync(trackingId, ct);
        if (tracking == null) throw new KeyNotFoundException($"Tracking {trackingId} not found");

        var statusExists = await _statusRepository.IfExistsAsync(trackingDto.StatusId,  ct);
        if (!statusExists) throw new KeyNotFoundException($"Status ID {trackingDto.StatusId} not found");

        tracking.MapUpdate(trackingDto);
        
        await _trackingRepository.Update(tracking, ct);
    }

    public async Task DeleteTrackingAsync(int trackingId, CancellationToken cancellationToken)
    {
        var tracking = await _trackingRepository.GetByIdAsync(trackingId, cancellationToken);
            
        if (tracking == null) 
            throw new KeyNotFoundException($"Tracking record with id {trackingId} not found");

        await _trackingRepository.Delete(tracking, cancellationToken);
    }
}