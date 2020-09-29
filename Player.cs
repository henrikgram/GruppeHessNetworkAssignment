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
                GameWorld.Instantiate(new Laser(new Vector2(Position.X + Asset.playerSprite.Width / 2 - 5, Position.Y - 30)));
                // Client sends a message to the server, so the server knows to shoot from the player as well.
                GameWorld.Instance.ClientInstance.Send("s");
                // Two next functions make sure shoot has a cool down.
                canShoot = false;
                cooldown = new TimeSpan(0, 0, 0, 0, 100);
            }

            // Gives the shoot function a cooldown, so the player can't shoot endlessly.
            if (keyState.IsKeyUp(Keys.Space) && cooldown <= TimeSpan.Zero)
            {
                // Once the cool down reaches 0, the player can shoot again.
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


            if (GameWorld.Instance.IsServer == true && GameWorld.Instance.ServerInstance.ReturnData != null)
            {

                //try
                {
                    string serverInput = GameWorld.Instance.ServerInstance.ReturnData;

                    if (serverInput == "s")
                    {
                        GameWorld.Instantiate(new Laser(new Vector2(Position.X + Asset.playerSprite.Width / 2 - 5, Position.Y - 30)));
                    }
                    //else if (serverInput == "P")
                    //{

                    //}
                    else
                    {
                        int test = test = Convert.ToInt32(Math.Round(Convert.ToDouble(GameWorld.Instance.ServerInstance.ReturnData)));

                        Position = new Vector2(test, Position.Y);
                    }
                }
                //catch (Exception e)
                {

                    //Console.WriteLine("dn " + e);
                }

            }

            else
            {
                HandleInput();
                Move(gameTime);

                if (cooldown > TimeSpan.Zero)
                {
                    cooldown -= gameTime.ElapsedGameTime;
                }
            }



        }

        public override void OnCollision(GameObject other)
        {
            // Player doesn't have any collision with anything.
        }
    }
}