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
        GraphicsDevice graphics;
   
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
            base.Initialize();
        }

        protected override void LoadContent()
        {
            line = currentGame.Content.Load<Texture2D>("DottedLine");
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
            //Store the previous mouse state
            prevMouseState = mouseState;
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            //currentGame.spriteBatch.Begin(SaveStateMode.SaveState);
            currentGame.spriteBatch.Begin();
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
