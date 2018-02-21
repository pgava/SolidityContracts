using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Zimrii.Solidity.Admin.Services;

namespace Zimrii.Solidity.Admin.Controllers
{
    public class SolidityBaseController : Controller
    {
        protected readonly IHostingEnvironment hostingEnvironment;
        protected readonly ISolidityInfrastructure solidityInfrastructure;
        protected readonly ISolidityService solidityService;

        public SolidityBaseController(IHostingEnvironment hostingEnvironment, ISolidityInfrastructure solidityInfrastructure, ISolidityService solidityService)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.solidityInfrastructure = solidityInfrastructure;
            this.solidityService = solidityService;
        }
    }
}
