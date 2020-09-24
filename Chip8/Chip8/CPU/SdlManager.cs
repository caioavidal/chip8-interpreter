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
        uint lastFrame = 0;
        uint lastTime = 0;
        int fps = 0;
        int frameCount = 0;
        static IntPtr window;

        private static SdlManager instance;
        public static SdlManager Instance => instance = instance ?? new SdlManager();

        private SdlManager()
        {

        }

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
                    while (SDL.SDL_PollEvent(out var sdlEvent) > 0)
                    {
                        switch (sdlEvent.type)
                        {
                            case SDL.SDL_EventType.SDL_QUIT:
                                running = false;
                                break;

                          //  case SDL.SDL_EventType.SDL_KEYDOWN:

                        }
                    }

                    lastFrame = SDL.SDL_GetTicks();
                    if (lastFrame >= (lastTime + 1000))
                    {
                        lastTime = lastFrame;
                        fps = frameCount;
                        frameCount = 0;
                    }

                    SDL.SDL_UpdateWindowSurface(window);
                    SDL.SDL_Delay(10);
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

            //frameCount++;
            //int timerFPS = (int)(SDL.SDL_GetTicks() - lastFrame);
            //if (timerFPS < 2)
            //{
            //    SDL.SDL_Delay((uint)(2 - timerFPS));
            //}
            SDL.SDL_RenderPresent(sdlRenderer);
        }

    }
}
