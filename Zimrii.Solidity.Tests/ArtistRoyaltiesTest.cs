using System.Threading.Tasks;
using FluentAssertions;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Hex.HexTypes;
using Xunit;

namespace Zimrii.Solidity.Tests
{
    public class ArtistRoyaltiesTest : NethereumTestsBase
    {

        public ArtistRoyaltiesTest() : base(new[] { "Owned", "ArtistRoyalties" })
        {
        }

        [Fact]
        public async Task ArtistRoyaltiesSolidityMethodsTest()
        {
            await Setup(true);

            var contractAddress = Receipts["ArtistRoyalties"].ContractAddress;
            var contract = Web3.Eth.GetContract(Abi["ArtistRoyalties"], contractAddress);

            var addRoyaltiesEvent = contract.GetEvent("SetRoyalties");
            var filterAll = await addRoyaltiesEvent.CreateFilterAsync();
            var filterRoyaltiesId = await addRoyaltiesEvent.CreateFilterAsync("royaltiesId");

            var setRoyalties = contract.GetFunction("setRoyalties");
            var getRoyaltiesHash = contract.GetFunction("getRoyaltiesHash");

            var unlockResult =
                await Web3.Personal.UnlockAccount.SendRequestAsync(AccountAddress, PassPhrase, new HexBigInteger(120));
            unlockResult.Should().BeTrue();

            var gas = await setRoyalties.EstimateGasAsync(AccountAddress, null, null, "61BF375C77BC4C089DCAD2AC4935E600", "6B2M2Y8AsgTpgAmY7PhCfg==");
            var receipt1 = await setRoyalties.SendTransactionAndWaitForReceiptAsync(AccountAddress, gas, null, null, "61BF375C77BC4C089DCAD2AC4935E600", "6B2M2Y8AsgTpgAmY7PhCfg==");

            var log = await addRoyaltiesEvent.GetFilterChanges<AddRoyaltiesEvent>(filterAll);
            log.Count.Should().Be(1);

            var logMusicId = await addRoyaltiesEvent.GetFilterChanges<AddRoyaltiesEvent>(filterRoyaltiesId);
            logMusicId.Count.Should().Be(1);

            var res1 = await getRoyaltiesHash.CallAsync<string>("royaltiesId");
            res1.Should().Be("royaltiesHash");
        }
    }

    public class AddRoyaltiesEvent
    {
        [Parameter("bytes32", "royaltiesId", 1, true)]
        public string RoyaltiesId { get; set; }

        [Parameter("bytes32", "royaltiesHash", 2, false)]
        public string RoyaltiesHash { get; set; }
    }
}
