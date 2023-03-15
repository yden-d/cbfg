using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    internal class GameLogic
    {
        private static Dictionary<string, bool[]> damageTracker = new Dictionary<string, bool[]>();
        public static void Update()
        {
            ThreadManager.UpdateMain();
        }

        public static async Task FairPlayEnforcer(float damage, int observerClient, int hitClient)
        {
            //no client will have id = 0
            observerClient--;

            string key = hitClient + "" + damage;

            //only the first player to report witnessing the collision enters this loop
            if (!damageTracker.ContainsKey(key))
            {
                bool[] clientVerification = new bool[Server.currPlayers];
                clientVerification[observerClient] = true;
                damageTracker.Add(key, clientVerification);

                //sleep for 1sec
                await Task.Delay(1000);

                //remove if not fully verified
                damageTracker.Remove(key);
                return;
            }

            damageTracker[key][observerClient] = true;

            for(int i = 0; i < Server.currPlayers; i++) if (!damageTracker[key][i]) return;

            ServerSend.SendDamage(hitClient, damage);

        }
    }
}
