using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace CPI311.GameEngine
{
    public class Ship : GameObject
    {
        public Model model;
        public Matrix[] Transforms;
        public Vector3 Position = Vector3.Zero;

        public Vector3 Velocity = Vector3.Zero;
        public Matrix RotationMatrix = Matrix.Identity;
        private float rotation;

        SoundEffect soundEngine;
        SoundEffectInstance soundEngineInstance;
        SoundEffect soundHyperspaceActivation;

        public bool isDead;
        public bool canShoot;
        private int reloadTimer = 0;

        public Ship(ContentManager Content, Camera camera, GraphicsDevice graphicsDevice, Light light) : base()
        {
            model = Content.Load<Model>("p1_wedge");
            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            Add<Rigidbody>(rigidbody);


            Texture2D texture = Content.Load<Texture2D>("wedge_p1_diff_v1");
            Renderer renderer = new Renderer(model, Transform, camera, Content, graphicsDevice, light, 1, "SimpleShading", 20f, texture);
            Add<Renderer>(renderer);

            soundEngine = Content.Load<SoundEffect>("engine_2");
            soundEngineInstance = soundEngine.CreateInstance();
            soundHyperspaceActivation = Content.Load<SoundEffect>("hyperspace_activate");

            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = renderer.ObjectModel.Meshes[0].BoundingSphere.Radius;
            sphereCollider.Transform = Transform;
            Add<Collider>(sphereCollider);

            this.Transform.Rotate(Vector3.Right, MathHelper.PiOver2);
        }


        public float Rotation
        {
            get { return rotation; }
            set
            {
                float newVal = value; while (newVal >= MathHelper.TwoPi)
                {
                    newVal -= MathHelper.TwoPi;
                }
                while (newVal < 0)
                {
                    newVal += MathHelper.TwoPi;
                }
                if (rotation != value)
                {
                    rotation = value; RotationMatrix = Matrix.CreateRotationX(MathHelper.PiOver2) * Matrix.CreateRotationZ(rotation);
                }

                }
            }
        public override void Update()
        {
            if (InputManager.IsKeyDown(Keys.W))
            {
                this.Transform.LocalPosition += this.Transform.Forward * Time.ElapsedGameTime * GameConstants.PlayerSpeed;

            }            
            if (InputManager.IsKeyDown(Keys.S))
            {
                this.Transform.LocalPosition += this.Transform.Backward * Time.ElapsedGameTime * GameConstants.PlayerSpeed;
            }
            if (InputManager.IsKeyDown(Keys.A))
            {
                this.Transform.Rotate(Vector3.Up, Time.ElapsedGameTime * GameConstants.PlayerRotationSpeed);
            }
            if (InputManager.IsKeyDown(Keys.D))
            {
                this.Transform.Rotate(Vector3.Down, Time.ElapsedGameTime * GameConstants.PlayerRotationSpeed);
            }
            
            if (this.Transform.Position.X > GameConstants.PlayfieldSizeX)
            {
                this.Transform.LocalPosition -= Vector3.UnitX * 2 * GameConstants.PlayfieldSizeX;
            }
            if (this.Transform.Position.X < -GameConstants.PlayfieldSizeX)
            {
                this.Transform.LocalPosition += Vector3.UnitX * 2 * GameConstants.PlayfieldSizeX;
            }
            if (this.Transform.Position.Y > GameConstants.PlayfieldSizeY)
            {
                this.Transform.LocalPosition -= Vector3.UnitY * 2 * GameConstants.PlayfieldSizeY;
            }
            if (this.Transform.Position.Y < -GameConstants.PlayfieldSizeY)
            {
                this.Transform.LocalPosition += Vector3.UnitY * 2 * GameConstants.PlayfieldSizeY;
            }

            base.Update();
        }
        /*
        protected void UpdateInput()
        {
            GamePadState currentState = GamePad.GetState(PlayerIndex.One);
            if (currentState.IsConnected)
            {
                if (currentState.Triggers.Right > 0)
                {
                    if (soundEngine.State == SoundState.Stopped)
                    {
                        soundEngine.Volume = 0.75f;
                        soundEngine.IsLooped = true;
                        soundEngine.Play();
                    }
                    else soundEngine.Resume();
                }
                else if (currentState.Triggers.Right == 0)
                {
                    if (soundEngine.State == SoundState.Playing)
                        soundEngine.Pause();
                }

                if (currentState.Buttons.A == ButtonState.Pressed)
                {
                    ship.Position = Vector3.Zero;
                    ship.Velocity = Vector3.Zero;
                    ship.Rotation = 0.0f;
                    soundHyperspaceActivation.Play();
                }
            }
        }
        */
    }
}
