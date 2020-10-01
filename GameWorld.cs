﻿using GruppeHessNetworkAssignment.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading;

namespace GruppeHessNetworkAssignment
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class GameWorld : Game
    {
        private List<GameObject> gameObjects = new List<GameObject>();
        private  List<GameObject> newGameObjects = new List<GameObject>();
        private  List<GameObject> deletedGameObjects = new List<GameObject>();

        private TimeSpan timeTillNewInvasionForce = TimeSpan.Zero;
        private Random rnd = new Random(500);
        private int screenHeight = 1000;

        private Server server;
        private Client client;
        private static GameWorld instance;
        private Player player;

        private bool startScreen = true;
        private bool isServer = false;
        private byte maxPlayers = 1;

        private int objectID = 0;

        bool test = true;

        public byte PlayerCount { get; set; } = 0;
        public bool ProgramRunning { get; set; } = true;


        public static GameWorld Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameWorld();
                }
                return instance;
            }
        }

        public Vector2 ScreenSize { get; private set; }
        public bool IsServer { get => isServer; }
        public Server ServerInstance { get => server; set => server = value; }
        internal Client ClientInstance { get => client; set => client = value; }
        public int ObjectID { get => objectID++; set => objectID = value; }
        public List<GameObject> GameObjects { get => gameObjects; set => gameObjects = value; }
        public  List<GameObject> NewGameObjects { get => newGameObjects; set => newGameObjects = value; }

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
            Asset.LoadContent(Content);
            ServerClientSetup();

            IsMouseVisible = true;
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
                    isServer = true;
                    startScreen = false;

                    Console.WriteLine($"Server started on port: {server.Port} ");

                    //server.Send((player.Position.X).ToString());
                }

                else if (input == "C")
                {
                    // Instantiates a client, if the game starts in player mode.

                    Console.WriteLine("What port would you like to connect to?");
                    client = new Client(Int32.Parse(Console.ReadLine()));


                    isServer = false;
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

            //Asset.LoadContent(Content);

            gameObjects.Add(player = new Player(new Vector2(ScreenSize.X / 2, ScreenSize.Y - Asset.playerSprite.Height)));


           
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

          

            // Makes sure the client sends the player's opdated position to the server.
            if (!isServer)
            {
                //sending position to the server
                client.Send(player.Position.X.ToString());

                ////sending position to the server
                //client.Send(player.Position.X.ToString());

                ////saves the recieved message from server
                //string input = ClientInstance.ReturnData;


                ////makes sure to only work with Object position strings
                //if (input.Contains("OP"))
                //{
                //    //removes the "OP" tag
                //    input = input.Remove(0, 2);

                //    //splitting the string into multiple object strings
                //    string[] inputObjects = input.Split('|');


                //    //add new object if the amount is not the same
                //    if (gameObjects.Count - 1 < inputObjects.Length - 1)
                //    {
                //        for (int i = 0; i != inputObjects.Length-1; i++)
                //        {
                //            string[] inputParameters = inputObjects[i].Split(',');

                //            int tmpx  = Int32.Parse(inputParameters[0]);
                //            int tmpy  = Int32.Parse(inputParameters[1]);
                //            int tmpID = Int32.Parse(inputParameters[2]);
                //            string objectType = inputParameters[3];

                //            switch (objectType)
                //            {
                //                case ("Enemy"):
                //                    newGameObjects.Add(new Enemy(new Vector2(tmpx, tmpy), tmpID));
                //                    break;

                //                //case ("Laser"):
                //                //    newGameObjects.Add(new Laser(new Vector2(tmpx, tmpy), tmpID));
                //                //    break;
                //            }

                //        }
                //    }

                //    else
                //    {
                //        //update object

                //        for (int i = 0; i < inputObjects.Length-1; i++)
                //        {
                //            string[] inputParameters = inputObjects[i].Split(',');

                //            int tmpx = Int32.Parse(inputParameters[0]);
                //            int tmpy = Int32.Parse(inputParameters[1]);
                //            int tmpID = Int32.Parse(inputParameters[2]);

                //            foreach (GameObject gameObject in gameObjects)
                //            {
                //                if (gameObject.Id == tmpID)
                //                {
                //                    gameObject.Position = new Vector2(tmpx,tmpy);
                //                }
                //            }
                //        }
                //    }

                //    //Console.WriteLine(inputObjects.Length);
                //}
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                // Makes sure all the threads stop running.
                ProgramRunning = false;
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

            player.Update(gameTime);

            if (isServer)
            {
                foreach (GameObject gameObject in gameObjects)
                {
                    gameObject.Update(gameTime);

                    foreach (GameObject other in gameObjects)
                    {
                        gameObject.CheckCollision(other);
                    }
                }
            }
        


            if (timeTillNewInvasionForce > TimeSpan.Zero)
            {
                timeTillNewInvasionForce -= gameTime.ElapsedGameTime;
            }

            if (IsServer)
            {
                if (test)
                {
                    newGameObjects.Add(new Enemy(new Vector2(500, 0)));
                    test = false;
                }

                AddNewEnemyShips(1);

                string output = "OP";

                //foreach (GameObject gameObject in gameObjects)
                for (int i = 0; i < gameObjects.Count; i++)
                {
                    if (gameObjects[i].GetType().Name == "Player")
                    {
                        continue;
                    }

                    output += ($"{(int)gameObjects[i].Position.X},{(int)gameObjects[i].Position.Y},{gameObjects[i].Id},{gameObjects[i].GetType().Name}");

                    server.Send(output);
                }
                //server.Send(output);
            }

            base.Update(gameTime);
            //}           
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.Draw(spriteBatch);
                DrawCollisionBox(gameObject);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

       

        public  void Instantiate(GameObject gameObject)
        {
            newGameObjects.Add(gameObject);
        }

        public  void Destroy(GameObject gameObject)
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

        private void AddNewEnemyShips(int amount)
        {
            if (timeTillNewInvasionForce <= TimeSpan.Zero)
            {
                for (int i = 0; i < amount; i++)
                {
                    newGameObjects.Add(new Enemy(new Vector2(rnd.Next(0, (int)ScreenSize.X - Asset.enemySprite.Width), 0 - Asset.enemySprite.Height)));
                }

                timeTillNewInvasionForce = new TimeSpan(0, 0, 3);
            }
        }
    }
}