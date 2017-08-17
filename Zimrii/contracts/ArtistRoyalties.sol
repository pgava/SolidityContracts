pragma solidity ^0.4.10;
import "./AccessRestriction.sol";

// Implements the storage of the artist royalties.
// The underline structure maps a royalties guid to a royalties hash.
// The guids are a reference to the Zimrii database.
// The hash must match the data stored in the zimrii database.
// From the royalties guid is possible to get the royalties hash and
// the resource endpoint url from where to get all the data stored
// in the Zinrii database.
contract ArtistRoyalties is AccessRestriction {

    // Default constructor.
    // The only reason to be here is for Nethereum:
    // System.NullReferenceException : Object reference not set to an instance of an object.
    // at Nethereum.Web3.DeployContract.BuildEncodedData(String abi, String contractByteCode, Object[] values)
    function ArtistRoyalties() { }

    // Event triggered when a royalties is set.
    // Params:
    // royaltiesId: The royalties guid. It's an indexed patameter.
    // royaltiesHash: The hash of the royalties.
    event SetRoyalties(
        bytes32 indexed royaltiesId, 
        bytes32 royaltiesHash
    );

    // Royalties structure.
    struct RoyaltiesData {
        // Used to check if the record exists
        bool exists;
        // Represents the hash of the royalties
        bytes32 royaltiesHash;
    }
    
    // Holds the root endpoint url.
    // This is used to build the endpoint for a royalties.
    // For example: https://zimrii.api.com/royalties/<royalties guid>
    string royaltiesEndpointResourceRoot = "";

    // Holds all the mappings for royalties
    mapping(bytes32 => RoyaltiesData) royalties;
    
    // Sets a royalties into the mapping
    // Params:
    // musicId: the guid of the music
    // copyrightId: the guid of the copyright
    // Remarks:
    // Can only be executed by Zimrii
    function setRoyalties(
        bytes32 royaltiesId,
        bytes32 royaltiesHash
    ) onlyOwner payable 
    {
        
        royalties[royaltiesId] = RoyaltiesData(true,  royaltiesHash);   

        // todo: do I need to return also  data: {'endpoint': 'xxxx'}
        SetRoyalties(royaltiesId, royaltiesHash);
    }

    // Gets the royalties hash
    // Params:
    // royaltiesId: The guid of the royalties
    // Returns:
    // The hash of the royalties
    function getRoyaltiesHash(
        bytes32 royaltiesId
    ) constant returns (bytes32 res) 
    {
         res = "";
        
        if (royalties[royaltiesId].exists) {
            RoyaltiesData memory r = royalties[royaltiesId];            
            res = r.royaltiesHash;
        }

        return res;
    }

    // Sets the root endpoint for royalties references
    // Params:
    // endpoint: the root endpoint    
    function setRoyaltiesEndpointResourceRoot(
        string endpoint
    ) onlyOwner payable 
    {
        
        royaltiesEndpointResourceRoot = endpoint;
    }

    // Gets the endpoint for the royalties
    // Params:
    // royaltiesId: The guid of the royalties
    // Returns:
    // the endpoint for example https://zimrii.api.com/royalties/<royalties guid>
    function getRoyaltiesResourceEndpoint(
        bytes32 royaltiesId
    ) constant returns (string res) 
    {
         res = "";
        
        if (royalties[royaltiesId].exists) {                   
            res = strConcat(royaltiesEndpointResourceRoot, royaltiesId);
        }

        return res;
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