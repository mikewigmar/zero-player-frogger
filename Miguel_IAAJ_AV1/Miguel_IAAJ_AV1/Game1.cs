using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Miguel_IAAJ_AV1
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Random rnd = new Random();
        float difficultyLevel = 1f;
        Texture2D frog, bg, car, ball, log, truck, flower, flowerFrog;
        Player player;
        List<Obstacle> listObstacle = new List<Obstacle>();
        List<Platform> listPlatform = new List<Platform>();
        List<Goal> listGoal = new List<Goal>();
        
        List<GameObject> objectList = new List<GameObject>();
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = Refs.WindowHeight;
            graphics.PreferredBackBufferWidth = Refs.WindowWidth;
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
            // TODO: Add your initialization logic here

            base.Initialize();
            
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            bg = Content.Load<Texture2D>("Sprites/background");
            frog = Content.Load<Texture2D>("Sprites/frog");
            car = Content.Load<Texture2D>("Sprites/car");
            ball = Content.Load<Texture2D>("Sprites/ball");
            log = Content.Load<Texture2D>("Sprites/log");
            truck = Content.Load<Texture2D>("Sprites/truck");
            flower = Content.Load<Texture2D>("Sprites/flower");
            flowerFrog = Content.Load<Texture2D>("Sprites/flowerfrog");

            CreateNewLevel();
            //player.Parent = platform;
            
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.E))
                CreateNewLevel();

            float deltaTime = gameTime.ElapsedGameTime.Milliseconds * 0.001f;

            player.Update(gameTime, deltaTime);

            foreach (GameObject go in objectList)
            {
                go.Update(gameTime, deltaTime);
            }

            foreach (Goal go in listGoal) 
            {
                if (!go.IsReached) break;
                CreateNewLevel();
            }
            Window.Title = "Frogger Lua IA, current difficulty: " + difficultyLevel;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            spriteBatch.Draw(bg, Vector2.Zero, Color.White);

            foreach (GameObject go in objectList)
            {
                spriteBatch.Draw(go.Texture, go.position, Color.White);
            }

            foreach (Goal go in listGoal) spriteBatch.Draw(go.Text, go.position, Color.White);
            spriteBatch.Draw(player.Texture, player.position, Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        void CreateNewLevel() 
        {
            float speedMod = 0.75f * difficultyLevel ;
            float speed = 0f;
            int r = 0;
            if (objectList.Count != 0) 
            {
                player = null;
                for (int i = 0; i < objectList.Count; i++)
                {
                    objectList[0] = null;
                }
                difficultyLevel += 0.5f;
            }
            listPlatform = new List<Platform>();
            listGoal = new List<Goal>();
            listObstacle = new List<Obstacle>();
            objectList = new List<GameObject>();
            //goals
            listGoal.Add(new Goal(new Vector2(25, 0), 100, 50, flower, flowerFrog));
            listGoal.Add(new Goal(new Vector2(225, 0), 100, 50, flower, flowerFrog));
            listGoal.Add(new Goal(new Vector2(425, 0), 100, 50, flower, flowerFrog));
            listGoal.Add(new Goal(new Vector2(625, 0), 100, 50, flower, flowerFrog));
            //platforms
            speed = (float)rnd.NextDouble() * 300f + 50f;
            r = rnd.Next(3) + 1; for (int i = 0; i < r; i++)
            {
                Vector2 v2 = new Vector2(0 + i*50*rnd.Next(0,2),50);
                listPlatform.Add(new Platform(v2, 50, 50, ball, speed * speedMod ));
            }
            speed = (float)rnd.NextDouble() * 300f + 50f;
            r = rnd.Next(2) + 1; for (int i = 0; i < r; i++)
            {
                Vector2 v2 = new Vector2(0 + i * 100 * rnd.Next(0, 2), 100);
                listPlatform.Add(new Platform(v2, 100, 50, log, speed * speedMod));
            }
            speed = (float)rnd.NextDouble() * 300f + 50f;
            r = rnd.Next(3) + 1; for (int i = 0; i < r; i++)
            {
                Vector2 v2 = new Vector2(0 + i * 100 * rnd.Next(0, 2), 150);
                listPlatform.Add(new Platform(v2, 100, 50, log, speed * speedMod));
            }
            speed = (float)rnd.NextDouble() * 300f + 50f;
            r = rnd.Next(7) + 1; for (int i = 0; i < r; i++)
            {
                Vector2 v2 = new Vector2(0 + i * 50 * rnd.Next(0, 5), 200);
                listPlatform.Add(new Platform(v2, 50, 50, ball, speed * speedMod));
            }

            //obstacles

            speed = (float)rnd.NextDouble() * 300f + 50f;
            r = rnd.Next(3) + 1; for (int i = 0; i < r; i++)
            {
                Vector2 v2 = new Vector2(0 + i * 50 * rnd.Next(0, 2), 300);
                listObstacle.Add(new Obstacle(v2, 50, 50, car, speed * speedMod));
            }
            speed = (float)rnd.NextDouble() * 300f + 50f;
            r = rnd.Next(4) + 1; for (int i = 0; i < r; i++)
            {
                Vector2 v2 = new Vector2(0 + i * 100 * rnd.Next(0, 5), 350);
                listObstacle.Add(new Obstacle(v2, 100, 50, car, speed * speedMod));
            }
            speed = (float)rnd.NextDouble() * 300f + 50f;
            r = rnd.Next(2) + 1; for (int i = 0; i < r; i++)
            {
                Vector2 v2 = new Vector2(0 + i * 100 * rnd.Next(0, 4), 400);
                listObstacle.Add(new Obstacle(v2, 100, 50, car, speed * speedMod));
            }
            speed = (float)rnd.NextDouble() * 300f + 50f;
            r = rnd.Next(1) + 1; for (int i = 0; i < r; i++)
            {
                Vector2 v2 = new Vector2(0 + i * 50 * rnd.Next(0, 5), 450);
                listObstacle.Add(new Obstacle(v2, 100, 50, truck, speed * speedMod));
            }
            speed = (float)rnd.NextDouble() * 150f + 50f;
            r = rnd.Next(4) + 1; for (int i = 0; i < r; i++)
            {
                Vector2 v2 = new Vector2(0 + i * 50 * rnd.Next(0, 10), 500);
                listObstacle.Add(new Obstacle(v2, 50, 50, car, speed * speedMod));
            }

            objectList.AddRange(listObstacle);
            objectList.AddRange(listPlatform);
            objectList.AddRange(listGoal);

            
            player = new Player(new Vector2(300, 600), 50, 50, frog, objectList);
        }
    }

    public static class Refs 
    {
        public static int WindowHeight = 600, WindowWidth = 700;
    }
}
