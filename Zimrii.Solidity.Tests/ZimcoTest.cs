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

        public ZimcoTest() : base(new[] { "TokenBase", "ZimcoToken" }) //"tokenRecipient", 
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
                        deploy = await web3.Eth.DeployContract.SendRequestAsync(Abi[contract], Code[contract], AccountAddress, new HexBigInteger(2000000),
                            "0x9a035d1f23d3d645d4c1feb358162d7c78c64ba7",
                            5000000,
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
            ReadContractsDetails();

            var contractAddress = "0xd68d97363aedf990c2e9b96b7db51b4dc6f30f6b";
            var contract = Web3.Eth.GetContract(Abi["ZimcoToken"], contractAddress);

            //var mintToken = contract.GetFunction("mintToken");
            var balanceOf = contract.GetFunction("balanceOf");

            var unlockResult =
                await Web3.Personal.UnlockAccount.SendRequestAsync(AccountAddress, PassPhrase, 120);
            unlockResult.Should().BeTrue();

            //var transactionHash = await mintToken.SendTransactionAsync(AccountAddress,
            //    AccountAddress, 3000000);
            //var receipt1 = await MineAndGetReceiptAsync(Web3, transactionHash, true);

            var res3 = await balanceOf.CallAsync<int>(AccountAddress);
            res3.Should().Be(5000000);

        }

    }

}
