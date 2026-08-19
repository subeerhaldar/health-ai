namespace McpClient.Api.Connectors;

public class HospitalSystemConnectorFactory : IHospitalSystemConnectorFactory
{
    private readonly IEnumerable<IHospitalSystemConnector> _connectors;

    public HospitalSystemConnectorFactory(IEnumerable<IHospitalSystemConnector> connectors)
    {
        _connectors = connectors;
    }

    public IHospitalSystemConnector GetConnector(string systemName)
    {
        var connector = _connectors.FirstOrDefault(c => c.SystemName.Equals(systemName, StringComparison.OrdinalIgnoreCase));
        
        if (connector == null)
        {
            throw new ArgumentException($"Connector for system '{systemName}' not found.");
        }

        return connector;
    }

    public IEnumerable<IHospitalSystemConnector> GetAllConnectors()
    {
        return _connectors;
    }
}
