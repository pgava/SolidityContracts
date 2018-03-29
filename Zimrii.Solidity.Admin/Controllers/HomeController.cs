using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Zimrii.Solidity.Admin.Models;
using Zimrii.Solidity.Admin.Services;

namespace Zimrii.Solidity.Admin.Controllers
{
    public class HomeController : SolidityBaseController
    {
        private readonly ILogger<HomeController> logger;

        // Ctor
        public HomeController(IHostingEnvironment hostingEnvironment, ISolidityInfrastructure solidityInfrastructure, 
            ISolidityService solidityService, INethereumService nethereumService, ILogger<HomeController> logger)
            : base(hostingEnvironment, solidityInfrastructure, solidityService, nethereumService)
        {
            this.logger = logger;
        }

        public IActionResult Index()
        {
            var eth = solidityService.GetEthAccount(solidityInfrastructure, SolidityEnvironment.Test);

            HttpContext.Session.SetObjectAsJson("EthereumAccountModel", new EthereumAccountModel
            {
                AccountAddress = eth.AccountAddress,
                Url = eth.Url,
                SolidityEnvironment = SolidityEnvironment.Test,
                IsMine = eth.IsMine
            });


            return View(new EthereumAccountModel
            {
                Url = eth.Url,
                AccountAddress = eth.AccountAddress,
                SolidityEnvironment = SolidityEnvironment.Test
            });
        }

        [HttpPost]
        public async Task<IActionResult> Index(string solidityEnvironment, string pwd)
        {
            EthAccount eth = null;
            SolidityEnvironment solEnv = SolidityEnvironment.Test;

            if (solidityEnvironment == SolidityEnvironment.Production.ToString())
            {
                eth = solidityService.GetEthAccount(solidityInfrastructure, SolidityEnvironment.Production);
            }

            eth = solidityService.GetEthAccount(solidityInfrastructure, solEnv);

            HttpContext.Session.SetObjectAsJson("EthereumAccountModel", new EthereumAccountModel
            {
                AccountAddress = eth.AccountAddress,
                Url = eth.Url,
                SolidityEnvironment = solEnv,
                IsMine = eth.IsMine
            });

            var isUnlocked = await nethereumService.UnlockAccountAsync(eth.Url, eth.AccountAddress, pwd);
            //var isUnlocked = true;

            logger.LogInformation("{@zimco}", new
            {
                Url = eth.Url,
                AccountAddress = eth.AccountAddress,
                UnLock = isUnlocked
            });

            var res = isUnlocked;
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
