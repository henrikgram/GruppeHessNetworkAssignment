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
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Lists of gameobjects.
        private List<GameObject> gameObjects = new List<GameObject>();
        private List<GameObject> newGameObjects = new List<GameObject>();
        private List<GameObject> deletedGameObjects = new List<GameObject>();

        private TimeSpan timeTillNewInvasionForce = new TimeSpan(0, 0, 2);
        private Random rnd = new Random(500);
        private int screenHeight = 1000;

        private int objectID = 0;
        private UdpServerManager udpServer;
        private UdpClientManager udpClient;
        private static GameWorld instance;
        private Highscore highscore;

        private int tmpScore = 5;
        private string tmpName = "Nej";

        private bool isStartScreen = true;
        private bool isServer = false;

        private byte maxPlayers = 1;

        public DBHandler DbHandler { get; private set; }
        public Player PlayerServer { get; private set; }
        public Player PlayerClient { get; private set; }
        public UdpClientManager ClientInstance { get => udpClient; set => udpClient = value; }
        public UdpServerManager ServerInstance { get => udpServer; set => udpServer = value; }
        public bool Instantiated { get; set; } = false;
        public int ScreenHeight { get; } = 1000;
        public bool ProgramRunning { get; set; } = true;
        public bool IsServer { get => isServer; }
        public bool SetUpServerPlayer { get; set; }
        public Vector2 ScreenSize { get; private set; }

        public int ObjectID { get => objectID++; set => objectID = value; }


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

        public List<GameObject> NewGameObjects { get => newGameObjects; set => newGameObjects = value; }
        public List<GameObject> GameObjects { get => gameObjects; }

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
            DbHandler = new DBHandler();

            DbHandler.BuildDatabase();
            //DbHandler.InsertIntoTable("NameTable", $"NULL, '{tmpName}'", new SQLiteConnection(DbHandler.LoadSQLiteConnectionString()));
            //DbHandler.InsertIntoTable("ScoreTable", $"NULL, 2, 500", new SQLiteConnection(DbHandler.LoadSQLiteConnectionString()));
            DbHandler.InsertIntoTable("Highscore", "NULL, 'Stinna', 500", new SQLiteConnection(DbHandler.LoadSQLiteConnectionString()));
            DbHandler.InsertIntoTable("Highscore", "NULL, 'Henrik', 1000", new SQLiteConnection(DbHandler.LoadSQLiteConnectionString()));
            DbHandler.InsertIntoTable("Highscore", "NULL, 'Signe', 800", new SQLiteConnection(DbHandler.LoadSQLiteConnectionString()));
            DbHandler.InsertIntoTable("Highscore", "NULL, 'Emma', 1400", new SQLiteConnection(DbHandler.LoadSQLiteConnectionString()));

            List<string> highscores = new List<string>();
            highscores = DbHandler.CreateHighscoreList();

            foreach (string value in highscores)
            {
                Console.WriteLine(value);
            }

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
                    new TcpServerManager();
                    //udpServer = new UdpServerManager();
                    highscore = new Highscore();
                    isServer = true;
                    isStartScreen = false;
                    Window.Title = "Server";
                    Console.Title = "Server";
                }

                // Instantiates a client, if the game starts in player mode.
                else if (input == "C")
                {
                    //Console.WriteLine("What port would you like to connect to?");
                    //udpClient = new UdpClientManager();

                    //Instantiates a client, if the game starts in client/player mode.

                    //Console.WriteLine("What IP would you like to connect to?");
                    //string ip = Console.ReadLine();

                    //Console.WriteLine("What port would you like to connect to?");
                    //string port = Console.ReadLine();

                    //Console.WriteLine("Write the server password: ");
                    //string password = Console.ReadLine();

                    //new TcpClientManager(ip, int.Parse(port), password);

                    new TcpClientManager("192.168.0.100", /*11000,*/ "12345678");

                    isServer = false;
                    isStartScreen = false;
                }

                else
                {
                    Console.WriteLine("Invalid input.");
                    isStartScreen = true;
                }
            }

            if (isServer)
            {
                udpServer = new UdpServerManager();
            }

            else
            {
                udpClient = new UdpClientManager();
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
            // TODO: Unload any non ContentManager content here
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
                gameObjects.Add(PlayerServer = new Player(new Vector2(0, ScreenSize.Y - Asset.playerSprite.Height)));
                gameObjects.Add(PlayerClient = new Player(new Vector2(ScreenSize.X - Asset.clientPlayerSprite.Width, ScreenSize.Y - Asset.clientPlayerSprite.Height)));

                Instantiated = true;
            }

            //For two player, so the server can send a pos back to the client.
            if (isServer)
            {
                AddNewEnemyShipsServer();
                SendEnemyShipInfoToClient();

                if (timeTillNewInvasionForce > TimeSpan.Zero)
                {
                    timeTillNewInvasionForce -= gameTime.ElapsedGameTime;
                }

                udpServer.Send("Update|Player|" + PlayerServer.Position.X + "|" + PlayerServer.Position.Y);
            }

            // Makes sure the client sends the player's updated position to the server.
            if (!isServer)
            {
                udpClient.Send("Update|Player|" + PlayerClient.Position.X + "|" + PlayerClient.Position.Y);
            }

            // To exit the game.
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                // Makes sure all the threads stop running.
                ProgramRunning = false;
                Exit();
            }

            //ads all objects in list-newobjects to list-gameobjects.
            gameObjects.AddRange(NewGameObjects);
            //deletes objects in list-newobjects.
            NewGameObjects.Clear();

            //foreach (GameObject gameObject in gameObjects)
            for (int i = 0; i < gameObjects.Count; i++)
            {
                gameObjects[i].Update(gameTime);

                foreach (GameObject other in gameObjects)
                {
                    gameObjects[i].CheckCollision(other);
                }
            }

            for (int i = 0; i < deletedGameObjects.Count; i++)
            {
                gameObjects.Remove(deletedGameObjects[i]);
            }

            //deletes objects in list-deleteobjects.
            deletedGameObjects.Clear();

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

            if (Instantiated)
            {
                spriteBatch.DrawString(Asset.scoreFont, $"Points: {Highscore.Instance.Points}", new Vector2(0, ScreenHeight / 25), Color.DarkRed, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
                spriteBatch.DrawString(Asset.scoreFont, $"Health: {PlayerServer.PlayerHealth}", new Vector2(0, 0), Color.DarkBlue, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            }

            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.Draw(spriteBatch);
                DrawCollisionBox(gameObject);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void Instantiate(GameObject gameObject)
        {
            NewGameObjects.Add(gameObject);
        }

        public void Destroy(GameObject gameObject)
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

        /// <summary>
        /// Sends a wave of enemyships at set intervals of time. Also sends a message to the client to create ships at the specified
        /// positions
        /// </summary>
        private void AddNewEnemyShipsServer()
        {
            if (timeTillNewInvasionForce <= TimeSpan.Zero)
            {
                for (int i = 0; i < 5; i++)
                {
                    Enemy tmpEnemy = new Enemy(new Vector2(rnd.Next(0, (int)ScreenSize.X - Asset.enemySprite.Width), 0 - Asset.enemySprite.Height)/*, enemyID*/);
                    NewGameObjects.Add(tmpEnemy);

                    udpServer.Send("New|Enemy|" + tmpEnemy.ID + "|" + tmpEnemy.Position.X + "|" + tmpEnemy.Position.Y);
                }
                timeTillNewInvasionForce = new TimeSpan(0, 0, 5);
            }
        }

        /// <summary>
        /// Sends information about the enemmy ships location to the client so their positions can be updated.
        /// </summary>
        private void SendEnemyShipInfoToClient()
        {
            List<GameObject> enemies = (gameObjects.FindAll(e => e is Enemy));
            for (int i = 0; i < enemies.Count; i++)
            {
                Enemy currentEnemy = (Enemy)enemies[i];
                udpServer.Send("Update|Enemy|" + currentEnemy.ID + "|" + currentEnemy.Position.X + "|" + currentEnemy.Position.Y);
            }
        }
    }
}
