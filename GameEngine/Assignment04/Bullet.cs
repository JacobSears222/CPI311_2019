using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace CPI311.GameEngine
{
    public class Bullet: GameObject
    {
        public bool isActive { get; set; }
        Ship ship;
        Model model;
        public Bullet(ContentManager Content, Camera camera, GraphicsDevice graphicsDevice, Light light, Ship ship): base()
        {
            model = Content.Load<Model>("bullet");
            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            Add<Rigidbody>(rigidbody);

            Texture2D texture = Content.Load<Texture2D>("pea_proj");
            Renderer renderer = new Renderer(model, Transform, camera, Content, graphicsDevice, light, 1, null, 20f, texture);
            Add<Renderer>(renderer);

            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = renderer.ObjectModel.Meshes[0].BoundingSphere.Radius;
            sphereCollider.Transform = Transform;
            Add<Collider>(sphereCollider);

            isActive = false;
        }

        public override void Update()
        {
            if (!isActive) return;
            
            if (Transform.Position.X > GameConstants.PlayfieldSizeX ||
               Transform.Position.X < -GameConstants.PlayfieldSizeX ||
               Transform.Position.Y > GameConstants.PlayfieldSizeY ||
               Transform.Position.Y < -GameConstants.PlayfieldSizeY)
            {
                isActive = false;
                Rigidbody.Velocity = Vector3.Zero;
            }             
            base.Update();
        }

        public override void Draw()
        {
            if (isActive) base.Draw();
        }
    }
}
