using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using CPI311.GameEngine;

namespace Lab11
{
    public class Lab11 : Game
    {
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

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D texture;
        SpriteFont font;
        Color background = Color.Blue;

        Dictionary<string, Scene> scenes;
        Scene currentScene;
        List<GUIElement> guiElements;


        public Lab11()
        {
            IsMouseVisible = true;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
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
            texture = Content.Load<Texture2D>("Square");
            font = Content.Load<SpriteFont>("Font");

            GUIGroup group = new GUIGroup();

            Button exitButton = new Button();
            exitButton.Texture = texture;
            exitButton.Text = "Exit";
            exitButton.Bounds = new Rectangle(50, 50, 300, 20);
            //exitButton.Action += ExitGame;
            exitButton.Action += playScreen;
            //guiElements.Add(exitButton); Changed to GUIGroup
            group.Children.Add(exitButton);

            CheckBox optionBox = new CheckBox();
            optionBox.Texture = texture;
            optionBox.Box = texture;
            optionBox.Bounds = new Rectangle(50, 75, 300, 20);
            optionBox.Text = "Full Screen";
            optionBox.Action += MakeFullScreen;
            group.Children.Add(optionBox);

            guiElements.Add(group);

            scenes.Add("Menu", new Scene(MainMenuUpdate, MainMenuDraw));
            scenes.Add("Play", new Scene(PlayUpdate, PlayDraw));
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
            spriteBatch.End();
        }
        void PlayUpdate()
        {
            if (InputManager.IsKeyReleased(Keys.Escape)) currentScene = scenes["Menu"];
        }
        void PlayDraw()
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "Play Mode! Press \"Esc\" to go back", Vector2.Zero, Color.Yellow);
            spriteBatch.End();
        }

        protected override void Update(GameTime gameTime)
        {
            Time.Update(gameTime);
            InputManager.Update();

            currentScene.Update();
            background = (currentScene == scenes["Play"] ? Color.Blue : Color.White);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(background);

            currentScene.Draw();

            base.Draw(gameTime);
        }
    }
}