using Chip8.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Chip8.CPU
{
    public class Cpu
    {
        private RAM ram = new RAM();
        private byte DelayTimer;
        private byte SoundTimer;
        private byte Keyboard;
        private Register register = new Register();
        private Display display;

        public Cpu()
        {
            display = new Display(register);
        }
        private Random Random = new Random(Environment.TickCount);

        public bool ProgramLoaded { get; private set; }


        public void LoadProgram(byte[] program)
        {
            ram.LoadProgram(program);
            ProgramLoaded = true;
        }
        public void StartProgram()
        {
            if (ProgramLoaded)
            {
                display.Init();
                Run();
            }
        }
        public void StartProgram(byte[] program)
        {
            LoadProgram(program);
            StartProgram();
        }

        private void Run()
        {
            while (true)
            {
                var opcode = ram.GetNextInstruction();

                var nnn = (ushort)(opcode & 0x0FFF);
                var nn = (byte)(opcode & 0x00FF);
                byte n = (byte)(opcode & 0x000F);
                var x = (ushort)(opcode & 0x0F00) >> 8;
                var y = (byte)(opcode & 0x00F0) >> 4;
                
                ushort nibble = (ushort)(opcode & 0x0F000);

                switch (nibble)
                {
                    case 0x0000:
                        if (opcode == 0x00e0) display.ClearDisplay();
                        if (opcode == 0x00ee) ram.ReturnFromSubroutine();
                        break;

                    case 0x1000:
                        ram.JumpToAddress(nnn);
                        break;

                    case 0x2000:
                        ram.CallSubroutine(nnn);
                        break;

                    case 0x3000:
                        if (register[x] == nn) ram.SkipNextInstruction();
                        break;

                    case 0x4000:
                        if (register[x] != nn) ram.SkipNextInstruction();
                        break;

                    case 0x5000:
                        if (register[x] != register[y]) ram.SkipNextInstruction();
                        break;

                    case 0x6000:
                        register.StoreValue(x, nn);
                        break;

                    case 0x7000:
                        register.IncrementRegisterValue(x, nn);
                        break;

                    case 0x8000:
                        byte last = (byte)(opcode & 0x000F);

                        switch (last)
                        {
                            case 0: register.StoreValueOfRegisterInOther(y, x); break;
                            case 1: register.CalculateRegistersAndStoreValue(x, x, y, Helpers.BitOperationType.OR); break;
                            case 2: register.CalculateRegistersAndStoreValue(x, x, y, Helpers.BitOperationType.AND); break;
                            case 3: register.CalculateRegistersAndStoreValue(x, x, y, Helpers.BitOperationType.XOR); break;
                            case 4:
                                register.StoreValueOnRegister0xF((byte)(register[x] + register[y] > 255 ? 1 : 0));
                                register.IncrementRegisterValue(x, register[y]);
                                break;
                            case 5:
                                register.StoreValueOnRegister0xF((byte)(register[x] > register[y] ? 1 : 0));
                                register.SubtractRegisterValue(y, x);
                                break;
                            case 6:
                                register.StoreValueOnRegister0xF((byte)(register[x] & 0x0001));
                                register.StoreValue(x, (byte)(register[x] / 2));
                                break;
                            case 7:
                                register.StoreValueOnRegister0xF((byte)(register[y] > register[x] ? 1 : 0));
                                register.StoreValue(x, (byte)(register[y] - register[x]));
                                break;
                            case 0xE:
                                register.StoreValueOnRegister0xF((byte)(register[x] & 0x0001));
                                register.StoreValue(x, (byte)(register[x] * 2));
                                break;
                        }

                        break;

                    case 0x9000:
                        if (register[x] != register[y]) ram.SkipNextInstruction(); break;

                    case 0xA000:
                        ram.SetRegisterI(nnn); break;

                    case 0xB000:
                        ram.JumpToAddress((ushort)(register[0] + nnn)); break;

                    case 0xC000:
                        register.StoreValue(x, (byte)((byte)Random.Next(0, byte.MaxValue) & nn)); break;

                    case 0xE000:
                        last = (byte)(opcode & 0x000F);

                        switch (last)
                        {
                            case 14:
                                if (register[x] == Keyboard) ram.SkipNextInstruction();
                                break;
                            case 1:
                                if (register[x] != Keyboard) ram.SkipNextInstruction();
                                break;
                        }

                        break;

                    case 0xD000:
                        var sprites = ram.GetSprites(n);
                        display.Draw(sprites, register[x], register[y]);
                        break;

                    case 0xF000:
                        last = (byte)(opcode & 0x00FF);

                        switch (last)
                        {
                            case 0x7: //FX07
                                register.StoreValue(x, DelayTimer);
                                break;
                            case 0x0A:
                                register.StoreValue(x, (byte)Console.ReadKey().KeyChar);
                                break;
                            case 0x15:
                                DelayTimer = register[x];
                                break;
                            case 0x18: //FX18
                                SoundTimer = register[x];
                                break;
                            case 0x1E:
                                ram.IncreaseRegisterI(register[x]);
                                break;
                            case 0x29:
                                ram.SetRegisterI((ushort)(register[x] * 5));
                                break;
                            case 0x33:
                                var bcd = BCD.GetBytes(register[x]);
                                ram.SetRegisterI(bcd);
                                break;
                            case 0x55:
                                for (int i = 0; i <= x; i++) ram.SetRegisterI(register[i]);
                                break;
                                //case 0x65:
                                //    address = I;

                                //    for (int i = 0; i <= x; i++)
                                //    {
                                //        V[i] = ram[address++];
                                //    }
                                //break;

                        }
                        break;
                }
            }
        }
    }
}

