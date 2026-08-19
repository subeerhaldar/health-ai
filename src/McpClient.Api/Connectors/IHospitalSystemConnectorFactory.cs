namespace McpClient.Api.Connectors;

public interface IHospitalSystemConnectorFactory
{
    IHospitalSystemConnector GetConnector(string systemName);
    IEnumerable<IHospitalSystemConnector> GetAllConnectors();
}
