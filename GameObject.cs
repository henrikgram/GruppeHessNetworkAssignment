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
    public abstract class GameObject
    {
        public Vector2 Position { get; set; }
        protected Vector2 origin;
        protected Texture2D sprite;
        protected Vector2 velocity;
        protected float speed;

        public virtual Rectangle CollisionBox
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, sprite.Width, sprite.Height);
            }
        }
     
        public abstract void Update(GameTime gameTime);

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, Position, null, Color.White, 0, origin, 1, SpriteEffects.None, 0);
        }

        public virtual void Move(GameTime gameTime)
        {
            //Calculates deltaTime based on t he gameTime
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Moves the player based on the result from HandleInput, speed and deltatime
            Position += ((velocity * speed) * deltaTime);
        }

        public abstract void OnCollision(GameObject other);

        public void CheckCollision(GameObject other)
        {
            if (CollisionBox.Intersects(other.CollisionBox))
            {
                OnCollision(other);
            }
        }
    }
}
