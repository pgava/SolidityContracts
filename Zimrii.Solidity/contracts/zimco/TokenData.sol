pragma solidity ^0.4.13;

import "./Owned.sol";

/// @title Implements the token database
contract TokenData is Owned { 
    
    /* This creates an array with all balances */
    mapping (address => uint256) private balanceOf;
    mapping (address => mapping (address => uint256)) private allowance;
    mapping (address => bool) private frozenAccount;

    /// @notice Sets the balance of a user
    /// @param _user The user to change the balance
    /// @param _newValue The new value to assign
    function setBalanceOf(address _user, uint256 _newValue) onlyOwner {
        balanceOf[_user] = _newValue;
    }

    /// @notice Gets the balance of a user
    /// @param _user The user to get the balance
    function getBalanceOf(address _user) constant returns (uint256) {
        return balanceOf[_user];
    }

    /// @notice Sets the allowance 
    /// @param _from The from user
    /// @param _to The to user
    /// @param _newValue the new value
    function setAllowance(address _from, address _to, uint256 _newValue) onlyOwner {
        allowance[_from][_to] = _newValue;
    }

    /// @notice Gets the allowance 
    /// @param _from The from user
    /// @param _to The to user
    function getAllowance(address _from, address _to) constant returns (uint256) {
        return allowance[_from][_to];
    }

    /// @notice Sets the account of user to frozen status
    /// @param _user The user to change the frozen status
    /// @param _newValue The new value to assign
    function setFrozenAccount(address _user, bool _newValue) onlyOwner {
        frozenAccount[_user] = _newValue;
    }

    /// @notice Gets the balance of a user
    /// @param _user The user to get the frozen status
    function getFrozenAccount(address _user) constant returns (bool) {
        return frozenAccount[_user];
    }
}