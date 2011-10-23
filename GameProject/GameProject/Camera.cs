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
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace GameProject
{
    public class Camera
    {
        //Camera matrices
        public Matrix viewMatrix { get; protected set; }
        public Matrix projectionMatrix { get; protected set; }
        public Vector3 position;
        Game1 currentGame;
        public Vector3 angle = new Vector3();
        float speed = 100f;
        public float turnSpeed = 90f;
        public float ZoomSpeed = 20f;
        public MouseState lastState;
        Texture2D mousePointer; //texture for mouse
        


        public Camera(Game1 game)
        {
            // Build camera view matrix
            currentGame = game;
            position = new Vector3(0, 300, 0);
            ZoomSpeed = 0f;
            lastState = Mouse.GetState();
            
            //CreateLookAt(); //make camera face target
            //mousePointer = currentGame.Content.Load<Texture2D>("mouse");
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)currentGame.Window.ClientBounds.Width / (float)currentGame.Window.ClientBounds.Height, 10, 20000);
        }

        public void LoadContent()
        {
            mousePointer = currentGame.Content.Load<Texture2D>("mouse");
        }
      



        public  void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
         
            MouseState mouse = Mouse.GetState();
            KeyboardState keyboard = Keyboard.GetState();

            angle.X = 0.40f;

            if (mouse.MiddleButton == ButtonState.Pressed)
            {

                angle.Y += (mouse.X - lastState.X) * delta * 0.15f;
                int centerX = currentGame.Window.ClientBounds.Width / 2;
                int centerY = currentGame.Window.ClientBounds.Width / 2;

                Mouse.SetPosition(centerX, centerY);

            }

            if (keyboard.IsKeyDown(Keys.Q))
                angle.Y -= speed * delta * 0.005f;

            if (keyboard.IsKeyDown(Keys.E))
                angle.Y += speed * delta * 0.005f;

            ZoomSpeed -= (float)(mouse.ScrollWheelValue) - (lastState.ScrollWheelValue);
            ZoomSpeed *= 0.9f;
            position.Z += ZoomSpeed * delta;
            position.Y += ZoomSpeed * delta/3;
            position.X += ZoomSpeed/2 * delta /3;

            if (keyboard.IsKeyDown(Keys.LeftShift))
                speed = 2000f;
            else
                speed = 500f;

            //
            Vector3 forward = Vector3.Normalize(new Vector3((float)Math.Sin(-angle.Y), (float)Math.Sin(0f), (float)Math.Cos(-angle.Y)));
            Vector3 left = Vector3.Normalize(new Vector3((float)Math.Cos(angle.Y), 0f, (float)Math.Sin(angle.Y)));

            if (keyboard.IsKeyDown(Keys.W))
                position -= forward * speed * delta;

            if (keyboard.IsKeyDown(Keys.S))
                position += forward * speed * delta;

            if (keyboard.IsKeyDown(Keys.A))
                position -= left * speed * delta;

            if (keyboard.IsKeyDown(Keys.D))
                position += left * speed * delta;

            if (keyboard.IsKeyDown(Keys.Z))
                position += Vector3.Up * speed * delta;

            if (keyboard.IsKeyDown(Keys.X))
                position -= Vector3.Up * speed * delta;
            // Recreate the camera view matrix
            CreateLookAt();
            lastState = Mouse.GetState();
        }

        public void CreateLookAt()
        {
            viewMatrix = Matrix.Identity;
            viewMatrix *= Matrix.CreateTranslation(-position);
            viewMatrix *= Matrix.CreateRotationZ(angle.Z);
            viewMatrix *= Matrix.CreateRotationY(angle.Y);
            viewMatrix *= Matrix.CreateRotationX(angle.X);
         
        }

        public  void Draw()
        {
  
            currentGame.spriteBatch.Begin();
            currentGame.spriteBatch.Draw(mousePointer, new Vector2(Mouse.GetState().X, Mouse.GetState().Y), Color.White);
            currentGame.spriteBatch.End();
            currentGame.GraphicsDevice.BlendState = BlendState.Opaque;
            currentGame.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            currentGame.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
        }
    }
}
