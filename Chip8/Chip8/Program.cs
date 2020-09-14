using System;
using System.Collections.Generic;
using System.IO;

namespace Chip8
{
    class Program
    {
        static void Main(string[] args)
        {
            var cpu = new CPU();

            var program = new List<ushort>();

            using (BinaryReader reader = new BinaryReader(new FileStream("roms/Pong.ch8", FileMode.Open)))
            {

                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    program.Add((ushort)(reader.ReadByte() << 8 | reader.ReadByte()));
                }

                cpu.LoadProgram(program);

            }
        }

        public class CPU
        {
            public CPU()
            {

            }
            private byte[] RAM = new byte[4096];
            private byte[] V = new byte[16];
            private ushort I = 0;
            private Stack<ushort> Stack = new Stack<ushort>(16);
            private byte DelayTimer;
            private byte SoundTimer;
            private byte Keyboard;
            private byte[] Display = new byte[64 * 32];
            private ushort PC;
            private byte StackPointer;

            private bool WaitingForKeyboard; 

            private List<ushort> Program;

            public Random Random { get; set; } = new Random(Environment.TickCount);

            public void Step()
            {
                var opcode = BitConverter.ToUInt16(new byte[] { RAM[PC], RAM[PC + 1] });

                if (WaitingForKeyboard)
                {
                  

                }

                ushort nibble = (ushort)(opcode & 0x0F000);

                if (nibble == 0x0000)
                {
                    switch (opcode)
                    {
                        case 0x00e0:
                            for (int i = 0; i < Display.Length; i++) Display[i] = 0;
                            break;
                        case 0x00ee:
                            PC = Stack.Pop();
                            StackPointer--;
                            break;
                        default: throw new NotSupportedException($"Opcode not supported: {opcode:X4}");
                    }

                }
                if (nibble == 0x1000) //1NNN
                {
                    //goto NNN;
                    PC = (ushort)(opcode & 0x0FFF);
                    return;
                }
                if (nibble == 0x2000)
                {
                    var nnn = (ushort)(opcode & 0x0FFF);
                    PC = nnn;
                    Stack.Push(nnn);
                    StackPointer++;
                    return;
                }
                if (nibble == 0x3000) //3XNN
                {
                    var x = (ushort)(opcode & 0x0F00) >> 8;
                    var nn = (byte)(opcode & 0x00FF);
                    if (V[x] == nn)
                    {
                        PC += 2;
                    }
                }
                if (nibble == 0x4000) //3XNN
                {
                    var x = (ushort)(opcode & 0x0F00) >> 8;
                    var nn = (byte)(opcode & 0x00FF);
                    if (V[x] != nn)
                    {
                        PC += 2;
                    }
                }
                if (nibble == 0x5000) //3XNN
                {
                    var x = (ushort)(opcode & 0x0F00) >> 8;
                    var y = (byte)(opcode & 0x00F0) >> 4;
                    if (V[x] != V[y])
                    {
                        PC += 2;
                    }
                }
                if (nibble == 0x6000) //6XNN
                {
                    var x = (ushort)(opcode & 0x0F00) >> 8;
                    var nn = (byte)(opcode & 0x00FF);
                    V[x] = nn;
                }
                if (nibble == 0x7000) //7XNN
                {
                    //Vx += NN
                    var x = (ushort)(opcode & 0x0F00) >> 8;
                    var nn = (byte)(opcode & 0x00FF);

                    V[x] += nn;
                }
                if (nibble == 0x8000) //8XNN
                {
                    byte last = (byte)(opcode & 0x000F);

                    var x = (ushort)(opcode & 0x0F00) >> 8;
                    var y = (byte)(opcode & 0x00F0) >> 4;

                    switch (last)
                    {
                        case 0: V[x] = V[y]; break;
                        case 1: V[x] = (byte)(V[x] | V[y]); break;
                        case 2: V[x] = (byte)(V[x] & V[y]); break;
                        case 3: V[x] = (byte)(V[x] ^ V[y]); break;
                        case 4:
                            V[x] += V[y];
                            V[0xF] = (byte)(V[x] + V[y] > 255 ? 1 : 0);
                            break;
                        case 5:
                            V[x] -= V[y];
                            V[0xF] = (byte)(V[x] > V[y] ? 1 : 0);
                            break;
                        case 6:
                            V[x] = (byte)(V[x] >> 1);
                            V[0xF] = (byte)(V[x] & 0x0001);
                            break;
                        case 7:
                            V[x] = (byte)(V[y] - V[x]);
                            V[0xF] = (byte)(V[y] > V[x] ? 1 : 0);
                            break;
                        case 0xE:
                            V[x] = (byte)(V[x] << 1);
                            V[0xF] = (byte)(V[x] & 0x1000);
                            break;
                    }

                }
                if (nibble == 0x9000) //9XY0
                {
                    var x = (ushort)(opcode & 0x0F00) >> 8;
                    var y = (byte)(opcode & 0x00F0) >> 4;

                    if (V[x] != V[y])
                    {
                        PC += 2;
                    }
                }

                if (nibble == 0xA000)
                {
                    I = (ushort)(opcode & 0x0FFF);
                }
                if (nibble == 0xB000)
                {
                    var nnn = (ushort)(opcode & 0x0FFF);

                    PC = (ushort)(V[0] + nnn);
                }
                if (nibble == 0xC000)
                {
                    //Vx=rand()&NN
                    var x = (ushort)(opcode & 0x0F00) >> 8;
                    var nn = (ushort)(opcode & 0x00FF);

                    V[x] = (byte)((byte)Random.Next(0, byte.MaxValue) & nn);
                }

                if (nibble == 0xE000)
                {
                    byte last = (byte)(opcode & 0x000F);
                    var x = (ushort)(opcode & 0x0F00) >> 8;


                    switch (last)
                    {
                        case 14:
                            if (V[x] == Keyboard) { PC += 2; }
                            break;
                        case 1:
                            if (V[x] != Keyboard) { PC += 2; }
                            break;

                    }
                }

                if (nibble == 0xD000) //DXYN
                {
                    byte x = (byte)((opcode & 0x0F00) >> 8);
                    byte y = (byte)((opcode & 0x00F0) >> 4);
                    byte n = (byte)(opcode & 0x000F);

                    var address = I;

                    for (int i = 0; i <= n; i++)
                    {
                        var row = RAM[address++];

                        int currentX = x;

                        for (int j = 0; j < 8; j++)
                        {

                            var pixel = (byte)(Display[currentX * y] ^ (byte)((row >> 7 - j) & 1));

                            V[0xF] = (byte)((pixel == 0 && Display[currentX++ * y] == 1) ? 1 : 0);

                            Display[currentX * y] = pixel;

                            currentX++;
                        }

                        y++;
                    }

                    Draw(x, y, (byte)(n + 1));
                }



                if (nibble == 0x8000) //7XNN
                {

                    //Vx += NN
                    var x = (ushort)(opcode & 0x0F00) >> 8;
                    var nn = (byte)(opcode & 0x00FF);

                    V[x] += nn;
                }
                if (nibble == 0xF000)
                {
                    byte last = (byte)(opcode & 0x00FF);
                    var x = (ushort)(opcode & 0x0F00) >> 8;

                    switch (last)
                    {
                        case 0x7: //FX07
                            V[x] = DelayTimer;
                            break;
                        case 0x0A:
                            WaitingForKeyboard = true;
                            V[x] = (byte)Console.ReadKey().KeyChar;
                            break;
                        case 0x15:
                            DelayTimer = V[x];
                            break;
                        case 0x18: //FX18
                            SoundTimer = V[x];
                            break;
                        case 0x1E:
                            I += V[x];
                            break;
                        case 0x29:
                            I = (ushort)(V[x] * 5);
                            break;
                        case 0x33:
                            var bcd = GetBCD(V[x]);

                            RAM[I] = bcd[0];
                            RAM[I + 1] = bcd[1];
                            RAM[I + 2] = bcd[2];


                            break;
                        case 0x55:
                            var address = I;
                            for (int i = 0; i <= x; i++)
                            {
                                RAM[address++] = V[i];
                            }
                            break;
                        case 0x65:
                            address = I;

                            for (int i = 0; i <= x; i++)
                            {
                                V[i] = RAM[address++];
                            }
                            break;
                    }
                }

                PC += 2;
            }

            public void Draw(byte x, byte y, byte n)
            {
                //
            }

            private byte[] GetBCD(int input)
            {
                int hundreds = input / 100;
                int tens = (input -= hundreds * 100) / 10;
                int ones = (input -= tens * 10);

                byte[] bcd = new byte[] {
                    (byte)(hundreds),
                    (byte)(tens << 4),
                    (byte)(ones)
                };

                return bcd;
            }

            internal void LoadProgram(List<ushort> program)
            {
                Program = program;
                var address = 0x200;
                for (int i = 0; i < program.Count; i++)
                {
                    var bytes = BitConverter.GetBytes(program[i]);

                    RAM[address++] = bytes[0];
                    RAM[address++] = bytes[1];
                }

                PC = 0x200;
                while(true)
                {
                    Step();
                }
            }
        }
    }
}
