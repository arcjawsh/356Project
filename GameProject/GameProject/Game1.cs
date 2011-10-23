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
        Skybox skybox;
        Powerup power;
        Boolean randomBool = false;
        SpriteFont font;
        public List<Tank> tanks = new List<Tank>();
        public List<Powerup> powerups = new List<Powerup>();
        public List<Obstacle> obstacles = new List<Obstacle>();
        public Predicate<Powerup> deletePowerups;
        public Predicate<Tank> deleteTanks;
        public static Random random = new Random();
        public static Random random2 = new Random(34);
        public int RandomNumber(int min, int max)
        {
            if (randomBool == true)
            {
                randomBool = false;
                return random.Next(min, max);
            }
            else
            {
                randomBool = true;
                return random2.Next(min, max);
            }
        }

        public int tanksCount = 0;
        public Camera camera;



        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
      
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
  
            box = new SelectionBox(this);
            camera = new Camera(this);
            Components.Add(box);
            //this.IsMouseVisible = true;
            base.Initialize();
            deletePowerups = new Predicate<Powerup>(IsPowerupDead);
            deleteTanks = new Predicate<Tank>(IsTankDead);
        }

    
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            music = Content.Load<Song>("backgroundMusic");
            font = Content.Load<SpriteFont>("font");
            skybox = new Skybox("Sunset", Content);
            //terrain = Content.Load<Model>("terrain");
            terrain = Content.Load<Model>("heightmap8");
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
            camera.LoadContent();

            box.setHM(heightMapInfo);

            setTanks();
            setPowerups();
            setObstacles();


        }

        public void setObstacles()
        {
            Obstacle obs = new Obstacle(this, heightMapInfo, new Vector3(500, 0, -500));
            //Obstacle obs2 = new Obstacle(this, heightMapInfo, new Vector3(-4000, 0, 4000));
            Obstacle obs3 = new Obstacle(this, heightMapInfo, new Vector3(4000, 0, 4000));
            Obstacle obs4 = new Obstacle(this, heightMapInfo, new Vector3(4000, 0, -4000));
            Obstacle obs5 = new Obstacle(this, heightMapInfo, new Vector3(-4000, 0, -4000));
            obstacles.Add(obs);
            //obstacles.Add(obs2);
            obstacles.Add(obs3);
            obstacles.Add(obs4);
            obstacles.Add(obs5);

            
        }


        public void setTanks()
        {
            Tank tank1 = new Tank(this, heightMapInfo, Vector3.Zero); //create a tank
            tanks.Add(tank1);

            ++tanksCount;
  
            for (int i = 0; i < 5; i++)
            {
                Tank t = new Tank(this, heightMapInfo, new Vector3(RandomNumber(-6000, 6000), 0, RandomNumber(-6000, 6000)));
                tanks.Add(t);
                tanksCount++;
            }

            foreach (Tank tank in tanks)
            {
                tank.LoadContent(Content);
            }
        }

        public void setPowerups()
        {
            
            for (int i = 0; i < 8; i++)
            {
   
                Powerup power = new Powerup(this, heightMapInfo);
                power.LoadContent();
                powerups.Add(power);
            }
        }

        public bool IsPowerupDead(Powerup p) 
        {
            return p.delete;
        }

        public bool IsTankDead(Tank t)
        {
            return t.delete;
        }

       
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

       
        protected override void Update(GameTime gameTime)
        {
            //Handle Input

                foreach (Tank tank in tanks)
                    tank.HandleInput(gameTime);
 
                foreach (Powerup p in powerups)
                {
                    p.Update(gameTime);
                }

                //
            foreach(Obstacle ob in obstacles)
                ob.Update();
                //

                powerups.RemoveAll(deletePowerups);
                tanks.RemoveAll(deleteTanks);

                if(tanks.Count < tanksCount)
                {
                    Tank t = new Tank(this, heightMapInfo, new Vector3(RandomNumber(-6000, 6000), 0, RandomNumber(-6000, 6000)));
                    tanks.Add(t);
                    t.LoadContent(Content);
                    System.Diagnostics.Debug.Write(" adding new tank");
                    //add new tank / tanks depending
                }

            //if user presses T, make new tank
              KeyboardState currentKeyboardState = Keyboard.GetState();
              if (currentKeyboardState.IsKeyDown(Keys.T))
                  ++tanksCount;

     
  

            camera.Update(gameTime);
            base.Update(gameTime);
        }

  
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
      
            
            //skybox
            RasterizerState originalRasterizerState = graphics.GraphicsDevice.RasterizerState;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            graphics.GraphicsDevice.RasterizerState = rasterizerState;
            skybox.Draw(camera.viewMatrix, camera.projectionMatrix, new Vector3(0, 500, 0));//camera.position);
            graphics.GraphicsDevice.RasterizerState = originalRasterizerState;
            ///
            DrawModel(terrain); //draws the terrain
            //
            foreach (Obstacle ob in obstacles)
                ob.Draw();
                //
            if (tanks.Count != 0)
            {
                foreach (Tank tank in tanks)
                    tank.Draw(camera.viewMatrix, camera.projectionMatrix);
            }
            if (powerups.Count != 0)
            {
                foreach (Powerup p in powerups)
                    p.Draw(gameTime);
            }
            //each powerup in powerups
            camera.Draw();
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "Current Tanks in list: " + tanks.Count.ToString(), new Vector2(10, 10), Color.CornflowerBlue);
            spriteBatch.DrawString(font, "Current Tanks count: " + tanksCount.ToString(), new Vector2(10, 28), Color.CornflowerBlue);
            spriteBatch.End();
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
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
                    //effect.FogColor = Vector3.Zero;
                    effect.FogStart = 6000;
                    effect.FogEnd = 9000;
                }

                mesh.Draw();
            }
        }

       
    }
}
