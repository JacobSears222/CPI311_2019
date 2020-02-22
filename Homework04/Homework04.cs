using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

using CPI311.GameEngine;

namespace Homework04
{    
    public class Homework04 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Camera camera;
        Light light;

        Vector3 cameraPosition = new Vector3(0.0f, 0.0f, GameConstants.CameraHeight);
        Matrix projectionMatrix;
        Matrix viewMatrix;

        SoundEffect gunSound;
        SoundEffect playerDiedSound;
        SoundEffect explosionSound;
        SoundEffectInstance soundInstance;

        Ship ship;

        Asteroid[] asteroidList = new Asteroid[GameConstants.NumAsteroids];
        Bullet[] bulletList = new Bullet[GameConstants.NumBullets];
        Asteroid asteroid;
        Bullet bullet;

        int score, playerHealth;
        Texture2D stars;
        SpriteFont font;
        SpriteFont lucidaConsole;
        Vector2 scorePosition = new Vector2(100, 50);
        Transform cameraTransform, lightTransform;

        ParticleManager particleManager;
        Texture2D particleTex;
        Effect particleEffect;

        Random random;

        public Homework04()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            Time.Initialize();
            InputManager.Initialize();

            camera = new Camera();
            cameraTransform = new Transform();
            camera.Transform = cameraTransform;
            camera.Transform.Position = cameraPosition;
            cameraPosition = new Vector3(0.0f, 0.0f, GameConstants.CameraHeight);
            camera.FieldOfView = MathHelper.ToRadians(45.0f);
            camera.AspectRatio = GraphicsDevice.DisplayMode.AspectRatio;
            camera.NearPlane = 10000.0f;
            camera.FarPlane = 30000.0f;
            playerHealth = GameConstants.PlayerHealth;

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), 
                GraphicsDevice.DisplayMode.AspectRatio, GameConstants.CameraHeight - 1000.0f, GameConstants.CameraHeight + 1000.0f);

            ship = new Ship(Content, camera, GraphicsDevice, light);
            ship.Transform.LocalScale = new Vector3(0.2f, 0.2f, 0.2f);
            asteroid = new Asteroid(Content, camera, GraphicsDevice, light);
            //asteroid.Transform.LocalPosition = new Vector3(500, 0, 500);
            asteroid.Transform.LocalScale = new Vector3(3.0f, 3.0f, 3.0f);

            viewMatrix = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);

            light = new Light();
            lightTransform = new Transform();
            light.Transform = lightTransform;

            bullet = new Bullet(Content, camera, GraphicsDevice, light, ship);

            random = new Random();

            ResetAsteroids();
            base.Initialize();
        }

        private void ResetAsteroids()
        {
            float xStart;
            float yStart;
            for (int i = 0; i < GameConstants.NumAsteroids; i++)
            {
                if (random.Next(2) == 0)
                    xStart = (float)-GameConstants.PlayfieldSizeX;
                else
                    xStart = (float)GameConstants.PlayfieldSizeX;
                yStart = (float)random.NextDouble() * GameConstants.PlayfieldSizeY;
                asteroidList[i] = new Asteroid(Content, camera, GraphicsDevice, light);
                asteroidList[i].Transform.Position = new Vector3(xStart, yStart, ship.Transform.Position.Z);
                double angle = random.NextDouble() * 2 * Math.PI;
                asteroidList[i].Rigidbody.Velocity = new Vector3((-(float)Math.Cos(angle)) * (GameConstants.AsteroidMinSpeed + (float)random.NextDouble() * GameConstants.AsteroidMaxSpeed), ((float)Math.Cos(angle)) * (GameConstants.AsteroidMinSpeed + (float)random.NextDouble() * GameConstants.AsteroidMaxSpeed), 0);
                asteroidList[i].isActive = true;
            }
        }

        private Matrix[] SetupEffectDefaults(Model myModel)
        {
            Matrix[] absoluteTransforms = new Matrix[myModel.Bones.Count];
            myModel.CopyAbsoluteBoneTransformsTo(absoluteTransforms);
            foreach (ModelMesh mesh in myModel.Meshes) { foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.Projection = projectionMatrix;
                    effect.View = viewMatrix;
                }
            }
            return absoluteTransforms;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            ship = new Ship(Content, camera, GraphicsDevice, light);

            for (int i = 0; i < GameConstants.NumBullets; i++)
            {
                bulletList[i] = new Bullet(Content, camera, GraphicsDevice, light, ship);
            }

            ResetAsteroids();

            stars = Content.Load<Texture2D>("B1_stars");
            font = Content.Load<SpriteFont>("font");
            lucidaConsole = Content.Load<SpriteFont>("font");

            gunSound = Content.Load<SoundEffect>("tx0_fire1");
            playerDiedSound = Content.Load<SoundEffect>("explosion2");
            explosionSound = Content.Load<SoundEffect>("explosion3");
            
            particleManager = new ParticleManager(GraphicsDevice, 100);
            particleEffect = Content.Load<Effect>("ParticleShader-complete");
            particleTex = Content.Load<Texture2D>("fire");
        }

        protected override void UnloadContent() { }

        protected override void Update(GameTime gameTime)
        {
            float timeDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Time.Update(gameTime);
            InputManager.Update();
            ship.Update();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            for(int i = 0; i < GameConstants.NumBullets; i++) bulletList[i].Update();        
            for(int i = 0; i < GameConstants.NumAsteroids; i++) asteroidList[i].Update();

            if (InputManager.IsMousePressed(0))
            {
                for (int i = 0; i < GameConstants.NumBullets; i++)
                {
                    if (!bulletList[i].isActive)
                    {
                        bulletList[i].Rigidbody.Velocity = (ship.Transform.Forward) * GameConstants.BulletSpeedAdjustment;
                        bulletList[i].Transform.LocalPosition = ship.Transform.Position + (200 * bulletList[i].Transform.Forward);
                        bulletList[i].isActive = true;
                        score -= GameConstants.ShotPenalty;
                        soundInstance = gunSound.CreateInstance();
                        soundInstance.Play();
                        break;     
                    }
                }
            }

            Vector3 normal;
            for (int i = 0; i < asteroidList.Length; i++)
                if (asteroidList[i].isActive)
                    for (int j = 0; j < bulletList.Length; j++)
                        if (bulletList[j].isActive)
                            if (asteroidList[i].Collider.Collides(bulletList[j].Collider, out normal))
                            {
                                Particle particle = particleManager.getNext();
                                particle.Position = asteroidList[i].Transform.Position;
                                particle.Velocity = new Vector3(random.Next(-5, 5), 2, 0);
                                particle.Acceleration = new Vector3(0, 3, 0);
                                particle.MaxAge = random.Next(5, 10);
                                particle.Init();
                                asteroidList[i].isActive = false;
                                bulletList[j].isActive = false;
                                score += GameConstants.KillBonus;
                                soundInstance = explosionSound.CreateInstance();
                                soundInstance.Play();
                                break;
                            }

            for (int i = 0; i < asteroidList.Length; i++)
                if (asteroidList[i].isActive)
                    if (asteroidList[i].Collider.Collides(ship.Collider, out normal))
                    {
                        Particle particle = particleManager.getNext();
                        particle.Position = asteroidList[i].Transform.Position;
                        particle.Velocity = new Vector3(random.Next(-5, 5), 2, 0);
                        particle.Acceleration = new Vector3(0, 3, 0);
                        particle.MaxAge = random.Next(5, 10);
                        particle.Init();
                        asteroidList[i].isActive = false;
                        soundInstance = explosionSound.CreateInstance();
                        soundInstance.Play();
                        playerHealth -= 1;
                        break;
                    }

            if(playerHealth <= 0)
            {
                Exit();
            }

            particleManager.Update();
            base.Update(gameTime);
        }      
       
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            spriteBatch.Draw(stars, new Rectangle(0, 0, 800, 600), Color.White);
            spriteBatch.DrawString(font, "Ships Pos:" + ship.Transform.LocalPosition, new Vector2(10, 20), Color.Blue);
            spriteBatch.DrawString(font, "Camera Pos:" + cameraPosition, new Vector2(10, 40), Color.Blue);
            spriteBatch.DrawString(font, "Player Health: " + playerHealth, new Vector2(10, 60), Color.Blue);
            spriteBatch.DrawString(font, "Score: " + score, new Vector2(10, 80), Color.Blue);
            spriteBatch.DrawString(font, "Remaining Asteroids: " + (asteroidList.Length), new Vector2(10, 100), Color.Blue);
            spriteBatch.DrawString(font, "Remaining Bullets: " + (bulletList.Length), new Vector2(10, 120), Color.Blue);

            spriteBatch.End();
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            particleEffect.CurrentTechnique = particleEffect.Techniques["particle"];
            particleEffect.CurrentTechnique.Passes[0].Apply();
            particleEffect.Parameters["ViewProj"].SetValue(camera.View * camera.Projection);
            particleEffect.Parameters["World"].SetValue(Matrix.Identity);
            particleEffect.Parameters["CamIRot"].SetValue(Matrix.Invert(Matrix.CreateFromQuaternion(camera.Transform.Rotation)));
            particleEffect.Parameters["Texture"].SetValue(particleTex);

            particleManager.Draw(GraphicsDevice);

            Matrix shipTransformMatrix = ship.RotationMatrix * Matrix.CreateTranslation(ship.Position);
            
            for (int i = 0; i < GameConstants.NumBullets; i++) bulletList[i].Draw();
            for (int i = 0; i < GameConstants.NumAsteroids; i++) asteroidList[i].Draw();            

            ship.Draw();
            base.Draw(gameTime);
        }
    }
}
