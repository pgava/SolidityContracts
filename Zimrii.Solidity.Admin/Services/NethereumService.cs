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
            var web3 = new Web3(url);

            var deploy = await web3.Eth.DeployContract.SendRequestAsync(abi, code, accountAddress, new HexBigInteger(300000));
            var receipt = await MineAndGetReceiptAsync(web3, deploy, accountAddress, passPhrase, isMine);

            return receipt;
        }

        public async Task<string> GetCopyrightsAsync(string url, string abi, string accountAddress, string contractAddress, string copyrightsGuid, string passPhrase)
        {
            var web3 = new Web3(url);

            var contract = web3.Eth.GetContract(abi, contractAddress);
            var getCopyrightsHash = contract.GetFunction("getCopyrightsHash");

            var res = await getCopyrightsHash.CallAsync<string>(copyrightsGuid);

            return res;
        }

        public async Task<string> GetRoyaltiesAsync(string url, string abi, string accountAddress, string contractAddress, string royaltiesGuid, string passPhrase)
        {
            var web3 = new Web3(url);

            var contract = web3.Eth.GetContract(abi, contractAddress);
            var getRoyaltiesHash = contract.GetFunction("getRoyaltiesHash");

            var res = await getRoyaltiesHash.CallAsync<string>(royaltiesGuid);

            return res;
        }

        public async Task<TransactionReceipt> SetCopyrightsAsync(string url, string abi, string accountAddress, string contractAddress, string copyrightsGuid, string copyrightsHash, string passPhrase, bool isMine)
        {
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

            return receipt;
        }

        public async Task<TransactionReceipt> SetRoyaltiesAsync(string url, string abi, string accountAddress, string contractAddress, string royaltiesGuid, string royaltiesHash, string passPhrase, bool isMine)
        {
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

            return receipt;
        }

        public async Task<bool> UnlockAccountAsync(string url, string accountAddress, string passPhrase)
        {
            var web3 = new Web3(url);

            return await web3.Personal.UnlockAccount.SendRequestAsync(accountAddress, passPhrase, 120);
        }

        protected virtual async Task<TransactionReceipt> MineAndGetReceiptAsync(Web3 web3, string transactionHash, string accountAddress, string passPhrase, bool isMining)
        {
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

            return receipt;
        }

    }
}
