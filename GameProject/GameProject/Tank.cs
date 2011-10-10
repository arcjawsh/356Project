//mostly taken from the Microsoft Website Example
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;


namespace GameProject
{
    class Tank
    {
        SoundEffect beep;
        const float TankVelocity = 10;
        const float TankWheelRadius = 18;
        const float TankTurnSpeed = .025f;
        public bool isSelected = false;
        public Vector3 Position
        {
            get { return position; }            
        }
        private Vector3 position;        
        public float FacingDirection
        {
            get { return facingDirection; }            
        }
        private float facingDirection;
        Model model;
        Matrix orientation = Matrix.Identity;
        Matrix wheelRollMatrix = Matrix.Identity;

        ModelBone leftBackWheelBone;
        ModelBone rightBackWheelBone;
        ModelBone leftFrontWheelBone;
        ModelBone rightFrontWheelBone;

        Matrix leftBackWheelTransform;
        Matrix rightBackWheelTransform;
        Matrix leftFrontWheelTransform;
        Matrix rightFrontWheelTransform;

  
        public void LoadContent(ContentManager content)
        {
            model = content.Load<Model>("Tank");
            beep = content.Load<SoundEffect>("beep");
            // as discussed in the Simple Animation Sample, we'll look up the bones
            // that control the wheels.
            leftBackWheelBone = model.Bones["l_back_wheel_geo"];
            rightBackWheelBone = model.Bones["r_back_wheel_geo"];
            leftFrontWheelBone = model.Bones["l_front_wheel_geo"];
            rightFrontWheelBone = model.Bones["r_front_wheel_geo"];

            // Also, we'll store the original transform matrix for each animating bone.
            leftBackWheelTransform = leftBackWheelBone.Transform;
            rightBackWheelTransform = rightBackWheelBone.Transform;
            leftFrontWheelTransform = leftFrontWheelBone.Transform;
            rightFrontWheelTransform = rightFrontWheelBone.Transform;
        }

      
        public void HandleInput(HeightMapInfo heightMapInfo)
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();
            float turnAmount = 0; 
            if (currentKeyboardState.IsKeyDown(Keys.Left))
            {
                turnAmount += 1;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Right))
            {
                turnAmount -= 1;
            }
            // clamp the turn amount between -1 and 1, and then use the finished
            // value to turn the tank.
            turnAmount = MathHelper.Clamp(turnAmount, -1, 1);
            facingDirection += turnAmount * TankTurnSpeed;
            //woodjosh
            Vector3 movement = Vector3.Zero;
            movement.Z = 0;

            if (currentKeyboardState.IsKeyDown(Keys.Down))
            {
                movement.Z = -1;
            }
            if ( currentKeyboardState.IsKeyDown(Keys.Up))
            {
                movement.Z = 1;
            }

            // next, we'll create a rotation matrix from the direction the tank is 
            // facing, and use it to transform the vector.
            orientation = Matrix.CreateRotationY(FacingDirection);
            Vector3 velocity = Vector3.Transform(movement, orientation);
            velocity *= TankVelocity;

            // Now we know how much the user wants to move. We'll construct a temporary
            // vector, newPosition, which will represent where the user wants to go. If
            // that value is on the heightmap, we'll allow the move.
            Vector3 newPosition = Position + velocity;
            if (heightMapInfo.IsOnHeightmap(newPosition))
            {
                // now that we know we're on the heightmap, we need to know the correct
                // height and normal at this position.
                Vector3 normal;
                heightMapInfo.GetHeightAndNormal(newPosition,
                    out newPosition.Y, out normal);


                // As discussed in the doc, we'll use the normal of the heightmap
                // and our desired forward direction to recalculate our orientation
                // matrix. It's important to normalize, as well.
                orientation.Up = normal;

                orientation.Right = Vector3.Cross(orientation.Forward, orientation.Up);
                orientation.Right = Vector3.Normalize(orientation.Right);

                orientation.Forward = Vector3.Cross(orientation.Up, orientation.Right);
                orientation.Forward = Vector3.Normalize(orientation.Forward);

                // now we need to roll the tank's wheels "forward." to do this, we'll
                // calculate how far they have rolled, and from there calculate how much
                // they must have rotated.
                float distanceMoved = Vector3.Distance(Position, newPosition);
                float theta = distanceMoved / TankWheelRadius;
                int rollDirection = movement.Z > 0 ? 1 : -1;

                wheelRollMatrix *= Matrix.CreateRotationX(theta * rollDirection);

                // once we've finished all computations, we can set our position to the
                // new position that we calculated.
                position = newPosition;
            }
        }

        public void mouseOverTank()
        {
            MouseState mouse = Mouse.GetState(); //gets position of current mouse
        }

        #region Draw

        public void Draw(Matrix viewMatrix, Matrix projectionMatrix)
        {
            // Apply matrices to the relevant bones, as discussed in the Simple 
            // Animation Sample.
            leftBackWheelBone.Transform = wheelRollMatrix * leftBackWheelTransform;
            rightBackWheelBone.Transform = wheelRollMatrix * rightBackWheelTransform;
            leftFrontWheelBone.Transform = wheelRollMatrix * leftFrontWheelTransform;
            rightFrontWheelBone.Transform = wheelRollMatrix * rightFrontWheelTransform;

            // now that we've updated the wheels' transforms, we can create an array
            // of absolute transforms for all of the bones, and then use it to draw.
            Matrix[] boneTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(boneTransforms);
            
            // calculate the tank's world matrix, which will be a combination of our
            // orientation and a translation matrix that will put us at at the correct
            // position.
            Matrix worldMatrix = orientation * Matrix.CreateTranslation(Position);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = boneTransforms[mesh.ParentBone.Index] * worldMatrix;
                    effect.View = viewMatrix;
                    effect.Projection = projectionMatrix;

                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;

                    // Set the fog to match the black background color
                    effect.FogEnabled = true;
                    effect.FogColor = Vector3.Zero;
                    effect.FogStart = 1000;
                    effect.FogEnd = 3200;
                }
                mesh.Draw();
            }
        }

        #endregion
    }
}
