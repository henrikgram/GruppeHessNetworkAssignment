using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GruppeHessNetworkAssignment
{
    /// <summary>
    /// Class for GameObjects, with methods that all gameObjects in the game need.
    /// </summary>
    public abstract class GameObject
    {
        #region Fields

        public Vector2 Position { get; set; }
        protected Vector2 origin;
        protected Texture2D sprite;
        protected Vector2 velocity;
        protected float speed;
        protected SpriteFont spriteFont;

        #endregion

        public int ID = GameWorld.Instance.ObjectID;

        public virtual Rectangle CollisionBox
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, sprite.Width, sprite.Height);
            }
        }

        #region Methods

        /// <summary>
        /// Update method for gameobjects to use.
        /// </summary>
        /// <param name="gameTime"></param>
        public abstract void Update(GameTime gameTime);

        /// <summary>
        /// Draw method to make sure all GameObjects are drawn.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, Position, null, Color.White, 0, origin, 1, SpriteEffects.None, 0);
        }

        /// <summary>
        /// To enable movement.
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Move(GameTime gameTime)
        {
            //Calculates deltaTime based on t he gameTime
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Moves the player based on the result from HandleInput, speed and deltatime
            Position += ((velocity * speed) * deltaTime);
        }

        /// <summary>
        /// What happens on collision.
        /// </summary>
        /// <param name="other"></param>
        public abstract void OnCollision(GameObject other);

        /// <summary>
        /// Makes sure to check collision.
        /// </summary>
        /// <param name="other"></param>
        public void CheckCollision(GameObject other)
        {
            if (CollisionBox.Intersects(other.CollisionBox))
            {
                OnCollision(other);
            }
        }

        #endregion
    }
}
