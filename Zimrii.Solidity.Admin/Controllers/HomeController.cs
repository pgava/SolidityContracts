using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Zimrii.Solidity.Admin.Models;
using Zimrii.Solidity.Admin.Services;

namespace Zimrii.Solidity.Admin.Controllers
{
    public class HomeController : SolidityBaseController
    {
        public HomeController(IHostingEnvironment hostingEnvironment, ISolidityInfrastructure solidityInfrastructure, ISolidityService solidityService) 
            : base(hostingEnvironment, solidityInfrastructure, solidityService)
        { }

        public IActionResult Index()
        {
            var eth = solidityService.GetEthAccount(solidityInfrastructure, SolidityEnvironment.Test);
            //var royalties = solidityService.GetRoyalties(solidityInfrastructure, SolidityEnvironment.Test);

            return View(new EthereumAccountModel {
                AccountAddress = eth.AccountAddress,
                SolidityEnvironment = SolidityEnvironment.Test
            });
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
