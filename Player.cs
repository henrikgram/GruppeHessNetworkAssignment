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
        private bool playerIsSet = false;
        private TimeSpan cooldown;
        private Player player;

        public int PlayerHealth { get; set; }

        public Player(Vector2 position)
        {
            this.Position = position;
            speed = 800f;
            cooldown = new TimeSpan(0, 0, 0, 0, 0);

            // Sets the correct sprite depending on whether it's a client or the server
            // making a player character.
            // SetUpServerPlayer becomes true in GameWorld if the game is run in server-mode.
            if (GameWorld.Instance.SetUpServerPlayer)
            {
                // If server.
                PlayerHealth = 3;
                sprite = Asset.playerSprite;
                // Sets SetUpServerPlayer to falls to make sure we can continue in our code once the setup is over.
                GameWorld.Instance.SetUpServerPlayer = false;
            }
            else
            {
                // If client.
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

            if (keyState.IsKeyDown(Keys.Left) && GameWorld.Instance.IsServer && GameWorld.Instance.PlayerServer == this ||
                keyState.IsKeyDown(Keys.Left) && !GameWorld.Instance.IsServer && GameWorld.Instance.PlayerClient == this)
            {
                //Move left if inside bounds.
                if (player.Position.X >= 0)
                {
                    velocity += new Vector2(-1, 0);
                }
            }

            if (keyState.IsKeyDown(Keys.Right) && GameWorld.Instance.IsServer && GameWorld.Instance.PlayerServer == this ||
                keyState.IsKeyDown(Keys.Right) && !GameWorld.Instance.IsServer && GameWorld.Instance.PlayerClient == this)
            {
                //Move right if inside bounds.
                if (player.Position.X <= GameWorld.Instance.ScreenSize.X - sprite.Width)
                {
                    velocity += new Vector2(1, 0);
                }
            }

            if (keyState.IsKeyDown(Keys.Space) && canShoot)
            {
                // Creates a Laser object with a position.
                Laser newLaser = new Laser(new Vector2(player.Position.X + sprite.Width / 2 - 5, player.Position.Y - 30));

                // Client shoot.
                if (!GameWorld.Instance.IsServer && GameWorld.Instance.PlayerClient == this)
                {
                    // Shoot, instantiates the created laser on the correct player ship.
                    // In this case, the client-player.
                    GameWorld.Instance.Instantiate(newLaser);
                    // Sends the information to the server.
                    GameWorld.Instance.ClientInstance.Send("New|Laser|" + newLaser.ID + "|" + newLaser.Position.X + "|" + newLaser.Position.Y);
                    Console.WriteLine("New Laser : ID : " + newLaser.ID + " Position : " + newLaser.Position.ToString());
                }
                // Server shoot.
                if (GameWorld.Instance.IsServer && GameWorld.Instance.PlayerServer == this)
                {
                    // Shoot, instantiates the created laser on the correct player ship.
                    // In this case, the server-player.
                    GameWorld.Instance.Instantiate(newLaser);
                    // Sends the information to the client.
                    GameWorld.Instance.ServerInstance.Send("New|Laser|" + newLaser.ID + "|" + newLaser.Position.X + "|" + newLaser.Position.Y);
                }

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
            // Sets player to a certain player,
            // depending on whether the game is running as a server or client.
            if(!playerIsSet)
            {
                if (GameWorld.Instance.IsServer)
                {
                    // If server.
                    player = GameWorld.Instance.PlayerServer;
                }
                else
                {
                    // If client.
                    player = GameWorld.Instance.PlayerClient;
                }
                // Makes sure we only set player once during runtime.
                playerIsSet = true;
            }

            // Only runs if player isn't null. Makes sure we get no weird exceptions/errors.
            if (player != null)
            {
                HandleInput(player);
                Move(gameTime);

                // Cooldown counter.
                if (cooldown > TimeSpan.Zero)
                {
                    cooldown -= gameTime.ElapsedGameTime;
                }
            }

            // Makes sure the players die if their shared hp reaches 0.
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
