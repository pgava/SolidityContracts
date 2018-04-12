using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Zimrii.Solidity.Admin.Models;
using Zimrii.Solidity.Admin.Services;

namespace Zimrii.Solidity.Admin.Controllers
{
    public class CopyrightsController : SolidityBaseController
    {
        private readonly ILogger<CopyrightsController> logger;

        public CopyrightsController(IHostingEnvironment hostingEnvironment, ISolidityInfrastructure solidityInfrastructure,
            ISolidityService solidityService, INethereumService nethereumService, ILogger<CopyrightsController> logger)
            : base(hostingEnvironment, solidityInfrastructure, solidityService, nethereumService)
        {
            this.logger = logger;
        }

        public IActionResult Index()
        {
            var eth = HttpContext.Session.GetObjectFromJson<EthereumAccountModel>("EthereumAccountModel");

            var copyrights = solidityService.GetCopyrights(solidityInfrastructure, eth.SolidityEnvironment);

            return View(new CopyrightsModel
            {
                AccessControlAbi = copyrights.AccessControlAbi,
                AccessControlBin = copyrights.AccessControlBin,
                CopyrightsAbi = copyrights.CopyrightsAbi,
                CopyrightsBin = copyrights.CopyrightsBin,
                ContractAddress = copyrights.CopyrightsContractAddress,
                DeployResult = new Result(),
                SetCopyrightsResult = new Result(),
                GetCopyrightsResult = new Result()
            });
        }

        [HttpPost]
        public async Task<IActionResult> DeployAsync(CopyrightsModel model)
        {
            var eth = HttpContext.Session.GetObjectFromJson<EthereumAccountModel>("EthereumAccountModel");

            var copyrights = solidityService.GetCopyrights(solidityInfrastructure, eth.SolidityEnvironment);

            var receiptAccessControl = await nethereumService.DeployContractAsync(eth.Url, model.AccessControlAbi, model.AccessControlBin, eth.AccountAddress, model.Pwd, eth.IsMine);

            var receiptDataAccessControl = new
            {
                TransactionHash = receiptAccessControl.TransactionHash,
                ContractAddress = receiptAccessControl.ContractAddress,
                BlockHash = receiptAccessControl.BlockHash,
                BlockNumber = receiptAccessControl.BlockNumber.Value.ToString(),
                CumulativeGasUsed = receiptAccessControl.CumulativeGasUsed.Value.ToString(),
                GasUsed = receiptAccessControl.GasUsed.Value.ToString(),
                TransactionIndex = receiptAccessControl.TransactionIndex.Value.ToString()
            };
            logger.LogInformation("{@receiptDataAccessControl}", receiptDataAccessControl);

            copyrights.AccessControlAbi = model.AccessControlAbi;
            copyrights.AccessControlBin = model.AccessControlBin;

            var receiptCopyrights = await nethereumService.DeployContractAsync(eth.Url, model.CopyrightsAbi, model.CopyrightsBin, eth.AccountAddress, model.Pwd, eth.IsMine);

            var receiptDataCopyrights = new
            {
                TransactionHash = receiptCopyrights.TransactionHash,
                ContractAddress = receiptCopyrights.ContractAddress,
                BlockHash = receiptCopyrights.BlockHash,
                BlockNumber = receiptCopyrights.BlockNumber.Value.ToString(),
                CumulativeGasUsed = receiptCopyrights.CumulativeGasUsed.Value.ToString(),
                GasUsed = receiptCopyrights.GasUsed.Value.ToString(),
                TransactionIndex = receiptCopyrights.TransactionIndex.Value.ToString()
            };
            logger.LogInformation("{@receiptDataCopyrights}", receiptDataCopyrights);

            copyrights.CopyrightsAbi = model.CopyrightsAbi;
            copyrights.CopyrightsBin = model.CopyrightsBin;

            copyrights.CopyrightsContractAddress = receiptCopyrights.ContractAddress;
            copyrights.AccessControlContractAddress = receiptAccessControl.ContractAddress;

            solidityService.SetCopyrights(solidityInfrastructure, eth.SolidityEnvironment, copyrights);

            return View("Index", new CopyrightsModel
            {
                AccessControlAbi = copyrights.AccessControlAbi,
                AccessControlBin = copyrights.AccessControlBin,
                CopyrightsAbi = copyrights.CopyrightsAbi,
                CopyrightsBin = copyrights.CopyrightsBin,
                ContractAddress = copyrights.CopyrightsContractAddress,
                DeployResult = new Result
                {
                    Message = "All good",
                    ResultType = "info",
                    ShowResult = true
                },
                SetCopyrightsResult = new Result(),
                GetCopyrightsResult = new Result()
            });
        }

        [HttpPost]
        public async Task<IActionResult> GetCopyrights(CopyrightsModel model)
        {
            var eth = HttpContext.Session.GetObjectFromJson<EthereumAccountModel>("EthereumAccountModel");

            var copyrights = solidityService.GetCopyrights(solidityInfrastructure, eth.SolidityEnvironment);

            var receiptSetCopyrights = await nethereumService.SetCopyrightsAsync(eth.Url, copyrights.CopyrightsAbi, eth.AccountAddress, copyrights.CopyrightsContractAddress, 
                model.CopyrightsGuid, model.CopyrightsHash, model.Pwd, eth.IsMine);

            var receiptDataSetCopyrights = new
            {
                TransactionHash = receiptSetCopyrights.TransactionHash,
                ContractAddress = receiptSetCopyrights.ContractAddress,
                BlockHash = receiptSetCopyrights.BlockHash,
                BlockNumber = receiptSetCopyrights.BlockNumber.Value.ToString(),
                CumulativeGasUsed = receiptSetCopyrights.CumulativeGasUsed.Value.ToString(),
                GasUsed = receiptSetCopyrights.GasUsed.Value.ToString(),
                TransactionIndex = receiptSetCopyrights.TransactionIndex.Value.ToString()
            };
            logger.LogInformation("{@receiptDataSetCopyrights}", receiptDataSetCopyrights);

            return View("Index", new CopyrightsModel
            {
                AccessControlAbi = copyrights.AccessControlAbi,
                AccessControlBin = copyrights.AccessControlBin,
                CopyrightsAbi = copyrights.CopyrightsAbi,
                CopyrightsBin = copyrights.CopyrightsBin,
                ContractAddress = copyrights.CopyrightsContractAddress,
                DeployResult = new Result(),
                SetCopyrightsResult = new Result
                {
                    Message = "All good",
                    ResultType = "info",
                    ShowResult = true
                },
                GetCopyrightsResult = new Result()
            });
        }

        [HttpPost]
        public async Task<IActionResult> SetCopyrights(CopyrightsModel model)
        {
            var eth = HttpContext.Session.GetObjectFromJson<EthereumAccountModel>("EthereumAccountModel");

            var copyrights = solidityService.GetCopyrights(solidityInfrastructure, eth.SolidityEnvironment);

            var hash = await nethereumService.GetCopyrightsAsync(eth.Url, copyrights.CopyrightsAbi, eth.AccountAddress, copyrights.CopyrightsContractAddress, model.CopyrightsGuid, model.Pwd);

            logger.LogInformation("{@hash}", new
            {
                Guid = model.CopyrightsGuid,
                Hash = hash
            });

            return View("Index", new CopyrightsModel
            {
                CopyrightsHashRead = hash,
                AccessControlAbi = copyrights.AccessControlAbi,
                AccessControlBin = copyrights.AccessControlBin,
                CopyrightsAbi = copyrights.CopyrightsAbi,
                CopyrightsBin = copyrights.CopyrightsBin,
                ContractAddress = copyrights.CopyrightsContractAddress,
                GetCopyrightsResult = new Result
                {
                    Message = $"Hash: {hash}",
                    ResultType = "info",
                    ShowResult = true
                },
                DeployResult = new Result(),
                SetCopyrightsResult = new Result()
            });
        }
    }
}