using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GraphicsProgramming
{
	class HWLesson3 : Lesson
	{
		//private BasicEffect effect;
		private Effect myEffect;

		Vector3 sunPosition = new Vector3(0, 26, 6);
		Vector3 sunColor = new Vector3(1, 1, 1);

		float cameraOffset = 0;
		float previousOffset = 0;

		Vector3 cameraPos = -Vector3.Forward * 5;

		Model sphere, cube;

		// Earth textures
		Texture2D dayEarth, nightEarth, cloudsEarth;
		TextureCube sky;

		Texture2D moon;

		// Get sun texture
		Texture2D sun;

		// Get all day textures where mercury is 0 in list and neptune is 6 (Skipping earth)
		Texture2D[] planet = new Texture2D[7];

		float yaw, pitch;
		int prevX, prevY;

		public override void LoadContent(ContentManager Content, GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
		{
			myEffect = Content.Load<Effect>("Effects/HWLesson3");

			sphere = Content.Load<Model>("Lesson3Models/uv_sphere");
			moon = Content.Load<Texture2D>("Lesson3Models/2k_moon");
			cube = Content.Load<Model>("Lesson3Models/cube");
			sky = Content.Load<TextureCube>("Lesson3Textures/Universe");

			// Load in Textures
			sun = Content.Load<Texture2D>("Lesson3Textures/sun");

			dayEarth = Content.Load<Texture2D>("Lesson3Textures/day");
			nightEarth = Content.Load<Texture2D>("Lesson3Textures/night");
			cloudsEarth = Content.Load<Texture2D>("Lesson3Textures/clouds");

			// Load in planetTextures
            for (int i = 0; i < planet.Length; i++)
            {
				planet[i] = Content.Load<Texture2D>("Lesson3Textures/Planet" + i.ToString());
			}         


			foreach (ModelMesh mesh in sphere.Meshes)
			{
				foreach (ModelMeshPart meshPart in mesh.MeshParts)
				{
					meshPart.Effect = myEffect;
				}
			}

			foreach (ModelMesh mesh in cube.Meshes)
			{
				foreach (ModelMeshPart meshPart in mesh.MeshParts)
				{
					meshPart.Effect = myEffect;
				}
			}

		}

		public override void Update(GameTime gameTime)
		{
			MouseState mState = Mouse.GetState();

			if (mState.RightButton == ButtonState.Pressed)
			{
				// Update yaw and pitch
				yaw += (prevX - mState.X) * 0.01f;
				pitch += (prevY - mState.Y) * 0.01f;

				pitch = MathF.Min(MathF.Max(pitch, -MathF.PI * 0.45f), MathF.PI * 0.45f);
			}

			if (Mouse.GetState().ScrollWheelValue < previousOffset)
			{
				cameraOffset = Math.Clamp(cameraOffset + 1, -10, 20);
			}
			else if (Mouse.GetState().ScrollWheelValue > previousOffset)
			{
				cameraOffset = Math.Clamp(cameraOffset - 1, -10, 20);
			}
			previousOffset = Mouse.GetState().ScrollWheelValue;

			prevX = mState.X;
			prevY = mState.Y;
		}

		void RenderModel(Model m, Matrix parentMatrix)
		{
			Matrix[] transforms = new Matrix[m.Bones.Count];
			m.CopyAbsoluteBoneTransformsTo(transforms);

			myEffect.CurrentTechnique.Passes[0].Apply();

			foreach (ModelMesh mesh in m.Meshes)
			{
				myEffect.Parameters["World"].SetValue(parentMatrix * transforms[mesh.ParentBone.Index]);

				mesh.Draw();
			}


		}

		public override void Draw(GameTime gameTime, GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
		{
			GraphicsDevice device = graphics.GraphicsDevice;

			Vector3 cameraPos = -Vector3.Forward * (15 + cameraOffset);// * 5 + Vector3.Up * 2.5f + Vector3.Right * 2.5f;
			cameraPos = Vector3.Transform(cameraPos, Quaternion.CreateFromYawPitchRoll(yaw, pitch, 0));

			Matrix World = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);
			Matrix View = Matrix.CreateLookAt(cameraPos, Vector3.Zero, Vector3.Up);

			float time = (float)gameTime.TotalGameTime.TotalSeconds;
			sunPosition = new Vector3(0, 0, 0);

			myEffect.Parameters["World"].SetValue(World);
			myEffect.Parameters["View"].SetValue(View);
			myEffect.Parameters["Projection"].SetValue(Matrix.CreatePerspective((MathF.PI / 180f) * 85f, graphics.PreferredBackBufferWidth / graphics.PreferredBackBufferHeight, 1f, 1000f));

			myEffect.Parameters["Time"].SetValue((float)gameTime.TotalGameTime.TotalSeconds);

			// sky and sun
			myEffect.Parameters["SkyTex"].SetValue(sky);
			myEffect.Parameters["SunTex"].SetValue(sun);
			myEffect.Parameters["MoonTex"].SetValue(moon);

			// earth
			myEffect.Parameters["DayTex"].SetValue(dayEarth);
			myEffect.Parameters["NightTex"].SetValue(nightEarth);
			myEffect.Parameters["CloudsTex"].SetValue(cloudsEarth);


			myEffect.Parameters["LightPosition"].SetValue(sunPosition);
			myEffect.Parameters["LightColor"].SetValue(sunColor);
			myEffect.Parameters["CameraPosition"].SetValue(cameraPos);

			myEffect.CurrentTechnique.Passes[0].Apply();

			device.Clear(Color.Black);

			//Sky
			myEffect.CurrentTechnique = myEffect.Techniques["Sky"];
			device.DepthStencilState = DepthStencilState.None;
			device.RasterizerState = RasterizerState.CullNone;
			RenderModel(cube, Matrix.CreateScale(1.5f) * Matrix.CreateTranslation(cameraPos));
			device.RasterizerState = RasterizerState.CullCounterClockwise;
			device.DepthStencilState = DepthStencilState.Default;

			myEffect.CurrentTechnique = myEffect.Techniques["Sun"];
			// Sun
			RenderModel(sphere, Matrix.CreateScale(0.01f) * World);

			myEffect.CurrentTechnique = myEffect.Techniques["Earth"];
			// Earth
			RenderModel(sphere, Matrix.CreateScale(0.0023f) * Matrix.CreateRotationZ(time * 0.5f) * Matrix.CreateTranslation(Vector3.Down * 0.04f)  * Matrix.CreateRotationZ(time - time * 0.33333f) * World);
			//RenderModel(sphere, Matrix.CreateScale(0.01f) * Matrix.CreateRotationZ(time * 0.5f) * World);

			// Mercury
			myEffect.Parameters["PlanetTex"].SetValue(planet[0]);
			myEffect.CurrentTechnique = myEffect.Techniques["Planet"];
			RenderModel(sphere, Matrix.CreateRotationZ(time * 0.5f) * Matrix.CreateTranslation(Vector3.Down * 20) * Matrix.CreateScale(0.0009f) * Matrix.CreateRotationZ(time - time * 0.233333f) * World);

			// Venus
			myEffect.Parameters["PlanetTex"].SetValue(planet[1]);
			myEffect.CurrentTechnique = myEffect.Techniques["Planet"];
			RenderModel(sphere, Matrix.CreateRotationZ(time * 0.5f) * Matrix.CreateTranslation(Vector3.Down * 20) * Matrix.CreateScale(0.0013f) * Matrix.CreateRotationZ(time - time * 0.15f) * World);

			// Mars
			myEffect.Parameters["PlanetTex"].SetValue(planet[2]);
			myEffect.CurrentTechnique = myEffect.Techniques["Planet"];
			RenderModel(sphere, Matrix.CreateRotationZ(time * 0.5f) * Matrix.CreateTranslation(Vector3.Down * 52) * Matrix.CreateScale(0.0009f) * Matrix.CreateRotationZ(time - time * 0.433333f) * World);

			// Jupiter
			myEffect.Parameters["PlanetTex"].SetValue(planet[3]);
			myEffect.CurrentTechnique = myEffect.Techniques["Planet"];
			RenderModel(sphere, Matrix.CreateRotationZ(time * 0.5f) * Matrix.CreateTranslation(Vector3.Down * 6) * Matrix.CreateScale(0.013f) * Matrix.CreateRotationZ(time - time * 0.533333f) * World);

			// Saturn
			myEffect.Parameters["PlanetTex"].SetValue(planet[4]);
			myEffect.CurrentTechnique = myEffect.Techniques["Planet"];
			RenderModel(sphere, Matrix.CreateRotationZ(time * 0.5f) * Matrix.CreateTranslation(Vector3.Down * 9) * Matrix.CreateScale(0.013f) * Matrix.CreateRotationZ(time - time * 0.54f) * World);

			// Uranus
			myEffect.Parameters["PlanetTex"].SetValue(planet[5]);
			myEffect.CurrentTechnique = myEffect.Techniques["Planet"];
			RenderModel(sphere, Matrix.CreateRotationZ(time * 0.5f) * Matrix.CreateTranslation(Vector3.Down * 18) * Matrix.CreateScale(0.0083f) * Matrix.CreateRotationZ(time - time * 0.6f) * World);

			// Neptune
			myEffect.Parameters["PlanetTex"].SetValue(planet[5]);
			myEffect.CurrentTechnique = myEffect.Techniques["Planet"];
			RenderModel(sphere, Matrix.CreateRotationZ(time * 0.5f) * Matrix.CreateTranslation(Vector3.Down * 22) * Matrix.CreateScale(0.0083f) * Matrix.CreateRotationZ(time - time * 0.7f) * World);


		}
	}
}