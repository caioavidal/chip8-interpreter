using Chip8.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Chip8.CPU
{
    public class Cpu
    {
        private readonly RAM ram;
        private byte DelayTimer;
        private byte SoundTimer;
        private byte Keyboard;
        private Register register = new Register();
        private readonly Display display;
        private Random Random = new Random(Environment.TickCount);

        public Cpu(Display display, RAM ram)
        {
            this.ram = ram;
            this.display = display;
        }

        public void Run()
        {
            while (true)
            {
                var opcode = Fetch();
                var param = Decode(opcode);
                Execute(param);
            }
        }

        private ushort Fetch()
        {
            var opcode = ram.GetNextInstruction(register.PC);
            register.IncrementPC();
            return opcode;
        }

        public Opcode Decode(ushort opcode) => new Opcode(opcode);

        private void Execute(Opcode param)
        {
            switch (param.Nibble)
            {
                case 0x0000:
                    if (param.Value == 0x00e0) display.ClearDisplay();
                    if (param.Value == 0x00ee)
                    {
                        var address = ram.ReturnFromSubroutine();
                        register.JumpToAddress(address);
                    }
                    break;

                case 0x1000:
                    register.JumpToAddress(param.NNN);
                    break;

                case 0x2000:
                    ram.CallSubroutine(register.PC);
                    register.JumpToAddress(param.NNN);
                    break;

                case 0x3000:
                    if (register[param.X] == param.NN) register.IncrementPC();
                    break;

                case 0x4000:
                    if (register[param.X] != param.NN) register.IncrementPC();
                    break;

                case 0x5000:
                    if (register[param.X] == register[param.Y]) register.IncrementPC();
                    break;

                case 0x6000:
                    register.StoreValue(param.X, param.NN);
                    break;

                case 0x7000:
                    register.IncrementRegisterValue(param.X, param.NN);
                    break;

                case 0x8000:
                    byte last = (byte)(param.Value & 0x000F);

                    switch (last)
                    {
                        case 0: register.StoreValueOfRegisterInOther(param.Y, param.X); break;
                        case 1: register.CalculateRegistersAndStoreValue(param.X, param.X, param.Y, Helpers.BitOperationType.OR); break;
                        case 2: register.CalculateRegistersAndStoreValue(param.X, param.X, param.Y, Helpers.BitOperationType.AND); break;
                        case 3: register.CalculateRegistersAndStoreValue(param.X, param.X, param.Y, Helpers.BitOperationType.XOR); break;
                        case 4:
                            register.StoreValueOnRegister0xF((byte)(register[param.X] + register[param.Y] > 255 ? 1 : 0));
                            register.IncrementRegisterValue(param.X, register[param.Y]);
                            break;
                        case 5:
                            register.StoreValueOnRegister0xF((byte)(register[param.X] > register[param.Y] ? 1 : 0));
                            register.SubtractRegisterValue(param.Y, param.X);
                            break;
                        case 6:

                            register.StoreValueOnRegister0xF((byte)(register[param.X] & 0x0001));
                            register.StoreValue(param.X, (byte)(register[param.X] >> 1));
                            break;
                        case 7:
                            register.StoreValueOnRegister0xF((byte)(register[param.Y] > register[param.X] ? 1 : 0));
                            register.StoreValue(param.X, (byte)(register[param.Y] - register[param.X]));
                            break;
                        case 0xE:
                            register.StoreValueOnRegister0xF((byte)((register[param.X] & 0x1000) >> 16));
                            register.StoreValue(param.X, (byte)(register[param.X] << 1));
                            break;
                    }

                    break;

                case 0x9000:
                    if (register[param.X] != register[param.Y]) register.IncrementPC(); break;

                case 0xA000:
                    register.SetRegisterI(param.NNN); break;

                case 0xB000:
                    register.JumpToAddress((ushort)(register[0] + param.NNN)); break;

                case 0xC000:
                    register.StoreValue(param.X, (byte)((byte)Random.Next(0, byte.MaxValue) & param.NN)); break;

                case 0xE000:
                    last = (byte)(param.Value & 0x000F);

                    switch (last)
                    {
                        case 14:
                            if (register[param.X] == Keyboard) register.IncrementPC();
                            break;
                        case 1:
                            if (register[param.X] != Keyboard) register.IncrementPC();
                            break;
                    }

                    break;

                case 0xD000:
                    var sprites = ram.GetValues(register.I, param.N);
                    display.Draw(sprites, register[param.X], register[param.Y], out var overridePixel);
                    register.StoreValueOnRegister0xF(Convert.ToByte(overridePixel));

                    break;

                case 0xF000:
                    last = (byte)(param.Value & 0x00FF);

                    switch (last)
                    {
                        case 0x7: //FX07
                            register.StoreValue(param.X, DelayTimer);
                            break;
                        case 0x0A:
                            var keyPressed = (byte)Console.ReadKey().Key;
                            keyPressed = 1;
                            register.StoreValue(param.X, keyPressed);
                            break;
                        case 0x15:
                            DelayTimer = register[param.X];
                            break;
                        case 0x18: //FX18
                            SoundTimer = register[param.X];
                            break;
                        case 0x1E:
                            register.IncreaseRegisterI(register[param.X]);
                            break;
                        case 0x29:
                            register.SetRegisterI(ram.GetFontAddress(register[param.X]));
                            break;
                        case 0x33:
                            var bcd = BCD.GetBytes(register[param.X]);
                            ram.CopyValuesToRam(bcd, register.I);
                            break;
                        case 0x55:
                            ram.CopyValuesToRam(register.GetRegisters(0, param.X), register.I);
                            break;
                        case 0x65:
                            register.CopyValuesToRegisters(ram.GetValues(register.I, param.X), 0);
                            break;

                    }
                    break;
            }

            Thread.Sleep(1);
        }
    }
}

