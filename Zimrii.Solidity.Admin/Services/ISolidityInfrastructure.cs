using Microsoft.AspNetCore.Hosting;

namespace Zimrii.Solidity.Admin.Services
{
    public interface ISolidityInfrastructure
    {
        string Location { get; }
    }

    public class SolidityInfrastructure : ISolidityInfrastructure
    {
        private readonly IHostingEnvironment hostingEnvironment;

        public string Location { get => hostingEnvironment.WebRootPath; }

        public SolidityInfrastructure(IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }
    }
}