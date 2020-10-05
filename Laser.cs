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
            sprite = Asset.laserSprite;
        }

        public override void OnCollision(GameObject other)
        {
            if (other is Enemy)
            {
                // Destroys the laser and the enemy once collision between aforementioned GameObjects happens.
                GameWorld.Instance.Destroy(other);

                if (GameWorld.Instance.IsServer)
                {
                    GameWorld.Instance.ServerInstance.Send("Destroy|" + other.ID);
                    //GameWorld.Instance.ServerInstance.Send("Destroy|" + this.ID);
                    
                    GameWorld.Instance.ServerInstance.Send("Point");
                    Highscore.Instance.Points++;
                }
                else
                {
                    GameWorld.Instance.ClientInstance.Send("Destroy|" + other.ID);
                    //GameWorld.Instance.ClientInstance.Send("Destroy|" + this.ID);
                }

                GameWorld.Instance.Destroy(this);
            }
        }

        public override void Update(GameTime gameTime)
        {
            Move(gameTime);
        }
    }
}
