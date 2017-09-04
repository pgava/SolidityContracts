pragma solidity ^0.4.13;

import "./TokenRecipient.sol";
import "./TokenData.sol";

contract TokenBase { 
    /* Public variables of the token */ 
    string private name; 
    string private symbol; 
    uint8 private decimals; 
    uint256 internal totalSupply;

    /* Private variables of the token */ 
    address private _tokenData;

  /* This generates a public event on the blockchain that will notify clients */
  event Transfer(address indexed from, address indexed to, uint256 value);

  /* This notifies clients about the amount burnt */
  event Burn(address indexed from, uint256 value);

  /* Initializes contract with initial supply tokens to the creator of the contract */
  function TokenBase(
      address tokenData,
      uint256 initialSupply,
      string tokenName,
      uint8 decimalUnits,
      string tokenSymbol
      ) {
      _tokenData = tokenData;
      TokenData database = TokenData(_tokenData);
      database.setBalanceOf(msg.sender, initialSupply); // Give the creator all initial tokens
      totalSupply = initialSupply;                        // Update total supply
      name = tokenName;                                   // Set the name for display purposes
      symbol = tokenSymbol;                               // Set the symbol for display purposes
      decimals = decimalUnits;                            // Amount of decimals for display purposes
  }

  /* Internal transfer, only can be called by this contract */
  function _transfer(address _from, address _to, uint _value) internal {
      TokenData database = TokenData(_tokenData);
      uint256 fromValue = database.getBalanceOf(_from);
      uint256 toValue = database.getBalanceOf(_to);

      require (_to != 0x0);                             // Prevent transfer to 0x0 address. Use burn() instead
      require (fromValue > _value);                     // Check if the sender has enough
      require (toValue + _value > toValue);             // Check for overflows
      database.setBalanceOf(_from, fromValue - _value); // Subtract from the sender
      database.setBalanceOf(_to, toValue + _value);     // Add the same to the recipient
      Transfer(_from, _to, _value);
  }

  /// @notice Send `_value` tokens to `_to` from your account
  /// @param _to The address of the recipient
  /// @param _value the amount to send
  function transfer(address _to, uint256 _value) {
      _transfer(msg.sender, _to, _value);
  }

  /// @notice Send `_value` tokens to `_to` in behalf of `_from`
  /// @param _from The address of the sender
  /// @param _to The address of the recipient
  /// @param _value the amount to send
  function transferFrom(address _from, address _to, uint256 _value) returns (bool success) {
      TokenData database = TokenData(_tokenData);
      uint256 fromAllowance = database.getAllowance(_from, msg.sender);

      require(_value < fromAllowance);     // Check allowance
      database.setAllowance(_from, msg.sender, fromAllowance - _value);
      _transfer(_from, _to, _value);
      return true;
  }

  /// @notice Allows `_spender` to spend no more than `_value` tokens in your behalf
  /// @param _spender The address authorized to spend
  /// @param _value the max amount they can spend
  function approve(address _spender, uint256 _value) returns (bool success) {
      TokenData database = TokenData(_tokenData);
      database.setAllowance(msg.sender, _spender, _value);
      return true;
  }

  /// @notice Allows `_spender` to spend no more than `_value` tokens in your behalf, and then ping the contract about it
  /// @param _spender The address authorized to spend
  /// @param _value the max amount they can spend
  /// @param _extraData some extra information to send to the approved contract
  function approveAndCall(address _spender, uint256 _value, bytes _extraData) returns (bool success) {
      TokenRecipient spender = TokenRecipient(_spender);
      if (approve(_spender, _value)) {
          spender.receiveApproval(msg.sender, _value, this, _extraData);
          return true;
      }
  }        

  /// @notice Remove `_value` tokens from the system irreversibly
  /// @param _value the amount of money to burn
  function burn(uint256 _value) returns (bool success) {
    TokenData database = TokenData(_tokenData);
    uint256 senderValue = database.getBalanceOf(msg.sender);
    
    require (senderValue > _value);                             // Check if the sender has enough
    database.setBalanceOf(msg.sender, senderValue - _value);    // Subtract from the sender
    totalSupply -= _value;                                      // Updates totalSupply
    Burn(msg.sender, _value);
    return true;
  }

  function burnFrom(address _from, uint256 _value) returns (bool success) {
    TokenData database = TokenData(_tokenData);
    uint256 fromBalance = database.getBalanceOf(_from);
    uint256 fromAllowance = database.getAllowance(_from, msg.sender);

    require(fromBalance >= _value);                                     // Check if the targeted balance is enough
    require(_value <= fromAllowance);                                   // Check allowance
    database.setBalanceOf(_from, fromBalance - _value);                 // Subtract from the targeted balance
    database.setAllowance(_from, msg.sender, fromAllowance - _value);    // Subtract from the sender's allowance
    totalSupply -= _value;                                              // Update totalSupply
    Burn(_from, _value);
    return true;
  }
}