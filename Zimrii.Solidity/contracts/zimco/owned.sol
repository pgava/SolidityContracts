pragma solidity ^0.4.13; 
contract Owned { 
    address private owner;

  function Owned() public {
      owner = msg.sender;
  }

  modifier onlyOwner {
      require(msg.sender == owner);
      _;
  }

  function transferOwnership(address newOwner) public onlyOwner {
      owner = newOwner;
  }
}
