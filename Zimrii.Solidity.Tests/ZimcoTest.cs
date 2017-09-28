using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.DebugGeth.DTOs;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Xunit;

namespace Zimrii.Solidity.Tests
{
    public class ZimcoTest : NethereumTestsBase
    {

        public ZimcoTest() : base(new[] { "owned", "TokenData", "TokenBase", "ZimcoToken" }) //"tokenRecipient", 
        {
            RootPath = @"..\..\..\..\..\Zimrii.Solidity\contracts\zimco\metadata";
        }

        protected override async Task<Dictionary<string, TransactionReceipt>> DeployContract(Web3 web3, IEnumerable<string> contracts, bool isMining)
        {
            var receipts = new Dictionary<string, TransactionReceipt>();

            var unlockResult =
                await web3.Personal.UnlockAccount.SendRequestAsync(AccountAddress, PassPhrase, 120);
            unlockResult.Should().BeTrue();

            foreach (var contract in contracts)
            {
                string deploy;
                switch (contract)
                {
                    case "TokenBase":
                    case "ZimcoToken":
                        var contractDataAddress = receipts["TokenData"].ContractAddress;
                        deploy = await web3.Eth.DeployContract.SendRequestAsync(Abi[contract], Code[contract], AccountAddress, new HexBigInteger(2000000),
                            contractDataAddress,
                            "zimco",
                            2,
                            "ZMC");
                        break;

                    default:
                        deploy = await web3.Eth.DeployContract.SendRequestAsync(Abi[contract], Code[contract], AccountAddress, new HexBigInteger(2000000));
                        break;
                }

                var receipt = await MineAndGetReceiptAsync(web3, deploy, isMining);

                receipts.Add(contract, receipt);
            }

            return receipts;
        }

        [Fact]
        public async Task DeployZimcoTest()
        {
            await Setup(true);
        }

        [Fact]
        public async Task ZimcoOperationTest()
        {
            await Setup(true);

            var contractDataAddress = Receipts["TokenData"].ContractAddress;
            var contractZimcoAddress = Receipts["ZimcoToken"].ContractAddress;

            var contractData = Web3.Eth.GetContract(Abi["TokenData"], contractDataAddress);
            var contractZimco = Web3.Eth.GetContract(Abi["ZimcoToken"], contractZimcoAddress);

            var transferEvent = contractZimco.GetEvent("Transfer");
            var filterAll = await transferEvent.CreateFilterAsync();

            var changeOwners = contractData.GetFunction("changeOwners");
            var setTotalSupply = contractZimco.GetFunction("setTotalSupply");
            var balanceOf = contractZimco.GetFunction("balanceOf");
            var transfer = contractZimco.GetFunction("transfer");

            var unlockResult =
                await Web3.Personal.UnlockAccount.SendRequestAsync(AccountAddress, PassPhrase, 120);
            unlockResult.Should().BeTrue();

            var transactionHash1 = await changeOwners.SendTransactionAsync(AccountAddress, contractZimcoAddress, true);
            var receipt1 = await MineAndGetReceiptAsync(Web3, transactionHash1, true);

            var transactionHash2 = await setTotalSupply.SendTransactionAsync(AccountAddress, 5000000);
            var receipt2 = await MineAndGetReceiptAsync(Web3, transactionHash2, true);

            var transactionHash3 = await transfer.SendTransactionAsync(AccountAddress, "0x13f022d72158410433cbd66f5dd8bf6d2d129924", 750000);
            var receipt3 = await MineAndGetReceiptAsync(Web3, transactionHash3, true);

            var log = await transferEvent.GetFilterChanges<TransferEvent>(filterAll);
            log.Count.Should().Be(1);

            var res3 = await balanceOf.CallAsync<int>(AccountAddress);
            res3.Should().Be(5000000 - 750000);
        }

    }

    public class TransferEvent
    {
        [Parameter("address", "from", 1, true)]
        public string From { get; set; }

        [Parameter("address", "to", 2, true)]
        public string To { get; set; }

        [Parameter("uint256", "value", 3, false)]
        public int Value { get; set; }
    }
}
