using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using Zimrii.Solidity.Admin.Services;

namespace Zimrii.Solidity.Admin.Models
{
    public class EthereumAccountModel
    {
        public string AccountAddress { get; set; }
        public SolidityEnvironment SolidityEnvironment { get; set; }

        public IEnumerable<SelectListItem> SolidityEnvironments => new List<SelectListItem>
        {
            new SelectListItem{ Value = "Test", Text = "Test" },
            new SelectListItem{ Value = "Production", Text = "Production" }
        };
    }
}