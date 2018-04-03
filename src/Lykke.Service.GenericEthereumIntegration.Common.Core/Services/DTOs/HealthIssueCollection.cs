using System.Collections;
using System.Collections.Generic;

namespace Lykke.Service.GenericEthereumIntegration.Common.Core.Services.DTOs
{
    public class HealthIssueCollection : IReadOnlyCollection<HealthIssue>
    {
        private readonly IList<HealthIssue> _innerList;


        public HealthIssueCollection()
        {
            _innerList = new List<HealthIssue>();
        }


        // ReSharper disable once UnusedMember.Global
        public void Add(string type, string value)
        {
            _innerList.Add(new HealthIssue(type, value));
        }

        public IEnumerator<HealthIssue> GetEnumerator()
        {
            return _innerList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count
            => _innerList.Count;
    }
}
