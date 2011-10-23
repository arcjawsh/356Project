//mostly taken from the Microsoft Website Example
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.IO;



namespace GameProject
{
    public class Tank
    {
        SoundEffect beep;
        SoundEffect hover;
        public SoundEffect explode;
        //for 3d positional
        public Boolean delete = false;
        public BoundingSphere targSphere;
        Texture2D healthTexture;
        Rectangle healthRectangle;
        SpriteFont font;
        Game1 game;
        Model circle;
        HeightMapInfo heightMapInfo;

        public float MoveSpeed;
        public Vector3 target;
        public BoundingSphere sphere;
        const float TankVelocity = 10;
        const float TankWheelRadius = 18;
        const float TankTurnSpeed = .025f;
        public bool isSelected = false;
        public bool isHovered = false; //safeguard againt multiple hover sounds
        public bool isMoving = false;
        public Vector3 Position
        {
            get { return position; }            
        }
        public float currentHealth = 100;
        public float startHealth = 100;
        public float firePower = 100;
        public float HealthPercentage
        {
            get { return currentHealth / startHealth; }
        }
        public Vector3 position; //tank osition        
        public float FacingDirection
        {
            get { return facingDirection; }            
        }
        private float facingDirection;
        Model model;
        public Matrix orientation = Matrix.Identity;
        Matrix wheelRollMatrix = Matrix.Identity;

        ModelBone leftBackWheelBone;
        ModelBone rightBackWheelBone;
        ModelBone leftFrontWheelBone;
        ModelBone rightFrontWheelBone;

        Matrix leftBackWheelTransform;
        Matrix rightBackWheelTransform;
        Matrix leftFrontWheelTransform;
        Matrix rightFrontWheelTransform;
        //Boolean moving = false;

        public Tank(Game1 g, HeightMapInfo h, Vector3 pos)
        {
            game = g;
            position = pos;
            heightMapInfo = h;
            //HandleInput();
            
        }
   

        public void LoadContent(ContentManager content)
        {
            model = content.Load<Model>("Tank");
            beep = content.Load<SoundEffect>("beep");
            explode = content.Load<SoundEffect>("explosion");
            hover = content.Load<SoundEffect>("hover");
            healthTexture = content.Load<Texture2D>("health bar");
            font = content.Load<SpriteFont>("font");
            circle = content.Load<Model>("sphere");
            //engine = content.Load<SoundEffect>("engine");
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

       
            BoundingSphereRenderer.Initialize(game.GraphicsDevice, 45);
            
        }

   
        public void input(HeightMapInfo heightMapInfo)
        {
            /*
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
            if (currentKeyboardState.IsKeyDown(Keys.Up))
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
            //set 3d emitter position
            //emitter.Position = Position;

            //engineInstance.Apply3D(game.camera.listener, emitter);
           */
        }

        public void handleMovement(GameTime gameTime)
        {
            //do turn speed
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            float previousMoveSpeed = MoveSpeed;
            float desiredMoveSpeed = FindMaxMoveSpeed(target);
            MoveSpeed = MathHelper.Clamp(desiredMoveSpeed,
                previousMoveSpeed - (TankVelocity /2) * elapsedTime,
                previousMoveSpeed + (TankVelocity / 2) * elapsedTime);
            
            MoveSpeed = MathHelper.Clamp(desiredMoveSpeed * 1.5f, -1, 1);
            //do direction

            float facingDirection = (float)Math.Atan2(
                this.orientation.Backward.X, this.orientation.Backward.Z);
            facingDirection = TurnToFace(this.position,target,
                    facingDirection, 1 *elapsedTime);

            

            orientation = Matrix.CreateRotationY(facingDirection);
            /*
            if (heightMapInfo.IsOnHeightmap(position))
            {
                Vector3 normal;
                heightMapInfo.GetHeightAndNormal(position,
                    out position.Y, out normal);

                orientation.Up = normal;
                orientation.Right = Vector3.Cross(orientation.Forward, orientation.Up);
                orientation.Right = Vector3.Normalize(orientation.Right);
                orientation.Forward = Vector3.Cross(orientation.Up, orientation.Right);
                orientation.Forward = Vector3.Normalize(orientation.Forward);
                float distanceMoved = Vector3.Distance(Position, newPosition);
                float theta = distanceMoved / TankWheelRadius;
                int rollDirection = movement.Z > 0 ? 1 : -1;

                wheelRollMatrix *= Matrix.CreateRotationX(theta * rollDirection);

                position = newPosition;
            }*/

            //got the new forward, so move in + that direction
            position = position + (orientation.Backward * MoveSpeed * elapsedTime * 325) ;
     

            targSphere = new BoundingSphere(target, 50f);
            //BoundingSphereRenderer.Draw(targSphere, game.camera.viewMatrix, game.camera.projectionMatrix);
            if (targSphere.Intersects(sphere))
            {
                isMoving = false;
            }
        }
        public void HandleInput(GameTime g)
        {
            
            if (heightMapInfo.IsOnHeightmap(position))
            {
                Vector3 normal;
                heightMapInfo.GetHeightAndNormal(position,
                    out position.Y, out normal);
            }

            if (isSelected == true) //if current tank is selected, allow moving
            {
                if (isMoving == false)
                    input(heightMapInfo);
                else
                {
                    handleMovement(g);
                }

            }
            //check other tanks for collision
            foreach(Tank t in game.tanks)
            {
                if (this != t) //if not the current tank
                {
                    if (this.sphere.Intersects(t.sphere))
                    {
                        explode.Play();
                        delete = true; //set to delete
                        t.delete = true;
                        //game.tanksCount = game.tanksCount - 1;
                    }
                }
              }
            
        }

      

        public void moveTo(Ray ray, HeightMapInfo heightMapInfo)
        { //move to order issued
        //check heightmap
            
            //start at ray position/ move in ray direction  until it hits heightmap
            //ray.Position
            //ray.Direciton
            isMoving = true;
            Boolean posFound = false;
            Vector3 normal = Vector3.Zero;
            while (posFound == false)
            { //while position not on heightmap
                Vector3 tempPos = ray.Position;
                heightMapInfo.GetHeightAndNormal(tempPos,
                    out tempPos.Y, out normal);
                if (ray.Position.Y - tempPos.Y < 1 && ray.Position.Y - tempPos.Y >-1)
                    posFound = true;

                ray.Position += ray.Direction;
            }
                //move to direction
                //orientation = Matrix.CreateRotationY(FacingDirection);
                //orientation.Up = normal;
            target = ray.Position;
                //position = ray.Position;
      
          
        }
    
        public void checkHover(Ray ray)
        {
            sphere = new BoundingSphere(position + new Vector3(0, 60, 0), 140f); //position and radius
            //check collision
            if (sphere.Intersects(ray) != null) //if in bounds
            { //play sound if not already hovering
                if (isHovered == false)
                {
                    hover.Play();
                    isHovered = true;
                }
            }
            else
                isHovered = false;
        }


        public void checkClick(Ray ray)
        {
            //play sound for lulz
            //check intersect on bounding sphere
            sphere = new BoundingSphere(position + new Vector3(0, 60, 0), 140f); //position and radius
            //check collision
            if (sphere.Intersects(ray) != null)
            {
                beep.Play();
                isSelected = true;
            }
            else
                isSelected = false;

        }

        public void mouseOverTank()
        {
            MouseState mouse = Mouse.GetState(); //gets position of current mouse
        }

        #region Draw

      

        public void Draw(Matrix viewMatrix, Matrix projectionMatrix)
        {
            //game.GraphicsDevice.RasterizerState.CullMode = CullMode.None;
            // Apply matrices to the relevant bones, as discussed in the Simple 
            // Animation Sample.
            leftBackWheelBone.Transform = wheelRollMatrix * leftBackWheelTransform;
            rightBackWheelBone.Transform = wheelRollMatrix * rightBackWheelTransform;
            leftFrontWheelBone.Transform = wheelRollMatrix * leftFrontWheelTransform;
            rightFrontWheelBone.Transform = wheelRollMatrix * rightFrontWheelTransform;

           
            Matrix[] boneTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(boneTransforms);
            
           Matrix worldMatrix = Matrix.CreateScale(0.4f, 0.4f, 0.4f) * orientation * Matrix.CreateTranslation(position);

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
                    //effect.FogEnabled = true;
                    //effect.FogColor = Vector3.Zero;
                    //effect.FogStart = 1000;
                   // effect.FogEnd = 3200;
                }
                BoundingSphereRenderer.Draw(sphere, viewMatrix, projectionMatrix);
                mesh.Draw();


                //drawing bounding spheres
            }

            if (isSelected == true)
            { //draw model of circle
                Vector3 temp = new Vector3();
                temp = position + new Vector3(0, 300, 0);

                Matrix worldMatrix2 = Matrix.CreateScale(5f, 5f, 5f) * orientation * Matrix.CreateTranslation(temp);
                Matrix[] boneTransform2s = new Matrix[circle.Bones.Count];
                circle.CopyAbsoluteBoneTransformsTo(boneTransforms);

               
                foreach (ModelMesh mesh in circle.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.World = boneTransforms[mesh.ParentBone.Index] * worldMatrix2;
                        effect.View = viewMatrix;
                        effect.Projection = projectionMatrix;

                        effect.EnableDefaultLighting();
                        effect.PreferPerPixelLighting = true;

                        // Set the fog to match the black background color
                        //effect.FogEnabled = true;
                        //effect.FogColor = Vector3.Zero;
                        //effect.FogStart = 1000;
                        //effect.FogEnd = 3200;
                    }
                    mesh.Draw();

                    //BoundingSphere sphere = T

                    //sphere = new BoundingSphere(position + new Vector3(0, 60, 0), 180f); //position and radius
                    BoundingSphereRenderer.Draw(sphere, viewMatrix, projectionMatrix);

                    if(isMoving)
                        BoundingSphereRenderer.Draw(targSphere, viewMatrix, projectionMatrix);
                }

            }

            //draw health bar
            
            game.spriteBatch.Begin();
            
            if (isSelected)
            {
                Vector3 rect = game.GraphicsDevice.Viewport.Project(position, game.camera.projectionMatrix, game.camera.viewMatrix, Matrix.Identity);

                rect += new Vector3(-40, -80, 0);
                Rectangle healthRectangle = new Rectangle((int)rect.X,
                                         (int)rect.Y,
                                         healthTexture.Width/2,
                                         healthTexture.Height/2);

                game.spriteBatch.Draw(healthTexture, healthRectangle, Color.Gray);
                //
                float healthPercentage = this.HealthPercentage;
                float visibleWidth = (float)healthTexture.Width * healthPercentage;

                healthRectangle = new Rectangle((int)rect.X,
                               (int)rect.Y,
                               (int)(visibleWidth/2),
                               healthTexture.Height/2);
                game.spriteBatch.Draw(healthTexture, healthRectangle, Color.Red);

            
            }
                
            game.spriteBatch.End();
            game.GraphicsDevice.BlendState = BlendState.Opaque;
            game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            game.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;



        }


        #endregion




        private static float WrapAngle(float radians)
        {
            while (radians < -MathHelper.Pi)
            {
                radians += MathHelper.TwoPi;
            }
            while (radians > MathHelper.Pi)
            {
                radians -= MathHelper.TwoPi;
            }
            return radians;
        }

        private static float TurnToFace(Vector3 position, Vector3 faceThis,
            float currentAngle, float turnSpeed)
        {
            
            float x = faceThis.X - position.X;
            float z = faceThis.Z - position.Z;

            float desiredAngle = (float)Math.Atan2(x, z);

            float difference = WrapAngle(desiredAngle - currentAngle);

            // clamp that between -turnSpeed and turnSpeed.
            difference = MathHelper.Clamp(difference, -turnSpeed, turnSpeed);


            return WrapAngle(currentAngle + difference);
        }

        private float FindMaxMoveSpeed(Vector3 waypoint)
        {
            float finalSpeed = TankVelocity;

            float turningRadius = TankVelocity / TankTurnSpeed;

            Vector3 orth = new Vector3(this.orientation.Forward.X, 0, this.orientation.Forward.Z);

           
            float closestDistance = Math.Min(
                Vector3.Distance(waypoint, position + (orth * turningRadius)),
                Vector3.Distance(waypoint, position - (orth * turningRadius)));


          
            if (closestDistance < turningRadius)
            {
             
                float radius = Vector3.Distance(position, waypoint) / 2;

                finalSpeed = TankTurnSpeed * radius;
            }

            return finalSpeed;
        }
    }
}
