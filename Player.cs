using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.MediaFoundation.DirectX;

namespace ThreeDimensionalGame
{
    internal static class Keybinds
    {
        internal static Keys forwardKey;
        internal static Keys backKey;
        internal static Keys rightKey;
        internal static Keys leftKey;
        internal static Keys jumpKey;
        internal static Keys sprintKey;
        internal static Keys turnRightKey;
        internal static Keys turnLeftKey;
    }

    internal static class Player
    {
        internal static Vector3 position;
        internal static Vector3 velocity;
        internal static Vector3 acceleration;

        internal static float rotationX;

        // Determines the angle the user is facing
        internal static float rotationY;

        // Speeds up player movement.
        internal static float moveSpeedMultiplier;

        // Checks if the user is in air.
        internal static bool isMidAir;

        /// <summary>
        /// Gets a forward vector
        /// </summary>
        /// <param name="distance">The length of the vector to return</param>
        /// <returns>A vector in the direction the user is facing</returns>
        internal static Vector3 Forward(float distance)
        {
            Vector3 baseForward = Vector3.UnitX;
            Matrix yaw = Matrix.CreateRotationY(MathHelper.ToRadians(rotationY));
            Vector3 result = Vector3.Transform(baseForward, yaw);
            return result * distance;
        }

        /// <summary>
        /// Gets a rightward vector
        /// </summary>
        /// <param name="distance">The length of the vector to return</param>
        /// <returns>A vector in the user's right</returns>
        internal static Vector3 Right(float distance)
        {
            Vector3 baseRight = Vector3.UnitZ;
            Matrix yaw = Matrix.CreateRotationY(MathHelper.ToRadians(rotationY));
            Vector3 result = Vector3.Transform(baseRight, yaw);
            return result * distance;
        }

        internal static Vector3 Up(float distance)
        {
            Vector3 baseRight = Vector3.UnitY;
            Matrix yaw = Matrix.CreateRotationY(MathHelper.ToRadians(rotationY));
            Vector3 result = Vector3.Transform(baseRight, yaw);
            return result * distance;
        }
    }
}
