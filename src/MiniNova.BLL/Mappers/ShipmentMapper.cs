using MiniNova.BLL.DTO.People;
using MiniNova.BLL.DTO.Shipment;
using MiniNova.BLL.DTO.Tracking;
using MiniNova.BLL.Generators;
using MiniNova.DAL.Models;

namespace MiniNova.BLL.Mappers;

public static class ShipmentMapper
{
    public static ShipmentAllDTO ToAllDto(this Shipment p)
    {
        return new ShipmentAllDTO()
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
                .FirstOrDefault() ?? "null"
        };
    }

    public static ShipmentByIdDTO ToDetailedDto(this Shipment shipment)
    {
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

    public static Shipment ToEntity(this CreateShipmentDTO dto, string trackNo, int senderId, int receiverId)
    {
        return new Shipment()
        {
            TrackId = trackNo,
            ShipperId = senderId,
            ConsigneeId = receiverId,
            DestinationId = dto.DestinationId,
            OriginId = dto.OriginId,
            Description = dto.Description,
            Weight = dto.Weight,
            SizeId = dto.SizeId,
            
            Trackings = new List<Tracking>()
        };
    }

    public static void MapUpdate(this Shipment shipment, UpdateShipmentDTO shipmentDto)
    {
        shipment.Description = shipmentDto.Description;
        shipment.SizeId = shipmentDto.SizeId;
        shipment.Weight = shipmentDto.Weight;
        shipment.DestinationId = shipmentDto.DestinationId;
    }

    public static ShipmentByIdDTO ToLessDetailedDto(this Shipment p)
    {
        var lastStatus = p.Trackings
            .OrderByDescending(t => t.UpdateTime)
            .Select(t => t.Status.Name)
            .FirstOrDefault() ?? "Unknown";
        
        return new ShipmentByIdDTO()
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
    }
    
}