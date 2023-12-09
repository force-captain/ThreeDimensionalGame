using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ThreeDimensionalGame
{
    internal abstract class GameObject
    {
        internal Vector3 position;
        internal Vector3 velocity;
        internal Vector3 acceleration;

        // Yaw/pitch/roll
        internal Vector3 rotation;

        internal Vector3 rotationalVelocity;

        internal void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            position += velocity * deltaTime;
            velocity += acceleration * deltaTime;
            rotation += rotationalVelocity * deltaTime;
        }
    }

    internal abstract class Object3D : GameObject
    {
        internal VertexBuffer buffer;
        internal BasicEffect effect;

        internal Object3D(GraphicsDevice gDevice)
        {
            effect = new BasicEffect(gDevice);
        }
    }

    internal class Line : Object3D
    {
        internal Color color;
        internal VertexPositionColorNormal[] vertices;

        internal Line(GraphicsDevice gDevice, Vector3 p1, Vector3 p2, Color color) : base(gDevice)
        {
            
            buffer = new VertexBuffer(gDevice, typeof(VertexPositionColorNormal), 2, BufferUsage.None);
            effect.TextureEnabled = false;
            effect.VertexColorEnabled = true;

            Vector3 p12 = p2 - p1;
            vertices = new VertexPositionColorNormal[2]
            {
                new VertexPositionColorNormal(p1, color, Vector3.Normalize(-p12)),
                new VertexPositionColorNormal(p2, color, Vector3.Normalize(p12))
            };

            buffer.SetData(vertices);
        }

        internal void Draw(Effect effect)
        {
            GraphicsDevice gDevice = effect.GraphicsDevice;
            gDevice.SetVertexBuffer(buffer);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                gDevice.DrawPrimitives(PrimitiveType.LineList, 0, 2);
            }
        }

        internal void Draw(Matrix world, Matrix view, Matrix projection)
        {
            effect.World = world;
            effect.View = view;
            effect.Projection = projection;

            GraphicsDevice gDevice = effect.GraphicsDevice;
            gDevice.DepthStencilState = DepthStencilState.Default;

            Draw(effect);
        }
    }

    internal class ModelObject : GameObject
    {
        internal Model model;
        internal float scaleFactor;
        internal Vector3 originVector;
        internal Matrix rotationMatrix;

        internal ModelObject(Model model)
        {
            this.model = model;
        }

        internal void Draw(Matrix world, Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect bEffect in mesh.Effects)
                {
                    bEffect.World = Matrix.CreateScale(scaleFactor) * rotationMatrix * world;
                    bEffect.View = view;
                    bEffect.Projection = projection;
                }
                mesh.Draw();
            }
        }

        internal new void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Vector3 normalisedVelocity = Vector3.Normalize(velocity);

            rotation.Z = MathHelper.ToDegrees((float)Math.Asin(normalisedVelocity.Y));
            //rotation.Z = MathHelper.ToDegrees((float)Math.Atan2(normalisedVelocity.X, normalisedVelocity.Z));

            rotationMatrix = Matrix.CreateFromYawPitchRoll(
                MathHelper.ToRadians(rotation.X),
                MathHelper.ToRadians(rotation.Y),
                MathHelper.ToRadians(rotation.Z));

            originVector = velocity;
        }
    }


    internal abstract class TexturedObject : Object3D
    {
        internal Texture2D texture;
        internal VertexPositionNormalTexture[] vertices;

        internal TexturedObject(GraphicsDevice gDevice, Texture2D texture) : base(gDevice)
        {
            this.texture = texture;
            effect.TextureEnabled = true;
            effect.VertexColorEnabled = false;
            effect.Texture = texture;
        }

        internal void Draw(Effect effect)
        {
            GraphicsDevice gDevice = effect.GraphicsDevice;
            gDevice.SetVertexBuffer(buffer);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                gDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, vertices.Length);
            }
        }

        internal void Draw(Matrix world, Matrix view, Matrix projection)
        {
            effect.World = world;
            effect.View = view;
            effect.Projection = projection;

            GraphicsDevice gDevice = effect.GraphicsDevice;
            gDevice.DepthStencilState = DepthStencilState.Default;

            Draw(effect);
        }
    }

    internal class TexturedRectangle : TexturedObject
    {
        internal TexturedRectangle(GraphicsDevice gDevice, Texture2D texture, Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4) : base(gDevice, texture)
        {
            buffer = new VertexBuffer(gDevice, typeof(VertexPositionNormalTexture), 6, BufferUsage.None);

            Vector3 p12 = p2 - p1;
            Vector3 p13 = p3 - p1;
            Vector3 p24 = p4 - p2;
            Vector3 p34 = p4 - p3;
            vertices = new VertexPositionNormalTexture[6]
            {

                new VertexPositionNormalTexture(p1, Vector3.Normalize(Vector3.Cross(p12, p13)), Vector2.Zero),
                new VertexPositionNormalTexture(p2, Vector3.Normalize(Vector3.Cross(p12, p24)), Vector2.UnitX),
                new VertexPositionNormalTexture(p3, Vector3.Normalize(Vector3.Cross(p13, p34)), Vector2.UnitY),
                new VertexPositionNormalTexture(p2, Vector3.Normalize(Vector3.Cross(p12, p24)), Vector2.UnitX),
                new VertexPositionNormalTexture(p3, Vector3.Normalize(Vector3.Cross(p13, p34)), Vector2.UnitY),
                new VertexPositionNormalTexture(p4, Vector3.Normalize(Vector3.Cross(p24, p34)), Vector2.One)
            };

            buffer.SetData(vertices);
        }   
    }
}
