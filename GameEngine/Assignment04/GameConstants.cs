using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CPI311.GameEngine
{
    public class GameConstants
    {
        //camera constants  
        public const float CameraHeight = 25000.0f;
        public const float PlayfieldSizeX = 16000f;
        public const float PlayfieldSizeY = 12500f;
        //asteroid constants  
        public const int NumAsteroids = 10;
        public const int NumBullets = 10;
        public const float AsteroidSpeedAdjustment = 5.0f;
        public const float AsteroidMinSpeed = 1200.0f;
        public const float AsteroidMaxSpeed = 1500.0f;
        public const float PlayerSpeed = 2500.0f;
        public const float PlayerRotationSpeed = 3.0f;
        public const float BulletSpeedAdjustment = 5000.0f;
        public const int PlayerHealth = 5;

        public const int ShotPenalty = 1;
        public const int KillBonus = 100;
    }
}
