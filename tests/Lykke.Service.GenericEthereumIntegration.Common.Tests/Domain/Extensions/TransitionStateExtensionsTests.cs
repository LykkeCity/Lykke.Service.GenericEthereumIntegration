using System;
using System.Linq;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Domain;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Domain.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lykke.Service.GenericEthereumIntegration.Common.Tests.Domain.Extensions
{
    [TestClass]
    public class TransitionStateExtensionsTests
    {
        [DataTestMethod]
        [DataRow(TransactionState.Built,      TransactionState.Built,      false)]
        [DataRow(TransactionState.Built,      TransactionState.Completed,  false)]
        [DataRow(TransactionState.Built,      TransactionState.Failed,     false)]
        [DataRow(TransactionState.Built,      TransactionState.InProgress,  true)]
        [DataRow(TransactionState.Completed,  TransactionState.Built,      false)]
        [DataRow(TransactionState.Completed,  TransactionState.Completed,  false)]
        [DataRow(TransactionState.Completed,  TransactionState.Failed,     false)]
        [DataRow(TransactionState.Completed,  TransactionState.InProgress, false)]
        [DataRow(TransactionState.Failed,     TransactionState.Built,      false)]
        [DataRow(TransactionState.Failed,     TransactionState.Completed,  false)]
        [DataRow(TransactionState.Failed,     TransactionState.Failed,     false)]
        [DataRow(TransactionState.Failed,     TransactionState.InProgress, false)]
        [DataRow(TransactionState.InProgress, TransactionState.Built,      false)]
        [DataRow(TransactionState.InProgress, TransactionState.Completed,   true)]
        [DataRow(TransactionState.InProgress, TransactionState.Failed,      true)]
        [DataRow(TransactionState.InProgress, TransactionState.InProgress, false)]
        public void IsAllowedToSwitch(TransactionState from, TransactionState to, bool expectedResult)
        {
            Assert.AreEqual(expectedResult, from.IsAllowedToSwitch(to));
        }
        
        [TestMethod]
        public void IsAllowedToSwitch__DoesNotThrowArgumentOutOfRangeException()
        {
            var states = Enum
                .GetValues(typeof(TransactionState))
                .Cast<TransactionState>()
                .ToList();

            foreach (var from in states)
            {
                foreach (var to in states)
                {
                    // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                    from.IsAllowedToSwitch(to);
                }
            }
        }
    }
}
