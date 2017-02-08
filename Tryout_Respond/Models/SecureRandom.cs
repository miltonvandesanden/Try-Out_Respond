using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace Tryout_Respond.Models
{
    public class SecureRandom
    {
        public static int Next(int min, int max)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

            byte[] rndBytes = new byte[4];

            rng.GetBytes(rndBytes);
            int rand = BitConverter.ToInt32(rndBytes, 0);
            const Decimal oldRange = (Decimal)int.MaxValue - (Decimal)int.MinValue;
            Decimal newRange = max - min;
            Decimal newValue = ((Decimal)rand - (Decimal)int.MinValue) / oldRange * newRange + (Decimal)min;

            return (int)newValue;
        }
    }
}