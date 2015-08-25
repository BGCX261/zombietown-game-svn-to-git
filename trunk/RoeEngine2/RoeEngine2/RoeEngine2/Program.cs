using System;
using RoeEngine2.Managers;
using Microsoft.Xna.Framework;

#if !XBOX360
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;
#endif

namespace Kynskavion
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
#if !XBOX360
        [STAThread]
#endif
        static void Main(string[] args)
        {
#if DEBUG
            StartUnitTests();
#else
	            StartGame();
#endif
        }

        private static void StartUnitTests()
        {
            StartGame();
        }

        private static void StartGame()
        {
#if !XBOX360
            try
            {
#endif
                using (EngineManager game = new EngineManager())
                {
                    EngineManager.Game = game;
                    SetupScene();
                    game.Run();
                }
#if !XBOX360
            }
            catch (NoSuitableGraphicsDeviceException)
            {
                MessageBox.Show("Pixel and vertex shaders 2.0 or greater are required.",
                    "Kynskavion",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (OutOfVideoMemoryException)
            {
                //GameSettings.SetMinimumGraphics();

                MessageBox.Show("Insufficent video memory.\n\n" +
                    "The graphics settings have been reconfigured to the minimum. " +
                    "Please restart the application. \n\nIf you continue to receive " +
                    "this error message, your system may not meet the " +
                    "minimum requirements.  \n\nCheck documentation for minimum requirements.",
                    "Kynskavion",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Kynskavion", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
#endif
        }

        private static void SetupScene()
        {
            //throw new Exception("The method or operation is not implemented.");
        }
    }
}