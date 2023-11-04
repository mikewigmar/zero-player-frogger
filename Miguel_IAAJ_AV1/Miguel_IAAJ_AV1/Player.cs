using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NLua;
using KopiLua;

namespace Miguel_IAAJ_AV1
{
    public class Player:GameObject
    {
        List<GameObject> listGo;
        public int Lives { get; private set; }
        Vector2 startPos;
        KeyboardState currentInputState, previousInputState;
        bool timerStart;
        float counter;
        int posX, posY, luaCounter;
        public MovementInput movement;
        NLua.Lua lua;

        public bool isFrontObstacle, isRightObstacle, isLeftObstacle, isFrontPlatform, isFrontGoal;

        public Player(Vector2 position, int width, int height, Texture2D texture, List<GameObject> listGo) : base(position, width, height, texture, ObjectTag.PLAYER) 
        {
            this.listGo = listGo;
            this.startPos = position;
            Lives = 5;

            posX = (int)this.position.X;
            posY = (int)this.position.Y;

            this.lua = new NLua.Lua();
            this.lua.RegisterFunction("Print", this, this.GetType().GetMethod("Print"));
            //this.lua.RegisterFunction("Move", this, this.GetType().GetMethod("Move"));

            this.lua.RegisterFunction("MoveUp", this, this.GetType().GetMethod("MoveUp"));
            this.lua.RegisterFunction("MoveDown", this, this.GetType().GetMethod("MoveDown"));
            this.lua.RegisterFunction("MoveLeft", this, this.GetType().GetMethod("MoveLeft"));
            this.lua.RegisterFunction("MoveRight", this, this.GetType().GetMethod("MoveRight"));

            //this.lua.RegisterFunction("CheckFrontObstacle", this, this.GetType().GetMethod("CheckFrontObstacle"));
            //this.lua.RegisterFunction("CheckLeftObstacle", this, this.GetType().GetMethod("CheckLeftObstacle"));
            //this.lua.RegisterFunction("CheckRightObstacle", this, this.GetType().GetMethod("CheckRightObstacle"));
            //this.lua.RegisterFunction("CheckPlatformFront", this, this.GetType().GetMethod("CheckPlatformFront"));
            //this.lua.RegisterFunction("CheckGoalFront", this, this.GetType().GetMethod("CheckGoalFront"));

            this.lua["posY"] = position.Y;
            this.lua["counter"] = luaCounter;
            this.lua["frontObstacle"] = isFrontObstacle;
            this.lua["rightObstacle"] = isRightObstacle;
            this.lua["lefttObstacle"] = isLeftObstacle;
            this.lua["frontPlatform"] = isFrontPlatform;
            this.lua["frontGoal"] = isFrontGoal;

            try
            {
                ScriptLua("Start");
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
           
        }

        public string ParentCount() { if (Parent == null) return "no"; else return "yes"; }

        public override void Update(GameTime gameTime, float deltaTime)
        {
            luaCounter++;
            this.lua["counter"] = luaCounter;
            if (luaCounter > 600) luaCounter = 0;
            base.Update(gameTime, deltaTime);
            CollisionProcessing();
            UpdateAllSensors();
            
            if (this.Parent == null)
            {
                if (this.position.X <= (0 - (int)this.width))
                    this.position.X = (float)Refs.WindowWidth - this.width;
                else if (this.position.X >= Refs.WindowWidth)
                    this.position.X = 0f;
            }

            else
            {
                this.position = this.Parent.position;
            }
            
            if (this.position.Y <= 200 && this.Parent == null) timerStart = true;
            

            currentInputState = Keyboard.GetState();

            if (currentInputState.IsKeyDown(Keys.W) && previousInputState.IsKeyUp(Keys.W)) Move(MovementInput.UP);
            if (currentInputState.IsKeyDown(Keys.S) && previousInputState.IsKeyUp(Keys.S)) Move(MovementInput.DOWN);
            if (currentInputState.IsKeyDown(Keys.A) && previousInputState.IsKeyUp(Keys.A)) Move(MovementInput.LEFT);
            if (currentInputState.IsKeyDown(Keys.D) && previousInputState.IsKeyUp(Keys.D)) Move(MovementInput.RIGHT);
            if (currentInputState.IsKeyDown(Keys.Q) && previousInputState.IsKeyUp(Keys.Q)) Kill();

            previousInputState = currentInputState;

            try
            {
                ScriptLua("Update");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            //water death
            if (timerStart) 
            { 
                counter += deltaTime;
                if (counter >= 0.016f) Kill();
            }

            if (this.position.Y >= Refs.WindowHeight)
                this.position.Y = (float)Refs.WindowHeight - this.height;
            posX = (int)this.position.X;
            posY = (int)this.position.Y;
            

        }

        private void UpdateAllSensors() 
        {
            isFrontObstacle = CheckFrontObstacle();
            isRightObstacle = CheckRightObstacle();
            isLeftObstacle = CheckLeftObstacle();
            isFrontPlatform = CheckPlatformFront();
            isFrontGoal = CheckGoalFront();
            
            this.lua["posY"] = position.Y;
            this.lua["frontObstacle"] = isFrontObstacle;
            this.lua["rightObstacle"] = isRightObstacle;
            this.lua["lefttObstacle"] = isLeftObstacle;
            this.lua["frontPlatform"] = isFrontPlatform;
            this.lua["frontGoal"] = isFrontGoal;
        }

        private void CollisionProcessing()
        { 
            foreach (GameObject go in this.listGo)
	        {
		        if (this.Rec.Intersects(go.Rec))
                    switch (go.ObTag)
	                {
                        case ObjectTag.OBSTACLE:
                            {
                                Kill();
                                break;
                            }
                        case ObjectTag.GOAL:
                            {
                                //EVENT
                                go.GoalMade();
                                this.position = this.startPos;
                                break;
                            }
                        case ObjectTag.PLATFORM:
                            {
                                if (this.Parent == null) { this.Parent = go; timerStart = false; counter = 0; }
                                break;
                            }
                        default:
                            break;
	                }
	        }  
        }

        public void MoveUp() { if (this.Parent != null) { this.Parent = null; } this.position.Y -= 50f; }
        public void MoveDown() { if (this.Parent != null) { this.Parent = null; } this.position.Y += 50f; }
        public void MoveLeft() { if (this.Parent != null) { this.Parent = null; } this.position.X -= 50f; }
        public void MoveRight() { if (this.Parent != null) { this.Parent = null; } this.position.X += 50f; }

        public bool CheckFrontObstacle()
        {
            Rectangle checkRec = new Rectangle((int)this.position.X - 75, (int)this.position.Y - 50, 150, 50);
            foreach (GameObject go in this.listGo)
            {
                if (checkRec.Intersects(go.Rec))
                {
                    if (go.ObTag == ObjectTag.OBSTACLE) return true;
                }
            }
            return false;   
        }

        public bool CheckLeftObstacle()
        {
            Rectangle checkRec = new Rectangle((int)this.position.X - 75, (int)this.position.Y, 50, 50);
            foreach (GameObject go in this.listGo)
            {
                if (checkRec.Intersects(go.Rec))
                {
                    if (go.ObTag == ObjectTag.OBSTACLE) return true;
                }
            }
            return false;
        }

        public bool CheckRightObstacle()
        {
            Rectangle checkRec = new Rectangle((int)this.position.X + 75, (int)this.position.Y, 50, 50);
            foreach (GameObject go in this.listGo)
            {
                if (checkRec.Intersects(go.Rec))
                {
                    if (go.ObTag == ObjectTag.OBSTACLE) return true;
                }
            }
            return false;
        }

        public bool CheckPlatformFront()
        {
            Rectangle checkRec = new Rectangle((int)this.position.X, (int)this.position.Y - 50, 50, 30);
            foreach (GameObject go in this.listGo)
            {
                if (checkRec.Intersects(go.Rec))
                {
                    if (go.ObTag == ObjectTag.PLATFORM) return true;
                }
            }
            return false;
        }

        public bool CheckGoalFront()
        {
            Rectangle checkRec = new Rectangle((int)this.position.X, (int)this.position.Y - 50, 50, 30);
            foreach (GameObject go in this.listGo)
            {
                if (checkRec.Intersects(go.Rec))
                {
                    if (go.ObTag == ObjectTag.GOAL) return true;
                }
            }
            return false;
        }

        public void Move(MovementInput input)
        {
            if (this.Parent != null) { this.Parent = null; }
            switch (input)
            {
                case MovementInput.UP:
                    {
                        this.position.Y -= 50f;
                        break;
                    }
                case MovementInput.DOWN:
                    {
                        this.position.Y += 50f;
                        break;
                    }
                case MovementInput.LEFT:
                    {
                        this.position.X -= 50f;
                        break;
                    }
                case MovementInput.RIGHT:
                    {
                        this.position.X += 50f;
                        break;
                    }
                default:
                    break;

            }

            
        }

        private void Kill() 
        {
            Lives--;
            timerStart = false;
            counter = 0;
            this.position = this.startPos;
        }

        private void ScriptLua(string function)
        {
            this.lua.DoFile(@"PlayerLua.txt");
            ((LuaFunction)this.lua[function]).Call();
        }

        public void Print(string s)
        {
            Console.WriteLine(s);
        }
    }

    public enum MovementInput 
    { 
        NULL, UP, DOWN, LEFT, RIGHT
    }

}
