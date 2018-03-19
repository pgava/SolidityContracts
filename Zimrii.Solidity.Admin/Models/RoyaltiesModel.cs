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
        public string Password { get; set; }
        public SolidityEnvironment SolidityEnvironment { get; set; }

        public IEnumerable<SelectListItem> SolidityEnvironments => new List<SelectListItem>
        {
            new SelectListItem{ Value = "Test", Text = "Test" },
            new SelectListItem{ Value = "Production", Text = "Production" }
        };

        public RoyaltiesModel()
        {
            DeployResult = new Result();
            SetRoyaltiesResult = new Result();
            GetRoyaltiesResult = new Result();
        }

        public string RoyaltiesAbi { get; set; }
        public string RoyaltiesBin { get; set; }

        public string AccessControlAbi { get; set; }
        public string AccessControlBin { get; set; }

        public string ContractAddress { get; set; }
        public string RoyaltiesGuid { get; set; }
        public string RoyaltiesHash { get; set; }
        
        public Result DeployResult { get; set; }
        public Result SetRoyaltiesResult { get; set; }
        public Result GetRoyaltiesResult { get; set; }

    }

    public class Result
    {
        public bool ShowResult { get; set; }
        public string ResultType { get; set; }
        public string Message { get; set; }
    }
}