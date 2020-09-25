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
        private Timer cooldown;

        public Player(Vector2 position)
        {
            this.position = position;
            speed = 200f;
            sprite = Asset.playerSprite;
            cooldown = new TimeSpan(0, 0, 1);
        }


      

        private void HandleInput()
        {
            //Resets velocity
            //Makes sure that we will stop moving
            //When no keys are pressed
            velocity = Vector2.Zero;

            //Get the current keyboard state
            KeyboardState keyState = Keyboard.GetState();

    
            if (keyState.IsKeyDown(Keys.A))
            {
                //Move left
                velocity += new Vector2(-1, 0);
            }
          
            if (keyState.IsKeyDown(Keys.D))
            {
                //Move right
                velocity += new Vector2(1, 0);
            }





            if (keyState.IsKeyDown(Keys.Space) && canShoot)
            {
                GameWorld.Instantiate(new Laser(position));
                canShoot = false;
            }

            if (keyState.IsKeyUp(Keys.Space))
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
        }

    

        public override void OnCollision(GameObject other)
        {
         
        }
    }
}
