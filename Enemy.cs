﻿using Microsoft.Xna.Framework;
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
        public Enemy(Vector2 position)
        {
            this.Position = position;
            velocity = new Vector2(0, 1);
            speed = 100f;
            sprite = Asset.enemySprite;
        }
      

        public override void OnCollision(GameObject other)
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            Move(gameTime);

        }
    }
}
