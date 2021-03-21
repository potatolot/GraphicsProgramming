using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GraphicsProgramming
{
	class Lesson2 : Lesson
	{ 

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct VertexPositionColorNormal : IVertexType
		{
			public Vector3 Position;
			public Color Color;
			public Vector3 Normal;
			public Vector2 Texture;

			static readonly VertexDeclaration _vertexDeclaration = new VertexDeclaration
			(
				new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
				new VertexElement(12, VertexElementFormat.Color, VertexElementUsage.Color, 0),
				new VertexElement(16, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
				new VertexElement(28, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
			);

			VertexDeclaration IVertexType.VertexDeclaration => _vertexDeclaration;

			public VertexPositionColorNormal(Vector3 position, Color color, Vector3 normal, Vector2 texture)
			{
				Position = position;
				Color = color;
				Normal = normal;
				Texture = texture;
			}
		}

		private VertexPositionColorNormal[] vertices = {
			//FRONT
			new VertexPositionColorNormal( new Vector3(-1f, 1f, 1f), Color.Red, Vector3.Backward, new Vector2(0,1) ),
			new VertexPositionColorNormal( new Vector3(1f, -1f, 1f), Color.Red, Vector3.Backward, new Vector2(1,0) ),
			new VertexPositionColorNormal( new Vector3(-1f, -1f, 1f), Color.Red, Vector3.Backward, new Vector2(0,0) ),
			new VertexPositionColorNormal( new Vector3(1f, 1f, 1f), Color.Red, Vector3.Backward, new Vector2(1,1) ),

			//BACK
			new VertexPositionColorNormal( new Vector3(-1f, 1f, -1f), Color.Green, Vector3.Forward, new Vector2(0,0) ),
			new VertexPositionColorNormal( new Vector3(1f, -1f, -1f), Color.Green, Vector3.Forward, new Vector2(1,1) ),
			new VertexPositionColorNormal( new Vector3(-1f, -1f, -1f), Color.Green, Vector3.Forward, new Vector2(0,1) ),
			new VertexPositionColorNormal( new Vector3(1f, 1f, -1f), Color.Green, Vector3.Forward, new Vector2(1,0) ),

			//LEFT
			new VertexPositionColorNormal( new Vector3(-1f, 1f, -1f), Color.Blue, Vector3.Left, new Vector2(0,0) ),
			new VertexPositionColorNormal( new Vector3(-1f, -1f, 1f), Color.Blue, Vector3.Left, new Vector2(1,1) ),
			new VertexPositionColorNormal( new Vector3(-1f, -1f, -1f), Color.Blue, Vector3.Left, new Vector2(1,0) ),
			new VertexPositionColorNormal( new Vector3(-1f, 1f, 1f), Color.Blue, Vector3.Left, new Vector2(0,1) ),

			//RIGHT
			new VertexPositionColorNormal( new Vector3(1f, 1f, -1f), Color.Cyan, Vector3.Right, new Vector2(1,0) ),
			new VertexPositionColorNormal( new Vector3(1f, -1f, 1f), Color.Cyan, Vector3.Right, new Vector2(0,1) ),
			new VertexPositionColorNormal( new Vector3(1f, -1f, -1f), Color.Cyan, Vector3.Right, new Vector2(0,0) ),
			new VertexPositionColorNormal( new Vector3(1f, 1f, 1f), Color.Cyan, Vector3.Right, new Vector2(1,1) ),

			//TOP
			new VertexPositionColorNormal( new Vector3(-1f, 1f, 1f), Color.Magenta, Vector3.Up, new Vector2(1,1) ),
			new VertexPositionColorNormal( new Vector3(1f, 1f, -1f), Color.Magenta, Vector3.Up, new Vector2(0,0) ),
			new VertexPositionColorNormal( new Vector3(-1f, 1f, -1f), Color.Magenta, Vector3.Up, new Vector2(1,0) ),
			new VertexPositionColorNormal( new Vector3(1f, 1f, 1f), Color.Magenta, Vector3.Up, new Vector2(0,1) ),

			//BOTTOM
			new VertexPositionColorNormal( new Vector3(-1f, -1f, 1f), Color.Yellow, Vector3.Down, new Vector2(0,1) ),
			new VertexPositionColorNormal( new Vector3(1f, -1f, -1f), Color.Yellow, Vector3.Down, new Vector2(1,0) ),
			new VertexPositionColorNormal( new Vector3(-1f, -1f, -1f), Color.Yellow, Vector3.Down, new Vector2(0,0) ),
			new VertexPositionColorNormal( new Vector3(1f, -1f, 1f), Color.Yellow, Vector3.Down, new Vector2(1,1) ),
		};

		private int[] indices = {
			//FRONT
			//triangle 1
			0, 1, 2,
			//triangle 2
			0, 3, 1,
			
			//BACK
			//triangle 1
			4, 6, 5,
			//triangle 2
			4, 5, 7,
			
			//LEFT
			//triangle 1
			8, 9, 10,
			//triangle 2
			8, 11, 9,

			//RIGHT
			//triangle 1
			12, 14, 13,
			//triangle 2
			12, 13, 15,

			//TOP
			//triangle 1
			16, 18, 17,
			//triangle 2
			16, 17, 19,

			//BOTTOM
			//triangle 1
			20, 21, 22,
			//triangle 2
			20, 23, 21
		};

		//private BasicEffect effect;
		private Effect myEffect;
		private Texture2D crateTexture;
		private Texture2D normalTexture;
		private Texture2D specularTexture;
		Vector3[] LightPositions = new Vector3[2] { new Vector3(0, 26, 6), new Vector3(5, 0, 5) };
		Vector4[] LightColors = new Vector4[2] { new Vector4(0.8f, 0.14f, 0.5f, 1f), new Vector4(0.01f, 0.5f, 0.5f, 1f)};
		float[] LightSpeeds = new float[2] { 1, 6 };
		

		public override void LoadContent(ContentManager Content, GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
		{
		//	effect = new BasicEffect(graphics.GraphicsDevice);
			myEffect = Content.Load<Effect>("Effects/Lesson2");
			crateTexture = Content.Load<Texture2D>("crate");
			normalTexture = Content.Load<Texture2D>("NormalMap");
			specularTexture = Content.Load<Texture2D>("SpecularMap");
		}

		public override void Draw(GameTime gameTime, GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
		{
			GraphicsDevice device = graphics.GraphicsDevice;


			/*for (int i = 0; i < LightPositions.Length; i++)
            {
				

				LightPositions[i].X += MathF.Cos(2 * MathF.PI * (float)gameTime.TotalGameTime.TotalSeconds * 0.1f) * LightSpeeds[i];
				LightPositions[i].Z += MathF.Sin(2 * MathF.PI * (float)gameTime.TotalGameTime.TotalSeconds * 0.1f) * LightSpeeds[i];
				LightPositions[i].Y = 0;
			}*/
			

			Vector3 cameraPos = -Vector3.Forward * 5 + Vector3.Up * 2.5f + Vector3.Right * 2.5f;

			Matrix World = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);
			Matrix View = Matrix.CreateLookAt(cameraPos, Vector3.Zero, Vector3.Up);

			myEffect.Parameters["World"].SetValue(World);
			myEffect.Parameters["View"].SetValue(View);
			myEffect.Parameters["Projection"].SetValue(Matrix.CreatePerspective((MathF.PI / 180f) * 85f, graphics.PreferredBackBufferWidth / graphics.PreferredBackBufferHeight, 1f, 100f));

			//myEffect.Parameters["SpecularTex"].SetValue(specularTexture);
			myEffect.Parameters["MainTex"].SetValue(crateTexture);
			myEffect.Parameters["NormalTex"].SetValue(normalTexture);

			myEffect.Parameters["LightPositions"].SetValue(LightPositions);
			myEffect.Parameters["LightColors"].SetValue(LightColors);
			myEffect.Parameters["CameraPosition"].SetValue(cameraPos);

			myEffect.CurrentTechnique.Passes[0].Apply();

			device.Clear(Color.Black);
			device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, indices.Length / 3);
		}
	}
}