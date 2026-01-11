namespace MiniNova.BLL.Generators;

public interface ITrackingNumberGeneratorService
{

    public string GenerateTrackingNumber(string countryName, int sizeId, decimal weight);
    public bool ValidateTrackingNumber(string trackingNumber);

}