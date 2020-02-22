using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CPI311.GameEngine
{
    public class ProgressBar : Sprite
    {
        public Color FillColor { get; set; }
        public float Value { get; set; }
        public float Speed { get; set; }

        public ProgressBar(Texture2D texture, float value = 1, float speed = 0) : base(texture)
        {
            Value = 1;
            Speed = speed;
            Color = Color.White;
        }

        public override void Update()
        {
            base.Update();
            Value = MathHelper.Clamp(Value + Speed * Time.ElapsedGameTime, 0, 1);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            spriteBatch.Draw(Texture, Position, new Rectangle(0, 0, (int)(Source.Width * Value), Source.Height), FillColor, Rotation, Origin, Scale, Effect, 1);
        }
    }
}