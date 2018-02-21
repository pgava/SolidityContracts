using System.Threading.Tasks;

namespace Zimrii.Solidity.Admin.Services
{
    public interface INethereumService
    {
        Task<bool> UnlockAccountAsync(string url, string accountAddress, string passPhrase);
    }
}