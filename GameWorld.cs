using GruppeHessNetworkAssignment.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Data.SQLite;
using System.Reflection.Emit;

namespace GruppeHessNetworkAssignment
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class GameWorld : Game
    {
        #region Fields

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private static GameWorld instance;
        private Highscore highscore;
        private bool isStartScreen = true;
        private int objectID = 0;

        #endregion

        #region Properties
        public DBHandler DBHandlerInstance { get; private set; }
        public Player PlayerServer { get; private set; }
        public Player PlayerClient { get; private set; }
        public UdpClientManager ClientInstance { get; set; }
        public UdpServerManager ServerInstance { get; set; }

        public bool Instantiated { get; set; } = false;
        public bool ProgramRunning { get; set; } = true;
        public bool IsServer { get; private set; } = false;
        public bool SetUpServerPlayer { get; set; }
        public bool ShowHighscore { get; set; } = false;
        public int ObjectID { get => objectID++; set => objectID = value; }
        public int ScreenHeight { get; } = 1000;
        public Vector2 ScreenSize { get; private set; }
        public string TeamName { get; set; }

        //Lists to handle gameobjects
        public List<GameObject> NewGameObjects { get; private set; } = new List<GameObject>();
        public List<GameObject> DeletedGameObjects { get; private set; } = new List<GameObject>();
        public List<GameObject> GameObjects { get; private set; } = new List<GameObject>();

        #endregion

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

        public GameWorld()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }


        #region Methods

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
            graphics.PreferredBackBufferHeight = ScreenHeight;
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
            while (isStartScreen == true)
            {
                Console.WriteLine("Server (S) or Client (C)?");
                string input = Console.ReadLine().ToUpper();

                // Instantiates the server, if the game starts in server mode.
                if (input == "S")
                {
                    Console.WriteLine("Write the team name: ");
                    TeamName = Console.ReadLine();

                    new TcpServerManager();
                    //udpServer = new UdpServerManager();
                    highscore = new Highscore();
                    IsServer = true;
                    isStartScreen = false;
                    Window.Title = "Server";
                    Console.Title = "Server";


                    //Create a database on the servers side.
                    DBHandlerInstance = new DBHandler();
                    DBHandlerInstance.BuildDatabase();
                }

                // Instantiates a client, if the game starts in player mode.
                else if (input == "C")
                {
                    //Instantiates a client, if the game starts in client / player mode.

                    Console.WriteLine("What IP would you like to connect to?");
                    string ip = Console.ReadLine();

                    Console.WriteLine("Write the server password: ");
                    string password = Console.ReadLine();

                    new TcpClientManager(ip, password);

                    IsServer = false;
                    isStartScreen = false;
                }

                else
                {
                    Console.WriteLine("Invalid input.");
                    isStartScreen = true;
                }
            }

            if (IsServer)
            {
                ServerInstance = new UdpServerManager();
            }

            else
            {
                ClientInstance = new UdpClientManager();
            }
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Asset.LoadContent(Content);
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
            if (!Instantiated)
            {
                // Makes sure a server is set up first.
                SetUpServerPlayer = true;
                GameObjects.Add(PlayerServer = new Player(new Vector2(0, ScreenSize.Y - Asset.PlayerSprite.Height)));
                GameObjects.Add(PlayerClient = new Player(new Vector2(ScreenSize.X - Asset.ClientPlayerSprite.Width, ScreenSize.Y - Asset.ClientPlayerSprite.Height)));

                Instantiated = true;
            }

            // Makes sure update only runs while the players are still alive.
            // So the game "stops" once the players die.
            if (!PlayerServer.IsDead)
            {
                if (IsServer)
                {
                    ServerInstance.UpdateServer(gameTime);
                }
                else
                {
                    ClientInstance.UpdateClient();
                }
               
                // To exit the game.
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                {
                    // Makes sure all the threads stop running once the game shuts down.
                    ProgramRunning = false;
                    Exit();
                }

                //adds all bjects in list-newobjects to list-gameobjects.
                GameObjects.AddRange(NewGameObjects);
                //deletes objects in list-newobjects.
                NewGameObjects.Clear();


                // Runs the update method and checkcollision method for all gameobjets on the GameObjects list.
                for (int i = 0; i < GameObjects.Count; i++)
                {
                    GameObjects[i].Update(gameTime);

                    foreach (GameObject other in GameObjects)
                    {
                        GameObjects[i].CheckCollision(other);
                    }

                }

                // Makes sure to remove all deleted objects from the GameObjects list.
                for (int i = 0; i < DeletedGameObjects.Count; i++)
                {
                    GameObjects.Remove(DeletedGameObjects[i]);
                }

                // Deletes objects in list-deleteobjects.
                DeletedGameObjects.Clear();
            }
            else
            {
                ServerInstance.PrintHighscores();
            }

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

            // Draws the point and health strings.
            if (Instantiated)
            {
                spriteBatch.DrawString(Asset.ScoreFont, $"Points: {Highscore.Instance.Points}", new Vector2(0, ScreenHeight / 25), Color.Orange, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
                spriteBatch.DrawString(Asset.ScoreFont, $"Health: {PlayerServer.PlayerHealth}", new Vector2(0, 0), Color.Yellow, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            }

            // Makes sure to draw all gameobjects and collisionboxes.
            foreach (GameObject gameObject in GameObjects)
            {
                gameObject.Draw(spriteBatch);
                //DrawCollisionBox(gameObject);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Adds a new objects to the NewGameObjects list.
        /// </summary>
        /// <param name="gameObject"></param>
        public void Instantiate(GameObject gameObject)
        {
            NewGameObjects.Add(gameObject);
        }

        /// <summary>
        /// Adds an object to the DeletedGameObjects list when objects need to be deleted from the game.
        /// </summary>
        /// <param name="gameObject"></param>
        public void Destroy(GameObject gameObject)
        {
            DeletedGameObjects.Add(gameObject);
        }

        /// <summary>
        /// Draws all collision boxes in the game.
        /// </summary>
        /// <param name="gameObject"></param>
        private void DrawCollisionBox(GameObject gameObject)
        {
            /// Draws the collisionboxes.
            Rectangle collisionBox = gameObject.CollisionBox;
            Rectangle topLine = new Rectangle(collisionBox.X, collisionBox.Y, collisionBox.Width, 1);
            Rectangle bottomLine = new Rectangle(collisionBox.X, collisionBox.Y + collisionBox.Height, collisionBox.Width, 1);
            Rectangle rightLine = new Rectangle(collisionBox.X + collisionBox.Width, collisionBox.Y, 1, collisionBox.Height);
            Rectangle leftLine = new Rectangle(collisionBox.X, collisionBox.Y, 1, collisionBox.Height);

            /// Makes sure the collisionbox adjusts to each sprite.
            spriteBatch.Draw(Asset.CollisionBox, topLine, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 1);
            spriteBatch.Draw(Asset.CollisionBox, bottomLine, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 1);
            spriteBatch.Draw(Asset.CollisionBox, rightLine, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 1);
            spriteBatch.Draw(Asset.CollisionBox, leftLine, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 1);
        }

        #endregion
    }
}
