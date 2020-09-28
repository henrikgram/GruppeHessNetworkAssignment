using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GruppeHessNetworkAssignment
{
    class Player : GameObject
    {
        private bool canShoot = true;
        private TimeSpan cooldown;

        public Player(Vector2 position)
        {
            this.Position = position;
            speed = 800f;
            sprite = Asset.playerSprite;
            cooldown = new TimeSpan(0, 0, 0, 0, 0);
        }

        private void HandleInput()
        {
            //Resets velocity
            //Makes sure that we will stop moving
            //When no keys are pressed
            velocity = Vector2.Zero;

            //Get the current keyboard state
            KeyboardState keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.Left))
            {
                //Move left if inside bounds.
                if (Position.X >= 0)
                {
                    velocity += new Vector2(-1, 0);
                }
            }

            if (keyState.IsKeyDown(Keys.Right))
            {
                //Move right if inside bounds.
                if (Position.X <= GameWorld.Instance.ScreenSize.X - sprite.Width)
                {
                    velocity += new Vector2(1, 0);
                }
            }

            if (keyState.IsKeyDown(Keys.Space) && canShoot)
            {
                // Shoot
                GameWorld.Instantiate(new Laser(new Vector2(Position.X+Asset.playerSprite.Width/2 - 5,Position.Y - 30)));
                canShoot = false;
                cooldown = new TimeSpan(0, 0, 0, 0, 100);
            }

            // Gives the shoot function a cooldown, so the player can't shoot endlessly.
            if (keyState.IsKeyUp(Keys.Space) && cooldown <= TimeSpan.Zero)
            {
                canShoot = true;
            }

            //If pressed a key, then we need to normalize the vector
            //If we don't do this we will move faster 
            //while pressing two keys at once
            if (velocity != Vector2.Zero)
            {
                velocity.Normalize();
            }
        }

        public override void Update(GameTime gameTime)
        {
            HandleInput();
            Move(gameTime);

            if (cooldown > TimeSpan.Zero)
            {
                cooldown -= gameTime.ElapsedGameTime;
            }
        }

        public override void OnCollision(GameObject other)
        {
            // Player doesn't have any collision with anything.
        }
    }
}