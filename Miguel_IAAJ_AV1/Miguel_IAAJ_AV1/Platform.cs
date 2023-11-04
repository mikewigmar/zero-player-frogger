using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Miguel_IAAJ_AV1
{
    public class Platform : GameObject
    {
        float speed;

        public Platform(Vector2 position, int width, int height, Texture2D texture, float speed)
            : base(position, width, height, texture, ObjectTag.PLATFORM)
        {
            this.speed = speed;
        }

        public override void Update(GameTime gameTime, float deltaTime)
        {
            base.Update(gameTime, deltaTime);


            this.position.X += this.speed * deltaTime;

            if (this.position.X >= Refs.WindowWidth)
            {
                this.position.X = 0 - this.width;
            }


        }

    }
}
