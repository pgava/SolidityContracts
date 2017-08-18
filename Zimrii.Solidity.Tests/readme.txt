
-- address
0x198e13017d2333712bd942d8b028610b95c363da

-- testrpc
testrpc --account="0x7231a774a538fce22a329729b03087de4cb4a1119494db1c10eae3bb491823e7, 10000000000000000000"


RD /S /Q %~dp0\devChain\geth\chainData
RD /S /Q %~dp0\devChain\geth\dapp
RD /S /Q %~dp0\devChain\geth\nodes
del %~dp0\devchain\geth\nodekey

geth.exe  --datadir=devChain init genesis_dev.json
geth.exe --mine --dev --minerthreads "10" --rpc --maxpeers=0 --datadir=devChain  --rpccorsdomain "*" --rpcapi "eth,web3,personal,net,miner,admin,debug" console 2>>geth.log

