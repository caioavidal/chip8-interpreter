using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chip8.CPU
{
    public class RAM
    {
        private byte[] ram = new byte[4096];
       // private ushort PC = 0x200;
        //private ushort I;
        private Dictionary<byte, ushort> fontsAddress = new Dictionary<byte, ushort>();

        public RAM()
        {
            StoreFonts();
        }

        public ushort GetFontAddress(byte font) => fontsAddress[font];

        public MemoryStack Stack { get; } = new MemoryStack();

     
        public void CopyValuesToRam(byte[] values, ushort start)
        {
            Buffer.BlockCopy(values, 0, ram, start, values.Length);
        }

        public byte[] GetValues(ushort start,int length)
        {
            var end = start + length;
            return ram[start..end];
        }

       // public void JumpToAddress(ushort address) => PC = address;

        public ushort ReturnFromSubroutine() =>  Stack.ReturnFromSubroutine();

        public void CallSubroutine(ushort address) => Stack.CallSubroutine(address);
        //public void SkipNextInstruction() => PC += 2;

        //public byte[] GetSprites(ushort start, int length)
        //{
        //    var end = start + length;
        //    return ram[start..end];
        //}

        public ushort GetNextInstruction(in ushort PC) => (ushort)(ram[PC] << 8 | ram[PC + 1]);

        private void StoreFonts()
        {
            var allFonts = Fonts.All.Values.SelectMany(x=>x).ToArray();

            ushort address = 0x050;

            foreach (var font in Fonts.All)
            {
                fontsAddress.Add(font.Key, address);
                address += 5;
            }

            Buffer.BlockCopy(allFonts, 0, ram, 0x050, allFonts.Length);
        }
    }
}
