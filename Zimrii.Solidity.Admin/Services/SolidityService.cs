using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Zimrii.Solidity.Admin.Services
{
    public class SolidityService : ISolidityService
    {
        public EthAccount GetEthAccount(ISolidityInfrastructure solidityInfrastructure, SolidityEnvironment environment)
        {
            IFileProvider provider = new PhysicalFileProvider(solidityInfrastructure.Location);
            
            // Get directory content
            //IDirectoryContents contents = provider.GetDirectoryContents("");

            IFileInfo fileInfo = provider.GetFileInfo(Path.Combine("metadata/", GetEthAccountFileName()));

            var content = File.ReadAllText(fileInfo.PhysicalPath);

            return JsonConvert.DeserializeObject<EthAccount>(content);
        }

        public Royalties GetRoyalties(ISolidityInfrastructure solidityInfrastructure, SolidityEnvironment environment)
        {
            IFileProvider provider = new PhysicalFileProvider(solidityInfrastructure.Location);

            IFileInfo fileInfo = provider.GetFileInfo(Path.Combine("metadata/", GetRoyaltiesFileName()));

            var content = File.ReadAllText(fileInfo.PhysicalPath);

            return JsonConvert.DeserializeObject<Royalties>(content);
        }

        private string GetEthAccountFileName()
        {
            // TODO
            return string.Format("ethaccount-{0}.json", "test");
        }

        private string GetRoyaltiesFileName()
        {
            // TODO
            return string.Format("royalties-{0}.json", "test");
        }
    }
}
