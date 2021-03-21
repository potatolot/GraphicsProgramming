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
	class Lesson3 : Lesson
	{
		//private BasicEffect effect;
		private Effect myEffect;

		Vector3 sunPosition = new Vector3(0, 26, 6);
		Vector3 sunColor = new Vector3(1, 1, 1);

		Model sphere;
		Texture2D day, night, clouds, moon;

		float yaw, pitch;
		int prevX, prevY;

		public override void LoadContent(ContentManager Content, GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
		{
			myEffect = Content.Load<Effect>("Effects/Lesson3");

			sphere = Content.Load<Model>("Lesson3Models/uv_sphere");
			day = Content.Load<Texture2D>("Lesson3Models/day");
			night = Content.Load<Texture2D>("Lesson3Models/night");
			clouds = Content.Load<Texture2D>("Lesson3Models/clouds");
			moon = Content.Load<Texture2D>("Lesson3Models/2k_moon");


			foreach(ModelMesh mesh in sphere.Meshes)
            {
				foreach(ModelMeshPart meshPart in mesh.MeshParts)
                {
					meshPart.Effect = myEffect;
                }
            }

		}

        public override void Update(GameTime gameTime)
        {
			MouseState mState = Mouse.GetState();

			if(mState.LeftButton == ButtonState.Pressed)
            {
				// Update yaw and pitch
				yaw += (prevX - mState.X) * 0.01f;
				pitch += (prevY - mState.Y) * 0.01f;

				pitch = MathF.Min(MathF.Max(pitch, -MathF.PI * 0.45f), MathF.PI * 0.45f);
            }

			prevX = mState.X;
			prevY = mState.Y;
        }

        void RenderModel(Model m, Matrix parentMatrix)
        {
			Matrix[] transforms = new Matrix[m.Bones.Count];
			m.CopyAbsoluteBoneTransformsTo(transforms);

			myEffect.CurrentTechnique.Passes[0].Apply();

			foreach(ModelMesh mesh in m.Meshes)
            {
				myEffect.Parameters["World"].SetValue(parentMatrix * transforms[mesh.ParentBone.Index]);

				mesh.Draw();
            }

        }

		public override void Draw(GameTime gameTime, GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
		{
			GraphicsDevice device = graphics.GraphicsDevice;

			Vector3 cameraPos = -Vector3.Forward * 5;// * 5 + Vector3.Up * 2.5f + Vector3.Right * 2.5f;
			cameraPos = Vector3.Transform(cameraPos, Quaternion.CreateFromYawPitchRoll(yaw, pitch, 0));

			Matrix World = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);
			Matrix View = Matrix.CreateLookAt(cameraPos, Vector3.Zero, Vector3.Up);

			float time = (float)gameTime.TotalGameTime.TotalSeconds;
			sunPosition = new Vector3(/*MathF.Cos(time)*/-500, 0, /*MathF.Sin(time)) **/ 60);

			myEffect.Parameters["World"].SetValue(World);
			myEffect.Parameters["View"].SetValue(View);
			myEffect.Parameters["Projection"].SetValue(Matrix.CreatePerspective((MathF.PI / 180f) * 85f, device.Viewport.AspectRatio, 1f, 100f));

			myEffect.Parameters["Time"].SetValue((float)gameTime.TotalGameTime.TotalSeconds);

			myEffect.Parameters["DayTex"].SetValue(day);
			myEffect.Parameters["NightTex"].SetValue(night);
			myEffect.Parameters["CloudsTex"].SetValue(clouds);
			myEffect.Parameters["MoonTex"].SetValue(moon);

			myEffect.Parameters["LightPosition"].SetValue(sunPosition);
			myEffect.Parameters["LightColor"].SetValue(sunColor);
			myEffect.Parameters["CameraPosition"].SetValue(cameraPos);

			myEffect.CurrentTechnique.Passes[0].Apply();

			device.Clear(Color.Black);

			myEffect.CurrentTechnique = myEffect.Techniques["Earth"];
			// Earth
			RenderModel(sphere, Matrix.CreateScale(0.01f) * Matrix.CreateRotationZ(time * 0.5f) * World);

			myEffect.CurrentTechnique = myEffect.Techniques["Moon"];
			// Moon
			RenderModel(sphere, Matrix.CreateTranslation(Vector3.Down * 10) * Matrix.CreateScale(0.0023f) *  Matrix.CreateRotationZ(time - time * 0.333333f) * World);
			RenderModel(sphere, Matrix.CreateTranslation(Vector3.Up * 8) * Matrix.CreateScale(0.0023f) *  Matrix.CreateRotationX(time - time * 0.0333333f) * Matrix.CreateRotationZ(time - time * 0.0333333f) * World);
		}
	}
}