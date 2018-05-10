using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using Zimrii.Solidity.Admin.Services;

namespace Zimrii.Solidity.Admin.Models
{
    public class CopyrightsModel
    {
        public string Url { get; set; }
        public string AccountAddress { get; set; }
        public string Pwd { get; set; }
        public SolidityEnvironment SolidityEnvironment { get; set; }

        public IEnumerable<SelectListItem> SolidityEnvironments => new List<SelectListItem>
        {
            new SelectListItem{ Value = "Test", Text = "Test" },
            new SelectListItem{ Value = "Uat", Text = "Uat" },
            new SelectListItem{ Value = "Production", Text = "Production" }
        };

        public string CopyrightsAbi { get; set; }
        public string CopyrightsBin { get; set; }

        public string AccessControlAbi { get; set; }
        public string AccessControlBin { get; set; }

        public string ContractAddress { get; set; }
        public string CopyrightsGuid { get; set; }
        public string CopyrightsHash { get; set; }

        public string CopyrightsHashResult { get; set; }

        public Result DeployResult { get; set; }
        public Result SetCopyrightsResult { get; set; }
        public Result GetCopyrightsResult { get; set; }

    }

}