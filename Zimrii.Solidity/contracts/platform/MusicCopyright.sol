pragma solidity ^0.4.10;

import "./AccessControl.sol";

/// @title Implements the storage of the music copyrights.
/// The underline structure maps a music guid to a copyright guid.
/// The guids are a reference to the Zimrii database.
/// From a music guid is possible to get the copyright guid and
/// the resource endpoint url from where to get all the data stored
/// for the copyright in the Zinrii database.
contract MusicCopyright is AccessControl {

    /* Default constructor.
     * The only reason to be here is for Nethereum:
     * System.NullReferenceException : Object reference not set to an instance of an object.
     * at Nethereum.Web3.DeployContract.BuildEncodedData(String abi, String contractByteCode, Object[] values)
     */
    function MusicCopyright() { }

    /* Event triggered when a copyrights is set.
     * Params:
     * musicId: The music guid. It's an indexed patameter.
     * copyrightId: The copyright guid.
     */
    event SetCopyright(bytes32 indexed musicId, bytes32 copyrightId);

    /* Copyright structure. */
    struct Copyright {
        /* Used to check if the record exists. */
        bool exists;
        /* Represents the guid of the copyright. */
        bytes32 copyrightId;  
         /* Represents the hash of the copyright. */
        bytes32 copyrightHash;     
    }
    
    /* Holds the root endpoint url.
     * This is used to build the endpoint for a copyright.
     * For example: https://zimrii.api.com/copyrights/<copyright guid>
     */
    string private copyrightEndpointResourceRoot = "";

    /* Holds all the mappings music -> copyrights. */
    mapping(bytes32 => Copyright) private copyrights;
    
    /// @notice Sets a copyright into the mapping.
    /// @param _musicId the guid of the music.
    /// @param _copyrightId the guid of the copyright.
    /// @param _copyrightHash the hash of the copyright.
    function setCopyright(bytes32 _musicId, bytes32 _copyrightId, bytes32 _copyrightHash) onlyOwner {
        copyrights[_musicId] = Copyright(true, _copyrightId, _copyrightHash);   

        // todo: do I need to return also  data: {'endpoint': 'xxxx'}
        SetCopyright(_musicId, _copyrightId);
    }

    /// @notice Gets the copyright guid for a particular piece of music.
    /// @param _musicId The guid of the music.
    /// @return The guid of the copyright.
    function getCopyrightId(bytes32 _musicId) constant returns (bytes32 res) {
         res = "";
        
        if (copyrights[_musicId].exists) {
            Copyright memory copyright = copyrights[_musicId];            
            res = copyright.copyrightId;
        }

        return res;
    }

    /// @notice Gets the copyright hash for a particular piece of music.
    /// @param _musicId The guid of the music.
    /// @return The guid of the copyright.
    function getCopyrightHash(bytes32 _musicId) constant returns (bytes32 res) {
         res = "";
        
        if (copyrights[_musicId].exists) {
            Copyright memory copyright = copyrights[_musicId];            
            res = copyright.copyrightHash;
        }

        return res;
    }

    /// @notice Sets the root endpoint for copyright references.
    /// @param _endpoint The root endpoint.
    function setCopyrightEndpointResourceRoot(string _endpoint) onlyOwner {
        copyrightEndpointResourceRoot = _endpoint;
    }

    /// @notice Gets the endpoint for the copyright of the music specified.
    /// @param _musicId The guid of the music.
    /// @return The endpoint for example https://zimrii.api.com/copyrights/<copyright guid>.
    function getCopyrightResourceEndpoint(bytes32 _musicId) constant returns (string res) {
         res = "";
        
        if (copyrights[_musicId].exists) {
            Copyright memory copyright = copyrights[_musicId];            
            res = strConcat(copyrightEndpointResourceRoot, copyright.copyrightId);
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