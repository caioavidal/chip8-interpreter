using System;
using System.Collections.Generic;
using System.Text;

namespace Chip8.CPU
{
    public class MemoryStack
    {
        private Stack<ushort> Stack = new Stack<ushort>(16);

        public ushort ReturnFromSubroutine() => Stack.Pop();

        public void CallSubroutine(ushort address, ref ushort PC)
        {
            Stack.Push(PC);
            PC = address;
        }
    }
}
