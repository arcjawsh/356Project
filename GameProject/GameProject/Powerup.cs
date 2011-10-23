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
using GameProject;


namespace GameProject
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Powerup
    {
        public Vector3 position;
        HeightMapInfo heightmap;
        BoundingSphere sphere;
        Model model;
        SoundEffect effect;
        public Boolean delete = false;
        enum Type
        {
            health,
            power,
            armor
        }
        Game1 game; ///pointer to current game

        int value; //value of power up +

        Type type;


        public Powerup(Game1 game, HeightMapInfo h)
        {
            this.game = game;
            int rand = game.RandomNumber(0, 2);
            if (rand == 0)
            {
                type = Type.health;
                value = 20;
            }
            else if (rand == 1)
            {
                type = Type.power;
                value = 40;
            }
            heightmap = h;
            //int x = xclass.Next(-2000, 2000);
            //int z = yclass.Next(-2000, 2000);
            position = new Vector3(game.RandomNumber(-6000, 6000), 0, game.RandomNumber(-6000, 6000));
            //position = new Vector3(-700, 0, 0);

 
        }

        public void LoadContent()
        {
            if (type == Type.health)
            {
                //load health models
            }
            if (type == Type.power)
            {
                //load power model
            }

            model = game.Content.Load<Model>("sphere");
            effect = game.Content.Load<SoundEffect>("beep");
            //load health power
            //load
        }

        public void Update(GameTime gameTime)
        {
            //Boolean delete = false;
            //create bounding sphere, check it
            //spheree = new BoundingSphere(position, 0.5f);
            //if not on heightmap, move down
            if (heightmap.IsOnHeightmap(position))
            {
                Vector3 normal;
                heightmap.GetHeightAndNormal(position,
                    out position.Y, out normal);
                position.Y += 30f;
            }
            checkCollision();
       
        }

        public void checkCollision()
        {
            
          
            foreach (Tank tank in game.tanks)
            {
                if (sphere.Intersects(tank.sphere))
                {
                    effect.Play();
                    if (type == Type.health)
                        tank.currentHealth += value;
                    else if (type == Type.power)
                        tank.currentHealth += value;

                    delete = true; ;
                }

            }
        }

        public void Draw(GameTime gameTime)
        {
            //draw model and bounding sphere
   
            Matrix worldMatrix2 = Matrix.CreateScale(5f, 5f, 5f) * Matrix.Identity * Matrix.CreateTranslation(position);
            Matrix[] boneTransforms = new Matrix[model.Bones.Count];
           model.CopyAbsoluteBoneTransformsTo(boneTransforms);

           DepthStencilState ss = new DepthStencilState();
           ss.DepthBufferEnable = true;
           
           game.GraphicsDevice.DepthStencilState = ss;
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    
                    //game.GraphicsDevice.DepthStencilState.DepthBufferEnable = true;
                    effect.World = boneTransforms[mesh.ParentBone.Index] * worldMatrix2;
                   effect.View = this.game.camera.viewMatrix;
                    effect.Projection = this.game.camera.projectionMatrix;
                    
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;

                    // Set the fog to match the black background color
                    //effect.FogEnabled = true;
                    //effect.FogColor = Vector3.Zero;
                    //effect.FogStart = 1000;
                    //effect.FogEnd = 3200;
                }
                mesh.Draw();

                ss = new DepthStencilState();
                ss.DepthBufferEnable = false; ;
                game.GraphicsDevice.DepthStencilState = ss;
                //BoundingSphere sphere = T

                sphere = new BoundingSphere(position, 70f); //position and radius
                BoundingSphereRenderer.Draw(sphere, game.camera.viewMatrix, game.camera.projectionMatrix);
            }
        }
    }
}
