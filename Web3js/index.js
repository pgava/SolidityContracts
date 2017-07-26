var Web3 = require('web3');

var web3 = new Web3(new Web3.providers.HttpProvider("http://localhost:8545"));
var version = web3.version.network;
console.log(version);
var accounts = web3.eth.accounts;
console.log(accounts);

var unlock = web3.personal.unlockAccount("0x12890d2cce102216644c59dae5baed380d84830c", "password", 1000);
console.log(unlock);

var abi = [
    {
        "constant": true,
        "inputs": [{ "name": "musicId", "type": "string" }],
        "name": "getCopyrightResourceEndpoint",
        "outputs": [{ "name": "res", "type": "string" }],
        "payable": false,
        "type": "function"
    },
    {
        "constant": true,
        "inputs": [{ "name": "musicId", "type": "string" }],
        "name": "getCopyrightId",
        "outputs": [{ "name": "res", "type": "string" }],
        "payable": false,
        "type": "function"
    },
    {
        "constant": false,
        "inputs": [{ "name": "endpoint", "type": "string" }],
        "name": "setCopyrightEndpointResourceRoot",
        "outputs": [],
        "payable": true,
        "type": "function"
    },
    {
        "constant": false,
        "inputs": [{ "name": "musicId", "type": "string" }, { "name": "copyrightId", "type": "string" }],
        "name": "setCopyright",
        "outputs": [],
        "payable": true,
        "type": "function"
    }, { "inputs": [], "payable": false, "type": "constructor" },
    {
        "anonymous": false,
        "inputs": [
            { "indexed": true, "name": "musicId", "type": "string" },
            { "indexed": false, "name": "copyrightId", "type": "string" },
            { "indexed": false, "name": "data", "type": "string" }
        ],
        "name": "SetCopyright",
        "type": "event"
    }
];
var MyContract = web3.eth.contract(abi);

// instantiate by address
var contractInstance = MyContract.at("0x41e92b7d70852b4daa5afed204b34925eaaf8407");

// deploy new contract
//var contractInstance = MyContract.new(
//    {
//        data: '0x6060604052341561000c57fe5b60405160208061024783398101604052515b60008190555b505b610212806100356000396000f300606060405263ffffffff60e060020a6000350416631df4f1448114610021575bfe5b61002c60043561003e565b60408051918252519081900360200190f35b604080518082018252600180825282518084018452600381527f616131000000000000000000000000000000000000000000000000000000000060208281019190915280840191825284517f610000000000000000000000000000000000000000000000000000000000000081528084018490529451948590036021019094208351815490151560ff199091161781559051805160009592936100e693908501920190610146565b50905050600054820290503373ffffffffffffffffffffffffffffffffffffffff16827f841774c8b4d8511a3974d7040b5bc3c603d304c926ad25d168dacd04e25c4bed836040518082815260200191505060405180910390a35b919050565b828054600181600116156101000203166002900490600052602060002090601f016020900481019282601f1061018757805160ff19168380011785556101b4565b828001600101855582156101b4579182015b828111156101b4578251825591602001919060010190610199565b5b506101c19291506101c5565b5090565b6101e391905b808211156101c157600081556001016101cb565b5090565b905600a165627a7a7230582054ca7676fd9e01d4b50fccd7aaab08823ac14cbc52104a42679d4b36ce3c1c160029',
//        from: "0x12890d2cce102216644c59dae5baed380d84830c",
//        gas: 1000000
//    });

var unlock = web3.personal.unlockAccount("0x12890d2cce102216644c59dae5baed380d84830c", "password", 1000);
console.log(unlock);
//contractInstance.setCopyright("musicId", "copyrightId", { from: "0x12890d2cce102216644c59dae5baed380d84830c", value: 200, gas: 200000 });
//contractInstance.setCopyrightEndpointResourceRoot("myservice2", { from: "0x12890d2cce102216644c59dae5baed380d84830c", value: 200, gas: 200000 });

var res = contractInstance.getCopyrightId("musicId", { from: "0x12890d2cce102216644c59dae5baed380d84830c" });
console.log(res);
res = contractInstance.getCopyrightResourceEndpoint("musicId", { from: "0x12890d2cce102216644c59dae5baed380d84830c" });
console.log(res);

//var _flagCheck = setInterval(function () {
//    console.log("checking...");
//    if (contractInstance.address !== undefined) {
//        console.log("ready...");
//        console.log(contractInstance.address);
//        clearInterval(_flagCheck);
//        contractInstance.multiply(7, { value: 200, gas: 200000 });        
//    }
//}, 5000);

