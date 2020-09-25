using Chip8.CPU;
using SDL2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace Chip8
{
    class Program
    {

        static void Main(string[] args)
        {
            new Machine().StartProgram(File.ReadAllBytes("roms/Pong.ch8"));
        }
    }
}
