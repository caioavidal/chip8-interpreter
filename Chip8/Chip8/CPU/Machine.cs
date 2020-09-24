using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chip8.CPU
{
    public class Machine
    {
        private readonly Cpu cpu;

        private readonly RAM ram = new RAM();

        private readonly Display display = new Display();


        public Machine()
        {
            cpu = new Cpu(display, ram);
        }
        public bool ProgramLoaded { get; private set; }

        public void LoadProgram(byte[] program)
        {
            ram.CopyValuesToRam(program, 0x200);
            ProgramLoaded = true;
        }
        public void StartProgram()
        {
            if (ProgramLoaded)
            {
                display.Init();
                Display.DisplayStarted.WaitOne();
                //Thread.Sleep(1500);
                cpu.Run();

            }
        }
        public void StartProgram(byte[] program)
        {
            LoadProgram(program);
            StartProgram();
        }
    }
}
