pragma solidity ^0.4.10;

import "./AccessControl.sol";

/// @title Implements the storage of the artist royalties.
/// The underline structure maps a royalties guid to a royalties hash.
/// The guids are a reference to the Zimrii database.
/// The hash must match the data stored in the zimrii database.
/// From the royalties guid is possible to get the royalties hash and
/// the resource endpoint url from where to get all the data stored
/// in the Zinrii database.
contract ArtistRoyalties is AccessControl {

    /* Default constructor.
     * The only reason to be here is for Nethereum:
     * System.NullReferenceException : Object reference not set to an instance of an object.
     * at Nethereum.Web3.DeployContract.BuildEncodedData(String abi, String contractByteCode, Object[] values)
     */
    function ArtistRoyalties() { }

    /* Event triggered when a royalties is set. */
    event SetRoyalties(bytes32 indexed royaltiesId, bytes32 royaltiesHash);

    /* Royalties structure. */
    struct RoyaltiesData {
        /* Used to check if the record exists. */
        bool exists;
        /* Represents the hash of the royalties. */
        bytes32 royaltiesHash;
    }
    
    /* Holds the root endpoint url.
     * This is used to build the endpoint for a royalties.
     * For example: https://zimrii.api.com/royalties/<royalties guid>
     */
    string private royaltiesEndpointResourceRoot = "";

    /* Holds all the mappings for royalties. */
    mapping(bytes32 => RoyaltiesData) private royalties;
    
    /// @notice Sets a royalties into the mapping.
    /// @param _royaltiesId The guid of the music.
    /// @param _royaltiesHash The guid of the copyright.
    function setRoyalties(bytes32 _royaltiesId, bytes32 _royaltiesHash) onlyOwner {
        
        royalties[_royaltiesId] = RoyaltiesData(true,  _royaltiesHash);   

        // todo: do I need to return also  data: {'endpoint': 'xxxx'}
        SetRoyalties(_royaltiesId, _royaltiesHash);
    }

    /// @notice Gets the royalties hash.
    /// @param _royaltiesId The guid of the royalties.
    /// @return The hash of the royalties.
    function getRoyaltiesHash(bytes32 _royaltiesId) constant returns (bytes32 res) {
         res = "";
        
        if (royalties[_royaltiesId].exists) {
            RoyaltiesData memory r = royalties[_royaltiesId];            
            res = r.royaltiesHash;
        }

        return res;
    }

    /// @notice Sets the root endpoint for royalties references.
    /// @param _endpoint the root endpoint.
    function setRoyaltiesEndpointResourceRoot(string _endpoint) onlyOwner {
        royaltiesEndpointResourceRoot = _endpoint;
    }

    /// @notice Gets the endpoint for the royalties.
    /// @param _royaltiesId The guid of the royalties.
    /// @return The endpoint for example https://zimrii.api.com/royalties/<royalties guid>.
    function getRoyaltiesResourceEndpoint(bytes32 _royaltiesId) constant returns (string res) {
         res = "";
        
        if (royalties[_royaltiesId].exists) {                   
            res = strConcat(royaltiesEndpointResourceRoot, _royaltiesId);
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