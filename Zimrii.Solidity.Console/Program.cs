using System;

namespace Zimrii.Solidity.Console
{
    public class Program
    {
        static void Main(string[] args)
        {
            var path = args.Length > 0 ? args[0] : "";

            var eth = new NethereumMusicCopyright(path);

            //eth.UploadMusicCopyright().Wait();

            eth.SetMusicCopyright("BC3FCB8DA4384619AAC2AEE0698D3ECC", "8037D23F5DE34B2AA0A87887BD0B0E78", "eUFp6J1PDrQwWX/6SSTuCQ==").Wait();

            System.Console.WriteLine("End");
        }
    }
}
