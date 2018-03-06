using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Zimrii.Solidity.Admin.Models;
using Zimrii.Solidity.Admin.Services;

namespace Zimrii.Solidity.Admin.Controllers
{
    public class HomeController : SolidityBaseController
    {
        // Ctor
        public HomeController(IHostingEnvironment hostingEnvironment, ISolidityInfrastructure solidityInfrastructure, 
            ISolidityService solidityService, INethereumService nethereumService)
            : base(hostingEnvironment, solidityInfrastructure, solidityService, nethereumService)
        { }

        public IActionResult Index()
        {
            var eth = solidityService.GetEthAccount(solidityInfrastructure, SolidityEnvironment.Test);

            return View(new EthereumAccountModel
            {
                Url = eth.Url,
                AccountAddress = eth.AccountAddress,
                SolidityEnvironment = SolidityEnvironment.Test
            });
        }

        [HttpPost]
        public IActionResult Index(string solidityEnvironment, string pwd)
        {
            EthAccount eth = null;
            SolidityEnvironment solEnv = SolidityEnvironment.Test;

            if (solidityEnvironment == SolidityEnvironment.Production.ToString())
            {
                eth = solidityService.GetEthAccount(solidityInfrastructure, SolidityEnvironment.Production);
            }

            eth = solidityService.GetEthAccount(solidityInfrastructure, solEnv);

            // TODO use another class 
            HttpContext.Session.SetObjectAsJson("EthereumAccountModel", new EthereumAccountModel
            {
                AccountAddress = eth.AccountAddress,
                Url = eth.Url,
                SolidityEnvironment = solEnv
            });

            //var res = nethereumService.UnlockAccountAsync(eth.Url, eth.AccountAddress, pwd);

            var res = true;
            return View(new EthereumAccountModel
            {
                AccountAddress = eth.AccountAddress,
                SolidityEnvironment = solEnv,
                ShowUnlockResult = true,
                UnlockResult = res ? "All good, account unlocked" : "Couldn't unlock the account",
                UnlockResultType = res ? "info" : "danger"
            });
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
