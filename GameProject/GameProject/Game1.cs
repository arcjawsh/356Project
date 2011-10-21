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
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        Model terrain;
        HeightMapInfo heightMapInfo;
        SelectionBox box;
        Song music;

        Tank tank;

        public Camera camera;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            box = new SelectionBox(this);
            tank = new Tank(); //create a tank
            Components.Add(box);
        }

       
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            camera = new Camera(this,new Vector3(0, 20, 0),
            new Vector3(0, 0, 0), Vector3.Up);
            this.IsMouseVisible = true;

            base.Initialize();
        }

       
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            music = Content.Load<Song>("backgroundMusic");
            terrain = Content.Load<Model>("terrain");
            // The terrain processor attached a HeightMapInfo to the terrain model's
           
            heightMapInfo = terrain.Tag as HeightMapInfo;            if (heightMapInfo == null)
            {
                string message = "The terrain model did not have a HeightMapInfo " +
                    "object attached. Are you sure you are using the " +
                    "TerrainProcessor?";
                throw new InvalidOperationException(message);
            }
            MediaPlayer.Play(music);
            MediaPlayer.IsRepeating = true; //loop song
            tank.LoadContent(Content);
            
        }

       
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

       
        protected override void Update(GameTime gameTime)
        {
            //Handle Input
            camera.Update(gameTime);//Update Camera
            tank.HandleInput(heightMapInfo);
            base.Update(gameTime);
        }

  
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            //DrawModel(terrain); //draws the terrain
            DrawModel(terrain); //draws the terrain
            tank.Draw(camera.viewMatrix, camera.projectionMatrix);
            base.Draw(gameTime);
        }

       


        void DrawModel(Model model)
        { //common function for drawing a model
            Matrix[] boneTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(boneTransforms);

            
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = boneTransforms[mesh.ParentBone.Index];
                    effect.View = camera.viewMatrix;
                    effect.Projection = camera.projectionMatrix;
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

       
    }
}
