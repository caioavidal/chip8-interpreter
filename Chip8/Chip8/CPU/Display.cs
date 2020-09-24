using SDL2;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Chip8.CPU
{
    public class Display
    {
        private Register register;

        public Display(Register register)
        {
            this.register = register;
        }
        private byte[,] display = new byte[64, 32];

        public void Init()
        {
            SdlManager.Instance.Init();
        }

        public void Draw(byte[] sprites, int x, int y)
        {
            foreach (var sprite in sprites)
            {
                int currentX = x % 64;
                int currentY = y % 32;

                for (int j = 0; j < 8; j++)
                {
                    var pixel = (byte)(display[currentX, currentY] ^ (byte)((sprite >> (7 - j)) & 1));

                    register.StoreValueOnRegister0xF((byte)((pixel == 0 && display[currentX, currentY] == 1) ? 1 : 0));

                    display[currentX, currentY] = pixel;

                    if (pixel == 1)
                    {
                        SdlManager.Instance.DrawPixel((byte)currentX, currentY);
                    }
                    else
                    {
                        SdlManager.Instance.ClearPixel((byte)currentX, currentY);
                    }

                    currentX = ++currentX%64;
                }
                y++;
            }
        }
        public void ClearDisplay() => Array.Clear(display, 0, display.Length);
    }
}
