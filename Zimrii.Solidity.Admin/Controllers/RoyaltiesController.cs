using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Zimrii.Solidity.Admin.Models;
using Zimrii.Solidity.Admin.Services;

namespace Zimrii.Solidity.Admin.Controllers
{
    public class RoyaltiesController : SolidityBaseController
    {
        public RoyaltiesController(IHostingEnvironment hostingEnvironment, ISolidityInfrastructure solidityInfrastructure,
            ISolidityService solidityService, INethereumService nethereumService)
            : base(hostingEnvironment, solidityInfrastructure, solidityService, nethereumService)
        { }

        public IActionResult Index()
        {
            string env = HttpContext.Session.GetString("Environment");

            var royalties = solidityService.GetRoyalties(solidityInfrastructure, SolidityEnvironment.Test);
            if (env == SolidityEnvironment.Production.ToString())
            {
                royalties = solidityService.GetRoyalties(solidityInfrastructure, SolidityEnvironment.Production);
            }

            return View(new RoyaltiesModel
            {
                Abi = royalties.Abi,
                Bin = royalties.Bin,
                ContractAddress = royalties.ContractAddress
            });
        }

        [HttpPost]
        public IActionResult Deploy(RoyaltiesModel model)
        {
            string env = HttpContext.Session.GetString("Environment");

            var royalties = solidityService.GetRoyalties(solidityInfrastructure, SolidityEnvironment.Test);
            if (env == SolidityEnvironment.Production.ToString())
            {
                royalties = solidityService.GetRoyalties(solidityInfrastructure, SolidityEnvironment.Production);
            }

            


            return View("Index", new RoyaltiesModel
            {
                Abi = royalties.Abi,
                Bin = royalties.Bin,
                ContractAddress = royalties.ContractAddress
            });
        }
    }
}