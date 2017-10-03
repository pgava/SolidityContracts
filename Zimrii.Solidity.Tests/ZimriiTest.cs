using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Nethereum.RPC.Eth.DTOs;
using Serilog;
using Xunit;

namespace Zimrii.Solidity.Tests
{
    public class ZimriiTest : NethereumTestsBase
    {

        public ZimriiTest() : base(new[] {"AccessRestriction", "ArtistContract", "ArtistRoyalties", "MusicCopyright"})
        {

            string curDir = AssemblyDirectory;
            var logPath = Path.Combine(curDir + RootPath, @"..\Zimrii-{Date}.txt");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.RollingFile(logPath)
                .CreateLogger();

        }

        [Fact]
        public async Task DeployZimcoToRinkeby()
        {
            await Setup(true, SaveZimriiDetails);
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
}
