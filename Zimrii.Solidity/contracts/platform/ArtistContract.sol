pragma solidity ^0.4.10;

import "./AccessControl.sol";

/// @title Implements the storage of the artist contracts.
/// The underline structure maps a contract guid to a contract hash.
/// The guids are a reference to the Zimrii database.
/// The hash must match the data stored in the zimrii database.
/// From the contract guid is possible to get the contract hash and
/// the resource endpoint from where to get all the data stored
/// in the Zinrii database.
contract ArtistContract is AccessControl {

    /* Default constructor.
     * The only reason to be here is for Nethereum:
     * System.NullReferenceException : Object reference not set to an instance of an object.
     * at Nethereum.Web3.DeployContract.BuildEncodedData(String abi, String contractByteCode, Object[] values)
     */
    function ArtistContract() { }

    /* Event triggered when a contract is set. */
    event SetContract(bytes32 indexed contractId, bytes32 contractHash);

    /* Contract structure. */
    struct Contract {
        /* Used to check if the record exists. */
        bool exists;
        /* Represents the hash of the contract. */
        bytes32 contractHash;
    }
    
    /* Holds the root endpoint url.
     * This is used to build the endpoint for a contract.
     * For example: https:///zimrii.api.com/contracts/<contract guid>
     */
    string private contractEndpointResourceRoot = "";

    /* Holds all the mappings for contracts */
    mapping(bytes32 => Contract) private contracts;
    
    /// @notice Sets a contract in the mapping structure
    /// @param _contractId the guid of the contract
    /// @param _contractHash the hash of the contract
    function setContract(bytes32 _contractId, bytes32 _contractHash) onlyOwner {        
        contracts[_contractId] = Contract(true,  _contractHash);   
        /// todo: do I need to return also  data: {'endpoint': 'xxxx'}
        SetContract(_contractId, _contractHash);
    }

    /// @notice Gets the contract hash
    /// @param _contractId The guid of the contract
    /// @return The hash of the contract
    function getContractHash(bytes32 _contractId) constant returns (bytes32 res) {
         res = "";
        
        if (contracts[_contractId].exists) {
            Contract memory c = contracts[_contractId];            
            res = c.contractHash;
        }

        return res;
    }

    /// @notice Sets the root endpoint for contract reference
    /// @param _endpoint the root endpoint    
    function setContractEndpointResourceRoot(string _endpoint) onlyOwner {        
        contractEndpointResourceRoot = _endpoint;
    }

    /// @notice Gets the endpoint for the contract
    /// @param _contractId The guid of the contract
    /// @return The endpoint for example https:///zimrii.api.com/contracts/<contract guid>
    function getContractResourceEndpoint(bytes32 _contractId) constant returns (string res) {
         res = "";
        
        if (contracts[_contractId].exists) {                   
            res = strConcat(contractEndpointResourceRoot, _contractId);
        }

        return res;
    }

    /// @notice Kill function to end the contract (useful for hard forks).
    function kill() onlyOwner returns(uint) {
        selfdestruct(msg.sender);
    }

    /// @notice Concatenates a string to a bytes32
    /// @param _a The frist string to concatenate
    /// @param _b The second bytes32 to concatenate
    /// @return A string which is _a + _b 
    function strConcat(string _a, bytes32 _b) internal returns (string) {
        bytes memory ba = bytes(_a);

        /// get length of bytes32
        uint len = 0;
        for (uint j = 0; j < 32; j++) {
            byte c = byte(bytes32(uint(_b) * 2 ** (8 * j)));
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
        for (i = 0; i < _b.length; i++) {
            byte char = byte(bytes32(uint(_b) * 2 ** (8 * i)));
            if (char != 0) {
                bab[k++] = char;
            }            
        }

        return string(bab);
    }

}