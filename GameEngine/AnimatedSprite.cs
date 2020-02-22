using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CPI311.GameEngine
{
    public class AnimatedSprite : Sprite
    {

        // Animation stuff
        public int Frames { get; set; }
        public static int currentRow { get; set; }
        public float currentFrame { get; set; }
        public bool loop = true;

        private int frameHeight { get { return Texture.Height / 5; } }
        private int frameWidth { get { return Texture.Width / Frames; } }

        public float Speed { get; set; }

        public AnimatedSprite(Texture2D texture, int frames) : base (texture)
        {
            Frames = frames;
            currentFrame = 0;
            Speed = 20f;
            currentRow = 0;
        }

        public override void Update()
        {
            currentFrame += (Speed * Time.ElapsedGameTime) % Frames;
            Source = new Rectangle(0, 0, frameWidth, frameHeight);
            if (currentFrame >= Frames)
            {
                currentFrame = 0;
            }

            Source = new Rectangle(0, currentRow * frameHeight, frameWidth, frameHeight);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //base.Draw(spriteBatch);
            spriteBatch.Draw(Texture, Position, new Rectangle((int) currentFrame * frameWidth, currentRow * frameHeight, frameWidth, frameHeight), Color.White);
        }
    }
}