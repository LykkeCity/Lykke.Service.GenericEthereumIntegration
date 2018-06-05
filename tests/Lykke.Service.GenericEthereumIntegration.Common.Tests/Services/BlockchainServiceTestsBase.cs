using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Lykke.Service.GenericEthereumIntegration.Common.Core.Services.Interfaces;
using Lykke.Service.GenericEthereumIntegration.TDK;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Lykke.Service.GenericEthereumIntegration.Common.Tests.Services
{
    public abstract class BlockchainServiceTestsBase
    {
        private readonly IConfigurationRoot _configuration;
        
        protected BlockchainServiceTestsBase()
        {
            _configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddJsonFile("appsettings.development.json")
                .Build();
        }
        
        
        protected abstract IBlockchainService BlockchainService { get; }

        protected abstract string RpcHostUrlKey { get; }

        protected string RpcHostUrl
            => _configuration.GetValue<string>(RpcHostUrlKey);
        

        
        #region BuildTransaction
        
        public virtual void BuildTransaction__ValidArgumentsPassed__TransactionBuilt()
        {
            var actualResult = BlockchainService.BuildTransaction("0xEA674fdDe714fd979de3EdF0F56AA9716B898ec8", 2, 0, 2, 2);

            Assert.AreEqual("0x95c40102c40102c40102c40100d92a307845413637346664446537313466643937396465334564463046353641413937313642383938656338", actualResult);
        }
        
        public virtual void BuildTransaction__InvalidArgumentsPassed__ExceptionThrown()
        {
            const string to = nameof(to);
            const string amount = nameof(amount);
            const string nonce = nameof(nonce);
            const string gasPrice = nameof(gasPrice);
            const string gasAmount = nameof(gasAmount);
            
            var testCasesGenerator = new TestCasesGenerator();

            testCasesGenerator
                .RegisterAddressParameter(to)
                .RegisterParameter(amount, new []
                {
                    (-1, false),
                    (0, false),
                    (1, true),
                })
                .RegisterParameter(nonce, new []
                {
                    (-1, false),
                    (0, true)
                })
                .RegisterParameter(gasPrice, new []
                {
                    (-1, false),
                    (0, false),
                    (1, true),
                })
                .RegisterParameter(gasAmount, new []
                {
                    (-1, false),
                    (0, false),
                    (1, true),
                });
            

            foreach (var testCase in testCasesGenerator.GenerateInvalidCases())
            {
                Assert.ThrowsException<ArgumentException>
                (
                    () => BlockchainService.BuildTransaction
                    (
                        to: testCase.GetParameterValue<string>(to),
                        amount: testCase.GetParameterValue<int>(amount),
                        nonce: testCase.GetParameterValue<int>(nonce),
                        gasPrice: testCase.GetParameterValue<int>(gasPrice),
                        gasAmount: testCase.GetParameterValue<int>(gasAmount)
                    )
                );
            }
        }
        
        #endregion

        #region CheckIfBroadcastedAsync

        public virtual async Task CheckIfBroadcastedAsync__InvalidArgumentsPassed__ExceptionThrown()
        {
            const string txHash = nameof(txHash);
            
            var testCasesGenerator = new TestCasesGenerator();

            testCasesGenerator
                .RegisterHexStringParameter(txHash);
            
            foreach (var testCase in testCasesGenerator.GenerateInvalidCases())
            {
                await Assert.ThrowsExceptionAsync<ArgumentException>
                (
                    () => BlockchainService.CheckIfBroadcastedAsync
                    (
                        txHash: testCase.GetParameterValue<string>(txHash)
                    )
                );
            }
        }
        
        #endregion
        
        #region EstimateGasPriceAsync

        public virtual async Task EstimateGasPriceAsync__ValidArgumentsPassed__TransactionBuilt()
        {
            var actualResult = await BlockchainService.EstimateGasPriceAsync("0xEA674fdDe714fd979de3EdF0F56AA9716B898ec8", 2);

            Assert.IsTrue(actualResult > 0);
        }
        
        public virtual async Task EstimateGasPriceAsync__InvalidArgumentsPassed__ExceptionThrown()
        {
            const string to = nameof(to);
            const string amount = nameof(amount);
            
            var testCasesGenerator = new TestCasesGenerator();

            testCasesGenerator
                .RegisterAddressParameter(to)
                .RegisterParameter(amount, new []
                {
                    (-1, false),
                    (0, false),
                    (1, true)
                });
            
            foreach (var testCase in testCasesGenerator.GenerateInvalidCases())
            {
                await Assert.ThrowsExceptionAsync<ArgumentException>
                (
                    () => BlockchainService.EstimateGasPriceAsync
                    (
                        to: testCase.GetParameterValue<string>(to),
                        amount: testCase.GetParameterValue<int>(amount)
                    )
                );
            }
        }
        
        #endregion
        
        #region GetBalanceAsync
        
        public virtual async Task GetBalanceAsync__ValidArgumentsPassed_And_BlockExists__ValidBalanceReturned()
        {
            var expectedResult = BigInteger.Parse("20950042116000000000");
            var actualResult = await BlockchainService.GetBalanceAsync
            (
                address: "0x21012e8dCd6B6B175875afc1Ee1748F70835296b",
                blockNumber: await BlockchainService.GetLatestBlockNumberAsync()
            );
            
            Assert.AreEqual(expectedResult, actualResult);
        }
        
        public virtual async Task GetBalanceAsync__ValidArgumentsPassed_And_BlockDoesNotExist__ExceptionThrown()
        {
            await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>
            (
                () => BlockchainService.GetBalanceAsync
                (
                    address: "0x21012e8dCd6B6B175875afc1Ee1748F70835296b",
                    blockNumber: 9000000
                )
            );
        }

        public virtual async Task GetBalanceAsync__InvalidArguemtnsPassed__ExceptionThrown()
        {
            const string address = nameof(address);
            const string blockNumber = nameof(blockNumber);
            
            var testCasesGenerator = new TestCasesGenerator();

            testCasesGenerator
                .RegisterAddressParameter(address)
                .RegisterParameter(blockNumber, new []
                {
                    (-1, false),
                    (0, true)
                });
            
            foreach (var testCase in testCasesGenerator.GenerateInvalidCases())
            {
                await Assert.ThrowsExceptionAsync<ArgumentException>
                (
                    () => BlockchainService.GetBalanceAsync
                    (
                        address: testCase.GetParameterValue<string>(address),
                        blockNumber: testCase.GetParameterValue<int>(blockNumber)
                    )
                );
            }
        }
        
        #endregion
        
        #region GetBlockHashAsync

        public virtual async Task GetBlockHashAsync__ValidArgumentsPassed_And_BlockExists__ValidBlockHashReturned()
        {
            var actualResult = await BlockchainService.GetBlockHashAsync(1);
            
            Assert.AreEqual("0x41800b5c3f1717687d85fc9018faac0a6e90b39deaa0b99e7fe4fe796ddeb26a", actualResult);
        }
        
        public virtual async Task GetBlockHashAsync__ValidArgumentsPassed_And_BlockDoesNotExist__ExceptionThrown()
        {
            await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>
            (
                () => BlockchainService.GetBlockHashAsync(90000000)
            );
        }
        
        public virtual async Task GetBlockHashAsync__InvalidArgumentsPassed__ExceptionThrown()
        {
            const string blockNumber = nameof(blockNumber);
            
            var testCasesGenerator = new TestCasesGenerator();

            testCasesGenerator
                .RegisterParameter(blockNumber, new []
                {
                    (-1, false),
                    (0, true)
                });
            
            foreach (var testCase in testCasesGenerator.GenerateInvalidCases())
            {
                await Assert.ThrowsExceptionAsync<ArgumentException>
                (
                    () => BlockchainService.GetBlockHashAsync
                    (
                        blockNumber: testCase.GetParameterValue<int>(blockNumber)
                    )
                );
            }
        }
        
        #endregion
        
        #region GetCodeAsync

        public virtual async Task GetCodeAsync__AddressIsWallet__EmptyResultReturned()
        {
            var actualResult = await BlockchainService.GetCodeAsync("0xEA674fdDe714fd979de3EdF0F56AA9716B898ec8");
            
            Assert.AreEqual("0x", actualResult);
        }
        
        public virtual async Task GetCodeAsync__AddressIsContract__ValidCodeReturned()
        {
            var actualResult = await BlockchainService.GetCodeAsync("0xdb2574F51ADA6dA152b98BD6C7C692DA635C101A");
            
            Assert.AreEqual
            (
                "0x6060604052600436106101a05763ffffffff7c0100000000000000000000000000000000000000000000000" +
                "00000000060003504166306fdde0381146101a55780630714a4071461022f578063095ea7b314610260578063" +
                "18160ddd14610296578063188d0aae146102a957806323b872dd146102c55780632c4e591b146102ed5780633" +
                "13ce5671461030057806334825a23146103295780633bc93f5a1461034257806354fd4d501461036157806360" +
                "104cef146103745780636c11bcd31461038d5780636f32b2ac146103a3578063704dd019146103c257806370a" +
                "08231146103e157806383b5ff8b146104005780638da5cb5b1461041357806395d89b411461044257806395e1" +
                "6aa9146104555780639ca423b314610474578063a0712d6814610493578063a6f81668146104a9578063a9059" +
                "cbb14610509578063a91abdcb1461052b578063ae8b9d7b1461053e578063b2d138d21461055a578063bb7e79" +
                "1a1461056d578063c27382d91461058c578063dd62ed3e146105a6578063ec8ac4d8146105cb578063f2fde38" +
                "b146105df578063fb282f92146105fe575b600080fd5b34156101b057600080fd5b6101b8610617565b604051" +
                "60208082528190810183818151815260200191508051906020019080838360005b838110156101f4578082015" +
                "1838201526020016101dc565b50505050905090810190601f1680156102215780820380516001836020036101" +
                "000a031916815260200191505b509250505060405180910390f35b341561023a57600080fd5b61024e600160a" +
                "060020a03600435166106b5565b60405190815260200160405180910390f35b341561026b57600080fd5b6102" +
                "82600160a060020a03600435166024356106c7565b604051901515815260200160405180910390f35b3415610" +
                "2a157600080fd5b61024e610733565b6102c3600435602435600160a060020a036044351661073a565b005b34" +
                "156102d057600080fd5b610282600160a060020a0360043581169060243516604435610752565b34156102f85" +
                "7600080fd5b61024e610834565b341561030b57600080fd5b61031361083a565b60405160ff90911681526020" +
                "0160405180910390f35b341561033457600080fd5b6102c3600435602435610843565b341561034d57600080f" +
                "d5b61024e600160a060020a0360043516610945565b341561036c57600080fd5b610313610957565b34156103" +
                "7f57600080fd5b6102c3600435602435610965565b341561039857600080fd5b6102c3600435610b15565b341" +
                "56103ae57600080fd5b6102c3600160a060020a0360043516610bbb565b34156103cd57600080fd5b6102c360" +
                "ff60043581169060243516610be2565b34156103ec57600080fd5b61024e600160a060020a0360043516610c2" +
                "2565b341561040b57600080fd5b610313610c3d565b341561041e57600080fd5b610426610c46565b60405160" +
                "0160a060020a03909116815260200160405180910390f35b341561044d57600080fd5b6101b8610c55565b341" +
                "561046057600080fd5b61024e600160a060020a0360043516610cc0565b341561047f57600080fd5b61042660" +
                "0160a060020a0360043516610cd2565b341561049e57600080fd5b6102c3600435610ced565b34156104b4576" +
                "00080fd5b6104bf600435610d12565b604051600160a060020a03978816815260208101969096526040808701" +
                "95909552929095166060850152608084015260a083019390935260c082019290925260e001905180910390f35" +
                "b341561051457600080fd5b610282600160a060020a0360043516602435610d8a565b341561053657600080fd" +
                "5b610313610dd9565b341561054957600080fd5b6102c3600435602435604435610de7565b341561056557600" +
                "080fd5b610426610e0d565b341561057857600080fd5b61024e600160a060020a0360043516610e22565b6102" +
                "c3600435602435600160a060020a0360443516610e34565b34156105b157600080fd5b61024e600160a060020" +
                "a0360043581169060243516610e47565b6102c3600160a060020a0360043516610e72565b34156105ea576000" +
                "80fd5b6102c3600160a060020a0360043516610fcb565b341561060957600080fd5b6102c3600435602435611" +
                "066565b60048054600181600116156101000203166002900480601f0160208091040260200160405190810160" +
                "405280929190818152602001828054600181600116156101000203166002900480156106ad5780601f1061068" +
                "2576101008083540402835291602001916106ad565b820191906000526020600020905b815481529060010190" +
                "60200180831161069057829003601f168201915b505050505081565b600860205260009081526040902054815" +
                "65b600160a060020a033381166000818152600260209081526040808320948716808452949091528082208590" +
                "55909291907f8c5be1e5ebec7d5bd14f71427d1e84f3dd0314c0f7b2291e5b200ac8c7c3b9259085905190815" +
                "260200160405180910390a350600192915050565b6000545b90565b61074381610e72565b61074d8383610965" +
                "565b505050565b6000600160a060020a038316151561076957600080fd5b600160a060020a038416600090815" +
                "26001602052604090205482111561078e57600080fd5b600160a060020a038085166000908152600260209081" +
                "52604080832033909416835292905220548211156107c157600080fd5b6107cc848484611072565b600160a06" +
                "0020a0380851660009081526002602090815260408083203390941683529290522054610803908363ffffffff" +
                "61112c16565b600160a060020a038086166000908152600260209081526040808320339094168352929052205" +
                "55060019392505050565b600c5490565b60065460ff1681565b600080600c8481548110151561085557fe5b60" +
                "009182526020909120600790910201805490925033600160a060020a039081169116141561088357600080fd5" +
                "b6003820154600160a060020a03161561089b57600080fd5b506002810154600160a060020a03331660009081" +
                "5260016020526040902054819010156108c757600080fd5b6108d1308261113e565b60038201805473fffffff" +
                "fffffffffffffffffffffffffffffffff191633600160a060020a0316908117909155600483018490557f24b3" +
                "1e1dbf1732a12bb700d3d77e5fb96ff3b693374d9fa15ccbc03f4271507685856040519182526020820152604" +
                "0908101905180910390a250505050565b600a6020526000908152604090205481565b600654610100900460ff" +
                "1681565b61096d6117b8565b600160a060020a033316600090815260016020526040812054849010156109935" +
                "7600080fd5b60065460ff16600a0a8410156109a857600080fd5b6109b2308561113e565b60e0604051908101" +
                "6040528033600160a060020a031681526020018481526020018581526020016000600160a060020a031681526" +
                "020016000815260200160008152602001600081525091506001600c8054806001018281610a14919061180856" +
                "5b600092835260209092208591600702018151815473ffffffffffffffffffffffffffffffffffffffff19166" +
                "00160a060020a0391909116178155602082015181600101556040820151816002015560608201516003820180" +
                "5473ffffffffffffffffffffffffffffffffffffffff1916600160a060020a039290921691909117905560808" +
                "20151816004015560a0820151816005015560c08201516006909101555003905063ffffffff81168114610acb" +
                "57600080fd5b33600160a060020a03167f27bd49aa3baf4aa9d565998bd41f40d6ff8cb737a8b4935d9ead830" +
                "220fb73bc828560405191825260208201526040908101905180910390a250505050565b6000610b2133836111" +
                "f8565b5033600160a060020a03166000818152600860205260409081902080546113888504908101909155919" +
                "07f47bc8b296a721ead4262b15df1a3123f2c4eed6afa70823038a3c34d60d478f29083908590519182526020" +
                "8201526040908101905180910390a2600160a060020a03331681156108fc02826040516000604051808303818" +
                "58888f193505050501515610bb757600080fd5b5050565b60035433600160a060020a03908116911614610bd6" +
                "57600080fd5b610bdf8161127d565b50565b60035433600160a060020a03908116911614610bfd57600080fd5" +
                "b600d805460ff9283166101000261ff00199490931660ff199091161792909216179055565b600160a060020a" +
                "031660009081526001602052604090205490565b600d5460ff1681565b600354600160a060020a031681565b6" +
                "0058054600181600116156101000203166002900480601f016020809104026020016040519081016040528092" +
                "9190818152602001828054600181600116156101000203166002900480156106ad5780601f106106825761010" +
                "08083540402835291602001916106ad565b60076020526000908152604090205481565b600960205260009081" +
                "526040902054600160a060020a031681565b60035433600160a060020a03908116911614610d0857600080fd5" +
                "b610bdf338261133b565b600080600080600080600080600c89815481101515610d2d57fe5b60009182526020" +
                "9091206007909102018054600182015460028301546003840154600485015460058601546006870154600160a" +
                "060020a039687169f50949d50929b50931698509196509094509250905050919395979092949650565b600060" +
                "0160a060020a0383161515610da157600080fd5b600160a060020a03331660009081526001602052604090205" +
                "4821115610dc657600080fd5b610dd0838361113e565b50600192915050565b600d54610100900460ff168156" +
                "5b60035433600160a060020a03908116911614610e0257600080fd5b61074d8383836113aa565b60065462010" +
                "0009004600160a060020a031681565b600b6020526000908152604090205481565b610e3d81610e72565b6107" +
                "4d8383610843565b600160a060020a03918216600090815260026020908152604080832093909416825291909" +
                "152205490565b6000610e863461138863ffffffff61177316565b60065490915060ff16600a0a811015610e9e" +
                "57600080fd5b610ea8338261133b565b600160a060020a0333811660009081526009602052604090205416158" +
                "015610ee2575033600160a060020a031682600160a060020a031614155b15610f5457600160a060020a038216" +
                "1515610f0657600354600160a060020a031691505b600160a060020a033381166000908152600960209081526" +
                "040808320805494871673ffffffffffffffffffffffffffffffffffffffff1990951685179055928252600a90" +
                "5220805460010190555b600160a060020a0333166000818152600760205260409081902080543490810190915" +
                "57ff9d280e51917e2a2843caebb7e3365656911aa5b307986d48ff5c66958997c169184908690519283526020" +
                "830191909152600160a060020a03166040808301919091526060909101905180910390a25050565b600354336" +
                "00160a060020a03908116911614610fe657600080fd5b600160a060020a0381161515610ffb57600080fd5b60" +
                "0354600160a060020a0380831691167f8be0079c531659141344cd1fd0a4f28419497f9722a3daafe3b4186f6" +
                "b6457e060405160405180910390a36003805473ffffffffffffffffffffffffffffffffffffffff1916600160" +
                "a060020a0392909216919091179055565b610bb7828260006113aa565b600160a060020a03831660009081526" +
                "001602052604090205461109b908263ffffffff61112c16565b600160a060020a038085166000908152600160" +
                "2052604080822093909355908416815220546110d0908263ffffffff6117a916565b600160a060020a0380841" +
                "6600081815260016020526040908190209390935591908516907fddf252ad1be2c89b69c2b068fc378daa952b" +
                "a7f163c4a11628f55a4df523b3ef9084905190815260200160405180910390a3505050565b600082821115611" +
                "13857fe5b50900390565b600160a060020a033316600090815260016020526040902054611167908263ffffff" +
                "ff61112c16565b600160a060020a0333811660009081526001602052604080822093909355908416815220546" +
                "1119c908263ffffffff6117a916565b600160a060020a03808416600081815260016020526040908190209390" +
                "93559133909116907fddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef9084905" +
                "190815260200160405180910390a35050565b600160a060020a03821660009081526001602052604090205461" +
                "1221908263ffffffff61112c16565b600160a060020a038316600081815260016020526040808220939093558" +
                "0548490038155917fddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef90849051" +
                "90815260200160405180910390a35050565b80600160a060020a03811663cb4c8ee4600060405160200152604" +
                "0518163ffffffff167c0100000000000000000000000000000000000000000000000000000000028152600401" +
                "602060405180830381600087803b15156112dd57600080fd5b6102c65a03f115156112ee57600080fd5b50505" +
                "060405180519050151561130357600080fd5b60068054600160a060020a03909216620100000275ffffffffff" +
                "ffffffffffffffffffffffffffffff00001990921691909117905550565b60005461134e908263ffffffff611" +
                "7a916565b6000908155600160a060020a038316808252600160205260408083208054850190559091907fddf2" +
                "52ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef90849051908152602001604051809" +
                "10390a35050565b600080600080600080600080600c8b8154811015156113c557fe5b60009182526020909120" +
                "6007909102016003810154909850600160a060020a031615156113f157600080fd5b600280890154600160a06" +
                "0020a03301660009081526001602052604090205490985090880290101561142257600080fd5b600d54606460" +
                "ff8083168a026002908102839004995061010090930416890282020495508781028790038690038a900394508" +
                "904870392508760050154600014801561146d57508915155b151561147857600080fd5b600588018a90556006" +
                "54600189015460048a015462010000909204600160a060020a031691639cda5a12918d9160006040516020015" +
                "26040517c010000000000000000000000000000000000000000000000000000000063ffffffff861602815260" +
                "0481019390935260248301919091526044820152606401602060405180830381600087803b151561150a57600" +
                "080fd5b6102c65a03f1151561151b57600080fd5b505050604051805191505060098106151561157457600083" +
                "101561153e57600080fd5b8754611555903090600160a060020a031685611072565b600388015461156f90309" +
                "0600160a060020a031685611072565b6116e0565b601181061580611584575080600c145b1561162957600084" +
                "101561159757600080fd5b8754600160a060020a0390811660009081526009602052604090205416915081151" +
                "56115cc57600354600160a060020a031691505b87546115e3903090600160a060020a031686611072565b6115" +
                "ee308387611072565b600354611606903090600160a060020a031688611072565b600160a060020a038216600" +
                "0908152600b602052604090208054860190556116e0565b600a810615806116395750806021145b156116e057" +
                "600084101561164c57600080fd5b6003880154600160a060020a0390811660009081526009602052604090205" +
                "416915081151561168457600354600160a060020a031691505b600388015461169e903090600160a060020a03" +
                "1686611072565b6116a9308387611072565b6003546116c1903090600160a060020a031688611072565b60016" +
                "0a060020a0382166000908152600b602052604090208054860190555b60008911156116f4576116f430338b61" +
                "1072565b60068801819055600160a060020a0333167f48fc50c127b4354a02f63db205d57cea3f2d3bbbabc3c" +
                "f6c9c848ac0f46784078c83858d6040518085815260200184815260200183600160a060020a0316600160a060" +
                "020a0316815260200182815260200194505050505060405180910390a25050505050505050505050565b60008" +
                "083151561178657600091506117a2565b5082820282848281151561179657fe5b041461179e57fe5b8091505b" +
                "5092915050565b60008282018381101561179e57fe5b60e0604051908101604052806000600160a060020a031" +
                "6815260200160008152602001600081526020016000600160a060020a03168152602001600081526020016000" +
                "8152602001600081525090565b81548183558181151161074d5760008381526020902061074d9161073791600" +
                "79182028101918502015b8082111561188e57805473ffffffffffffffffffffffffffffffffffffffff199081" +
                "16825560006001830181905560028301819055600383018054909216909155600482018190556005820181905" +
                "56006820155600701611832565b50905600a165627a7a72305820e0598a17e3bd6cac1a1b55f1d71c1feb1699" +
                "bed5706e8c7acc69b11fbb8470e80029",
                actualResult
            );
        }
        
        public virtual async Task GetCodeAsync__InvalidArgumentsPassed__ExceptionThrown()
        {
            const string address = nameof(address);
            
            var testCasesGenerator = new TestCasesGenerator();

            testCasesGenerator
                .RegisterAddressParameter(address);
            
            foreach (var testCase in testCasesGenerator.GenerateInvalidCases())
            {
                await Assert.ThrowsExceptionAsync<ArgumentException>
                (
                    () => BlockchainService.GetCodeAsync
                    (
                        address: testCase.GetParameterValue<string>(address)
                    )
                );
            }
        }

        #endregion
        
        #region GetLatestBlockNumberAsync

        public virtual async Task GetLatestBlockNumberAsync__BlockNumberReturned()
        {
            var actualResult = await BlockchainService.GetLatestBlockNumberAsync();
            
            Assert.IsTrue(actualResult > 3287121);
        }
        
        #endregion

        #region GetTimestampAsync

        public virtual async Task GetTimestampAsync__ValidArgumentsPassed_And_BlockExists__ValidTimestampReturned()
        {
            var actualResult = await BlockchainService.GetTimestampAsync(3287121);
            
            Assert.AreEqual(1526980142, actualResult);
        }
        
        public virtual async Task GetTimestampAsync__ValidArgumentsPassed_And_BlockDoesNotExist__ExceptionThrown()
        {
            await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>
            (
                () => BlockchainService.GetTimestampAsync(90000000)
            );
        }
        
        public virtual async Task GetTimestampAsync__InvalidArgumentsPassed__ExceptionThrown()
        {
            const string blockNumber = nameof(blockNumber);
            
            var testCasesGenerator = new TestCasesGenerator();

            testCasesGenerator
                .RegisterParameter(blockNumber, new []
                {
                    (-1, false),
                    (0, true)
                });
            
            foreach (var testCase in testCasesGenerator.GenerateInvalidCases())
            {
                await Assert.ThrowsExceptionAsync<ArgumentException>
                (
                    () => BlockchainService.GetTimestampAsync
                    (
                        blockNumber: testCase.GetParameterValue<int>(blockNumber)
                    )
                );
            }
        }

        #endregion
        
        #region GetTransactionHash

        public virtual void GetTransactionHash__ValidTransactionDataPassed__ValidTransactionHashReturned()
        {
            var actualResult = BlockchainService.GetTransactionHash
            (
                "0xf85d80020294ea674fdde714fd979de3edf0f56aa9716b898ec802801ba0710854a16dbcad522f92a7f9da5b0cf35f" +
                "41e41bd9959863042366fbf0e26fdca0557a2a0e4e3b01354de5633c4574c7c4d0749f3fdc5f24c0c9fc92f08d47a047"
            );
            
            Assert.AreEqual("0x54ff9c2645cbf4176b94871c07a9fed2f93ca179d99aaa5718a52815d1a40326", actualResult);
        }
        
        public virtual void GetTransactionHash__InvalidArgumentsPassed__ExceptionThrown()
        {
            const string signedTxData = nameof(signedTxData);
            
            var testCasesGenerator = new TestCasesGenerator();

            testCasesGenerator
                .RegisterHexStringParameter(signedTxData);
            
            foreach (var testCase in testCasesGenerator.GenerateInvalidCases())
            {
                Assert.ThrowsException<ArgumentException>
                (
                    () => BlockchainService.GetTransactionHash
                    (
                        signedTxData: testCase.GetParameterValue<string>(signedTxData)
                    )
                );
            }
        }
        
        #endregion
        
        #region GetTransactionsAsync

        public virtual async Task GetTransactionsAsync__BlockIsNotEmpty__ValidTransactionListReturned()
        {
            var actualResult = (await BlockchainService.GetTransactionsAsync(2805104)).ToList();
            
            Assert.AreEqual(5, actualResult.Count);
            Assert.AreEqual(0, actualResult.Count(x => x.TransactionFailed));
            Assert.AreEqual(1, actualResult.Count(x => x.TransactionIsInternal));
            
            Assert.AreEqual("0xf4937fC389AdcaFA913E6b7b7C32666daFE31B7D", actualResult[0].FromAddress);
            Assert.AreEqual("0xF4E6e6fa97E10ddc057c94F501B94C1d24EF85Aa", actualResult[0].ToAddress);
            Assert.AreEqual(BigInteger.Parse("1001110000000000"), actualResult[0].TransactionAmount);
            Assert.IsNull(actualResult[0].TransactionError);
            Assert.IsFalse(actualResult[0].TransactionFailed);
            Assert.AreEqual("0x6b5420430fa7d6773ac26c95eac40369623efb55099f5868f67bb63276b18ea0", actualResult[0].TransactionHash);
            Assert.AreEqual(BigInteger.Parse("24"), actualResult[0].TransactionIndex);
            Assert.IsTrue(actualResult[0].TransactionIsInternal);
            Assert.AreEqual(BigInteger.Parse("1520643052"), actualResult[0].TransactionTimestamp);
            
            Assert.AreEqual("0x81b7E08F65Bdf5648606c89998A9CC8164397647", actualResult[1].FromAddress);
            Assert.AreEqual("0x3DC62c0ceb148Abd425cdF5bFcC1b6Ff77f462ea", actualResult[1].ToAddress);
            Assert.AreEqual(BigInteger.Parse("1000000000000000000"), actualResult[1].TransactionAmount);
            Assert.IsNull(actualResult[1].TransactionError);
            Assert.IsFalse(actualResult[1].TransactionFailed);
            Assert.AreEqual("0xda95ac4fdb35aa2b9d56bf1d859ca70b67ff82a4100e7bf8209a07e4d2c87dbf", actualResult[1].TransactionHash);
            Assert.AreEqual(BigInteger.Parse("25"), actualResult[1].TransactionIndex);
            Assert.IsFalse(actualResult[1].TransactionIsInternal);
            Assert.AreEqual(BigInteger.Parse("1520643052"), actualResult[1].TransactionTimestamp);
        }
        
        public virtual async Task GetTransactionsAsync__BlockIsEmpty__EmptyTransactionListReturned()
        {
            var actualResult = await BlockchainService.GetTransactionsAsync(1000000);
            
            Assert.AreEqual(0, actualResult.Count());
        }
        
        public virtual async Task GetTransactionsAsync__InvalidArgumentsPassed__ExceptionThrown()
        {
            const string blockNumber = nameof(blockNumber);
            
            var testCasesGenerator = new TestCasesGenerator();

            testCasesGenerator
                .RegisterParameter(blockNumber, new []
                {
                    (-1, false),
                    (0, true)
                });
            
            foreach (var testCase in testCasesGenerator.GenerateInvalidCases())
            {
                await Assert.ThrowsExceptionAsync<ArgumentException>
                (
                    () => BlockchainService.GetTransactionsAsync
                    (
                        blockNumber: testCase.GetParameterValue<int>(blockNumber)
                    )
                );
            }
        }
        
        #endregion
        
        #region GetTransactionSigner
        
        public virtual void GetTransactionSigner__ValidArgumentsPassed__ValidTransactionSignerReturned()
        {
            const string expectedResult = "0x312eBa96F6c89a49E8A7C920d9e981207FeCC8db";

            var actualResult = BlockchainService.GetTransactionSigner("0xf85d80020294ea674fdde714fd979de3edf0f56aa9716b898ec802801ca043d278a942c00b03c700c49d72d4f156fa672f9bf00f133393573236b7f5d008a00da8f47050ddc2d83dc858be2e2497e214fb05681d0f90963b21a30a85abb919");
            
            Assert.AreEqual(expectedResult, actualResult);
        }
        
        public virtual void GetTransactionSigner__InvalidArgumentsPassed__ExceptionThrown()
        {
            const string signedTxData = nameof(signedTxData);
            
            var testCasesGenerator = new TestCasesGenerator();

            testCasesGenerator
                .RegisterHexStringParameter(signedTxData);
            
            foreach (var testCase in testCasesGenerator.GenerateInvalidCases())
            {
                Assert.ThrowsException<ArgumentException>
                (
                    () => BlockchainService.GetTransactionSigner
                    (
                        signedTxData: testCase.GetParameterValue<string>(signedTxData)
                    )
                );
            }
        }
        
        public virtual void GetTransactionSigner__TransactionHasNotBeenSigned__ExceptionThrown()
        {
            Exception exception = null;
            
            try
            {
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                BlockchainService.GetTransactionSigner("0xdd80020294ea674fdde714fd979de3edf0f56aa9716b898ec80280808080");
            }
            catch (Exception e)
            {
                exception = e;
            }
            
            Assert.IsNotNull(exception);
            Assert.AreEqual("Signature not initiated or calculatated", exception.Message);
        }
        
        #endregion
        
        #region SendRawTransactionAsync

        public virtual async Task SendRawTransactionAsync__InvalidArgumentsPassed__ExceptionThrown()
        {
            const string signedTxData = nameof(signedTxData);
            
            var testCasesGenerator = new TestCasesGenerator();

            testCasesGenerator
                .RegisterHexStringParameter(signedTxData);
            
            foreach (var testCase in testCasesGenerator.GenerateInvalidCases())
            {
                await Assert.ThrowsExceptionAsync<ArgumentException>
                (
                    () => BlockchainService.SendRawTransactionAsync
                    (
                        signedTxData: testCase.GetParameterValue<string>(signedTxData)
                    )
                );
            }
        }
        
        #endregion
        
        #region TryGetTransactionErrorAsync

        public virtual async Task TryGetTransactionErrorAsync__TransactionIsFailed__ValidErrorListReturned()
        {
            var actualResult = (await BlockchainService.TryGetTransactionErrorsAsync
            (
                "0x5c56e5387c39a22f6edb60794df7e7c95afb72c181a4a8ea45dacd428a1e9c0f"
            ))?.ToList();
            
            Assert.IsNotNull(actualResult);
            Assert.AreEqual(1, actualResult.Count);
            Assert.AreEqual("Reverted", actualResult[0]);
        }
        
        public virtual async Task TryGetTransactionErrorAsync__TransactionIsSuccessful__EmptyErrorListReturned()
        {
            var actualResult = (await BlockchainService.TryGetTransactionErrorsAsync
            (
                "0x257fdd029be3bac9a702194d8b3997254b5d48a8b47939c243a0d1cfeaf140e0"
            ))?.ToList();
            
            Assert.IsNotNull(actualResult);
            Assert.AreEqual(0, actualResult.Count);
        }
        
        public virtual async Task TryGetTransactionErrorAsync__TransactionDoesNotExist__NullReturned()
        {
            var actualResult = await BlockchainService.TryGetTransactionErrorsAsync
            (
                "0x257fdd029be3bac9a702194d8b3997254b5d48a8b47939c243a0d1cfeaf140e1"
            );
            
            Assert.IsNull(actualResult);
        }
        
        public virtual async Task TryGetTransactionErrorAsync__InvalidArgumentsPassed__ExceptionThrown()
        {
            const string txHash = nameof(txHash);
            
            var testCasesGenerator = new TestCasesGenerator();

            testCasesGenerator
                .RegisterHexStringParameter(txHash);
            
            foreach (var testCase in testCasesGenerator.GenerateInvalidCases())
            {
                await Assert.ThrowsExceptionAsync<ArgumentException>
                (
                    () => BlockchainService.TryGetTransactionErrorsAsync
                    (
                        txHash: testCase.GetParameterValue<string>(txHash)
                    )
                );
            }
        }
        
        #endregion
        
        #region TryGetTransactionReceiptAsync

        public virtual async Task TryGetTransactionReceiptAsync__ExistingTxHashPassed__ValidTransactionReceiptReturned()
        {
            var actualResult = await BlockchainService.TryGetTransactionReceiptAsync
            (
                "0x24f5a3219d74aee118d772d4d562ea4dd14c2c87e896ef2f9b4878956827d287"
            );
            
            Assert.IsNotNull(actualResult);
            Assert.AreEqual("0xb140c05ee0bf1a1632733c8e5cadf228ad433e68a789bd378259a925e08a1a1c", actualResult.BlockHash);
            Assert.AreEqual(BigInteger.Parse("3287198"), actualResult.BlockNumber);
            Assert.AreEqual(null, actualResult.ContractAddress);
            Assert.AreEqual(BigInteger.Parse("4691726"), actualResult.CumulativeGasUsed);
            Assert.AreEqual(BigInteger.Parse("25624"), actualResult.GasUsed);
            Assert.AreEqual(BigInteger.Parse("1"), actualResult.Status);
            Assert.AreEqual("0x24f5a3219d74aee118d772d4d562ea4dd14c2c87e896ef2f9b4878956827d287", actualResult.TransactionHash);
            Assert.AreEqual(BigInteger.Parse("36"), actualResult.TransactionIndex);
        }

        public virtual async Task TryGetTransactionReceiptAsync__NonExistingTxHashPassed__NullReturned()
        {
            var actualResult = await BlockchainService.TryGetTransactionReceiptAsync
            (
                "0x24f5a3219d74aee118d772d4d562ea4dd14c2c87e896ef2f9b4878956827d000"
            );
            
            Assert.IsNull(actualResult);
        }
        
        public virtual async Task TryGetTransactionReceiptAsync__InvalidArgumentsPassed__ExceptionThrown()
        {
            const string txHash = nameof(txHash);
            
            var testCasesGenerator = new TestCasesGenerator();

            testCasesGenerator
                .RegisterHexStringParameter(txHash);
            
            foreach (var testCase in testCasesGenerator.GenerateInvalidCases())
            {
                await Assert.ThrowsExceptionAsync<ArgumentException>
                (
                    () => BlockchainService.TryGetTransactionReceiptAsync
                    (
                        txHash: testCase.GetParameterValue<string>(txHash)
                    )
                );
            }
        }
        
        #endregion
        
        #region UnsignTransactionAsync

        public virtual async Task UnsignTransactionAsync__ValidArgumentsPassed__ValidUnsignedTxDataReturned()
        {
            var actualResult = await BlockchainService.UnsignTransactionAsync
            (
                "0xf85d80020294ea674fdde714fd979de3edf0f56aa9716b898ec802801ba0710854a16dbcad522f92a7f9da5b0cf35f" +
                "41e41bd9959863042366fbf0e26fdca0557a2a0e4e3b01354de5633c4574c7c4d0749f3fdc5f24c0c9fc92f08d47a047"
            );
            
            Assert.AreEqual("0x95c40102c40102c40102c40100d92a307845413637346664446537313466643937396465334564463046353641413937313642383938656338", actualResult);
        }

        public virtual async Task UnsignTransactionAsync__InvalidArgumentsPassed__ExceptionThrown()
        {
            const string signedTxData = nameof(signedTxData);
            
            var testCasesGenerator = new TestCasesGenerator();

            testCasesGenerator
                .RegisterHexStringParameter(signedTxData);
            
            foreach (var testCase in testCasesGenerator.GenerateInvalidCases())
            {
                await Assert.ThrowsExceptionAsync<ArgumentException>
                (
                    () => BlockchainService.UnsignTransactionAsync
                    (
                        signedTxData: testCase.GetParameterValue<string>(signedTxData)
                    )
                );
            }
        }
        
        public virtual async Task UnsignTransactionAsync__TransactionHasData__ExceptionThrown()
        {
            await Assert.ThrowsExceptionAsync<NotSupportedException>
            (
                () => BlockchainService.UnsignTransactionAsync
                (
                    "0xf85e80020294ea674fdde714fd979de3edf0f56aa9716b898ec80281ae1ca082fbbb1c1402f911aa430fc2b64349a0f" +
                    "cc18fa9da7aaa540e0afb4bf7aa05a6a00323800eb52d8b80af030548214d2a950b584fa6cc2d4de385dfe665babaf3b0"
                )
            );
        }
        
        public virtual async Task UnsignTransactionAsync__TransactionHasNotBeenSigned__EWxceptionThrown()
        {
            Exception exception = null;
            
            try
            {
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                await BlockchainService.UnsignTransactionAsync("0xdd80020294ea674fdde714fd979de3edf0f56aa9716b898ec80280808080");
            }
            catch (Exception e)
            {
                exception = e;
            }
            
            Assert.IsNotNull(exception);
            Assert.AreEqual("Signature not initiated or calculatated", exception.Message);
        }
        
        
        #endregion
    }
}
