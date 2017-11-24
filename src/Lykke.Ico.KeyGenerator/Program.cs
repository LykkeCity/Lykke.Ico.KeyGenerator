using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Nethereum.Hex.HexConvertors.Extensions;

namespace Lykke.Ico.KeyGenerator
{
    public class Program
    {
        public const string numberArg = "-n";
        public const string publicKeysPathArg = "-p";
        public const string secretKeysPathArg = "-s";
        public const string addressArg = "-a";
        public const string headersArg = "-h";
        public const string netArg = "-net";
        public const string entropyArg = "-e";
        public const string defaultPublicKeysPath = "public.csv";
        public const string defaultSecretKeysPath = "secret.csv";

        public static int Main(string[] args)
        {
            Arguments arguments = null;

            try
            {
                arguments = ParseArguments(args);
            }
            catch (ArgumentException aex)
            {
                Console.WriteLine("Error: " + aex.Message);
                Console.WriteLine("Usage: Lykke.Ico.KeyGenerator -n <Number of key pairs to generate> [-p <Path to public keys file>] [-s <Path to secret keys file>] [-net <Name of BTC net>] [-a] [-h] [-e <Entropy>]");
                return 1;
            }

            var publicKeysPath = arguments.PublicKeysPath ?? defaultPublicKeysPath;
            var secretKeysPath = arguments.SecretKeysPath ?? defaultSecretKeysPath;
            var startedAt = DateTime.Now;

            if (File.Exists(publicKeysPath))
            {
                File.Delete(publicKeysPath);
            }

            if (File.Exists(secretKeysPath))
            {
                File.Delete(secretKeysPath);
            }

            if (!string.IsNullOrWhiteSpace(arguments.Entropy))
            {
                NBitcoin.RandomUtils.AddEntropy(arguments.Entropy);
            }

            using (var publicWriter = new StreamWriter(File.OpenWrite(publicKeysPath)))
            using (var secretWriter = new StreamWriter(File.OpenWrite(secretKeysPath)))
            {
                if (arguments.WriteHeaders)
                {
                    if (arguments.WriteAddress)
                    {
                        publicWriter.WriteLine("btcAddress;btcPublic;ethAddress;ethPublic");
                        secretWriter.WriteLine("btcAddress;btcPublic;btcPrivate;ethAddress;ethPublic;ethPrivate");
                    }
                    else
                    {
                        publicWriter.WriteLine("btcPublic;ethPublic");
                        secretWriter.WriteLine("btcPublic;btcPrivate;ethPublic;ethPrivate");
                    }
                }

                for (int i = 0; i < arguments.Count; i++)
                {
                    // report out the progress

                    if (i % 1000 == 0)
                    {
                        Console.Write("\r{0} of {1}", i, arguments.Count);
                    }

                    // generate keys

                    var btcKey = new NBitcoin.Key();
                    var btcPvt = btcKey.GetBitcoinSecret(arguments.BitcoinNetwork).ToWif();
                    var btcPub = btcKey.PubKey.ToHex();

                    var ethKey = Nethereum.Signer.EthECKey.GenerateKey();
                    var ethPvt = ethKey.GetPrivateKey();
                    var ethPub = ethKey.GetPubKey().ToHex(true);

                    if (arguments.WriteAddress)
                    {
                        var btcAddress = btcKey.PubKey.GetAddress(arguments.BitcoinNetwork).ToString();
                        var ethAddress = ethKey.GetPublicAddress();

                        publicWriter.WriteLine("{0};{1};{2};{3}", btcAddress, btcPub, ethAddress, ethPub);
                        secretWriter.WriteLine("{0};{1};{2};{3};{4};{5}", btcAddress, btcPub, btcPvt, ethAddress, ethPub, ethPvt);
                    }
                    else
                    {
                        publicWriter.WriteLine("{0};{1}", btcPub, ethPub);
                        secretWriter.WriteLine("{0};{1};{2};{3}", btcPub, btcPvt, ethPub, ethPvt);
                    }
                }
            }

            // report final state
            Console.WriteLine("\r{0} of {1}", arguments.Count, arguments.Count);
            Console.WriteLine("Completed in {0}", DateTime.Now - startedAt);

            // return success code
            return 0;
        }

        static Arguments ParseArguments(string[] args)
        {
            var arguments = new Arguments();

            var count = 0L;
            if (long.TryParse(GetArgument(args, numberArg), out count) && count > 0)
            {
                arguments.Count = count;
            }
            else
            {
                throw new ArgumentException("Number of key pairs to generate is mandatory");
            }

            arguments.PublicKeysPath = GetArgument(args, publicKeysPathArg);
            arguments.SecretKeysPath = GetArgument(args, secretKeysPathArg);
            arguments.WriteAddress = args.Contains(addressArg);
            arguments.WriteHeaders = args.Contains(headersArg);

            var net = GetArgument(args, netArg);
            if (net != null)
            {
                arguments.BitcoinNetwork = NBitcoin.Network.GetNetwork(net) ?? NBitcoin.Network.Main;
            }

            arguments.Entropy = GetArgument(args, entropyArg);

            return arguments;
        }

        static string GetArgument(string[] args, string arg)
        {
            var argIndex = Array.IndexOf(args, arg);
            if (argIndex > -1)
            {
                if (args.Length > argIndex + 1)
                {
                    return args[argIndex + 1];
                }
                else
                {
                    throw new ArgumentException("Wrong arguments");
                }
            }

            return null;
        }
    }

    class Arguments
    {
        public long Count { get; set; }
        public string PublicKeysPath { get; set; }
        public string SecretKeysPath { get; set; }
        public NBitcoin.Network BitcoinNetwork { get; set; } = NBitcoin.Network.Main;
        public bool WriteAddress { get; set; }
        public bool WriteHeaders { get; set; }
        public string Entropy { get; set; }
    }
}
