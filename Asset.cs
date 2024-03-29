﻿using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GruppeHessNetworkAssignment
{
    static class Asset
    {
        public static Texture2D playerSprite;
        public static Texture2D enemySprite;
        public static Texture2D collisionBox;
        public static Texture2D laserSprite;

        public static void LoadContent(ContentManager content)
        {
            playerSprite = content.Load<Texture2D>("1fwd");
            enemySprite = content.Load<Texture2D>("enemyBlack1");
            collisionBox = content.Load<Texture2D>("CollisionBox");
            laserSprite = content.Load<Texture2D>("laserGreen05");
        }

    }
}
