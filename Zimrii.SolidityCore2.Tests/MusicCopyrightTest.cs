using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Hex.HexTypes;

namespace Zimrii.SolidityCore2.Tests
{
    [TestClass]
    public class MusicCopyrightTest : NethereumTestsBase
    {

        public MusicCopyrightTest() : base(new[] { "AccessRestriction", "ArtistRoyalties" })
        {
        }

        [TestMethod]
        public async Task MusicCopyrightSolTestCore3()
        {
            await Setup(true);

            var contractAddress = Receipts["MusicCopyright"].ContractAddress;
            var contract = Web3.Eth.GetContract(Abi["MusicCopyright"], contractAddress);
            var multiply = contract.GetFunction("multiply");
            var setCopyright = contract.GetFunction("setCopyright");
            var getCopyrightId = contract.GetFunction("getCopyrightId");

            var res1 = await multiply.CallAsync<int>(7);

            var unlockResult =
                await Web3.Personal.UnlockAccount.SendRequestAsync(AccountAddress, PassPhrase, 120);
            unlockResult.Should().BeTrue();

            var transactionHash = await setCopyright.SendTransactionAsync(AccountAddress, new HexBigInteger(210000), null, null,
                "61BF375C77BC4C089DCAD2AC4935E600", "6345D89C498A4EB79DB670F46F25EF00", "6B2M2Y8AsgTpgAmY7PhCfg==");

            var receipt1 = await MineAndGetReceiptAsync(Web3, transactionHash, true);

            var res3 = await getCopyrightId.CallAsync<string>("61BF375C77BC4C089DCAD2AC4935E600");
            res3.Should().Be("6345D89C498A4EB79DB670F46F25EF00");

        }

        [TestMethod]
        public async Task MusicCopyrightSolTestCore4()
        {
            await Setup(true);

            var contractAddress = Receipts["ArtistRoyalties"].ContractAddress;
            var contract = Web3.Eth.GetContract(Abi["ArtistRoyalties"], contractAddress);
            var multiply = contract.GetFunction("multiply");
            var setRoyalties = contract.GetFunction("setRoyalties");
            var getRoyaltiesHash = contract.GetFunction("getRoyaltiesHash");

            var res1 = await multiply.CallAsync<int>(7);

            var unlockResult =
                await Web3.Personal.UnlockAccount.SendRequestAsync(AccountAddress, PassPhrase, 120);
            unlockResult.Should().BeTrue();

            var transactionHash = await setRoyalties.SendTransactionAsync(AccountAddress, new HexBigInteger(210000), null, null,
                "61BF375C77BC4C089DCAD2AC4935E600", "6B2M2Y8AsgTpgAmY7PhCfg==");

            var receipt1 = await MineAndGetReceiptAsync(Web3, transactionHash, true);

            var res3 = await getRoyaltiesHash.CallAsync<string>("61BF375C77BC4C089DCAD2AC4935E600");
            res3.Should().Be("6B2M2Y8AsgTpgAmY7PhCfg==");

        }

        [TestMethod]
        public async Task MusicCopyrightSolTestCore2()
        {
            await Setup(true);

            var contractAddress = Receipts["MusicCopyright"].ContractAddress;
            //var contractAddress = "0x936069329eca4e962447a4816197e50017a4753c";

            var contract = Web3.Eth.GetContract(Abi["MusicCopyright"], contractAddress);

            var addCopyrightEvent = contract.GetEvent("SetCopyright");
            var filterAll = await addCopyrightEvent.CreateFilterAsync();
            var filterMusicId = await addCopyrightEvent.CreateFilterAsync("61BF375C77BC4C089DCAD2AC4935E600");

            #region Filter Error
            // error when filter strings
            /*
               Nethereum.JsonRpc.Client.RpcResponseException : invalid topic(s)
               at Nethereum.JsonRpc.Client.RpcRequestResponseHandler`1.<SendRequestAsync>d__7.MoveNext()
            --- End of stack trace from previous location where exception was thrown ---
               at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
               at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
               at System.Runtime.CompilerServices.TaskAwaiter`1.GetResult()
               at Contracts.Tests.SolidityTest.<MusicCopyrightTest>d__6.MoveNext()
            --- End of stack trace from previous location where exception was thrown ---
               at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
               at NUnit.Framework.Internal.AsyncInvocationRegion.AsyncTaskInvocationRegion.WaitForPendingOperationsToComplete(Object invocationResult)
               at NUnit.Framework.Internal.Commands.TestMethodCommand.RunAsyncTestMethod(TestExecutionContext context)
            */

            #endregion

            var setCopyright = contract.GetFunction("setCopyright");
            var getCopyrightId = contract.GetFunction("getCopyrightId");
            var getCopyrightHash = contract.GetFunction("getCopyrightHash");
            var setCopyrightEndpointResourceRoot = contract.GetFunction("setCopyrightEndpointResourceRoot");
            var getCopyrightResourceEndpoint = contract.GetFunction("getCopyrightResourceEndpoint");

            var unlockResult =
                await Web3.Personal.UnlockAccount.SendRequestAsync(AccountAddress, PassPhrase, 120);
            unlockResult.Should().BeTrue();

            var transactionHash = await setCopyright.SendTransactionAsync(AccountAddress, new HexBigInteger(210000), null, null,
                "61BF375C77BC4C089DCAD2AC4935E600", "6345D89C498A4EB79DB670F46F25EF00", "6B2M2Y8AsgTpgAmY7PhCfg==");

            var receipt1 = await MineAndGetReceiptAsync(Web3, transactionHash, true);

            transactionHash = await setCopyrightEndpointResourceRoot.SendTransactionAsync(AccountAddress, new HexBigInteger(210000), null, null,
                @"http:\\api.zimrii\");

            var receipt2 = await MineAndGetReceiptAsync(Web3, transactionHash, true);

            ////var debuginfo = await Web3.DebugGeth.TraceTransaction.SendRequestAsync(transactionHash,
            ////    new TraceTransactionOptions { DisableMemory = false, DisableStorage = false, DisableStack = false });

            var log = await addCopyrightEvent.GetFilterChanges<AddCopyrightEvent>(filterAll);
            log.Count.Should().Be(1);

            var logMusicId = await addCopyrightEvent.GetFilterChanges<AddCopyrightEvent>(filterMusicId);
            logMusicId.Count.Should().Be(1);

            var res3 = await getCopyrightId.CallAsync<string>("61BF375C77BC4C089DCAD2AC4935E600");
            res3.Should().Be("6345D89C498A4EB79DB670F46F25EF00");

            var res4 = await getCopyrightResourceEndpoint.CallAsync<string>("61BF375C77BC4C089DCAD2AC4935E600");
            res4.Should().Be(@"http:\\api.zimrii\6345D89C498A4EB79DB670F46F25EF00");

            var res5 = await getCopyrightHash.CallAsync<string>("61BF375C77BC4C089DCAD2AC4935E600");
            res5.Should().Be("6B2M2Y8AsgTpgAmY7PhCfg==");

        }
    }



    public class AddCopyrightEvent
    {
        [Parameter("bytes32", "musicId", 1, true)]
        public string MusicId { get; set; }

        [Parameter("bytes32", "copyrightId", 2, false)]
        public string CopyrightId { get; set; }
    }
}
