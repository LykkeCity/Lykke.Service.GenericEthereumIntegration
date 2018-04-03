namespace Lykke.Service.GenericEthereumIntegration.Common.Core.Services.DTOs
{
    public class HealthIssue
    {
        public HealthIssue(string type, string value)
        {
            Type = type;
            Value = value;
        }

        public string Type { get; }

        public string Value { get; }
    }
}
