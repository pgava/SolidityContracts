using System.Threading.Tasks;
using Xunit;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.DebugGeth.DTOs;
using FluentAssertions;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Zimrii.Tests
{
    public class MusicCopyrightTest : NethereumTestsBase
    {

        public MusicCopyrightTest() : base(new[] { "AccessRestriction", "MusicCopyright" })
        {
        }

        [Fact]
        public async Task MusicCopyrightSolTest()
        {
            await Setup();

            var contractAddress = Receipts["MusicCopyright"].ContractAddress;
            var contract = Web3.Eth.GetContract(Abi["MusicCopyright"], contractAddress);

            var addCopyrightEvent = contract.GetEvent("SetCopyright");
            var filterAll = await addCopyrightEvent.CreateFilterAsync();
            var filterMusicId = await addCopyrightEvent.CreateFilterAsync("musicId");

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
                await Web3.Personal.UnlockAccount.SendRequestAsync(AccountAddress, PassPhrase, new HexBigInteger(120));
            unlockResult.Should().BeTrue();

            var transactionHash = await setCopyright.SendTransactionAsync(AccountAddress, 
                new HexBigInteger(2000000), new HexBigInteger(120), "musicId", "copyrightId", "copyrightHash");

            var receipt1 = await MineAndGetReceiptAsync(Web3, transactionHash);

            transactionHash = await setCopyrightEndpointResourceRoot.SendTransactionAsync(AccountAddress,
                new HexBigInteger(2000000), new HexBigInteger(120), @"http:\\myservice\");

            var receipt2 = await MineAndGetReceiptAsync(Web3, transactionHash);

            var debuginfo = await Web3.DebugGeth.TraceTransaction.SendRequestAsync(transactionHash,
                new TraceTransactionOptions { DisableMemory = false, DisableStorage = false, DisableStack = false });

            var log = await addCopyrightEvent.GetFilterChanges<AddCopyrightEvent>(filterAll);
            log.Count.Should().Be(1);

            var logMusicId = await addCopyrightEvent.GetFilterChanges<AddCopyrightEvent>(filterMusicId);
            logMusicId.Count.Should().Be(1);

            var res3 = await getCopyrightId.CallAsync<string>("musicId");
            res3.Should().Be("copyrightId");

            var res4 = await getCopyrightResourceEndpoint.CallAsync<string>("musicId");
            res4.Should().Be(@"http:\\myservice\copyrightId");

            var res5 = await getCopyrightHash.CallAsync<string>("musicId");
            res5.Should().Be("copyrightHash");

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
