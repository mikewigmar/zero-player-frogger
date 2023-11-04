using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Miguel_IAAJ_AV1
{
    public class GameObject
    {

        public Vector2 position;
        public Texture2D Texture { get; private set; }
        public Rectangle Rec{ get; private set;}
        protected int width, height;
        public ObjectTag ObTag {get; private set;}
        //public List<GameObject> Children { get; set; }
        public GameObject Parent { get; set; }
        

        public GameObject(Vector2 position,int width, int height, Texture2D texture, ObjectTag tag = ObjectTag.NULL) 
        {
            this.position = position;
            this.width = width;
            this.height = height;
            this.Texture = texture;
            this.ObTag = tag;
            //this.Children = new List<GameObject>();
        }

        public virtual void Update(GameTime gameTime, float deltaTime)
        {
            UpdateCollision();
            //if (this.Children.Count > 0) TransformChildren();
            
            
                
            
        }

        public virtual void Draw(GameTime gameTime, float deltaTime)
        {
            //talvez precise
        }

        //public void TransformChildren() 
        //{   
        //    foreach (GameObject gO in Children)
        //    {
        //            gO.position = this.position;
        //    }    
        //}
        public void ChangeTag(ObjectTag tag) { this.ObTag = tag; }

        public void UpdateCollision()
        {
            this.Rec = new Rectangle((int)this.position.X, (int)this.position.Y, this.width, this.height);
        }

        //public void AddChild(GameObject gO)
        //{
        //    this.Children.Add(gO);
        //    gO.Parent = this;
        //}

        //public void RemoveChild(GameObject gO)
        //{
        //    this.Children.Remove(gO);
        //    gO.Parent = null;
        //}

        //public void AddParent(GameObject gO)
        //{
        //    gO.Children.Add(this);
        //    this.Parent = gO;
        //}

        //public void RemoveParent(GameObject gO)
        //{
        //    gO.Children.Remove(this);
        //    this.Parent = null;
        //}

        public virtual void GoalMade() { }
    }

    public enum ObjectTag
    {
        OBSTACLE,
        GOAL,
        PLATFORM,
        PLAYER,
        NULL
    }
}
