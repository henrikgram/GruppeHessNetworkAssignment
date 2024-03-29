﻿using GruppeHessNetworkAssignment.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Threading;

namespace GruppeHessNetworkAssignment
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class GameWorld : Game
    {
        private List<GameObject> gameObjects = new List<GameObject>();
        private static List<GameObject> newGameObjects = new List<GameObject>();
        private static List<GameObject> deletedGameObjects = new List<GameObject>();

        private TimeSpan timeTillNewInvasionForce = TimeSpan.Zero;
        private Random rnd = new Random();
        private int screenHeight = 1000;
        public bool ProgramRunning { get; set; } = true;

        private Server server;
        private Client client;
        private static GameWorld instance;
        private Player player;

        private bool startScreen = true;
        private bool serverMode = false;


        public static GameWorld Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new GameWorld();
                }
                return instance;
            }
        }

        public Vector2 ScreenSize { get; private set; }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public GameWorld()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            ServerClientSetup();

            // CHANGES THE SCREEN SIZE.
            graphics.PreferredBackBufferHeight = screenHeight;
            graphics.ApplyChanges();
            ScreenSize = new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            base.Initialize();
        }

        /// <summary>
        /// Determine whether the game should start in Server-mode (host) or Client-mode (join server).
        /// Runs at the beginning of the game thought the Initialize method.
        /// </summary>
        private void ServerClientSetup()
        {
            while (startScreen == true)
            {
                Console.WriteLine("Server (S) or Client (C)?");
                string input = Console.ReadLine().ToUpper();

                if (input == "S")
                {
                    // Instantiates the server, if the game starts in server mode.

                  
                    server = new Server();
                    serverMode = true;
                    startScreen = false;

                    Console.WriteLine($"Server started on port: {server.Port} ");

                    //server.Send((player.Position.X).ToString());
                }

                else if (input == "C")
                {
                    // Instantiates a client, if the game starts in player mode.

                    Console.WriteLine("What port would you like to connect to?");
                    client = new Client(Int32.Parse(Console.ReadLine()));


                    serverMode = false;
                    startScreen = false;

                    //client.Send((player.Position.X).ToString());
                }

                else
                {
                    Console.WriteLine("Invalid input.");
                    startScreen = true;
                }
            }
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Asset.LoadContent(Content);

            gameObjects.Add(player = new Player(new Vector2(ScreenSize.X/2, ScreenSize.Y-Asset.playerSprite.Height)));
            gameObjects.Add(new Enemy(new Vector2(300, 300)));

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
           

           

            if (serverMode)
            {
                server.Send((player.Position.X).ToString());
            }

            if (!serverMode)
            {
                client.Send((player.Position.X).ToString());
            }


            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            //ads all objects in list-newobjects to list-gameobjects.
            gameObjects.AddRange(newGameObjects);

            foreach (GameObject deletedObject in deletedGameObjects)
            {
                gameObjects.Remove(deletedObject);
            }

            //deletes objects in list-deleteobjects.
            deletedGameObjects.Clear();
            //deletes objects in list-newobjects.
            newGameObjects.Clear();


            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.Update(gameTime);

                foreach (GameObject other in gameObjects)
                {
                    gameObject.CheckCollision(other);
                }
            }

            AddNewEnemyShips();

            if (timeTillNewInvasionForce > TimeSpan.Zero)
            {
                timeTillNewInvasionForce -= gameTime.ElapsedGameTime;
            }

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            // TODO: Add your drawing code here
            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.Draw(spriteBatch);
                DrawCollisionBox(gameObject);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public static void Instantiate(GameObject gameObject)
        {
            newGameObjects.Add(gameObject);
        }

        public static void Destroy(GameObject gameObject)
        {
            deletedGameObjects.Add(gameObject);
        }

        private void DrawCollisionBox(GameObject gameObject)
        {
            /// Draws the collisionboxes.
            Rectangle collisionBox = gameObject.CollisionBox;
            Rectangle topLine = new Rectangle(collisionBox.X, collisionBox.Y, collisionBox.Width, 1);
            Rectangle bottomLine = new Rectangle(collisionBox.X, collisionBox.Y + collisionBox.Height, collisionBox.Width, 1);
            Rectangle rightLine = new Rectangle(collisionBox.X + collisionBox.Width, collisionBox.Y, 1, collisionBox.Height);
            Rectangle leftLine = new Rectangle(collisionBox.X, collisionBox.Y, 1, collisionBox.Height);

            /// Makes sure the collisionbox adjusts to each sprite.
            spriteBatch.Draw(Asset.collisionBox, topLine, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 1);
            spriteBatch.Draw(Asset.collisionBox, bottomLine, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 1);
            spriteBatch.Draw(Asset.collisionBox, rightLine, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 1);
            spriteBatch.Draw(Asset.collisionBox, leftLine, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 1);
        }

        private void AddNewEnemyShips()
        {
            if (timeTillNewInvasionForce <= TimeSpan.Zero)
            {
                for (int i = 0; i < 5; i++)
                {
                    newGameObjects.Add(new Enemy(new Vector2(rnd.Next(0, (int)ScreenSize.X - Asset.enemySprite.Width), 0 - Asset.enemySprite.Height)));
                }

                timeTillNewInvasionForce = new TimeSpan(0, 0, 3);
            }
        }
    }
}
