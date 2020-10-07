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

        /// <summary>
        /// Update method for Highscore.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            // Has nothing it needs to update.
        }

        /// <summary>
        /// Collision method for Highscore.
        /// </summary>
        /// <param name="other"></param>
        public override void OnCollision(GameObject other)
        {
            // Has no collision.
        }
    }
}
