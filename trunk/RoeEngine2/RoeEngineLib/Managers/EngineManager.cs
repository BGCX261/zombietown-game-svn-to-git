using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
namespace RoeEngine2.Managers
{
    public class EngineManager : RoeEngine2
    {
        private static Game _game;
        /// <summary>
        /// The XNA game.
        /// </summary>
        public static Game Game
        {
            get { return _game; }
            set { _game = value; }
        }
        public EngineManager(string unitTestName)
            : base(unitTestName)
        {
        }
        public EngineManager()
            : base("Engine")
        {
        }
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            Device.Clear(BackgroundColor);
        }
    }
}