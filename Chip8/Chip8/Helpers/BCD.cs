using System;
using System.Collections.Generic;
using System.Text;

namespace Chip8.Helpers
{
    public class BCD
    {
        public static byte[] GetBytes(int input)
        {
            int hundreds = input / 100;
            int tens = (input -= hundreds * 100) / 10;
            int ones = (input -= tens * 10);

            byte[] bcd = new byte[] {
                    (byte)(hundreds),
                    (byte)(tens),
                    (byte)(ones)
                };

            return bcd;
        }
    }
}
