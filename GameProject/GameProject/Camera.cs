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
    public class Camera : Microsoft.Xna.Framework.GameComponent
    {
        //Camera matrices
        public Matrix viewMatrix { get; protected set; }
        public Matrix projectionMatrix { get; protected set; }
        public Vector3 cameraPosition { get; set; }
        public Vector3 cameraDirection;
        public Vector3 cameraTarget; //target of camera
        public Vector3 angle = new Vector3();
        Vector3 cameraUp;
        float speed = 10;


        public Camera(Game game, Vector3 pos, Vector3 target, Vector3 up)
            : base(game)
        {
            // Build camera view matrix
            cameraPosition = pos;
            cameraDirection = target - pos;
            cameraTarget = target;
            cameraDirection.Normalize();
            cameraUp = up;
            CreateLookAt(); //make camera face target
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)Game.Window.ClientBounds.Width / (float)Game.Window.ClientBounds.Height, 1, 3000);
        }
        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            angle.X = 0.60f;
            angle.Y = 0.55f;
            // Move forward/backward
            Vector3 forward = Vector3.Normalize(new Vector3((float)Math.Sin(-angle.Y), (float)Math.Sin(0f), (float)Math.Cos(-angle.Y)));
            Vector3 left = Vector3.Normalize(new Vector3((float)Math.Cos(angle.Y), 0f, (float)Math.Sin(angle.Y)));

            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                //cameraPosition += cameraDirection * speed;
                cameraPosition -= forward * speed;

            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                //cameraPosition = cameraPosition - (cameraDirection * speed);
                cameraPosition += forward * speed;
            }
            // Move side to side
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                //cameraPosition += Vector3.Cross(cameraUp, cameraDirection) * speed;
                cameraPosition -= left * speed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                //cameraPosition = cameraPosition - (Vector3.Cross(cameraUp, cameraDirection) * speed);
                cameraPosition += left * speed;
            }

            // Recreate the camera view matrix
            CreateLookAt();

            base.Update(gameTime);
        }

        private void CreateLookAt()
        {
            
            //viewMatrix = Matrix.CreateLookAt(cameraPosition,
           // cameraPosition + cameraDirection, cameraUp);
            viewMatrix = Matrix.Identity;
            viewMatrix *= Matrix.CreateTranslation(-cameraPosition);
            viewMatrix *= Matrix.CreateRotationZ(angle.Z);
            viewMatrix *= Matrix.CreateRotationY(angle.Y);
            viewMatrix *= Matrix.CreateRotationX(angle.X);
        }
    }
}
