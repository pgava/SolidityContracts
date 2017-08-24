Solidity

for testing you can use geth to create a private test blockchain.

use this as genesis block:
->genesis_dev.json
{
    "config": {},
    "nonce": "0x0000000000000042",
    "difficulty": "0x200",
    "alloc": {
            "12890d2cce102216644c59dae5baed380d84830c": {
            "balance": "10000000000000000000000"
        }
    },
    "mixhash": "0x0000000000000000000000000000000000000000000000000000000000000000",
    "coinbase": "0x0000000000000000000000000000000000000000",
    "timestamp": "0x00",
    "parentHash": "0x0000000000000000000000000000000000000000000000000000000000000000",
    "extraData": "0x",
    "gasLimit": "0x4c4b40"
}



geth 1.5+
=========

RD /S /Q %~dp0\devChain\geth\chainData
RD /S /Q %~dp0\devChain\geth\dapp
RD /S /Q %~dp0\devChain\geth\nodes
del %~dp0\devchain\geth\nodekey

geth.exe  --datadir=devChain init genesis_dev.json
geth.exe --mine --dev --minerthreads "10" --rpc --maxpeers=0 --datadir=devChain  --rpccorsdomain "*" --rpcapi "eth,web3,personal,net,miner,admin,debug" console 2>>geth.log

geth 1.6+
=========

RD /S /Q %~dp0\devChain\geth\chainData
RD /S /Q %~dp0\devChain\geth\dapp
RD /S /Q %~dp0\devChain\geth\nodes
del %~dp0\devchain\geth\nodekey

geth.exe  --datadir=devChain init genesis_dev.json
geth.exe --mine --rpc --networkid=39318 --cache=2048 --maxpeers=0 --datadir=devChain  --rpccorsdomain "*" --rpcapi "eth,web3,personal,net,miner,admin,debug" --verbosity 0 console  
