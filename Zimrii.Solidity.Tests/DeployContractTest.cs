using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Xunit;

namespace Zimrii.Solidity.Tests
{
    public class DeployContractTest : NethereumTestsBase
    {

        public DeployContractTest() : base(new[] { "AccessRestriction" })
        {
        }

        [Fact]
        public async Task DeployContractToEthereum()
        {
            ReadContractsDetails();

            /**
             * Set account & password
             * AccountAddress = "";
             * PassPhrase = "";
             */

            var unlockResult = await Web3.Personal.UnlockAccount.SendRequestAsync(AccountAddress, PassPhrase, 120);
            unlockResult.Should().BeTrue();

            foreach (var contract in Contracts)
            {
                string deploy;
                switch (contract)
                {
                    default:
                        deploy = await Web3.Eth.DeployContract.SendRequestAsync(Abi[contract], Code[contract], AccountAddress, new HexBigInteger(2000000));
                        break;
                }

                var receipt = await MineAndGetReceiptAsync(Web3, deploy, false);

                string curDir = AssemblyDirectory;
                var contractFile = Path.Combine(curDir + RootPath, @"..\" + contract + "-eth.txt");
                string[] lines = { "Transaction Hash: " + receipt.TransactionHash,
                    "Contract Addres: " + receipt.ContractAddress,
                    "Block Hash: " + receipt.BlockHash,
                    "Block Number: " + receipt.BlockNumber.Value,
                    "Cumulative Gas Used: " + receipt.CumulativeGasUsed.Value,
                    "Gas Used: " + receipt.GasUsed.Value,
                    "Transaction Index: " + receipt.TransactionIndex.Value
                };
                File.WriteAllLines(contractFile, lines);
            }
        }        
    }

}
