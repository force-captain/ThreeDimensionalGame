using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace ThreeDimensionalGame
{
    internal static class Input
    {
        internal static MouseState mState;
        internal static KeyboardState kState;

        internal static MouseState lastMState;
        internal static KeyboardState lastKState;

        internal static void Update()
        {
            lastMState = mState;
            lastKState = kState;

            mState = Mouse.GetState();
            kState = Keyboard.GetState();
        }
    }
}
