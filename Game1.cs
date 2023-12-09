using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Reflection;

namespace ThreeDimensionalGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        internal static Texture2D pixelRectangle;
        internal static Texture2D cursorTexture;
        internal static Texture2D grassTexture;
        internal static SpriteFont font;

        internal static Model beachBallModel;
        internal static Model bulletModel;

        // Used for camera zoom (i.e. sprinting)
        internal static float cameraZoomOffset;

        // True when the mouse is currently being reset
        internal static bool isMouseRepositioned;

        // The boundaries of the screen
        internal static Point screenBounds;

        // The point the camera will be facing towards
        Vector3 cameraTarget;
        // The point the camera will be based at
        Vector3 cameraPosition;

        Matrix projectionMatrix;
        Matrix viewMatrix;
        Matrix worldMatrix;

        BasicEffect effect;
        List<GameObject> objects;

        // Axis
        VertexBuffer xBuffer;
        VertexBuffer yBuffer;
        VertexBuffer zBuffer;

        VertexPositionColorNormal[] xAxis = new VertexPositionColorNormal[2] { new VertexPositionColorNormal(new Vector3(-1000, 0, 0), Color.Red, -1 * Vector3.UnitX), new VertexPositionColorNormal(new Vector3(1000, 0, 0), Color.Red, Vector3.UnitX) };
        VertexPositionColorNormal[] yAxis = new VertexPositionColorNormal[2] { new VertexPositionColorNormal(new Vector3(0, -1000, 0), Color.Green, -1 * Vector3.UnitY), new VertexPositionColorNormal(new Vector3(0, 1000, 0), Color.Green, Vector3.UnitY) };
        VertexPositionColorNormal[] zAxis = new VertexPositionColorNormal[2] { new VertexPositionColorNormal(new Vector3(0, 0, -1000), Color.Blue, -1 * Vector3.UnitZ), new VertexPositionColorNormal(new Vector3(0, 0, 1000), Color.Blue, Vector3.UnitZ) };



        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            isMouseRepositioned = false;
            cameraZoomOffset = 0f;

            objects = new List<GameObject>();

            // Player init
            Player.roll = 0f;
            Player.yaw = 0f;
            Player.pitch = 0f;

            Player.isMidAir = false;
            Player.position = Vector3.UnitY;
            Player.velocity = Vector3.Zero;
            Player.acceleration = Vector3.Zero;
            Player.moveSpeedMultiplier = 1f;

            // Full screen
            _graphics.IsFullScreen = true;
            _graphics.HardwareModeSwitch = false;
            _graphics.ApplyChanges();

            // Set bounds
            screenBounds = new Point(
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);

            // Matrices
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), GraphicsDevice.DisplayMode.AspectRatio, 0.1f, 1000f);
            viewMatrix = Matrix.CreateLookAt(cameraPosition, cameraTarget, new Vector3(0f, 1f, 0f));
            worldMatrix = Matrix.CreateWorld(cameraTarget, Vector3.Forward, Vector3.Up);

            // BasicEffect
            effect = new BasicEffect(GraphicsDevice);
            effect.Alpha = 1f;
            effect.VertexColorEnabled = true;
            effect.EnableDefaultLighting();

            // Keybinds
            Keybinds.forwardKey = Keys.W;
            Keybinds.leftKey = Keys.A;
            Keybinds.backKey = Keys.S;
            Keybinds.rightKey = Keys.D;
            Keybinds.jumpKey = Keys.Space;
            Keybinds.turnLeftKey = Keys.Left;
            Keybinds.turnRightKey = Keys.Right;
            Keybinds.sprintKey = Keys.LeftShift;

            xBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColorNormal), 2, BufferUsage.WriteOnly);
            yBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColorNormal), 2, BufferUsage.WriteOnly);
            zBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColorNormal), 2, BufferUsage.WriteOnly);


            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Sprites and fonts
            cursorTexture = Content.Load<Texture2D>("cursorTexture");
            grassTexture = Content.Load<Texture2D>("grass");
            font = Content.Load<SpriteFont>("defaultFont");

            pixelRectangle = new Texture2D(GraphicsDevice, 1, 1);
            pixelRectangle.SetData(new[] { Color.White });


            // Models
            beachBallModel = Content.Load<Model>("BeachBall\\BeachBall");
            bulletModel = Content.Load<Model>("bullet\\bullet");

            for(int i = -10; i <= 10; i++)
            { 
                for(int j = -10; j <= 10; j++)
                {
                    TexturedRectangle tileRect = new TexturedRectangle(GraphicsDevice, grassTexture, Vector3.Zero, Vector3.UnitX, Vector3.UnitZ, Vector3.UnitX + Vector3.UnitZ);
                    tileRect.position = new Vector3(i, 0, j);
                    tileRect.velocity = Vector3.Zero;
                    tileRect.acceleration = Vector3.Zero;
                    objects.Add(tileRect);
                }
            }

            // Turn off mouse visibility
            IsMouseVisible = false;

            xBuffer.SetData(xAxis);
            yBuffer.SetData(yAxis);
            zBuffer.SetData(zAxis);



            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Update the input
            Input.Update();

            // Check for movement
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Forward
            if (Input.kState.IsKeyDown(Keybinds.forwardKey))
            {
                Player.position += Player.Forward(deltaTime * Player.moveSpeedMultiplier);
            }
            // Back
            if (Input.kState.IsKeyDown(Keybinds.backKey))
            {
                Player.position += Player.Forward(deltaTime * -Player.moveSpeedMultiplier);
            }
            // Move left
            if (Input.kState.IsKeyDown(Keybinds.leftKey))
            {
                Player.position += Player.Right(deltaTime * -1);
            }
            // Move right
            if (Input.kState.IsKeyDown(Keybinds.rightKey))
            {
                Player.position += Player.Right(deltaTime);
            }
            // Turn left
            if (Input.kState.IsKeyDown(Keybinds.turnLeftKey))
            {
                Player.yaw += deltaTime * 90;
            }
            // Turn right
            if (Input.kState.IsKeyDown(Keybinds.turnRightKey))
            {
                Player.yaw -= deltaTime * 90;
            }

            if (Input.kState.IsKeyDown(Keys.Up))
            {
                Player.roll += deltaTime * 90;
            }

            // Jump
            if (Input.kState.IsKeyDown(Keybinds.jumpKey) && Input.lastKState.IsKeyUp(Keybinds.jumpKey) && !Player.isMidAir)
            {
                Player.isMidAir = true;
                Player.velocity.Y = 5f;
                Player.acceleration.Y = -9.81f;
            }

            // Update position/velocity
            Player.position += Player.velocity * deltaTime;
            Player.velocity += Player.acceleration * deltaTime;

            if (Player.position.Y < 1)
            {
                Player.acceleration.Y = 0;
                Player.position.Y = 1;
                Player.isMidAir = false;
            }

            // Sprint (hold/release)
            if (Input.kState.IsKeyDown(Keybinds.sprintKey))
            {
                if (cameraZoomOffset > -0.15f)
                {
                    cameraZoomOffset -= 0.03f;
                }
                Player.moveSpeedMultiplier = 3f;
            }
            else
            {
                if (cameraZoomOffset < 0f)
                {
                    cameraZoomOffset += 0.03f;
                }
                Player.moveSpeedMultiplier = 1f;
            }

            // Check clicking
            if (Input.mState.LeftButton == ButtonState.Pressed && Input.lastMState.LeftButton == ButtonState.Released)
            {
                Line line = new Line(GraphicsDevice, Player.position, Player.position + Player.UserFacing(1), Color.Red);
                line.velocity = Player.UserFacing(3);
                objects.Add(line);
            }

            if (Input.mState.RightButton == ButtonState.Pressed && Input.lastMState.RightButton == ButtonState.Released)
            {
                ModelObject ball = new ModelObject(bulletModel);
                ball.position = Player.UserFacing(0.5f) + Player.position;
                ball.velocity = Player.UserFacing(10f);
                ball.acceleration = Vector3.UnitY * -9.81f;
                ball.scaleFactor = 4f;
                ball.rotation = new Vector3(Player.yaw, Player.pitch, Player.roll);
                ball.originVector = Player.UserFacing(1f);
                objects.Add(ball);
            }

            // Update mouse
            if (IsActive)
            { 
                Point change = Input.mState.Position - Input.lastMState.Position;
                if (!isMouseRepositioned)
                {
                    Player.yaw += change.X / -20f;
                    Player.roll += change.Y / -20f;
                }

                // Check if mouse is out of bounds.
                isMouseRepositioned = false;
                if (Input.mState.X == 0 || Input.mState.X > screenBounds.X - 10
                || Input.mState.Y == 0 || Input.mState.Y > screenBounds.Y - 10)
                {
                    Mouse.SetPosition(screenBounds.X / 2, screenBounds.Y / 2);
                    isMouseRepositioned = true;
                }
            }


            // Get new camera position and target
            cameraTarget = Player.UserFacing(1) + Player.position;
            cameraPosition = Player.position + Player.UserFacing(cameraZoomOffset);

            foreach(GameObject obj in objects)
            {
                if (obj.GetType() == typeof(ModelObject))
                {
                    (obj as ModelObject).Update(gameTime);
                }
                else
                obj.Update(gameTime);
            }

            // Update rotation
            if (Player.roll > 70)
            {
                Player.roll = 70;
            }
            if (Player.roll < -70)
            {
                Player.roll = -70;
            }



            // Get new matrix
            viewMatrix = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Up);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Set matrices
            effect.Projection = projectionMatrix;
            effect.View = viewMatrix;
            effect.World = worldMatrix;

            // Rasterization
            RasterizerState rastState = new RasterizerState();
            rastState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rastState;


            
            // Draw axes
            GraphicsDevice.SetVertexBuffer(xBuffer);
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawPrimitives(PrimitiveType.LineList, 0, 2);
            }

            GraphicsDevice.SetVertexBuffer(yBuffer);
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawPrimitives(PrimitiveType.LineList, 0, 2);
            }

            GraphicsDevice.SetVertexBuffer(zBuffer);
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawPrimitives(PrimitiveType.LineList, 0, 2);
            }

            // Draw 3d objects
            foreach (GameObject obj in objects)
            {

                if (obj.GetType() == typeof(TexturedRectangle))
                {
                    (obj as TexturedObject).Draw(Matrix.CreateTranslation(obj.position), viewMatrix, projectionMatrix);
                }
                else if (obj.GetType() == typeof(Line))
                {
                    (obj as Line).Draw(Matrix.CreateTranslation(obj.position), viewMatrix, projectionMatrix);
                }
                else if (obj.GetType() == typeof(ModelObject))
                {
                    (obj as ModelObject).Draw(Matrix.CreateTranslation(obj.position), viewMatrix, projectionMatrix);
                }
            }



            // 2D Drawing
            _spriteBatch.Begin();
            
            _spriteBatch.Draw(cursorTexture, new Vector2(screenBounds.X / 2f, screenBounds.Y / 2f), null, Color.White, 0f, new Vector2(cursorTexture.Width/2f, cursorTexture.Height/2f), 0.5f, SpriteEffects.None, 1f);


            _spriteBatch.DrawString(font, "Yaw: " + Player.yaw.ToString(), Vector2.UnitY * 20, Color.Black);
            _spriteBatch.DrawString(font, "Pitch: " + Player.pitch.ToString(), Vector2.Zero, Color.Black);
            _spriteBatch.DrawString(font, "Roll: " + Player.roll.ToString(), Vector2.UnitY * 40, Color.Black);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}