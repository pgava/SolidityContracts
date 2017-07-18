pragma solidity ^0.4.10;
import "./AccessRestriction.sol";

// Implements the storing of the music copyrights.
// The underline structure maps a music guid to a copyright guid.
// The guids are a reference to the Zimrii database.
// From a music guid is possible to get the copyright guid and
// the resource endpoint url from where to get all the data stored
// for the copyright in the Zinrii database.
contract MusicCopyright is AccessRestriction {

    // Default constructor.
    // The only reason to be here is for Nethereum:
    // System.NullReferenceException : Object reference not set to an instance of an object.
    // at Nethereum.Web3.DeployContract.BuildEncodedData(String abi, String contractByteCode, Object[] values)
    function MusicCopyright() { }

    // Event triggered when a copyrights is set.
    // Params:
    // musicId: The music guid. It's an indexed patameter.
    // copyrightId: The copyright guid.
    event SetCopyright(
        string indexed musicId, 
        string copyrightId
    );

    // Copyright structure.
    struct Copyright {
        // Used to check if the record exists
        bool exists;
        // Represents the guid of the copyright
        string copyrightId;
        // Is the record active? 
        bool active;
    }
    
    // Holds the root endpoint url.
    // This is used to build the endpoint for a copyright.
    // For example: https://zimrii.api.com/copyrights/<copyright guid>
    string copyrightEndpointResourceRoot = "";

    // Holds all the mappings music -> copyrights
    mapping(string => Copyright) copyrights;
    
    // Sets a copyright into the mapping
    // Params:
    // musicId: the guid of the music
    // copyrightId: the guid of the copyright
    // Remarks:
    // Can only be executed by Zimrii
    function setCopyright(
        string musicId,
        string copyrightId
    ) onlyOwner payable {
        
        copyrights[musicId] = Copyright(true, copyrightId, true);   

        // todo: do I need to return also  data: {'endpoint': 'xxxx'}
        SetCopyright(musicId, copyrightId);
    }

    // Gets the copyright for a particular piece of music
    // Params:
    // musicId: The guid of the music
    // Returns:
    // The guid of the copyright
    function getCopyrightId(
        string musicId
    ) constant returns (string res) {
         res = "";
        
        if (copyrights[musicId].exists) {
            if (copyrights[musicId].active) {
                Copyright copyright = copyrights[musicId];            
                res = copyright.copyrightId;
            }
        }

        return res;
    }

    // Sets the roor endpoint for copyright references
    // Params:
    // endpoint: the root endpoint    
    function setCopyrightEndpointResourceRoot(
        string endpoint
    ) onlyOwner payable {
        
        copyrightEndpointResourceRoot = endpoint;
    }

    // Gets the endpoint for the copyright of the music specified
    // Params:
    // musicId: The guid of the music
    // Returns:
    // the endpoint for example https://zimrii.api.com/copyrights/<copyright guid>
    function getCopyrightResourceEndpoint(
        string musicId
    ) constant returns (string res) {
         res = "";
        
        if (copyrights[musicId].exists) {
            if (copyrights[musicId].active) {
                Copyright copyright = copyrights[musicId];            
                res = strConcat(copyrightEndpointResourceRoot, copyright.copyrightId);
            }
        }

        return res;
    }

    // Concatenats 2 strings
    // Params:
    // a: The frist string to concatenate
    // b: The second string to concatenate
    // Returns:
    // a string which is a + b 
    function strConcat(
        string a, 
        string b
    ) internal returns (string) {
        bytes memory ba = bytes(a);
        bytes memory bb = bytes(b);
        string memory ab = new string(ba.length + bb.length);
        bytes memory bab = bytes(ab);
        uint k = 0;
        for (uint i = 0; i < ba.length; i++) bab[k++] = ba[i];
        for (i = 0; i < bb.length; i++) bab[k++] = bb[i];

        return string(bab);
    }

}