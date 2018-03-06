using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using Zimrii.Solidity.Admin.Services;

namespace Zimrii.Solidity.Admin.Models
{
    public class RoyaltiesModel
    {
        public string Url { get; set; }
        public string AccountAddress { get; set; }
        public SolidityEnvironment SolidityEnvironment { get; set; }

        public IEnumerable<SelectListItem> SolidityEnvironments => new List<SelectListItem>
        {
            new SelectListItem{ Value = "Test", Text = "Test" },
            new SelectListItem{ Value = "Production", Text = "Production" }
        };

        public string Abi { get; set; }
        public string Bin { get; set; }
        public string ContractAddress { get; set; }
        public string RoyaltiesGuid { get; set; }
        public string RoyaltiesHash { get; set; }

        public bool ShowDeployResult { get; set; }
        public string DeployResultType { get; set; }
        public string DeployResult { get; set; }
    }
}