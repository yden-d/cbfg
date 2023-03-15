using System;
using System.Threading;

namespace Game
{
    class Program
    {
        private static bool running = false;
        static void Main(string[] args)
        {
            running = true;
            Thread mainThread = new Thread(new ThreadStart(MainThread));
            mainThread.Start();

            Server.Start(5, 585);
        }

        private static void MainThread()
        {
            Console.WriteLine($"Main thread started. Operating at {Constants.TICKS_PER_SEC} ticks per second");
            DateTime nextLoop = DateTime.UtcNow;

            while (running)
            {
                while(nextLoop < DateTime.UtcNow)
                {
                    GameLogic.Update();
                    nextLoop = nextLoop.AddMilliseconds(Constants.MS_PER_TICK); ;
                    
                    //wait until the next proper tick
                    //reduce processing power
                    if(nextLoop > DateTime.UtcNow)
                    {
                        Thread.Sleep(nextLoop - DateTime.UtcNow);
                    }
                }
            }
        }
    }
}