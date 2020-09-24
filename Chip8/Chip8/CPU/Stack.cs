using System;
using System.Collections.Generic;
using System.Text;

namespace Chip8.CPU
{
    public class MemoryStack
    {
        private Stack<ushort> Stack = new Stack<ushort>(16);

        public ushort ReturnFromSubroutine() => Stack.Pop();

        public ushort CallSubroutine(ushort address)
        {
            Stack.Push(address);
            return address;
        }
    }
}
