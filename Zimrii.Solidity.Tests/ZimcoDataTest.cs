using System.Threading.Tasks;
using FluentAssertions;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.DebugGeth.DTOs;
using Xunit;

namespace Zimrii.Solidity.Tests
{
    public class ZimcoDataTest : NethereumTestsBase
    {

        public ZimcoDataTest() : base(new[] { "owned", "TokenData" })
        {
            RootPath = @"..\..\..\..\..\Zimrii.Solidity\contracts\zimco\metadata";
        }

        [Fact]
        public async Task DeployZimcoTest()
        {
            await Setup(true);
        }
    }

}
