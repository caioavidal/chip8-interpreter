using System;
using System.Collections.Generic;
using System.Text;

namespace Chip8.CPU
{
    public class RAM
    {
        private byte[] ram = new byte[4096];
        private ushort PC = 0x200;
        private ushort I;

        public MemoryStack Stack { get; } = new MemoryStack();

        public void SetRegisterI(ushort value) => I = value;
        public void IncreaseRegisterI(ushort value) => I += value;

        public void SetRegisterI(byte [] values)
        {
            foreach (var value in values)
            {
                ram[I++] = value;
            }
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
    }
}
