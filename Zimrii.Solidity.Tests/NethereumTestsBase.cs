using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

namespace Zimrii.Solidity.Tests
{
    public class NethereumTestsBase
    {
        protected const string RootPath = @"..\..\..\..\..\Zimrii.Solidity\contracts\bin";
        protected readonly IEnumerable<string> Contracts;

        protected const string AccountAddress = "0x12890d2cce102216644c59dae5baed380d84830c";
        protected const string PassPhrase = "password";
        protected Dictionary<string, TransactionReceipt> Receipts;
        protected Dictionary<string, string> Abi;
        protected Dictionary<string, string> Code;
        protected Web3 Web3;

        public NethereumTestsBase(IEnumerable<string> contracts)
        {
            Contracts = contracts;
            Web3 = new Web3();
        }

        protected async Task Setup()
        {
            ReadContractsDetails();
            Receipts = await DeployContract(Web3, Contracts);
        }

        protected void ReadContractsDetails()
        {
            Abi = ReadAbi(Contracts);
            Code = ReadCode(Contracts);
        }


        protected virtual async Task<TransactionReceipt> MineAndGetReceiptAsync(Web3 web3, string transactionHash)
        {
            var miningResult = await web3.Miner.Start.SendRequestAsync(200);
            miningResult.Should().BeTrue();

            var receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);

            while (receipt == null)
            {
                Thread.Sleep(1000);
                receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);
            }

            miningResult = await web3.Miner.Stop.SendRequestAsync();
            miningResult.Should().BeTrue();
            return receipt;
        }

        protected static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        private string ReadContent(string path)
        {
            return File.ReadAllText(path);
        }

        private Dictionary<string, string> ReadAbi(IEnumerable<string> contracts)
        {
            string curDir = AssemblyDirectory;
            var abis = new Dictionary<string, string>();

            foreach (var contract in contracts)
            {
                var abiPath = Path.Combine(curDir + RootPath, contract + ".abi");
                abis.Add(contract, ReadContent(abiPath));
            }

            return abis;
        }

        private Dictionary<string, string> ReadCode(IEnumerable<string> contracts)
        {
            string curDir = AssemblyDirectory;
            var codes = new Dictionary<string, string>();

            foreach (var contract in contracts)
            {
                var abiPath = Path.Combine(curDir + RootPath, contract + ".bin");
                codes.Add(contract, ReadContent(abiPath));
            }

            return codes;
        }

        protected async Task<Dictionary<string, TransactionReceipt>> DeployContract(Web3 web3, IEnumerable<string> contracts)
        {
            var receipts = new Dictionary<string, TransactionReceipt>();

            var unlockResult =
                await web3.Personal.UnlockAccount.SendRequestAsync(AccountAddress, PassPhrase, new HexBigInteger(120));            
            unlockResult.Should().BeTrue();

            foreach (var contract in contracts)
            {
                string deploy;
                switch (contract)
                {
                    default:
                        deploy = await web3.Eth.DeployContract.SendRequestAsync(Abi[contract], Code[contract], AccountAddress, new HexBigInteger(2000000));
                        break;
                }

                var receipt = await MineAndGetReceiptAsync(web3, deploy);

                receipts.Add(contract, receipt);
            }

            return receipts;
        }
    }
}

