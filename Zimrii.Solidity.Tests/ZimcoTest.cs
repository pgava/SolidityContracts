using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.DebugGeth.DTOs;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Serilog;
using Xunit;

namespace Zimrii.Solidity.Tests
{
    public class ZimcoTest : NethereumTestsBase
    {

        public ZimcoTest() : base(new[] { "owned", "TokenData", "ZimcoToken" }) 
        {
            RootPath = @"..\..\..\..\..\Zimrii.Solidity\contracts\zimco\metadata";
            string curDir = AssemblyDirectory;
            var logPath = Path.Combine(curDir + RootPath, @"..\Zimco-{Date}.txt");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.RollingFile(logPath)
                .CreateLogger();

        }

        protected override async Task<Dictionary<string, TransactionReceipt>> DeployContract(Web3 web3, IEnumerable<string> contracts, bool isMining,
            Action<Dictionary<string, TransactionReceipt>> saveContract = null)
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

            saveContract?.Invoke(receipts);
            return receipts;
        }

        [Fact]
        public async Task DeployZimcoToRinkeby()
        {
            AccountAddress = "0x564f83ae16af0741ce756adf35dcf9b17874b83f";
            PassPhrase = "";

            await Setup(false, SaveZimcoDetails);
        }

        void SaveZimcoDetails(Dictionary<string, TransactionReceipt> receipts)
        {
            foreach (var receipt in receipts)
            {
                Log.Information(receipt.Key);
                Log.Information("{@zimco}", new
                {
                    AccountAddress = AccountAddress,
                    TransactionHash = receipt.Value.TransactionHash,
                    ContractAddress = receipt.Value.ContractAddress,
                    BlockHash = receipt.Value.BlockHash,
                    CumulativeGasUsed = receipt.Value.CumulativeGasUsed.Value.ToString(),
                    GasUsed = receipt.Value.GasUsed.Value.ToString(),
                    TransactionIndex = receipt.Value.TransactionIndex.Value.ToString()
                });
            }
        }

        [Fact]
        public async Task ZimcoSetupZimcoTest()
        {
            AccountAddress = "0x564f83ae16af0741ce756adf35dcf9b17874b83f";
            PassPhrase = "";
            ReadContractsDetails();

            var contractDataAddress = "0x5125cf4b0588fe2b242183054f92f398bce6b2b6";
            var contractZimcoAddress = "0xcfa0a5d69651de58f7d9530a3b982103e2ea911f";

            var contractData = Web3.Eth.GetContract(Abi["TokenData"], contractDataAddress);
            var contractZimco = Web3.Eth.GetContract(Abi["ZimcoToken"], contractZimcoAddress);

            var changeOwners = contractData.GetFunction("changeOwners");
            var setTotalSupply = contractZimco.GetFunction("setTotalSupply");
            var balanceOf = contractZimco.GetFunction("balanceOf");

            var unlockResult =
                await Web3.Personal.UnlockAccount.SendRequestAsync(AccountAddress, PassPhrase, 120);
            unlockResult.Should().BeTrue();

            // give Zimco access to database
            var transactionHash1 = await changeOwners.SendTransactionAsync(AccountAddress, contractZimcoAddress, true);
            var receipt1 = await MineAndGetReceiptAsync(Web3, transactionHash1, false);

            unlockResult =
                await Web3.Personal.UnlockAccount.SendRequestAsync(AccountAddress, PassPhrase, 120);
            unlockResult.Should().BeTrue();

            // set initial supply
            var transactionHash2 = await setTotalSupply.SendTransactionAsync(AccountAddress, 1000000000);
            var receipt2 = await MineAndGetReceiptAsync(Web3, transactionHash2, false);

            // check the balance after transfer
            var res = await balanceOf.CallAsync<int>(AccountAddress);
            res.Should().Be(1000000000);


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

            // give Zimco access to database
            var transactionHash1 = await changeOwners.SendTransactionAsync(AccountAddress, contractZimcoAddress, true);
            var receipt1 = await MineAndGetReceiptAsync(Web3, transactionHash1, true);

            // set initial supply
            var transactionHash2 = await setTotalSupply.SendTransactionAsync(AccountAddress, 5000000);
            var receipt2 = await MineAndGetReceiptAsync(Web3, transactionHash2, true);

            // transfer funds
            var transactionHash3 = await transfer.SendTransactionAsync(AccountAddress, "0x13f022d72158410433cbd66f5dd8bf6d2d129924", 750000);
            var receipt3 = await MineAndGetReceiptAsync(Web3, transactionHash3, true);

            // read the log
            var log = await transferEvent.GetFilterChanges<TransferEvent>(filterAll);
            log.Count.Should().Be(1);

            // check the balance after transfer
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
