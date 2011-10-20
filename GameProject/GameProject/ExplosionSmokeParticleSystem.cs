#region File Description
//-----------------------------------------------------------------------------
// ExplosionSmokeParticleSystem.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion


namespace GameProject
{
    /// <summary>
    /// Smoke explosions.
    /// </summary>
    public class ExplosionSmokeParticleSystem : ParticleSystem
    {
        public ExplosionSmokeParticleSystem(Game1 game, int howManyEffects)
            : base(game, howManyEffects)
        {   
        }

        /// <summary>
        /// Constants
        /// </summary>
        protected override void InitializeConstants()
        {
            textureFilename = "smoke";

            // Slower initial speed than explosions
            minInitialSpeed = 20;
            maxInitialSpeed = 200;

            minAcceleration = -10;
            maxAcceleration = -50;

            // The smoke lingers after the explosion is gone
            minLifetime = 1.0f;
            maxLifetime = 2.5f;

            minScale = 1.0f;
            maxScale = 2.0f;

            minNumParticles = 10;
            maxNumParticles = 20;

            minRotationSpeed = -MathHelper.PiOver4;
            maxRotationSpeed = MathHelper.PiOver4;

			blendState = BlendState.AlphaBlend;

            DrawOrder = AlphaBlendDrawOrder;
        }
    }
}
