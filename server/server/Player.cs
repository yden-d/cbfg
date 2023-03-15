using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    class Player
    {
        public int id;
        public string username;

        public Vector3 position;
        public float rotation;

        public Player(int playerid, string user, Vector3 pos)
        {
            id = playerid;
            username = user;
            position = pos;
        }
    }
}
