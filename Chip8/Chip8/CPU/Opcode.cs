using System;
using System.Collections.Generic;
using System.Text;

namespace Chip8.CPU
{
    public readonly ref struct Opcode
    {
        public ushort NNN { get; }
        public byte NN { get; }
        public byte N { get; }
        public ushort X { get; }
        public byte Y { get; }

        public ushort Nibble { get; }

        public ushort Value { get; }

        public Opcode(ushort opcode)
        {
            NNN = (ushort)(opcode & 0x0FFF);
            NN = (byte)(opcode & 0x00FF);
            N = (byte)(opcode & 0x000F);
            X = (ushort)((opcode & 0x0F00) >> 8);
            Y = (byte)((opcode & 0x00F0) >> 4);
            Nibble = (ushort)(opcode & 0x0F000);
            Value = opcode;
        }


    }
}
