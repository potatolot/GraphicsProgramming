using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphicsProgramming
{
    class Lesson1 : Lesson
    {
        VertexPositionColor[] vertices =
        {
            new VertexPositionColor(new Vector3(0.5f, 0.5f, 0.5f), Color.Red),
            new VertexPositionColor(new Vector3(0.5f, -0.5f, 0.5f), Color.Green),
            new VertexPositionColor(new Vector3(-0.5f, -0.5f, 0.5f), Color.Blue),
            new VertexPositionColor(new Vector3(-0.5f, 0.5f, 0.5f), Color.Green),
            new VertexPositionColor(new Vector3(0.5f, 0.5f, -0.5f), Color.Red),
            new VertexPositionColor(new Vector3(0.5f, -0.5f, -0.5f), Color.Green),
            new VertexPositionColor(new Vector3(-0.5f, -0.5f, -0.5f), Color.Blue),
            new VertexPositionColor(new Vector3(-0.5f, 0.5f, -0.5f), Color.Green)
        };

        int[] indices =
        {
            //FRONT
            // Triangle 1
            0, 1, 2, 
            // Triangle 2
            0, 2, 3,
            // Triangle 3
            //BACK
            6, 5, 4,
            // Triangle 4
            7, 6, 4,

            //SIDE 1
            1, 0, 4,
            1, 4, 5,

            1, 5, 2,
            2, 5, 6,

            6, 7, 3,
            2, 6, 3, 

            7, 4, 3, 
            4, 0, 3
        };

        BasicEffect effect;

        public override void LoadContent(ContentManager Content, GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
        {
            effect = new BasicEffect(graphics.GraphicsDevice);
        }

        public override void Draw(GameTime gameTime, GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
        {
            GraphicsDevice device = graphics.GraphicsDevice;

            // World matrix]
            effect.World = Matrix.Identity * Matrix.CreateRotationX(0.5f * (float)gameTime.TotalGameTime.TotalSeconds) * Matrix.CreateRotationY(0.5f * (float)gameTime.TotalGameTime.TotalSeconds);
            // View matrix
            effect.View = Matrix.CreateLookAt(-Vector3.Forward * 2, Vector3.Zero, Vector3.Up);
            // Projection matrix
            effect.Projection = Matrix.CreatePerspectiveFieldOfView((MathF.PI / 180f) * 65f, device.Viewport.AspectRatio, 0.1f, 100f);

         

            effect.VertexColorEnabled = true;
            effect.CurrentTechnique.Passes[0].Apply();

            device.Clear(Color.White);
            device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, indices.Length / 3);
        }


    }
}
