pragma solidity ^0.4.13;

/// @title Implements the token database
contract TokenData { 

    /* This creates an array with all balances */
    mapping (address => uint256) private balanceOf;
    mapping (address => mapping (address => uint256)) private allowance;
    
    /// @notice Sets the balance of a user
    /// @param user The user to change the balance
    /// @param newValue The new value to assign
    function setBalanceOf(address user, uint256 newValue) {
        balanceOf[user] = newValue;
    }

    /// @notice Gets the balance of a user
    /// @param user The user to change the balance
    function getBalanceOf(address user) constant returns (uint256) {
        return balanceOf[user];
    }

    /// @notice Sets the allowance 
    /// @param from The from user
    /// @param to The to user
    /// @param newValue the new value
    function setAllowance(address from, address to, uint256 newValue) {
        allowance[from][to] = newValue;
    }

    /// @notice Gets the allowance 
    /// @param from The from user
    /// @param to The to user
    function getAllowance(address from, address to) constant returns (uint256) {
        return allowance[from][to];
    }    
}