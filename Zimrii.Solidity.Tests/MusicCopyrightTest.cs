using System.Threading.Tasks;
using FluentAssertions;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.DebugGeth.DTOs;
using Xunit;

namespace Zimrii.Solidity.Tests
{
    public class MusicCopyrightTest : NethereumTestsBase
    {

        public MusicCopyrightTest() : base(new[] { "AccessRestriction", "MusicCopyright" })
        {
        }

        [Fact]
        public async Task MusicCopyrightSolTest()
        {
            await Setup(true);

            var contractAddress = Receipts["MusicCopyright"].ContractAddress;
            var contract = Web3.Eth.GetContract(Abi["MusicCopyright"], contractAddress);

            var addCopyrightEvent = contract.GetEvent("SetCopyright");
            var filterAll = await addCopyrightEvent.CreateFilterAsync();
            var filterMusicId = await addCopyrightEvent.CreateFilterAsync("51BF375C77BC4C089DCAD2AC4935E600");

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

            var transactionHash = await setCopyright.SendTransactionAsync(AccountAddress, 
                new HexBigInteger(2000000), new HexBigInteger(120), 
                "51BF375C77BC4C089DCAD2AC4935E600", "3345D89C498A4EB79DB670F46F25EF00", "1B2M2Y8AsgTpgAmY7PhCfg==");

            var receipt1 = await MineAndGetReceiptAsync(Web3, transactionHash, true);

            transactionHash = await setCopyrightEndpointResourceRoot.SendTransactionAsync(AccountAddress,
                new HexBigInteger(2000000), new HexBigInteger(120), @"http:\\api.zimrii\");

            var receipt2 = await MineAndGetReceiptAsync(Web3, transactionHash, true);

            var debuginfo = await Web3.DebugGeth.TraceTransaction.SendRequestAsync(transactionHash,
                new TraceTransactionOptions { DisableMemory = false, DisableStorage = false, DisableStack = false });

            var log = await addCopyrightEvent.GetFilterChanges<AddCopyrightEvent>(filterAll);
            log.Count.Should().Be(1);

            var logMusicId = await addCopyrightEvent.GetFilterChanges<AddCopyrightEvent>(filterMusicId);
            logMusicId.Count.Should().Be(1);

            var res3 = await getCopyrightId.CallAsync<string>("51BF375C77BC4C089DCAD2AC4935E600");
            res3.Should().Be("3345D89C498A4EB79DB670F46F25EF00");

            var res4 = await getCopyrightResourceEndpoint.CallAsync<string>("51BF375C77BC4C089DCAD2AC4935E600");
            res4.Should().Be(@"http:\\api.zimrii\3345D89C498A4EB79DB670F46F25EF00");

            var res5 = await getCopyrightHash.CallAsync<string>("51BF375C77BC4C089DCAD2AC4935E600");
            res5.Should().Be("1B2M2Y8AsgTpgAmY7PhCfg==");

        }

        [Fact]
        public async Task MusicCopyrightRinkebyTest()
        {
            ReadContractsDetails();

            AccountAddress = "";
            PassPhrase = "";

            var contractAddress = "0x3eda8064f0459a6e8566e261067bc3e61a5ef2be";
            var contract = Web3.Eth.GetContract(Abi["MusicCopyright"], contractAddress);

            var addCopyrightEvent = contract.GetEvent("SetCopyright");
            var filterAll = await addCopyrightEvent.CreateFilterAsync();
            var filterMusicId = await addCopyrightEvent.CreateFilterAsync("51BF375C77BC4C089DCAD2AC4935E600");

            var setCopyright = contract.GetFunction("setCopyright");
            var getCopyrightId = contract.GetFunction("getCopyrightId");
            var getCopyrightHash = contract.GetFunction("getCopyrightHash");
            var setCopyrightEndpointResourceRoot = contract.GetFunction("setCopyrightEndpointResourceRoot");
            var getCopyrightResourceEndpoint = contract.GetFunction("getCopyrightResourceEndpoint");

            var unlockResult =
                await Web3.Personal.UnlockAccount.SendRequestAsync(AccountAddress, PassPhrase, 120);
            unlockResult.Should().BeTrue();

            var transactionHash = await setCopyright.SendTransactionAsync(AccountAddress,
                new HexBigInteger(2000000), new HexBigInteger(120),
                "51BF375C77BC4C089DCAD2AC4935E600", "3345D89C498A4EB79DB670F46F25EF00", "1B2M2Y8AsgTpgAmY7PhCfg==");

            var receipt1 = await MineAndGetReceiptAsync(Web3, transactionHash, false);

            transactionHash = await setCopyrightEndpointResourceRoot.SendTransactionAsync(AccountAddress,
                new HexBigInteger(2000000), new HexBigInteger(120), @"http:\\api.zimrii\");

            var receipt2 = await MineAndGetReceiptAsync(Web3, transactionHash, false);

            var debuginfo = await Web3.DebugGeth.TraceTransaction.SendRequestAsync(transactionHash,
                new TraceTransactionOptions { DisableMemory = false, DisableStorage = false, DisableStack = false });

            var log = await addCopyrightEvent.GetFilterChanges<AddCopyrightEvent>(filterAll);
            log.Count.Should().Be(1);

            var logMusicId = await addCopyrightEvent.GetFilterChanges<AddCopyrightEvent>(filterMusicId);
            logMusicId.Count.Should().Be(1);

            var res3 = await getCopyrightId.CallAsync<string>("51BF375C77BC4C089DCAD2AC4935E600");
            res3.Should().Be("3345D89C498A4EB79DB670F46F25EF00");

            var res4 = await getCopyrightResourceEndpoint.CallAsync<string>("51BF375C77BC4C089DCAD2AC4935E600");
            res4.Should().Be(@"http:\\api.zimrii\3345D89C498A4EB79DB670F46F25EF00");

            var res5 = await getCopyrightHash.CallAsync<string>("51BF375C77BC4C089DCAD2AC4935E600");
            res5.Should().Be("1B2M2Y8AsgTpgAmY7PhCfg==");

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
