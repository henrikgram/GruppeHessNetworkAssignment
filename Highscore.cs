using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GruppeHessNetworkAssignment
{
    class Highscore : GameObject
    {
        private static  Highscore instance;

        public int Points { get; set; } = 0;


        public static Highscore Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Highscore();
                }
                return instance;
            }
        }

        public Highscore()
        {

        }

        public override void Update(GameTime gameTime)
        {
            // Has nothing it needs to update.
        }

        public override void OnCollision(GameObject other)
        {
            // Has no collision.
        }
    }

    public struct SaveGameData
    {
        public int Score;
    }
}
