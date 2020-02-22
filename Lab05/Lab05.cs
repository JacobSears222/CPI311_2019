using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CPI311.GameEngine;

namespace Lab05
{
    public class Lab05 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Model model;
        Transform parentTransform, childTransform, cameraTransform;
        Texture2D texture;
        Camera camera;
        Effect effect;

        int index = 0;

        public Lab05()
        {
            graphics = new GraphicsDeviceManager(this);
            //Use HI-DEF
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            Time.Initialize();
            InputManager.Initialize();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            model = Content.Load<Model>("Torus");
            texture = Content.Load<Texture2D>("Square");
            effect = Content.Load<Effect>("SimpleShading");

            parentTransform = new Transform();
            childTransform = new Transform();
            cameraTransform = new Transform();
            cameraTransform.LocalPosition = Vector3.Forward * -1;
            camera = new Camera();
            camera.Transform = cameraTransform;

            foreach (ModelMesh mesh in model.Meshes)
                foreach (BasicEffect effect in mesh.Effects)
                    effect.EnableDefaultLighting();

        }

        protected override void UnloadContent() { }

        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            float rot = 0.5f;
            float speed = 5;

            Time.Update(gameTime);
            InputManager.Update();

            if (InputManager.IsKeyPressed(Keys.Tab)) index = (index + 1) % 4;
            if (InputManager.IsKeyDown(Keys.W)) camera.Transform.LocalPosition += speed * camera.Transform.Forward * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.S)) camera.Transform.LocalPosition += speed * camera.Transform.Backward * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.A)) camera.Transform.Rotate(Vector3.Up, rot * Time.ElapsedGameTime);
            if (InputManager.IsKeyDown(Keys.D)) camera.Transform.Rotate(Vector3.Down, rot * Time.ElapsedGameTime);
            if (InputManager.IsKeyDown(Keys.Up)) parentTransform.LocalPosition += Vector3.Forward *  Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.Down)) parentTransform.LocalPosition += Vector3.Backward * Time.ElapsedGameTime;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Matrix view = camera.View;
            Matrix projection = camera.Projection;

            effect.CurrentTechnique = effect.Techniques[index]; //"0" is the first technique
            effect.Parameters["World"].SetValue(parentTransform.World);
            effect.Parameters["View"].SetValue(view);
            effect.Parameters["Projection"].SetValue(projection);
            effect.Parameters["LightPosition"].SetValue(Vector3.Backward * 10 +
                                                                     Vector3.Right * 5);
            effect.Parameters["CameraPosition"].SetValue(cameraTransform.Position);
            effect.Parameters["Shininess"].SetValue(20f);
            effect.Parameters["AmbientColor"].SetValue(new Vector3(0.2f, 0.2f, 0.2f));
            effect.Parameters["DiffuseColor"].SetValue(new Vector3(0.5f, 0f, 0f));
            effect.Parameters["SpecularColor"].SetValue(new Vector3(0f, 0f, 0.9f));
            effect.Parameters["DiffuseTexture"].SetValue(texture);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                foreach (ModelMesh mesh in model.Meshes)
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                        GraphicsDevice.Indices = part.IndexBuffer;
                        GraphicsDevice.DrawIndexedPrimitives(
                            PrimitiveType.TriangleList, part.VertexOffset, part.StartIndex, part.PrimitiveCount);
                    }
            }
            base.Draw(gameTime);
        }
    }
}
