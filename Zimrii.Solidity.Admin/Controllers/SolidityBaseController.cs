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
        protected readonly INethereumService nethereumService;

        public SolidityBaseController(IHostingEnvironment hostingEnvironment, ISolidityInfrastructure solidityInfrastructure, 
            ISolidityService solidityService, INethereumService nethereumService)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.solidityInfrastructure = solidityInfrastructure;
            this.solidityService = solidityService;
            this.nethereumService = nethereumService;
        }

    }
}
