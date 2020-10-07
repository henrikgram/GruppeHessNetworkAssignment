using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GruppeHessNetworkAssignment
{
    class Laser : GameObject
    {
        public Laser(Vector2 position)
        {
            this.Position = position;
            velocity = new Vector2( 0, -1);
            speed = 300f;
            sprite = Asset.LaserSprite;
        }

        /// <summary>
        /// Determines what happens when a laserobject collides with another object.
        /// In this case a laser destroys an enemy upon collision and then sends a message to the client/server to destroy the enemy on their end as well. 
        /// </summary>
        /// <param name="other"></param>
        public override void OnCollision(GameObject other)
        {
            if (other is Enemy)
            {
                // Destroys the laser and the enemy once collision between aforementioned GameObjects happens.
                GameWorld.Instance.Destroy(other);

                if (GameWorld.Instance.IsServer)
                {
                    // Makes sure both the laser and the enemy sprite is destroyed on the client as well.
                    GameWorld.Instance.ServerInstance.Send("Destroy|" + other.ID);
                    GameWorld.Instance.ServerInstance.Send("Destroy|" + this.ID);

                    // Makes sure the client's points go up as well.
                    GameWorld.Instance.ServerInstance.Send("Point");
                    // Adds a point on the server.
                    Highscore.Instance.Points++;
                }
                else
                {
                    // Makes sure both the laser and the enemy sprite is destroyed on the server as well.
                    GameWorld.Instance.ClientInstance.Send("Destroy|" + other.ID);
                    GameWorld.Instance.ClientInstance.Send("Destroy|" + this.ID);
                }

                GameWorld.Instance.Destroy(this); // Finally the laser itself is destroyed.
            }
        }

        /// <summary>
        /// Update method for Laser. Moves the laser and destroys it when out of bounds. 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            // Makes sure the lasers can move.
            Move(gameTime);

            // Makes sure the lasers are removed from the game once they move above the top of the screen.
            if (Position.Y < 0 - Asset.LaserSprite.Height)
            {
                GameWorld.Instance.Destroy(this);
            }
        }
    }
}
