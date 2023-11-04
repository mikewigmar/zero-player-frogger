using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Miguel_IAAJ_AV1
{
    public class Goal : GameObject
    {
        Rectangle rec;
        public Texture2D Text { get; private set; }
        Texture2D newTexture;
        public bool IsReached { get; private set; }

        public Goal(Vector2 position, int width, int height, Texture2D texture, Texture2D newTexture)
            : base(position, width, height, texture, ObjectTag.GOAL)
        {
            this.Text = this.Texture;
            this.newTexture = newTexture;
            this.rec = new Rectangle((int)position.X, (int)position.Y, this.width, this.height);
            this.IsReached = false;
        }

        public override void GoalMade() 
        {
            this.ChangeTag(ObjectTag.NULL);
            this.Text = newTexture;
            this.IsReached = true;
        }
    }
}
