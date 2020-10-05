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
    public class Player : GameObject
    {
        private bool canShoot = true;
        private TimeSpan cooldown;

        public int PlayerHealth { get; set; }


        public Player(Vector2 position)
        {
            this.Position = position;
            speed = 800f;
            cooldown = new TimeSpan(0, 0, 0, 0, 0);

            // Sets the correct sprite depending on whether it's a client or the server
            // making a player character.
            if (GameWorld.Instance.SetUpServerPlayer)
            {
                PlayerHealth = 3;

                sprite = Asset.playerSprite;
                GameWorld.Instance.SetUpServerPlayer = false;
            }
            else
            {
                sprite = Asset.clientPlayerSprite;
            }
        }

        private void HandleInput(Player player)
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
                if (player.Position.X >= 0)
                {
                    velocity += new Vector2(-1, 0);
                }
            }

            if (keyState.IsKeyDown(Keys.Right))
            {
                //Move right if inside bounds.
                if (player.Position.X <= GameWorld.Instance.ScreenSize.X - sprite.Width)
                {
                    velocity += new Vector2(1, 0);
                }
            }

            if (keyState.IsKeyDown(Keys.Space) && canShoot && GameWorld.Instance.IsServer == false)
            {
                // Shoot
                Laser newLaser = new Laser(new Vector2(Position.X + Asset.playerSprite.Width / 2 - 5, Position.Y - 30));

                GameWorld.Instance.ClientInstance.Send("New|Laser|" + newLaser.ID + "|" + newLaser.Position.X + "|" + newLaser.Position.Y);

                Console.WriteLine("New Laser : ID : " + newLaser.ID + " Position : " + newLaser.Position.ToString());

                GameWorld.Instance.Instantiate(newLaser);
                //GameWorld.Instance.ClientInstance.Send("s");
                // Client sends a message to the server, so the server knows to shoot from the player as well.
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
                string serverInput = GameWorld.Instance.ServerInstance.ReturnData;

                if (serverInput.Contains("Update|Player"))
                {
                    string[] inputParameters = serverInput.Split('|');

                    float playPosX = float.Parse(inputParameters[2]);
                    float playposY = float.Parse(inputParameters[3]);

                    Position = new Vector2(playPosX, playposY);
                }
            }

            Player player;

            if (GameWorld.Instance.IsServer)
            {
                player = GameWorld.Instance.PlayerServer;
            }
            else
            {
                player = GameWorld.Instance.PlayerClient;
            }

            if (player != null)
            {
                HandleInput(player);
                Move(gameTime);

                if (cooldown > TimeSpan.Zero)
                {
                    cooldown -= gameTime.ElapsedGameTime;
                }
            }

            if (GameWorld.Instance.PlayerServer.PlayerHealth <= 0)
            {
                Death();
            }
        }

        public override void OnCollision(GameObject other)
        {
            // Player doesn't have any collision with anything.
        }

        public void Death()
        {
            // Insert end game lose shit. Maybe a loser screen (just a sprite font is fine).
        }
    }
}
