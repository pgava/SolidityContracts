using System.Threading.Tasks;
using FluentAssertions;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Hex.HexTypes;
//using Nethereum.RPC.DebugGeth.DTOs;
using Xunit;

namespace Zimrii.Solidity.Tests
{
    public class ArtistRoyaltiesTest : NethereumTestsBase
    {

        public ArtistRoyaltiesTest() : base(new[] { "AccessRestriction", "ArtistRoyalties" })
        {
        }

        [Fact]
        public async Task ArtistRoyaltiesSolTest()
        {
            await Setup(true);

            var contractAddress = Receipts["ArtistRoyalties"].ContractAddress;
            var contract = Web3.Eth.GetContract(Abi["ArtistRoyalties"], contractAddress);

            var addRoyaltiesEvent = contract.GetEvent("SetRoyalties");
            var filterAll = await addRoyaltiesEvent.CreateFilterAsync();
            var filterRoyaltiesId = await addRoyaltiesEvent.CreateFilterAsync("royaltiesId");

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

            var setRoyalties = contract.GetFunction("setRoyalties");
            var getRoyaltiesHash = contract.GetFunction("getRoyaltiesHash");
            var setRoyaltiesEndpointResourceRoot = contract.GetFunction("setRoyaltiesEndpointResourceRoot");
            var getRoyaltiesResourceEndpoint = contract.GetFunction("getRoyaltiesResourceEndpoint");

            var unlockResult =
                await Web3.Personal.UnlockAccount.SendRequestAsync(AccountAddress, PassPhrase, new HexBigInteger(120));
            unlockResult.Should().BeTrue();

            var transactionHash = await setRoyalties.SendTransactionAsync(AccountAddress, 
                new HexBigInteger(2000000), new HexBigInteger(120), "royaltiesId", "royaltiesIdHash");

            var receipt1 = await MineAndGetReceiptAsync(Web3, transactionHash, true);

            transactionHash = await setRoyaltiesEndpointResourceRoot.SendTransactionAsync(AccountAddress,
                new HexBigInteger(2000000), new HexBigInteger(120), @"http:\\myservice\");

            var receipt2 = await MineAndGetReceiptAsync(Web3, transactionHash, true);

            //var debuginfo = await Web3.DebugGeth.TraceTransaction.SendRequestAsync(transactionHash,
            //    new TraceTransactionOptions { DisableMemory = false, DisableStorage = false, DisableStack = false });

            var log = await addRoyaltiesEvent.GetFilterChanges<AddRoyaltiesEvent>(filterAll);
            log.Count.Should().Be(1);

            var logMusicId = await addRoyaltiesEvent.GetFilterChanges<AddRoyaltiesEvent>(filterRoyaltiesId);
            logMusicId.Count.Should().Be(1);

            var res3 = await getRoyaltiesHash.CallAsync<string>("royaltiesId");
            res3.Should().Be("royaltiesHash");

            var res4 = await getRoyaltiesResourceEndpoint.CallAsync<string>("royaltiesId");
            res4.Should().Be(@"http:\\myservice\royaltiesId");
        }
    }

    public class AddRoyaltiesEvent
    {
        [Parameter("bytes32", "royaltiesId", 1, true)]
        public string RoyaltiesId { get; set; }

        [Parameter("bytes32", "royaltiesHash", 2, false)]
        public string RoyaltiesHash { get; set; }
    }
}
