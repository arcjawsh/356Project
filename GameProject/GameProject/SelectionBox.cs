using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace GameProject
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class SelectionBox : Microsoft.Xna.Framework.DrawableGameComponent
    {

        MouseState prevMouseState;
        Rectangle selectionBox;
        Texture2D line;
        Game1 currentGame;
        SpriteFont font;
        Ray ray;
        HeightMapInfo heightMapInfo;
   
        public SelectionBox(Game1 game)
            : base(game)
        {
            currentGame = game;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            selectionBox = new Rectangle(-1, -1, 0, 0); //for the selection of objects
            prevMouseState = Mouse.GetState();
            BoundingSphereRenderer.Initialize(currentGame.GraphicsDevice, 45);
            base.Initialize();
        }

        public void setHM(HeightMapInfo h)
        {
            heightMapInfo = h;
        }

        protected override void LoadContent()
        {
            line = currentGame.Content.Load<Texture2D>("DottedLine");
            font = currentGame.Content.Load<SpriteFont>("font");
            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();

            //If the user has just clicked the Left mouse button, then set the start location for the Selection box
            if (mouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Released)
            {
                //Set the starting location for the selectiong box to the current location
                //where the Left button was initially clicked.
                selectionBox = new Rectangle(mouseState.X, mouseState.Y, 0, 0);
            }

            //If the user is still holding the Left button down, then continue to re-size the 
            //selection square based on where the mouse has currently been moved to.
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                //The starting location for the selection box remains the same, but increase (or decrease)
                //the size of the Width and Height but taking the current location of the mouse minus the
                //original starting location.
                selectionBox = new Rectangle(selectionBox.X, selectionBox.Y, mouseState.X - selectionBox.X, mouseState.Y - selectionBox.Y);
            }

            //If the user has released the left mouse button, then reset the selection square
            if (mouseState.LeftButton == ButtonState.Released)
            {
                //Reset the selection square to no position with no height and width
                selectionBox = new Rectangle(-1, -1, 0, 0);
            }

            if (mouseState.LeftButton == ButtonState.Released && prevMouseState.LeftButton == ButtonState.Pressed) //clicked
            {
                clicked(mouseState);
            }
            if (mouseState.RightButton == ButtonState.Released && prevMouseState.RightButton == ButtonState.Pressed) //clicked
            { //right click //moved to
                moveTo(mouseState);
            }
            //Store the previous mouse state
            checkHover(mouseState);
            prevMouseState = mouseState;
            base.Update(gameTime);
        }

        public void moveTo(MouseState mouseState)
        {
            Ray ray = getTankRay(mouseState);
            foreach (Tank tank in currentGame.tanks) //if the tank is selected, send the move command
            {
                if (tank.isSelected == true)
                    tank.moveTo(ray, heightMapInfo);
            }
        }


        public Ray getTankRay(MouseState mouseState)
        { //converts 2D space to 3D space
            Ray ret;
            Viewport vp = currentGame.GraphicsDevice.Viewport;
            Vector3 pos1 = vp.Unproject(new Vector3(mouseState.X, mouseState.Y, 0), currentGame.camera.projectionMatrix, currentGame.camera.viewMatrix, Matrix.Identity);
            Vector3 pos2 = vp.Unproject(new Vector3(mouseState.X, mouseState.Y, 1), currentGame.camera.projectionMatrix, currentGame.camera.viewMatrix, Matrix.Identity);
            //Vector3 pos1 = vp.Unproject(new Vector3(mouseState.X, mouseState.Y, 0), Matrix.Identity, Matrix.Identity, Matrix.Identity);
            //Vector3 pos2 = vp.Unproject(new Vector3(mouseState.X, mouseState.Y, 1), Matrix.Identity, Matrix.Identity, Matrix.Identity);
            Vector3 dir = Vector3.Normalize(pos2 - pos1);
            ret = new Ray(pos1, dir);
            //BoundingSphere sphere = new BoundingSphere((ret.Position), 150f); //position and radius
       //BoundingSphereRenderer.Draw(sphere, currentGame.camera.viewMatrix, currentGame.camera.projectionMatrix);
            return ret;
        }
       

        public void clicked(MouseState mouseState)
        { //converts to screen space where user clicked
            ray = getTankRay(mouseState);
            foreach (Tank tank in currentGame.tanks)
                tank.checkClick(ray);
        }

        public void checkHover(MouseState mouseState)
        { //checks if mouse is hovered over a model
            ray = getTankRay(mouseState);
            foreach (Tank tank in currentGame.tanks)
                tank.checkHover(ray); 

        }


        public void drawMousePosition()
        {
            //if ((prevMouseState.X >= 0 && prevMouseState.X <= Game.Window.ClientBounds.Width) && (prevMouseState.Y >= 0 && prevMouseState.Y <= Game.Window.ClientBounds.Height))
                //currentGame.spriteBatch.DrawString(font, String.Format("{0}, {1}", prevMouseState.X, prevMouseState.Y), new Vector2(10, 10), Color.CornflowerBlue);
            //MouseState state = Mouse.GetState();
            //Ray ray = getTankRay(Mouse.GetState());
            //currentGame.spriteBatch.DrawString(font, String.Format("{0}, {1}, {2}", ray.Position.X, ray.Position.Y, ray.Position.Z ), new Vector2(10, 25), Color.CornflowerBlue);
        
        }



        public override void Draw(GameTime gameTime)
        {
            //currentGame.spriteBatch.Begin(SaveStateMode.SaveState);
            currentGame.spriteBatch.Begin();
            //draw where mouse is in vector2
            drawMousePosition();
            //draw where mouse is in the world

            //Draw the horizontal portions of the selection box 
            DrawHorizontalLine(selectionBox.Y);
            DrawHorizontalLine(selectionBox.Y + selectionBox.Height);

            DrawVerticalLine(selectionBox.X);
            DrawVerticalLine(selectionBox.X + selectionBox.Width);
            currentGame.spriteBatch.End();
            currentGame.GraphicsDevice.BlendState = BlendState.Opaque;
            currentGame.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            currentGame.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            base.Draw(gameTime);
        }

        public void DrawHorizontalLine(int thePositionY)
        {
            //When the width is greater than 0, the user is selecting an area to the right of the starting point
            if (selectionBox.Width > 0)
            {
                //Draw the line starting at the startring location and moving to the right
                for (int aCounter = 0; aCounter <= selectionBox.Width - 10; aCounter += 10)
                {
                    if (selectionBox.Width - aCounter >= 0)
                    {
                        currentGame.spriteBatch.Draw(line, new Rectangle(selectionBox.X + aCounter, thePositionY, 10, 5), Color.White);
                    }
                }
            }
            //When the width is less than 0, the user is selecting an area to the left of the starting point
            else if (selectionBox.Width < 0)
            {
                //Draw the line starting at the starting location and moving to the left
                for (int aCounter = -10; aCounter >= selectionBox.Width; aCounter -= 10)
                {
                    if (selectionBox.Width - aCounter <= 0)
                    {
                        currentGame.spriteBatch.Draw(line, new Rectangle(selectionBox.X + aCounter, thePositionY, 10, 5), Color.White);
                    }
                }
            }
        }

        public void DrawVerticalLine(int thePositionX)
        {
            //When the height is greater than 0, the user is selecting an area below the starting point
            if (selectionBox.Height > 0)
            {
                //Draw the line starting at the starting loctino and moving down
                for (int aCounter = -2; aCounter <= selectionBox.Height; aCounter += 10)
                {
                    if (selectionBox.Height - aCounter >= 0)
                    {
                        currentGame.spriteBatch.Draw(line, new Rectangle(thePositionX, selectionBox.Y + aCounter, 10, 5), new Rectangle(0, 0, line.Width, line.Height), Color.White, MathHelper.ToRadians(90), new Vector2(0, 0), SpriteEffects.None, 0);
                    }
                }
            }
            //When the height is less than 0, the user is selecting an area above the starting point
            else if (selectionBox.Height < 0)
            {
                //Draw the line starting at the start location and moving up
                for (int aCounter = 0; aCounter >= selectionBox.Height; aCounter -= 10)
                {
                    if (selectionBox.Height - aCounter <= 0)
                    {
                        currentGame.spriteBatch.Draw(line, new Rectangle(thePositionX - 10, selectionBox.Y + aCounter, 10, 5), Color.White);
                    }
                }
            }
        }
        //
    }
}
