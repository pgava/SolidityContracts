using Microsoft.Extensions.FileProviders;
using Nethereum.Geth;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Zimrii.Solidity.Admin.Services
{
    public class NethereumService : INethereumService
    {
        public async Task<TransactionReceipt> DeployContractAsync(string url, string abi, string code, string accountAddress, string passPhrase, bool isMine)
        {
#if NO_ETHER
            var receipt = new TransactionReceipt
            {
                TransactionHash = "aa",
                ContractAddress = "aa",
                BlockHash = "aa",
                BlockNumber = new HexBigInteger(1),
                CumulativeGasUsed = new HexBigInteger(1),
                GasUsed = new HexBigInteger(1),
                TransactionIndex = new HexBigInteger(1)
            };
            
#else
            var web3 = new Web3(url);

            var deploy = await web3.Eth.DeployContract.SendRequestAsync(abi, code, accountAddress, new HexBigInteger(300000));
            var receipt = await MineAndGetReceiptAsync(web3, deploy, accountAddress, passPhrase, isMine);
#endif
            return receipt;
        }

        public async Task<string> GetCopyrightsAsync(string url, string abi, string accountAddress, string contractAddress, string copyrightsGuid, string passPhrase)
        {
#if NO_ETHER
            var res = "aa";
#else
            var web3 = new Web3(url);

            var contract = web3.Eth.GetContract(abi, contractAddress);
            var getCopyrightsHash = contract.GetFunction("getCopyrightsHash");

            var res = await getCopyrightsHash.CallAsync<string>(copyrightsGuid);
#endif
            return res;
        }

        public async Task<string> GetRoyaltiesAsync(string url, string abi, string accountAddress, string contractAddress, string royaltiesGuid, string passPhrase)
        {
#if NO_ETHER
            var res = "aa";
#else
            var web3 = new Web3(url);

            var contract = web3.Eth.GetContract(abi, contractAddress);
            var getRoyaltiesHash = contract.GetFunction("getRoyaltiesHash");

            var res = await getRoyaltiesHash.CallAsync<string>(royaltiesGuid);
#endif
            return res;
        }

        public async Task<TransactionReceipt> SetCopyrightsAsync(string url, string abi, string accountAddress, string contractAddress, string copyrightsGuid, string copyrightsHash, string passPhrase, bool isMine)
        {
#if NO_ETHER
            var receipt = new TransactionReceipt
            {
                TransactionHash = "aa",
                ContractAddress = "aa",
                BlockHash = "aa",
                BlockNumber = new HexBigInteger(1),
                CumulativeGasUsed = new HexBigInteger(1),
                GasUsed = new HexBigInteger(1),
                TransactionIndex = new HexBigInteger(1)
            };
#else
            var web3 = new Web3(url);

            var contract = web3.Eth.GetContract(abi, contractAddress);
            var setCopyrights = contract.GetFunction("setCopyrights");

            var unlockResult = await web3.Personal.UnlockAccount.SendRequestAsync(accountAddress, passPhrase, 120);
            if (!unlockResult)
            {
                throw new Exception("Couldn't unlock account");
            }

            var transactionHash = await setCopyrights.SendTransactionAsync(accountAddress, new HexBigInteger(210000), null, null, copyrightsGuid, copyrightsHash);

            var receipt = await MineAndGetReceiptAsync(web3, transactionHash, accountAddress, passPhrase, isMine);
#endif
            return receipt;
        }

        public async Task<TransactionReceipt> SetRoyaltiesAsync(string url, string abi, string accountAddress, string contractAddress, string royaltiesGuid, string royaltiesHash, string passPhrase, bool isMine)
        {
#if NO_ETHER
            var receipt = new TransactionReceipt
            {
                TransactionHash = "aa",
                ContractAddress = "aa",
                BlockHash = "aa",
                BlockNumber = new HexBigInteger(1),
                CumulativeGasUsed = new HexBigInteger(1),
                GasUsed = new HexBigInteger(1),
                TransactionIndex = new HexBigInteger(1)
            };
#else
            var web3 = new Web3(url);

            var contract = web3.Eth.GetContract(abi, contractAddress);
            var setRoyalties = contract.GetFunction("setRoyalties");

            var unlockResult = await web3.Personal.UnlockAccount.SendRequestAsync(accountAddress, passPhrase, 120);
            if (!unlockResult)
            {
                throw new Exception("Couldn't unlock account");
            }

            var transactionHash = await setRoyalties.SendTransactionAsync(accountAddress, new HexBigInteger(210000), null, null, royaltiesGuid, royaltiesHash);

            var receipt = await MineAndGetReceiptAsync(web3, transactionHash, accountAddress, passPhrase, isMine);
#endif
            return receipt;
        }

        public async Task<bool> UnlockAccountAsync(string url, string accountAddress, string passPhrase)
        {
#if NO_ETHER
            return true;
#else
            var web3 = new Web3(url);

            return await web3.Personal.UnlockAccount.SendRequestAsync(accountAddress, passPhrase, 120);
#endif
        }

        protected virtual async Task<TransactionReceipt> MineAndGetReceiptAsync(Web3 web3, string transactionHash, string accountAddress, string passPhrase, bool isMining)
        {
#if NO_ETHER
            var receipt = new TransactionReceipt
            {
                TransactionHash = "aa",
                ContractAddress = "aa",
                BlockHash = "aa",
                BlockNumber = new HexBigInteger(1),
                CumulativeGasUsed = new HexBigInteger(1),
                GasUsed = new HexBigInteger(1),
                TransactionIndex = new HexBigInteger(1)
            };
#else
            bool miningResult = true;

            var web3Geth = new Web3Geth();

            if (isMining)
            {
                miningResult = await web3Geth.Miner.Start.SendRequestAsync(20);
            }

            var receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);

            while (receipt == null)
            {
                Thread.Sleep(1000);
                receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);

                var unlockResult = await web3.Personal.UnlockAccount.SendRequestAsync(accountAddress, passPhrase, 120);

                if (!unlockResult)
                {
                    throw new Exception("Couldn't unlock account");
                }
            }

            if (isMining)
            {
                miningResult = await web3Geth.Miner.Stop.SendRequestAsync();
            }
#endif
            return receipt;
        }

    }
}
