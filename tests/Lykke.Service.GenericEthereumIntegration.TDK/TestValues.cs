using System;
using JetBrains.Annotations;


namespace Lykke.Service.GenericEthereumIntegration.TDK
{
    [PublicAPI]
    public static class TestValues
    {
        public const string ValidAddress1 = "0x81b7E08F65Bdf5648606c89998A9CC8164397647";
        public const string ValidAddress2 = "0x47e0010C50eebdb563cb0386AaD3460937990Bf4";
        public const string ValidAddress3 = "0x6Ef57BE1168628A2bD6c5788322A41265084408a";
        public const string ValidAddress4 = "0x17FaCa752378594c87A448c87636dC816A265027";


        public static string CreateValidAddress()
        {
            throw new NotImplementedException();
        }
        
        public const string ValidTransactionHash1 = "0x13d5275fa517163b7ad702d41c98ac1c99bd38a2fe431afe3ad622c79e97789e";
        public const string ValidTransactionHash2 = "0xba8f5472acf365b923f87d6db64d1993262e151ec52ffcd195df11bb93445458";
        public const string ValidTransactionHash3 = "0xccaa83ea613e26c2f5e18285940c28f0bd7b2a70e7e597c7816182e107faf3fa";
        public const string ValidTransactionHash4 = "0xba8f5472acf365b923f87d6db64d1993262e151ec52ffcd195df11bb93445458";
        
        
        public const string InvalidAddress1 = "0xea674fdde714fd979de3edf0f56aa9716b898ec8";
        public const string InvalidAddress2 = "0xEA674FDDE714FD979DE3EDF0F56AA9716B898EC8";
        public const string InvalidAddress3 = "0xEA674fdDe714fd979de3EdF0F56aa9716B898EC8";
        public const string InvalidAddress4 = "";
        
        public static string CreateInvalidAddress()
        {
            throw new NotImplementedException();
        }
    }
}
