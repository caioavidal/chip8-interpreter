using System;
using System.Collections.Generic;
using System.Text;

namespace Chip8.Helpers
{
    public sealed class BitOperation
    {
        public static byte Calculate(byte a, byte b, BitOperationType type) => type switch
        {
            BitOperationType.AND => (byte)(a & b),
            BitOperationType.OR => (byte)(a | b),
            BitOperationType.XOR => (byte)(a ^ b),
            _ => throw new ArgumentException("bit operation not found")
        };

    }

    public enum BitOperationType
    {
        OR,
        XOR,
        AND
    }
}
