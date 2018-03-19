using Microsoft.Extensions.FileProviders;
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

            var deploy = await web3.Eth.DeployContract.SendRequestAsync(abi, code, accountAddress, new HexBigInteger(21000), new HexBigInteger(40), new HexBigInteger(0));
            var receipt = await MineAndGetReceiptAsync(web3, deploy, accountAddress, passPhrase, isMine);

            return receipt;
        }

        public async Task<string> GetRoyaltiesAsync(string url, string abi, string accountAddress, string contractAddress, string royaltiesGuid, string passPhrase)
        {
            var web3 = new Web3(url);

            var contract = web3.Eth.GetContract(abi, contractAddress);
            var getRoyaltiesHash = contract.GetFunction("getRoyaltiesHash");

            var res = await getRoyaltiesHash.CallAsync<string>(royaltiesGuid);

            return res;
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

            var transactionHash = await setRoyalties.SendTransactionAsync(accountAddress, new HexBigInteger(21000), new HexBigInteger(40), new HexBigInteger(0), royaltiesGuid, royaltiesHash);

            var receipt = await MineAndGetReceiptAsync(web3, transactionHash, accountAddress, passPhrase, isMine);

            return receipt;
        }

        public async Task<bool> UnlockAccountAsync(string url, string accountAddress, string passPhrase)
        {
            var web3 = new Web3(url);

            return await web3.Personal.UnlockAccount.SendRequestAsync(accountAddress, passPhrase, new HexBigInteger(120));
        }

        protected virtual async Task<TransactionReceipt> MineAndGetReceiptAsync(Web3 web3, string transactionHash, string accountAddress, string passPhrase, bool isMining)
        {
            bool miningResult = true;

            if (isMining)
            {
                // TODO : where is miner now?
                //miningResult = await web3.Miner.Start.SendRequestAsync(20);
            }

            var receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);

            while (receipt == null)
            {
                Thread.Sleep(1000);
                receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);

                var unlockResult =
                    await web3.Personal.UnlockAccount.SendRequestAsync(accountAddress, passPhrase, 120);

                if (!unlockResult)
                {
                    throw new Exception("Couldn't unlock account");
                }
            }

            if (isMining)
            {
                // TODO
                //miningResult = await web3.Miner.Stop.SendRequestAsync();
            }

            return receipt;
        }

    }
}
