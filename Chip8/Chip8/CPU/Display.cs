using SDL2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chip8.CPU
{
    public class Display
    {
        public static AutoResetEvent DisplayStarted { get; } = new AutoResetEvent(false);

        private uint[,] display = new uint[64, 32];


        public void Init()
        {
            SdlManager.Instance.Init();
        }

        public void Draw(byte[] sprites, int x, int y, out byte overridePixel)
        {
            List<Pixel> pixels = new List<Pixel>();

            overridePixel = 0;

            foreach (var sprite in sprites)
            {
                
                int currentX = x % 64;
                int currentY = y % 32;

                for (int j = 0; j < 8; j++)
                {
                    var spritePixel = (byte)((sprite >> (7 - j)) & 0x01);

                    if (spritePixel == 1 && display[currentX, currentY] != 0)
                        overridePixel = 1;

                    display[currentX, currentY] = (display[currentX, currentY] != 0 && spritePixel == 0) || (display[currentX, currentY] ==0 && spritePixel == 1) ? 0xffffffff  : 0;

                    pixels.Add(new Pixel((byte)currentX, (byte)currentY, display[currentX, currentY]));

                    currentX = ++currentX % 64;
                }
                y++;
            }

            SpriteCreated?.Invoke(pixels.ToArray());
        }


        public void ClearDisplay() => Array.Clear(display, 0, display.Length);

        public delegate void DrawSprite(Pixel[] pixels);
        public event DrawSprite SpriteCreated;
    }
}
