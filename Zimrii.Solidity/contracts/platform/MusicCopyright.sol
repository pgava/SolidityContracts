pragma solidity ^0.4.10;

import "./AccessControl.sol";

/// @title Implements the storage of the music copyrights.
/// The underline structure maps a music guid to a copyright guid.
/// The guids are a reference to the Zimrii database.
/// From a music guid is possible to get the copyright guid and
/// the resource endpoint url from where to get all the data stored
/// for the copyright in the Zinrii database.
contract MusicCopyright is AccessControl {

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
    function setCopyright(bytes32 _musicId, bytes32 _copyrightId, bytes32 _copyrightHash) public onlyOwner returns(bytes32 id) {
        copyrights[_musicId] = Copyright(true, _copyrightId, _copyrightHash);   

        // todo: do I need to return also  data: {'endpoint': 'xxxx'}
        emit SetCopyright(_musicId, _copyrightId);

        return _musicId;
    }

    /// @notice Gets the copyright guid for a particular piece of music.
    /// @param _musicId The guid of the music.
    /// @return The guid of the copyright.
    function getCopyrightId(bytes32 _musicId) public view returns (bytes32 res) {
         res = "";
        
        if (copyrights[_musicId].exists) {
            res = copyrights[_musicId].copyrightId;
        }

        return res;
    }

    /// @notice Gets the copyright hash for a particular piece of music.
    /// @param _musicId The guid of the music.
    /// @return The hash of the copyright.
    function getCopyrightHash(bytes32 _musicId) public view returns (bytes32 res2) {
         res2 = "";
        
        if (copyrights[_musicId].exists) {
            res2 = copyrights[_musicId].copyrightHash;
        }

        return res2;
    }

    function multiply(uint a) public pure returns(uint d) {
             return a * 5;
        }
}