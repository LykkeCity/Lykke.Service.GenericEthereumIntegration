using System.Collections.Generic;
using System.Linq;

namespace Lykke.Service.GenericEthereumIntegration.TDK
{
    public class TestCasesGenerator
    {
        private readonly IDictionary<string, (object Value, bool IsValid)[]> _parameters;

        
        public TestCasesGenerator()
        {
            _parameters = new Dictionary<string, (object Value, bool IsValid)[]>();
        }


        public TestCasesGenerator RegisterParameter<T>(string name, IEnumerable<(T Value, bool IsValid)> values)
        {
            _parameters[name] = values
                .Select(x => ((object) x.Value, x.IsValid))
                .ToArray();
            
            return this;
        }

        public IEnumerable<TestCase> GenerateAllCases()
        {
            var permutationsCount = _parameters
                .Select(x => x.Value.Length)
                .Aggregate(1, (x, y) => x * y);

            var testCases = new TestCase[permutationsCount];
            

            for (var i = 0; i < testCases.Length; i++)
            {
                var testCase = new TestCase();

                foreach (var parameterName in _parameters.Keys)
                {
                    var parameter = _parameters[parameterName];
                    var valueIndex = i % parameter.Length;
                    
                    testCase.SetParameter
                    (
                        parameterName,
                        parameter[valueIndex].Value,
                        parameter[valueIndex].IsValid
                    );
                }

                testCases[i] = testCase;
            }

            return testCases;
        }

        public IEnumerable<TestCase> GenerateInvalidCases()
        {
            return GenerateAllCases().Where(x => !x.IsValid);
        }
    }
}
