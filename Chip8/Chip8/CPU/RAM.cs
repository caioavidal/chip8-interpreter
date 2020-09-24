using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chip8.CPU
{
    public class RAM
    {
        private byte[] ram = new byte[4096];
        private ushort PC = 0x200;
        private ushort I;
        private Dictionary<byte, ushort> fontsAddress = new Dictionary<byte, ushort>();

        public RAM()
        {
            StoreFonts();
        }

        public ushort GetFontAddress(byte font) => fontsAddress[font];

        public MemoryStack Stack { get; } = new MemoryStack();

        public void SetRegisterI(ushort value) => I = value;
        public void IncreaseRegisterI(ushort value) => I += value;

        public void SetRegisterI(byte [] values)
        {
            Buffer.BlockCopy(values, 0, ram, I, values.Length);
        }
        public void CopyValuesToRam(byte[] values)
        {
            Buffer.BlockCopy(values, 0, ram, I, values.Length);
        }

        public byte[] GetValues(int length)
        {
            var end = I + length + 1;
            return ram[I..end];
        }

        public void LoadProgram(byte[] program)
        {
            Buffer.BlockCopy(program, 0, ram, PC, program.Length);
        }

        public void JumpToAddress(ushort address) => PC = address;

        public ushort ReturnFromSubroutine() => PC = Stack.ReturnFromSubroutine();

        public void CallSubroutine(ushort address) => Stack.CallSubroutine(address, ref PC);
        public void SkipNextInstruction() => PC += 2;

        public byte[] GetSprites(int length)
        {
            var end = I + length;
            return ram[I..end];
        }

        public ushort GetNextInstruction()
        {
            var instruction = (ushort)(ram[PC] << 8 | ram[PC + 1]);

            PC += 2;

            return instruction;
        }

        private void StoreFonts()
        {
            var allFonts = Fonts.All.Values.SelectMany(x=>x).ToArray();

            ushort address = 0x050;

            foreach (var font in Fonts.All)
            {
                fontsAddress.Add(font.Key, address+=5);
            }

            Buffer.BlockCopy(allFonts, 0, ram, 0x050, allFonts.Length);
        }
    }
}
