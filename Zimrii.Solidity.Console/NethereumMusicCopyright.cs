using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Serilog;

namespace Zimrii.Solidity.Console
{
    public class NethereumMusicCopyright : NethereumBase
    {
        public NethereumMusicCopyright(string path) : base(new[] { "AccessRestriction", "MusicCopyright" })
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                RootPath = path;
            }
            AccountAddress = "0x564f83ae16af0741ce756adf35dcf9b17874b83f";
            PassPhrase = "";

            var logPath = Path.Combine(RootPath, @"ZimriiMusicCopyright-{Date}.txt");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.RollingFile(logPath)
                .CreateLogger();
        }

        public async Task UploadMusicCopyright()
        {
            await Setup(false, SaveZimriiDetails);
        }

        public async Task<string> SetMusicCopyright(string musicId, string copyrightId, string hash)
        {
            ReadContractsDetails();

            var contractAddress = "0x9471be3421eae1a10be604c46e4b7dba8bd90432";
            var contract = Web3.Eth.GetContract(Abi["MusicCopyright"], contractAddress);

            var addCopyrightEvent = contract.GetEvent("SetCopyright");
            var filterAll = await addCopyrightEvent.CreateFilterAsync();
            var filterMusicId = await addCopyrightEvent.CreateFilterAsync("BC3FCB8DA4384619AAC2AEE0698D3ECC");

            var setCopyright = contract.GetFunction("setCopyright");
            var getCopyrightId = contract.GetFunction("getCopyrightId");
            var getCopyrightHash = contract.GetFunction("getCopyrightHash");
            var setCopyrightEndpointResourceRoot = contract.GetFunction("setCopyrightEndpointResourceRoot");
            var getCopyrightResourceEndpoint = contract.GetFunction("getCopyrightResourceEndpoint");

            var unlockResult =
                await Web3.Personal.UnlockAccount.SendRequestAsync(AccountAddress, PassPhrase, 120);

            System.Console.WriteLine("Unlock");

            var transactionHash = await setCopyright.SendTransactionAsync(AccountAddress, new HexBigInteger(120000), new HexBigInteger(120000),
                musicId, copyrightId, hash);

            var receipt1 = await MineAndGetReceiptAsync(Web3, transactionHash, false);

            System.Console.WriteLine("setCopyright. Receipt: " + receipt1.TransactionHash);

            return receipt1.TransactionHash;
        }

        void SaveZimriiDetails(Dictionary<string, TransactionReceipt> receipts)
        {
            foreach (var receipt in receipts)
            {
                Log.Information(receipt.Key);
                Log.Information("{@zimrii}", new
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
    }

    public class AddCopyrightEvent
    {
        [Parameter("bytes32", "musicId", 1, true)]
        public string MusicId { get; set; }

        [Parameter("bytes32", "copyrightId", 2, false)]
        public string CopyrightId { get; set; }
    }
}
