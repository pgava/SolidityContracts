pragma solidity ^0.4.13; 
contract Owned { 
    address private owner;

  function Owned() {
      owner = msg.sender;
  }

  modifier onlyOwner {
      require(msg.sender == owner);
      _;
  }

  function transferOwnership(address newOwner) onlyOwner {
      owner = newOwner;
  }
}
