namespace Zimrii.Solidity.Admin.Services
{
    public interface ISolidityService
    {
        EthAccount GetEthAccount(ISolidityInfrastructure solidityInfrastructure, SolidityEnvironment environment);
        Royalties GetRoyalties(ISolidityInfrastructure solidityInfrastructure, SolidityEnvironment environment);
        Copyrights GetCopyrights(ISolidityInfrastructure solidityInfrastructure, SolidityEnvironment environment);
        void SetRoyalties(ISolidityInfrastructure solidityInfrastructure, SolidityEnvironment environment, Royalties royalties);
        void SetCopyrights(ISolidityInfrastructure solidityInfrastructure, SolidityEnvironment environment, Copyrights copyrights);
    }
}
