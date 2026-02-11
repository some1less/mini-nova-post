using Microsoft.EntityFrameworkCore;
using MiniNova.BLL.DTO.People;
using MiniNova.BLL.DTO.Shipment;
using MiniNova.BLL.DTO.Tracking;
using MiniNova.BLL.Generators;
using MiniNova.BLL.Interfaces;
using MiniNova.BLL.Pagination;
using MiniNova.DAL.Context;
using MiniNova.DAL.Models;
using MiniNova.DAL.Repositories.Interfaces;

namespace MiniNova.BLL.Services;

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

        var dtos = result.Items.Select(p => new ShipmentAllDTO
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
            DestinationAddress =
                $"{p.Destination.Address} [{p.Destination.Postcode}], {p.Destination.City}, {p.Destination.Country}",
            OriginAddress = $"{p.Origin.Address} [{p.Origin.Postcode}], {p.Origin.City}, {p.Origin.Country}",
            Status = p.Trackings
                .OrderByDescending(t => t.UpdateTime)
                .Select(t => t.Status.Name)
                .FirstOrDefault() ?? "null",
        }).ToList();

        return new PagedResponse<ShipmentAllDTO>
        {
            Items = dtos,
            Page = page,
            PageSize = pageSize,
            TotalCount = result.TotalCount
        };
    }

    public async Task<ShipmentByIdDTO> GetShipmentByIdAsync(int shipmentId, CancellationToken cancellationToken)
    {
        var shipment = await _shipmentRepository.GetByIdWithDetailsAsync(shipmentId, cancellationToken);
        if (shipment == null) throw new KeyNotFoundException($"Shipment with id {shipment} not found");

        var sortedHistory = shipment.Trackings.OrderByDescending(t => t.UpdateTime).ToList();
        var lastStatus = sortedHistory.FirstOrDefault()?.Status?.Name ?? "Registered";

        return new ShipmentByIdDTO()
        {
            Id = shipment.Id,
            TrackingNo = shipment.TrackId,
            Shipper = new PersonResponseDTO()
            {
                Id = shipment.Shipper.Id,
                FullName = $"{shipment.Shipper.FirstName} {shipment.Shipper.LastName}",
                Email = shipment.Shipper.Email,
                Phone = shipment.Shipper.Phone,
            },
            Consignee = new PersonResponseDTO()
            {
                Id = shipment.Consignee.Id,
                FullName = $"{shipment.Consignee.FirstName} {shipment.Consignee.LastName}",
                Email = shipment.Consignee.Email,
                Phone = shipment.Consignee.Phone,
            },
            Description = shipment.Description,
            Size = shipment.Size.Name,
            Weight = shipment.Weight,

            DestinationAddress = $"{shipment.Destination.Address}, {shipment.Destination.City}",

            Status = lastStatus,
            History = sortedHistory.Select(t => new TrackingResponseDTO
            {
                Id = t.Id,
                Status = t.Status?.Name ?? "Unknown",
                UpdateTime = t.UpdateTime.ToString("yyyy-MM-dd HH:mm"),
                OperatorName = t.Operator != null
                    ? $"{t.Operator.Person.FirstName} {t.Operator.Person.LastName}"
                    : "System",
                OperatorRole = t.Operator?.Occupation.Name ?? "Auto"
            })
        };
    }

    public async Task<ShipmentByIdDTO> CreateShipmentAsync(CreateShipmentDTO shipmentDto,
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

        string generateTrackNo =
            _trackingNumberGenerator.GenerateTrackingNumber(destination.Country, shipmentDto.SizeId,
                shipmentDto.Weight);

        var package = new Shipment()
        {
            TrackId = generateTrackNo,
            ShipperId = sender.Id,
            ConsigneeId = receiver.Id,
            DestinationId = shipmentDto.DestinationId,
            OriginId = shipmentDto.OriginId,
            Description = shipmentDto.Description,
            Weight = shipmentDto.Weight,
            SizeId = shipmentDto.SizeId,
        };

        await _shipmentRepository.AddAsync(package, cancellationToken);

        var initialTracking = new Tracking()
        {
            Shipment = package,
            StatusId = 1,
            UpdateTime = DateTime.UtcNow,
            OperatorId = 1
        };

        package.Trackings.Add(initialTracking);

        await _shipmentRepository.SaveChangesAsync(cancellationToken);
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

        shipment.Description = shipmentDto.Description;
        shipment.SizeId = shipmentDto.SizeId;
        shipment.Weight = shipmentDto.Weight;
        shipment.DestinationId = shipmentDto.DestinationId;

        _shipmentRepository.Update(shipment);
        await _shipmentRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteShipmentAsync(int shipmentId, CancellationToken cancellationToken)
    {
        var shipment = await _shipmentRepository.GetByIdWithDetailsAsync(shipmentId, cancellationToken);
        if (shipment == null)
            throw new KeyNotFoundException($"Shipment with id {shipmentId} not found");

        _shipmentRepository.Remove(shipment);
        await _shipmentRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<PagedResponse<ShipmentByIdDTO>> GetUserShipmentsAsync(int userId,
        CancellationToken cancellationToken, int page, int pageSize)
    {
        var skip = (page - 1) * pageSize;

        var data = await _shipmentRepository.GetByUserIdPagedAsync(userId, skip, pageSize, cancellationToken);

        var dtos = data.Items.Select(p =>
        {
            var lastStatus = p.Trackings
                .OrderByDescending(t => t.UpdateTime)
                .Select(t => t.Status.Name)
                .FirstOrDefault() ?? "Unknown";

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
        }).ToList();


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

        var sortedHistory = shipment.Trackings.OrderByDescending(t => t.UpdateTime).ToList();

        var lastStatus = shipment.Trackings
            .OrderByDescending(t => t.UpdateTime)
            .Select(t => t.Status.Name)
            .FirstOrDefault() ?? "null";

        return new ShipmentByIdDTO()
        {
            Id = shipment.Id,
            TrackingNo = shipment.TrackId,
            Shipper = new PersonResponseDTO()
            {
                Id = shipment.Shipper.Id,
                FullName = $"{shipment.Shipper.FirstName} {shipment.Shipper.LastName}",
                Email = shipment.Shipper.Email,
                Phone = shipment.Shipper.Phone,
            },
            Consignee = new PersonResponseDTO()
            {
                Id = shipment.Consignee.Id,
                FullName = $"{shipment.Consignee.FirstName} {shipment.Consignee.LastName}",
                Email = shipment.Consignee.Email,
                Phone = shipment.Consignee.Phone,
            },
            Description = shipment.Description,
            Size = shipment.Size.Name,
            Weight = shipment.Weight,

            DestinationAddress = $"{shipment.Destination.Address}, {shipment.Destination.City}",

            Status = lastStatus,
            History = sortedHistory.Select(t => new TrackingResponseDTO
            {
                Id = t.Id,
                Status = t.Status?.Name ?? "Unknown",
                UpdateTime = t.UpdateTime.ToString("yyyy-MM-dd HH:mm"),
                OperatorName = t.Operator != null
                    ? $"{t.Operator.Person.FirstName} {t.Operator.Person.LastName}"
                    : "System",
                OperatorRole = t.Operator?.Occupation.Name ?? "Auto"
            })
        };
    }
}