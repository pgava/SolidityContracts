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
                ContractAddress = royalties.RoyaltiesContractAddress
            });
        }

        [HttpPost]
        public async Task<IActionResult> DeployAsync(RoyaltiesModel model)
        {
            var eth = HttpContext.Session.GetObjectFromJson<EthereumAccountModel>("EthereumAccountModel");

            var royalties = solidityService.GetRoyalties(solidityInfrastructure, eth.SolidityEnvironment);

            var receiptAccessControl = await nethereumService.DeployContractAsync(eth.Url, model.AccessControlAbi, model.AccessControlBin, eth.AccountAddress, model.Password, eth.IsMine);

            var receiptDataAccessControl = new
            {
                TransactionHash = receiptAccessControl.TransactionHash,
                ContractAddress = receiptAccessControl.ContractAddress,
                BlockHash = receiptAccessControl.BlockHash,
                BlockNumber = receiptAccessControl.BlockNumber.Value,
                CumulativeGasUsed = receiptAccessControl.CumulativeGasUsed.Value,
                GasUsed = receiptAccessControl.GasUsed.Value,
                TransactionIndex = receiptAccessControl.TransactionIndex.Value
            };
            logger.LogInformation("{@receiptDataAccessControl}", receiptDataAccessControl);

            royalties.AccessControlAbi = model.AccessControlAbi;
            royalties.AccessControlBin = model.AccessControlBin;

            var receiptRoyalties = await nethereumService.DeployContractAsync(eth.Url, model.RoyaltiesAbi, model.RoyaltiesBin, eth.AccountAddress, model.Password, eth.IsMine);

            var receiptDataRoyalties = new
            {
                TransactionHash = receiptRoyalties.TransactionHash,
                ContractAddress = receiptRoyalties.ContractAddress,
                BlockHash = receiptRoyalties.BlockHash,
                BlockNumber = receiptRoyalties.BlockNumber.Value,
                CumulativeGasUsed = receiptRoyalties.CumulativeGasUsed.Value,
                GasUsed = receiptRoyalties.GasUsed.Value,
                TransactionIndex = receiptRoyalties.TransactionIndex.Value
            };
            logger.LogInformation("{@receiptDataRoyalties}", receiptDataRoyalties);

            royalties.RoyaltiesAbi = model.RoyaltiesAbi;
            royalties.RoyaltiesBin = model.RoyaltiesBin;

            royalties.RoyaltiesContractAddress = receiptAccessControl.ContractAddress;

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
                }
            });
        }

        [HttpPost]
        public async Task<IActionResult> SetRoyalties(RoyaltiesModel model)
        {
            var eth = HttpContext.Session.GetObjectFromJson<EthereumAccountModel>("EthereumAccountModel");

            var royalties = solidityService.GetRoyalties(solidityInfrastructure, eth.SolidityEnvironment);

            var receiptSetRoyalties = await nethereumService.SetRoyaltiesAsync(eth.Url, royalties.RoyaltiesAbi, eth.AccountAddress, royalties.RoyaltiesContractAddress, 
                model.RoyaltiesGuid, model.RoyaltiesHash, model.Password, eth.IsMine);

            var receiptDataSetRoyalties = new
            {
                TransactionHash = receiptSetRoyalties.TransactionHash,
                ContractAddress = receiptSetRoyalties.ContractAddress,
                BlockHash = receiptSetRoyalties.BlockHash,
                BlockNumber = receiptSetRoyalties.BlockNumber.Value,
                CumulativeGasUsed = receiptSetRoyalties.CumulativeGasUsed.Value,
                GasUsed = receiptSetRoyalties.GasUsed.Value,
                TransactionIndex = receiptSetRoyalties.TransactionIndex.Value
            };
            logger.LogInformation("{@receiptDataSetRoyalties}", receiptDataSetRoyalties);

            return View("Index", new RoyaltiesModel
            {
                AccessControlAbi = royalties.AccessControlAbi,
                AccessControlBin = royalties.AccessControlBin,
                RoyaltiesAbi = royalties.RoyaltiesAbi,
                RoyaltiesBin = royalties.RoyaltiesBin,
                ContractAddress = royalties.RoyaltiesContractAddress
            });
        }

        [HttpPost]
        public async Task<IActionResult> GetRoyalties(RoyaltiesModel model)
        {
            var eth = HttpContext.Session.GetObjectFromJson<EthereumAccountModel>("EthereumAccountModel");

            var royalties = solidityService.GetRoyalties(solidityInfrastructure, eth.SolidityEnvironment);

            var hash = await nethereumService.GetRoyaltiesAsync(eth.Url, royalties.RoyaltiesAbi, eth.AccountAddress, royalties.RoyaltiesContractAddress, model.RoyaltiesGuid, model.Password);

            logger.LogInformation("{@hash}", new
            {
                Guid = model.RoyaltiesGuid,
                Hash = hash
            });

            return View("Index", new RoyaltiesModel
            {
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
                }
            });
        }
    }
}