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
    /// <summary>
    /// Class used to load all the assets (sprites) used in the game.
    /// </summary>
    static class Asset
    {
        public static Texture2D PlayerSprite { get; private set; }
        public static Texture2D EnemySprite { get; private set; }
        public static Texture2D CollisionBox { get; private set; }
        public static Texture2D LaserSprite { get; private set; }
        public static Texture2D ClientPlayerSprite { get; private set; }

        public static SpriteFont ScoreFont { get; private set; }

        /// <summary>
        /// Loads all sprites in the game.
        /// </summary>
        /// <param name="content"></param>
        public static void LoadContent(ContentManager content)
        {
            PlayerSprite = content.Load<Texture2D>("1fwd");
            ClientPlayerSprite = content.Load<Texture2D>("1fwd");
            LaserSprite = content.Load<Texture2D>("laserGreen05");

            EnemySprite = content.Load<Texture2D>("enemyBlack1");
            CollisionBox = content.Load<Texture2D>("CollisionBox");

            ScoreFont = content.Load<SpriteFont>("ScoreFont");
        }
    }
}
