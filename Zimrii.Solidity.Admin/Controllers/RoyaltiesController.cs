using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Zimrii.Solidity.Admin.Models;
using Zimrii.Solidity.Admin.Services;

namespace Zimrii.Solidity.Admin.Controllers
{
    public class RoyaltiesController : SolidityBaseController
    {
        private readonly ILogger<RoyaltiesController> logger;

        public RoyaltiesController(IHostingEnvironment hostingEnvironment, ISolidityInfrastructure solidityInfrastructure,
            ISolidityService solidityService, INethereumService nethereumService, ILogger<RoyaltiesController> logger)
            : base(hostingEnvironment, solidityInfrastructure, solidityService, nethereumService)
        {
            this.logger = logger;
        }

        public IActionResult Index()
        {
            var eth = HttpContext.Session.GetObjectFromJson<EthereumAccountModel>("EthereumAccountModel");

            var royalties = solidityService.GetRoyalties(solidityInfrastructure, eth.SolidityEnvironment);

            return View(new RoyaltiesModel
            {
                AccessControlAbi = royalties.AccessControlAbi,
                AccessControlBin = royalties.AccessControlBin,
                RoyaltiesAbi = royalties.RoyaltiesAbi,
                RoyaltiesBin = royalties.RoyaltiesBin,
                ContractAddress = royalties.RoyaltiesContractAddress,
                DeployResult = new Result(),
                SetRoyaltiesResult = new Result(),
                GetRoyaltiesResult = new Result()
            });
        }

        [HttpPost]
        public async Task<IActionResult> DeployAsync(RoyaltiesModel model)
        {
            var eth = HttpContext.Session.GetObjectFromJson<EthereumAccountModel>("EthereumAccountModel");

            var royalties = solidityService.GetRoyalties(solidityInfrastructure, eth.SolidityEnvironment);

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

            royalties.AccessControlAbi = model.AccessControlAbi;
            royalties.AccessControlBin = model.AccessControlBin;

            var receiptRoyalties = await nethereumService.DeployContractAsync(eth.Url, model.RoyaltiesAbi, model.RoyaltiesBin, eth.AccountAddress, model.Pwd, eth.IsMine);

            var receiptDataRoyalties = new
            {
                TransactionHash = receiptRoyalties.TransactionHash,
                ContractAddress = receiptRoyalties.ContractAddress,
                BlockHash = receiptRoyalties.BlockHash,
                BlockNumber = receiptRoyalties.BlockNumber.Value.ToString(),
                CumulativeGasUsed = receiptRoyalties.CumulativeGasUsed.Value.ToString(),
                GasUsed = receiptRoyalties.GasUsed.Value.ToString(),
                TransactionIndex = receiptRoyalties.TransactionIndex.Value.ToString()
            };
            logger.LogInformation("{@receiptDataRoyalties}", receiptDataRoyalties);

            royalties.RoyaltiesAbi = model.RoyaltiesAbi;
            royalties.RoyaltiesBin = model.RoyaltiesBin;

            royalties.RoyaltiesContractAddress = receiptRoyalties.ContractAddress;
            royalties.AccessControlContractAddress = receiptAccessControl.ContractAddress;

            solidityService.SetRoyalties(solidityInfrastructure, eth.SolidityEnvironment, royalties);

            return View("Index", new RoyaltiesModel
            {
                AccessControlAbi = royalties.AccessControlAbi,
                AccessControlBin = royalties.AccessControlBin,
                RoyaltiesAbi = royalties.RoyaltiesAbi,
                RoyaltiesBin = royalties.RoyaltiesBin,
                ContractAddress = royalties.RoyaltiesContractAddress,
                DeployResult = new Result
                {
                    Message = "All good",
                    ResultType = "info",
                    ShowResult = true
                },
                SetRoyaltiesResult = new Result(),
                GetRoyaltiesResult = new Result()
            });
        }

        [HttpPost]
        public async Task<IActionResult> SetRoyalties(RoyaltiesModel model)
        {
            var eth = HttpContext.Session.GetObjectFromJson<EthereumAccountModel>("EthereumAccountModel");

            var royalties = solidityService.GetRoyalties(solidityInfrastructure, eth.SolidityEnvironment);

            var receiptSetRoyalties = await nethereumService.SetRoyaltiesAsync(eth.Url, royalties.RoyaltiesAbi, eth.AccountAddress, royalties.RoyaltiesContractAddress, 
                model.RoyaltiesGuid, model.RoyaltiesHash, model.Pwd, eth.IsMine);

            var receiptDataSetRoyalties = new
            {
                TransactionHash = receiptSetRoyalties.TransactionHash,
                ContractAddress = receiptSetRoyalties.ContractAddress,
                BlockHash = receiptSetRoyalties.BlockHash,
                BlockNumber = receiptSetRoyalties.BlockNumber.Value.ToString(),
                CumulativeGasUsed = receiptSetRoyalties.CumulativeGasUsed.Value.ToString(),
                GasUsed = receiptSetRoyalties.GasUsed.Value.ToString(),
                TransactionIndex = receiptSetRoyalties.TransactionIndex.Value.ToString()
            };
            logger.LogInformation("{@receiptDataSetRoyalties}", receiptDataSetRoyalties);

            return View("Index", new RoyaltiesModel
            {
                AccessControlAbi = royalties.AccessControlAbi,
                AccessControlBin = royalties.AccessControlBin,
                RoyaltiesAbi = royalties.RoyaltiesAbi,
                RoyaltiesBin = royalties.RoyaltiesBin,
                ContractAddress = royalties.RoyaltiesContractAddress,
                DeployResult = new Result(),
                SetRoyaltiesResult = new Result
                {
                    Message = "All good",
                    ResultType = "info",
                    ShowResult = true
                },
                GetRoyaltiesResult = new Result()
            });
        }

        [HttpPost]
        public async Task<IActionResult> GetRoyalties(RoyaltiesModel model)
        {
            var eth = HttpContext.Session.GetObjectFromJson<EthereumAccountModel>("EthereumAccountModel");

            var royalties = solidityService.GetRoyalties(solidityInfrastructure, eth.SolidityEnvironment);

            var hash = await nethereumService.GetRoyaltiesAsync(eth.Url, royalties.RoyaltiesAbi, eth.AccountAddress, royalties.RoyaltiesContractAddress, model.RoyaltiesGuid, model.Pwd);

            logger.LogInformation("{@hash}", new
            {
                Guid = model.RoyaltiesGuid,
                Hash = hash
            });

            return View("Index", new RoyaltiesModel
            {
                RoyaltiesHashRead = hash,
                AccessControlAbi = royalties.AccessControlAbi,
                AccessControlBin = royalties.AccessControlBin,
                RoyaltiesAbi = royalties.RoyaltiesAbi,
                RoyaltiesBin = royalties.RoyaltiesBin,
                ContractAddress = royalties.RoyaltiesContractAddress,
                GetRoyaltiesResult = new Result
                {
                    Message = $"Hash: {hash}",
                    ResultType = "info",
                    ShowResult = true
                },
                DeployResult = new Result(),
                SetRoyaltiesResult = new Result()
            });
        }
    }
}