pragma solidity ^0.4.10;

// @title Implements access restriction to contract methods
contract AccessControl {

    /* These will be assigned at the construction
     * phase, where msg.sender is the account
     * creating this contract.
     */
    address private _owner = msg.sender;

    /* Allows execution of a function only when the caller is the owner of the contract. */
    modifier onlyOwner () {
        require(msg.sender == _owner);        
        _;
    }

    // @notice Changes the ownership of the contract
    // @param _newOwner The new owner of the contract
    function changeOwner(address _newOwner) public onlyOwner {
        _owner = _newOwner;
    }

}