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
    class Enemy : GameObject
    {
        //private int iD;

        public Enemy(Vector2 position/*, int shipID*/)
        {
            this.Position = position;
            velocity = new Vector2(0, 1);
            speed = 100f;
            sprite = Asset.enemySprite;
            //this.ID = shipID;
        }


        public override void OnCollision(GameObject other)
        {

        }

        public override void Update(GameTime gameTime)
        {
            Move(gameTime);

            if (Position.Y > GameWorld.Instance.ScreenHeight + Asset.enemySprite.Height)
            {
                GameWorld.Destroy(this);

                if (GameWorld.Instance.IsServer)
                {
                    GameWorld.Instance.ServerInstance.Send("Lose hp");
                    GameWorld.Instance.PlayerServer.PlayerHealth--;
                }
            }
        }
    }
}
