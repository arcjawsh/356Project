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
    public class Obstacle
    {
        Game1 game;
        Model model;
        Vector3 position;
        HeightMapInfo heightMapInfo;
        BoundingSphere sphere;

        public Obstacle(Game1 g, HeightMapInfo h, Vector3 pos)
        { //load content
            game = g;
            heightMapInfo = h;
            model = game.Content.Load<Model>("obstacle");
            position = pos;

        }

        public void Update()
        {
            if (heightMapInfo.IsOnHeightmap(position))
            {
                Vector3 normal;
                heightMapInfo.GetHeightAndNormal(position,
                    out position.Y, out normal);
                position.Y -= 80f;
            }

            foreach (Tank t in game.tanks)
            {
                if(this.sphere.Intersects(t.sphere))
                {
                    t.explode.Play();
                    //t.isMoving = false;
                    for (int i = 0; i < 100; i++)
                        t.position += t.orientation.Forward * 4;
                    t.isMoving = false;
                }

            }

        }

        public void Draw()
        {
            Matrix[] boneTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(boneTransforms);

            //Matrix worldMatrix = Matrix.CreateScale(0.04f, 0.04f, 0.04f) *Matrix.CreateTranslation(position);
            Matrix worldMatrix = Matrix.CreateScale(0.007f, 0.007f, 0.007f) * Matrix.CreateRotationY(MathHelper.Pi) * Matrix.CreateTranslation(position);
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    //might need to scale
                    effect.World = boneTransforms[mesh.ParentBone.Index] * worldMatrix;
                    effect.View = game.camera.viewMatrix;
                    effect.Projection = game.camera.projectionMatrix;

                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                }

                mesh.Draw();

                sphere = new BoundingSphere(position + new Vector3(200, 0, 100), 490f); //position and radius
                BoundingSphereRenderer.Draw(sphere, game.camera.viewMatrix, game.camera.projectionMatrix);
            }
          }

        
    }
}
