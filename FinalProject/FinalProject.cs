using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CPI311.GameEngine;

namespace FinalProject
{
    public class FinalProject : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        TerrainRenderer terrain;
        Effect effect;
        Camera camera;
        Light light;

        Texture2D texture;

        SpriteFont font;
        int hits;
        int pacmanHealth = 3;
        Pacman pacman;
        Random random;
        //Make agent a array list
        Ghost Inky, Blinky, Pinky, Clyde, Blue;
        Box powerUp;

        List<Camera> cameras;
        List<Transform> transforms;

        Color background = Color.Black;

        Dictionary<string, Scene> scenes;
        Scene currentScene;
        List<GUIElement> guiElements;

        public class Scene
        {
            public delegate void CallMethod();
            public CallMethod Update;
            public CallMethod Draw;

            public Scene(CallMethod update, CallMethod draw)
            {
                Update = update;
                Draw = draw;
            }
        }

        public FinalProject()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Time.Initialize();
            InputManager.Initialize();
            ScreenManager.Initialize(graphics);

            scenes = new Dictionary<String, Scene>();
            guiElements = new List<GUIElement>();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            transforms = new List<Transform>();
            cameras = new List<Camera>();

            font = Content.Load<SpriteFont>("Font");
            texture = Content.Load<Texture2D>("Square");
            terrain = new TerrainRenderer(Content.Load<Texture2D>("PacManHeightMap"), Vector2.One * 100, Vector2.One * 200);
            terrain.NormalMap = Content.Load<Texture2D>("PacManNormalMap");
            terrain.Transform = new Transform();
            terrain.Transform.LocalScale *= new Vector3(1.7f, 1.5f, 1f);
            effect = Content.Load<Effect>("TerrainShader");
            effect.Parameters["AmbientColor"].SetValue(new Vector3(0.2f, 0.2f, 0.2f));
            effect.Parameters["DiffuseColor"].SetValue(new Vector3(0.1f, 0.1f, 0.1f));
            effect.Parameters["SpecularColor"].SetValue(new Vector3(0.2f, 0.2f, 0.2f));
            effect.Parameters["Shininess"].SetValue(20f);

            camera = new Camera();
            camera.Transform = new Transform();
            camera.Transform.LocalPosition += Vector3.Up * 50;
            camera.Transform.Rotate(Vector3.Right, -MathHelper.PiOver2);
            camera.Position = new Vector2(-0.10f, 0f);
            camera.Size = new Vector2(1f, 1f);
            camera.AspectRatio = camera.Viewport.AspectRatio;

            light = new Light();
            light.Transform = new Transform();
            light.Transform.LocalPosition = Vector3.Backward * 5 + Vector3.Right * 5 + Vector3.Up * 5;
            random = new Random();
            pacman = new Pacman(terrain, Content, camera, GraphicsDevice, light);
            pacman.Transform.LocalScale = new Vector3(2f, 2f, 2f);
            Inky = new Ghost(terrain, Content, camera, GraphicsDevice, light, random);
            Blinky = new Ghost(terrain, Content, camera, GraphicsDevice, light, random);
            Pinky = new Ghost(terrain, Content, camera, GraphicsDevice, light, random);
            Clyde = new Ghost(terrain, Content, camera, GraphicsDevice, light, random);
            Blue = new Ghost(terrain, Content, camera, GraphicsDevice, light, random);
            powerUp = new Box(Content, camera, GraphicsDevice, light);

            cameras.Add(camera);

            GUIGroup group = new GUIGroup();

            Button gameButton = new Button();
            gameButton.Texture = texture;
            gameButton.Text = "Play Game!";
            gameButton.Bounds = new Rectangle(0, 1, 300, 20);
            //exitButton.Action += ExitGame;
            gameButton.Action += playScreen;
            //guiElements.Add(exitButton); Changed to GUIGroup
            group.Children.Add(gameButton);

            CheckBox optionBox = new CheckBox();
            optionBox.Texture = texture;
            optionBox.Box = texture;
            optionBox.Bounds = new Rectangle(0, 25, 300, 20);
            optionBox.Text = "Full Screen";
            optionBox.Action += MakeFullScreen;
            group.Children.Add(optionBox);

            guiElements.Add(group);

            scenes.Add("Menu", new Scene(MainMenuUpdate, MainMenuDraw));
            scenes.Add("Play", new Scene(PlayUpdate, PlayDraw));
            scenes.Add("Credits", new Scene(CreditsUpdate, CreditsDraw));
            currentScene = scenes["Menu"];
        }

        protected override void UnloadContent() { }

        void playScreen(GUIElement element)
        {
            currentScene = scenes["Play"];
        }

        void MakeFullScreen(GUIElement element)
        {
            graphics.ToggleFullScreen();
        }

        void ExitGame(GUIElement element)
        {
            background = (background == Color.White ? Color.Blue : Color.White);
        }

        void MainMenuUpdate()
        {
            foreach (GUIElement element in guiElements) element.Update();

        }

        void MainMenuDraw()
        {
            spriteBatch.Begin();
            foreach (GUIElement element in guiElements) element.Draw(spriteBatch, font);
            spriteBatch.DrawString(font, "Credits: Heightmap and normal map: https://springfiles.com/spring/spring-maps/pacman-map", new Vector2(10, 200), Color.Yellow);

            spriteBatch.End();
        }
        void PlayUpdate()
        {
            if (InputManager.IsKeyReleased(Keys.Escape)) currentScene = scenes["Menu"];
        }
        void PlayDraw()
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "\"Esc\" to return to menu", Vector2.Zero, Color.Yellow);
            spriteBatch.End();
        }

        void CreditsUpdate()
        {
            if (InputManager.IsKeyReleased(Keys.Escape)) currentScene = scenes["Menu"];
        }
        void CreditsDraw()
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "\"Esc\" to return to menu", Vector2.Zero, Color.Yellow);
            spriteBatch.DrawString(font, "Credits", new Vector2(0, 25), Color.Yellow);
            spriteBatch.DrawString(font, "For the heightmap and normal map: https://springfiles.com/spring/spring-maps/pacman-map", new Vector2(0, 50), Color.Yellow);
            spriteBatch.End();
        }

        protected override void Update(GameTime gameTime)
        {
            Time.Update(gameTime);
            InputManager.Update();

            currentScene.Update();
            background = (currentScene == scenes["Play"] ? Color.Black : Color.Black);

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (gameTime.ElapsedGameTime.TotalSeconds == 60)
                Exit();

            //if (InputManager.IsKeyDown(Keys.Up)) camera.Transform.Rotate(Vector3.Right, Time.ElapsedGameTime);
            //if (InputManager.IsKeyDown(Keys.Down)) camera.Transform.Rotate(Vector3.Left, Time.ElapsedGameTime);

            if (Inky.CheckCollision(pacman)) hits++;        
            if (Blinky.CheckCollision(pacman)) hits++;            
            if (Pinky.CheckCollision(pacman)) hits++;
            if (Clyde.CheckCollision(pacman)) hits++;
            if (Blue.CheckCollision(pacman)) hits++;

            pacman.Update();
            Inky.Update();
            Blinky.Update();
            Pinky.Update();
            Clyde.Update();
            Blue.Update();
            powerUp.Update();

            Vector3 normal;
            if (pacman.Get<Collider>().Collides(powerUp.Get<Collider>(), out normal))
            {
                Console.WriteLine("HitBox");
                (powerUp.Get<Renderer>().ObjectModel.Meshes[0].Effects[0] as BasicEffect).DiffuseColor = Color.Red.ToVector3();
                pacmanHealth--;
            }
            else
            {
                Console.WriteLine("NotHitBox");
                (powerUp.Get<Renderer>().ObjectModel.Meshes[0].Effects[0] as BasicEffect).DiffuseColor = Color.Blue.ToVector3();
                pacmanHealth--;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.DepthStencilState = new DepthStencilState();
            GraphicsDevice.Clear(background);

            if (currentScene == scenes["Play"])
            {
                foreach (Camera camera in cameras)
                {
                    effect.Parameters["NormalMap"].SetValue(terrain.NormalMap);
                    effect.Parameters["World"].SetValue(terrain.Transform.World);
                    effect.Parameters["View"].SetValue(camera.View);
                    effect.Parameters["Projection"].SetValue(camera.Projection);
                    effect.Parameters["CameraPosition"].SetValue(camera.Transform.Position);
                    effect.Parameters["LightPosition"].SetValue(light.Transform.Position);
                    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        terrain.Draw();
                    }

                    pacman.Draw();
                    Inky.Draw();
                    Blinky.Draw();
                    Pinky.Draw();
                    Clyde.Draw();
                    powerUp.Draw();
                }
                spriteBatch.Begin();
                spriteBatch.DrawString(font, "# of Hits: " + hits, new Vector2(100, 50), Color.White);
                spriteBatch.DrawString(font, "Time left: " + Time.TotalGameTime, new Vector2(100, 70), Color.White);
                spriteBatch.DrawString(font, "WASD to move", new Vector2(100, 90), Color.White);
                spriteBatch.End();
            }
            currentScene.Draw();

            base.Draw(gameTime);
        }
    }
}
