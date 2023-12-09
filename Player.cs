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


        // Determines the angle the user is facing
        internal static float yaw;
        internal static float pitch;
        internal static float roll;

        // Speeds up player movement.
        internal static float moveSpeedMultiplier;

        // Checks if the user is in air.
        internal static bool isMidAir;

        internal static Vector3 UserFacing(float distance)
        {
            Vector3 baseForward = Vector3.UnitX;
            Quaternion rotation = Quaternion.CreateFromYawPitchRoll(
                MathHelper.ToRadians(yaw),
                MathHelper.ToRadians(pitch),
                MathHelper.ToRadians(roll));
            Vector3 result = Vector3.Transform(baseForward, rotation);
            return result * distance;
        }



        /// <summary>
        /// Gets a forward vector
        /// </summary>
        /// <param name="distance">The length of the vector to return</param>
        /// <returns>A vector forward the user is facing. Does not take into account pitch or roll</returns>
        internal static Vector3 Forward(float distance)
        {
            Vector3 baseForward = Vector3.UnitX;
            Matrix yawMatrix = Matrix.CreateRotationY(MathHelper.ToRadians(yaw));
            Vector3 result = Vector3.Transform(baseForward, yawMatrix);
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
            Matrix yawMatrix = Matrix.CreateRotationY(MathHelper.ToRadians(yaw));
            Vector3 result = Vector3.Transform(baseRight, yawMatrix);
            return result * distance;
        }
    }
}
