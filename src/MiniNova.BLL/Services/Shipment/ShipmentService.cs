using MiniNova.BLL.DTO.People;
using MiniNova.BLL.DTO.Shipment;
using MiniNova.BLL.DTO.Tracking;
using MiniNova.BLL.Generators;
using MiniNova.BLL.Mappers;
using MiniNova.BLL.Pagination;
using MiniNova.DAL.Repositories.Location;
using MiniNova.DAL.Repositories.Person;
using MiniNova.DAL.Repositories.Shipment;

namespace MiniNova.BLL.Services.Shipment;

public class ShipmentService : IShipmentService
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly IPersonRepository _personRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly ITrackingNumberGeneratorService _trackingNumberGenerator;

    public ShipmentService(IShipmentRepository shipmentRepository, IPersonRepository personRepository,
        ILocationRepository locationRepository, ITrackingNumberGeneratorService trackingNumberGenerator)
    {
        _shipmentRepository = shipmentRepository;
        _personRepository = personRepository;
        _locationRepository = locationRepository;
        _trackingNumberGenerator = trackingNumberGenerator;
    }

    public async Task<PagedResponse<ShipmentAllDTO>> GetAllAsync(CancellationToken cancellationToken, int page,
        int pageSize = 10)
    {
        var skip = (page - 1) * pageSize;
        var result = await _shipmentRepository.GetPagedAsync(skip, pageSize, cancellationToken);

        var dtos = result.Items
            .Select(p => p.ToAllDto())
            .ToList();

        return new PagedResponse<ShipmentAllDTO>
        {
            Items = dtos,
            Page = page,
            PageSize = pageSize,
            TotalCount = result.TotalCount
        };
    }

    public async Task<ShipmentByIdDTO?> GetShipmentByIdAsync(int shipmentId, CancellationToken cancellationToken)
    {
        var shipment = await _shipmentRepository.GetByIdWithDetailsAsync(shipmentId, cancellationToken);
        return shipment?.ToDetailedDto();
    }

    public async Task<ShipmentByIdDTO?> CreateShipmentAsync(CreateShipmentDTO shipmentDto,
        CancellationToken cancellationToken, int senderId)
    {
        var sender = await _personRepository.GetByIdAsync(senderId, cancellationToken);
        if (sender == null)
            throw new KeyNotFoundException("Hmmm... Sender not found. Please re-login");

        var receiver = await _personRepository.GetByEmailAsync(shipmentDto.ConsigneeEmail, cancellationToken);
        if (receiver == null)
            throw new KeyNotFoundException($"Receiver with email {shipmentDto.ConsigneeEmail} not found");

        var destination = await _locationRepository.GetByIdAsync(shipmentDto.DestinationId, cancellationToken);
        if (destination == null)
            throw new KeyNotFoundException($"Destination with id {shipmentDto.DestinationId} not found");

        if (sender.Id == receiver.Id)
        {
            throw new ArgumentException("You can't send a package to yourself :)");
        }

        string trackNo =
            _trackingNumberGenerator.GenerateTrackingNumber(destination.Country, shipmentDto.SizeId,
                shipmentDto.Weight);

        var package = shipmentDto.ToEntity(trackNo, sender.Id, receiver.Id);
        
        package.Trackings.Add(new DAL.Models.Tracking
        {
            StatusId = 1, // Registered
            OperatorId = 1, // System
            UpdateTime = DateTime.UtcNow
        });
        
        await _shipmentRepository.AddAsync(package, cancellationToken);
        return await GetShipmentByIdAsync(package.Id, cancellationToken);
    }

    public async Task UpdateShipmentAsync(UpdateShipmentDTO shipmentDto, int shipmentId,
        CancellationToken cancellationToken)
    {
        var shipment = await _shipmentRepository.GetByIdWithDetailsAsync(shipmentId, cancellationToken);
        if (shipment == null)
            throw new KeyNotFoundException($"Shipment with id {shipmentId} not found");

        var destination = await _locationRepository.GetByIdAsync(shipmentDto.DestinationId, cancellationToken);
        if (destination == null)
            throw new KeyNotFoundException($"Destination with id {shipmentDto.DestinationId} not found");

        shipment.MapUpdate(shipmentDto);
        
        await _shipmentRepository.Update(shipment, cancellationToken);
    }

    public async Task DeleteShipmentAsync(int shipmentId, CancellationToken cancellationToken)
    {
        var shipment = await _shipmentRepository.GetByIdWithDetailsAsync(shipmentId, cancellationToken);
        if (shipment == null)
            throw new KeyNotFoundException($"Shipment with id {shipmentId} not found");

        await _shipmentRepository.Remove(shipment, cancellationToken);
    }

    public async Task<PagedResponse<ShipmentByIdDTO>> GetUserShipmentsAsync(int userId,
        CancellationToken cancellationToken, int page, int pageSize)
    {
        var skip = (page - 1) * pageSize;
        var data = await _shipmentRepository
            .GetByUserIdPagedAsync(userId, skip, pageSize, cancellationToken);

        var dtos = data.Items
            .Select(p => p.ToLessDetailedDto())
            .ToList();
        
        return new PagedResponse<ShipmentByIdDTO>
        {
            Items = dtos,
            TotalCount = data.TotalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<ShipmentByIdDTO> GetShipmentByTrackingNumberAsync(string trackingNumber, CancellationToken ct)
    {
        if (!_trackingNumberGenerator.ValidateTrackingNumber(trackingNumber))
        {
            throw new ArgumentException("Invalid tracking number format or checksum.");
        }

        var cleanTrackNo = trackingNumber.Replace(" ", "").Replace("-", "").ToUpper();

        var shipment = await _shipmentRepository.GetByTrackNoAsync(cleanTrackNo, ct);
        if (shipment == null)
            throw new KeyNotFoundException($"Shipment with tracking number '{trackingNumber}' not found");

        return shipment.ToDetailedDto();
    }
}