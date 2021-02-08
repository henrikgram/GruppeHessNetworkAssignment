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
    /// <summary>
    /// Class for creating enemies.
    /// </summary>
    class Enemy : GameObject
    {
        public Enemy(Vector2 position)
        {
            this.Position = position;
            velocity = new Vector2(0, 1);
            speed = 100f;
            sprite = Asset.EnemySprite;
        }

        /// <summary>
        /// Collision code for enemies.
        /// In this case, it's not needed.
        /// </summary>
        /// <param name="other"></param>
        public override void OnCollision(GameObject other)
        {
            // No collision needed. It's all in the laser class.
        }

        /// <summary>
        /// Update method for Enemies.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            // Makes sure the enemies can move.
            Move(gameTime);

            // If an enemy moves down below the bottom edge of the screen,
            // the player looses health and the enemy sprite is removed from the game.
            if (Position.Y > GameWorld.Instance.ScreenHeight + Asset.EnemySprite.Height)
            {
                GameWorld.Instance.Destroy(this);

                if (GameWorld.Instance.IsServer)
                {
                  // Makes sure the client looses hp as well.
                  GameWorld.Instance.ServerInstance.Send("Lose hp");
                  GameWorld.Instance.PlayerServer.PlayerHealth--;
                }
            }
        }
    }
}
