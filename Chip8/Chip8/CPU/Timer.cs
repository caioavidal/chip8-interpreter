using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Chip8.CPU
{
    public class Timer
    {
        public byte DelayTimer { get; private set; }
        public byte SoundTimer { get; private set; }

        private Stopwatch stopWatch = new Stopwatch();
        private Stopwatch soundStopWatch = new Stopwatch();


        public void Start()
        {
            stopWatch.Start();
            soundStopWatch.Start();
        }

        public void UpdateTimers()
        {
            DecreaseSoundTimer();
            DecreaseDelayTimer();
        }

        public void SetTimer(TimerType timer, byte value)
        {
            switch (timer)
            {
                case TimerType.Delay: DelayTimer = value; break;
                case TimerType.Sound: SoundTimer = value; break;
            }
        }

        private void DecreaseDelayTimer() => DecreaseTimer(stopWatch, TimerType.Delay);
        private void DecreaseSoundTimer()
        {
            if (SoundTimer == 1)
            {
                Console.Beep();
            }
            DecreaseTimer(soundStopWatch, TimerType.Sound);
           
        }

        private void DecreaseTimer(Stopwatch stopWatch, TimerType timer)
        {
            stopWatch.Stop();

            if (stopWatch.ElapsedMilliseconds >= (1000 / 60))
            {
                var timerValue = timer == TimerType.Delay ? DelayTimer : SoundTimer;
                if (timerValue > 0)
                {
                    if (timer == TimerType.Delay) DelayTimer--;
                    if (timer == TimerType.Sound) SoundTimer--;
                }
                stopWatch.Restart();
            }
            else
            {
                stopWatch.Start();
            }
        }
    }
    public enum TimerType
    {
        Delay, Sound
    }
}
