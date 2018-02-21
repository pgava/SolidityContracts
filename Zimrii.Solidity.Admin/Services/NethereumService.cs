using Microsoft.Extensions.FileProviders;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Zimrii.Solidity.Admin.Services
{
    public class NethereumService : INethereumService
    {
        public async Task<bool> UnlockAccountAsync(string url, string accountAddress, string passPhrase)
        {
            var web3 = new Web3(url);

            return await web3.Personal.UnlockAccount.SendRequestAsync(accountAddress, passPhrase, new HexBigInteger(120));
        }
    }
}
