pragma solidity ^0.4.10;
import "./AccessRestriction.sol";

// Implements the storage of the artist contracts.
// The underline structure maps a contract guid to a contract hash.
// The guids are a reference to the Zimrii database.
// The hash must match the data stored in the zimrii database.
// From the contract guid is possible to get the contract hash and
// the resource endpoint from where to get all the data stored
// in the Zinrii database.
contract ArtistContract is AccessRestriction {

    // Default constructor.
    // The only reason to be here is for Nethereum:
    // System.NullReferenceException : Object reference not set to an instance of an object.
    // at Nethereum.Web3.DeployContract.BuildEncodedData(String abi, String contractByteCode, Object[] values)
    function ArtistContract() { }

    // Event triggered when a contract is set.
    // Params:
    // contractId: The contract guid. It's an indexed patameter.
    // contractHash: The hash of the contract.
    event SetContract(
        bytes32 indexed contractId, 
        bytes32 contractHash
    );

    // Contract structure.
    struct Contract {
        // Used to check if the record exists
        bool exists;
        // Represents the hash of the contract
        bytes32 contractHash;
    }
    
    // Holds the root endpoint url.
    // This is used to build the endpoint for a contract.
    // For example: https://zimrii.api.com/contracts/<contract guid>
    string contractEndpointResourceRoot = "";

    // Holds all the mappings for contracts
    mapping(bytes32 => Contract) contracts;
    
    // Sets a contract in the mapping structure
    // Params:
    // contractId: the guid of the contract
    // contractHash: the guid of the contract
    // Remarks:
    // Can only be executed by Zimrii
    function setContract(
        bytes32 contractId,
        bytes32 contractHash
    ) onlyOwner payable 
    {
        
        contracts[contractId] = Contract(true,  contractHash);   

        // todo: do I need to return also  data: {'endpoint': 'xxxx'}
        SetContract(contractId, contractHash);
    }

    // Gets the contract hash
    // Params:
    // contractId: The guid of the contract
    // Returns:
    // The hash of the contract
    function getContractHash(
        bytes32 contractId
    ) constant returns (bytes32 res) 
    {
         res = "";
        
        if (contracts[contractId].exists) {
            Contract memory c = contracts[contractId];            
            res = c.contractHash;
        }

        return res;
    }

    // Sets the root endpoint for contract reference
    // Params:
    // endpoint: the root endpoint    
    function setContractEndpointResourceRoot(
        string endpoint
    ) onlyOwner payable 
    {
        
        contractEndpointResourceRoot = endpoint;
    }

    // Gets the endpoint for the contract
    // Params:
    // contractId: The guid of the contract
    // Returns:
    // the endpoint for example https://zimrii.api.com/contracts/<contract guid>
    function getContractResourceEndpoint(
        bytes32 contractId
    ) constant returns (string res) 
    {
         res = "";
        
        if (contracts[contractId].exists) {                   
            res = strConcat(contractEndpointResourceRoot, contractId);
        }

        return res;
    }

    /**
     * Kill function to end the contract (useful for hard forks).
     */
    function kill() onlyOwner returns(uint) {
        selfdestruct(msg.sender);
    }

    // Concatenates a string to a bytes32
    // Params:
    // a: The frist string to concatenate
    // b: The second bytes32 to concatenate
    // Returns:
    // a string which is a + b 
    function strConcat(
        string a, 
        bytes32 b
    ) internal returns (string) 
    {
        bytes memory ba = bytes(a);

        // get length of bytes32
        uint len = 0;
        for (uint j = 0; j < 32; j++) {
            byte c = byte(bytes32(uint(b) * 2 ** (8 * j)));
            if (c != 0) {
                len += 1;
            }            
        }

        string memory ab = new string(ba.length + len);
        bytes memory bab = bytes(ab);
        uint k = 0;
        for (uint i = 0; i < ba.length; i++) {
            bab[k++] = ba[i];
        }
        for (i = 0; i < b.length; i++) {
            byte char = byte(bytes32(uint(b) * 2 ** (8 * i)));
            if (char != 0) {
                bab[k++] = char;
            }            
        }

        return string(bab);
    }

}