namespace Zimrii.Solidity.Admin.Services
{
    public interface ISolidityService
    {
        EthAccount GetEthAccount(ISolidityInfrastructure solidityInfrastructure, SolidityEnvironment environment);
        Royalties GetRoyalties(ISolidityInfrastructure solidityInfrastructure, SolidityEnvironment environment);
    }
}
