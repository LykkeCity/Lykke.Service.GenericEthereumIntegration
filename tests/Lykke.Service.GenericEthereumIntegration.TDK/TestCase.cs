using System.Collections.Generic;
using System.Linq;

namespace Lykke.Service.GenericEthereumIntegration.TDK
{
    public class TestCase
    {
        private readonly IDictionary<string, (object Value, bool IsValid)> _parameters;

        public TestCase()
        {
            _parameters = new Dictionary<string, (object, bool)>();
        }
        
        public bool IsValid
            => _parameters.Select(x => x.Value.IsValid).All(x => x);


        public bool ContainsParameter(string name)
            => _parameters.ContainsKey(name);
        
        public T GetParameterValue<T>(string name)
            => (T) _parameters[name].Value;

        internal void SetParameter(string name, object value, bool isValid)
        {
            _parameters[name] = (value, isValid);
        }
    }
}
