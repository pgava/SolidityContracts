using System.Threading.Tasks;
using FluentAssertions;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.DebugGeth.DTOs;
using Xunit;

namespace Zimrii.Solidity.Tests
{
    public class ArtistContractTest : NethereumTestsBase
    {

        public ArtistContractTest() : base(new[] { "AccessRestriction", "ArtistContract" })
        {
        }

        [Fact]
        public async Task ArtistContractSolTest()
        {
            await Setup();

            var contractAddress = Receipts["ArtistContract"].ContractAddress;
            var contract = Web3.Eth.GetContract(Abi["ArtistContract"], contractAddress);

            var addContractEvent = contract.GetEvent("SetContract");
            var filterAll = await addContractEvent.CreateFilterAsync();
            var filterContractId = await addContractEvent.CreateFilterAsync("contractId");

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

            var setContract = contract.GetFunction("setContract");
            var getContractHash = contract.GetFunction("getContractHash");
            var setContractEndpointResourceRoot = contract.GetFunction("setContractEndpointResourceRoot");
            var getContractResourceEndpoint = contract.GetFunction("getContractResourceEndpoint");

            var unlockResult =
                await Web3.Personal.UnlockAccount.SendRequestAsync(AccountAddress, PassPhrase, new HexBigInteger(120));
            unlockResult.Should().BeTrue();

            var transactionHash = await setContract.SendTransactionAsync(AccountAddress, 
                new HexBigInteger(2000000), new HexBigInteger(120), "contractId", "contractHash");

            var receipt1 = await MineAndGetReceiptAsync(Web3, transactionHash);

            transactionHash = await setContractEndpointResourceRoot.SendTransactionAsync(AccountAddress,
                new HexBigInteger(2000000), new HexBigInteger(120), @"http:\\myservice\");

            var receipt2 = await MineAndGetReceiptAsync(Web3, transactionHash);

            var debuginfo = await Web3.DebugGeth.TraceTransaction.SendRequestAsync(transactionHash,
                new TraceTransactionOptions { DisableMemory = false, DisableStorage = false, DisableStack = false });

            var log = await addContractEvent.GetFilterChanges<AddContractEvent>(filterAll);
            log.Count.Should().Be(1);

            var logMusicId = await addContractEvent.GetFilterChanges<AddContractEvent>(filterContractId);
            logMusicId.Count.Should().Be(1);

            var res3 = await getContractHash.CallAsync<string>("contractId");
            res3.Should().Be("contractHash");

            var res4 = await getContractResourceEndpoint.CallAsync<string>("contractId");
            res4.Should().Be(@"http:\\myservice\contractId");
        }
    }

    public class AddContractEvent
    {
        [Parameter("bytes32", "contractId", 1, true)]
        public string ContractId { get; set; }

        [Parameter("bytes32", "contractHash", 2, false)]
        public string ContractHash { get; set; }
    }
}
