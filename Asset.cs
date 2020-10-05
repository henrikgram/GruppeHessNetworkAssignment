using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
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
        public static Texture2D clientPlayerSprite;

        public static SpriteFont scoreFont;

        public static void LoadContent(ContentManager content)
        {
            playerSprite = content.Load<Texture2D>("1fwd");
            clientPlayerSprite = content.Load<Texture2D>("1fwd");
            laserSprite = content.Load<Texture2D>("laserGreen05");

            enemySprite = content.Load<Texture2D>("enemyBlack1");
            collisionBox = content.Load<Texture2D>("CollisionBox");

            scoreFont = content.Load<SpriteFont>("ScoreFont");
        }
    }
}
