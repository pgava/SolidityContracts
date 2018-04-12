using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;

namespace Zimrii.Solidity.Admin.Services
{
    public interface INethereumService
    {
        Task<bool> UnlockAccountAsync(string url, string accountAddress, string passPhrase);
        Task<TransactionReceipt> DeployContractAsync(string url, string abi, string code, string accountAddress, string passPhrase, bool isMine);
        Task<TransactionReceipt> SetRoyaltiesAsync(string url, string abi, string accountAddress, string contractAddress, string royaltiesGuid, string royaltiesHash, string passPhrase, bool isMine);
        Task<string> GetRoyaltiesAsync(string url, string abi, string accountAddress, string contractAddress, string royaltiesGuid, string passPhrase);
        Task<TransactionReceipt> SetCopyrightsAsync(string url, string abi, string accountAddress, string contractAddress, string copyrightsGuid, string copyrightsHash, string passPhrase, bool isMine);
        Task<string> GetCopyrightsAsync(string url, string abi, string accountAddress, string contractAddress, string copyrightsGuid, string passPhrase);

    }
}