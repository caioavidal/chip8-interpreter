using SDL2;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chip8.CPU
{
    public class SdlManager
    {
        static IntPtr sdlRenderer;
        const int FPS = 60;
        const int frameDelay = 1000 / FPS;
        uint frameStart;
        int frameTime;
        static IntPtr window;

        public Queue<Pixel[]> pixelsQueue = new Queue<Pixel[]>();

        private static SdlManager instance;
        public static SdlManager Instance => instance = instance ?? new SdlManager();

        private SdlManager()
        {

        }

        public Queue<Action> actions = new Queue<Action>();

        private static bool started = false;
        public void Init()
        {
            Task.Run(() =>
            {
                if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
                {
                    return;
                }

                if (SDL.SDL_CreateWindowAndRenderer(1024, 512, 0, out window, out sdlRenderer) < 0) return;

                SDL.SDL_SetWindowTitle(window, "Chip 8 Emulator");

                SDL.SDL_RenderSetScale(sdlRenderer, 16, 16);

                started = true;

                bool running = true;

                Display.DisplayStarted.Set();

                while (running)
                {

                    frameStart = SDL.SDL_GetTicks();

                    while (SDL.SDL_PollEvent(out var sdlEvent) > 0)
                    {
                        switch (sdlEvent.type)
                        {
                            case SDL.SDL_EventType.SDL_QUIT:
                                running = false;
                                break;

                            case SDL.SDL_EventType.SDL_KEYDOWN:
                                switch (sdlEvent.key.keysym.sym)
                                {
                                    case SDL.SDL_Keycode.SDLK_0: Cpu.Keyboard = 0; break;
                                    case SDL.SDL_Keycode.SDLK_1: Cpu.Keyboard = 1; break;
                                    case SDL.SDL_Keycode.SDLK_2: Cpu.Keyboard = 2; break;
                                    case SDL.SDL_Keycode.SDLK_3: Cpu.Keyboard = 3; break;
                                    case SDL.SDL_Keycode.SDLK_4: Cpu.Keyboard = 4; break;
                                    case SDL.SDL_Keycode.SDLK_5: Cpu.Keyboard = 5; break;
                                    case SDL.SDL_Keycode.SDLK_6: Cpu.Keyboard = 6; break;
                                    case SDL.SDL_Keycode.SDLK_7: Cpu.Keyboard = 7; break;
                                    case SDL.SDL_Keycode.SDLK_8: Cpu.Keyboard = 8; break;
                                    case SDL.SDL_Keycode.SDLK_9: Cpu.Keyboard = 9; break;
                                    case SDL.SDL_Keycode.SDLK_a: Cpu.Keyboard = 0xA; break;
                                    case SDL.SDL_Keycode.SDLK_b: Cpu.Keyboard = 0xB; break;
                                    case SDL.SDL_Keycode.SDLK_c: Cpu.Keyboard = 0xC; break;
                                    case SDL.SDL_Keycode.SDLK_d: Cpu.Keyboard = 0xD; break;
                                    case SDL.SDL_Keycode.SDLK_e: Cpu.Keyboard = 0xE; break;
                                    case SDL.SDL_Keycode.SDLK_f: Cpu.Keyboard = 0xF; break;
                                }
                                break;
                            case SDL.SDL_EventType.SDL_KEYUP:
                                Cpu.Keyboard = 0; break;
                                //switch (sdlEvent.key.keysym.sym)
                                //{
                                //    case SDL.SDL_Keycode.SDLK_0: Cpu.Keyboard = 0; break;
                                //}
                                //break;
                        }
                    }

                 
                    while(pixelsQueue.TryDequeue(out var pixels))
                    {
                        frameTime = (int)(SDL.SDL_GetTicks() - frameStart);

                        if (frameDelay > frameTime)
                        {
                            SDL.SDL_Delay((uint)(frameDelay - frameTime));
                        }

                        DrawPixels(pixels);

                        SDL.SDL_RenderPresent(sdlRenderer);
                    }

                    SDL.SDL_UpdateWindowSurface(window);

                    Thread.Sleep(1);
                }
            });
        }



        public void ClearPixel(int x, int y) => DrawPixel(x, y, 0, 0, 0);
        public void DrawPixel(int x, int y) => DrawPixel(x, y, 255, 255, 255);
        void DrawPixel(int x, int y, byte r, byte g, byte b)
        {
            if (started == false) return;

            SDL.SDL_SetRenderDrawColor(sdlRenderer, r, g, b, 255);

            SDL.SDL_RenderDrawPoint(sdlRenderer, x, y);
        }

        public void DrawPixels(Pixel[] pixels)
        {
            foreach (var pixel in pixels)
            {
                DrawPixel(pixel.X, pixel.Y, pixel.R, pixel.G, pixel.B);
            }
        }
        public void DrawSprite(Pixel[] pixels)
        {
            pixelsQueue.Enqueue(pixels);
        }

    }

    public readonly struct Pixel
    {
        public Pixel(byte x, byte y, uint pixel)
        {
            X = x;
            Y = y;
            if(pixel == 0)
            {
                R = 0;
                G = 0;
                B = 0;
            }
            else
            {
                R = 255;
                G = 255;
                B = 255;
              
            }
        }

        public byte X { get; }
        public byte Y { get; }
        public byte R { get; }
        public byte G { get; }
        public byte B { get; }

    }
}
