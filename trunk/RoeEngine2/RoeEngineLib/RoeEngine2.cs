using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using RoeEngine2.Settings;
using RoeEngine2.Managers;

namespace RoeEngine2
{
    public partial class RoeEngine2 : Game
    {

            /// <summary>
            /// Width and height of visible render area.
	        /// </summary>
	        protected static int width, height;

            /// <summary>
            /// Get Width of visible render area.
            /// </summary>
            public static int Width
            {
                get { return width; }
            }

            /// <summary>
            /// Get Height of visible render area.
            /// </summary>
            public static int Height
            {
                get { return height; }
            }

            /// <summary>
            /// Aspect ratio of render area.
            /// </summary>
            private static float aspectRatio = 1.0f;

            /// <summary>
            /// Get Aspect ratio of render area.
            /// </summary>
            public static float AspectRatio
            {
                get { return aspectRatio; }
            }

            /// <summary>
            /// Color used to redraw the background scene.
            /// </summary>
            private static Color _backgroundColor = Color.LightBlue;

            /// <summary>
            /// Color used to redraw the background scene.
            /// </summary>
            public static Color BackgroundColor
            {
                get { return _backgroundColor; }
                set { _backgroundColor = value; }
            }

            /// <summary>
            /// Platform we are running in
            /// </summary>
            public static PlatformID CurrentPlatform = Environment.OSVersion.Platform;

            /// <summary>
            /// Window title for test cases.
            /// </summary>
            private static string _windowTitle = "";

            /// <summary>
            /// Get Window title for test cases.
            /// </summary>
            public static string WindowTitle
            {
                get { return _windowTitle; }
            }

            /// <summary>
            /// Are we active?
            /// </summary>
            private bool _isAppActive = false;

            /// <summary>
            /// Is the application active?
            /// </summary>
            public bool IsAppActive
            {
                get { return _isAppActive; }
                set { _isAppActive = value; }
            }

            /// <summary>
            /// The graphics device, what we are rendering on.
            /// </summary>
            protected static GraphicsDeviceManager _graphicsDeviceManager = null;

            /// <summary>
            /// Get The graphics device, used to render.
            /// </summary>
            public static GraphicsDevice Device
            {
                get { return _graphicsDeviceManager.GraphicsDevice; }
            }

            /// <summary>
            /// Our Content Manager
            /// </summary>
            protected static ContentManager _contentManager = null;

            /// <summary>
            /// Content Manager
            /// </summary>
            public static ContentManager ContentManager
            {
                get { return _contentManager; }
            }

            /// <summary>
            /// Have the graphics options been checked?
            /// </summary>
            private static bool _checkedGraphicsOptions = false;

            /// <summary>
            /// Do we need to apply any device changes?
            /// </summary>
            private static bool _applyDeviceChanges = false;

            /// <summary>
            /// Create RoeEngine
            /// </summary>
            /// <param name="windowsTitle">Window Title</param>
            protected RoeEngine2(string windowsTitle)
            {
                _graphicsDeviceManager = new GraphicsDeviceManager(this);

                // Set minimum pixel and vertex shader requirements.
                _graphicsDeviceManager.MinimumPixelShaderProfile = ShaderProfile.PS_2_0;
                _graphicsDeviceManager.MinimumVertexShaderProfile = ShaderProfile.VS_2_0;

                _graphicsDeviceManager.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(GraphicsDeviceManager_PreparingDeviceSettings);

                GameSettings.Initialize();

                ApplyResolutionChange();

#if DEBUG
                // Disable vertical retrace to get highest framerates possible for
                // testing performance.
                _graphicsDeviceManager.SynchronizeWithVerticalRetrace = false;
#endif
                // Demand to update as fast as possible, do not use fixed time steps.
                // The whole game is designed this way, if you remove this line
                // the game will not behave normal any longer!
                this.IsFixedTimeStep = false;

                // Init content manager
                _contentManager = new ContentManager(this.Services);

                //TODO include other inits here!
            }

            /// <summary>
            /// Create an instance of the engine
            /// </summary>
            protected RoeEngine2()
                : this("Game")
            {

            }

            /// <summary>
            /// Prepare the graphics device.
            /// </summary>
            /// <param name="sender">sender</param>
            /// <param name="e">event args</param>
            void GraphicsDeviceManager_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
	        {
	            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
	            {
	                PresentationParameters presentParams = e.GraphicsDeviceInformation.PresentationParameters;
	                if (_graphicsDeviceManager.PreferredBackBufferHeight == 720)
	                {
	                    presentParams.MultiSampleType = MultiSampleType.FourSamples;
	#if !DEBUG
	                    presentParams.PresentationInterval = PresentInterval.One;
	#endif
	                }
	                else
	                {
	                    presentParams.MultiSampleType = MultiSampleType.TwoSamples;
	#if !DEBUG
	                    presentParams.PresentationInterval = PresentInterval.Two;
	#endif
	                }
	            }
	        }
            
            /// <summary>
            /// Apply resolution change
            /// </summary>
            public static void ApplyResolutionChange()
            {
                int resolutionWidth = GameSettings.Default.ResolutionWidth;
                int resolutionHeight = GameSettings.Default.ResolutionHeight;

                if (resolutionWidth <= 0 || resolutionWidth <= 0)
                {
                    resolutionWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                    resolutionHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                }
#if XBOX360
	            // Xbox  graphics settings are fixed
	            _graphicsDeviceManager.IsFullScreen = true;
	            _graphicsDeviceManager.PreferredBackBufferWidth =
	                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
	            _graphicsDeviceManager.PreferredBackBufferHeight =
	                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
#else
                _graphicsDeviceManager.PreferredBackBufferWidth = resolutionWidth;
                _graphicsDeviceManager.PreferredBackBufferHeight = resolutionHeight;
                _graphicsDeviceManager.IsFullScreen = GameSettings.Default.Fullscreen;

                _applyDeviceChanges = true;
#endif
            }

            /// <summary>
            /// Allows the game to perform any initialization it needs to before starting to run.
            /// This is where it can query for any required services and load any non-graphic
            /// related content.  Calling base.Initialize will enumerate through any components
            /// and initialize them as well.
            /// </summary>
            protected override void Initialize()
            {
                // TODO: Add your initialization logic here

                base.Initialize();

                _graphicsDeviceManager.DeviceReset += new EventHandler(GraphicsDeviceManager_DeviceReset);
                GraphicsDeviceManager_DeviceReset(null, EventArgs.Empty);
            }

            void GraphicsDeviceManager_DeviceReset(object sender, EventArgs e)
            {

            }

            /// <summary>
            /// LoadContent will be called once per game and is the place to load
            /// all of your content.
            /// </summary>
            protected override void LoadContent()
            {
                // TODO: use this.Content to load your game content here
            }

            /// <summary>
            /// UnloadContent will be called once per game and is the place to unload
            /// all content.
            /// </summary>
            protected override void UnloadContent()
            {
                // TODO: Unload any non ContentManager content here
            }

            /// <summary>
            /// Allows the game to run logic such as updating the world,
            /// checking for collisions, gathering input, and playing audio.
            /// </summary>
            /// <param name="gameTime">Provides a snapshot of timing values.</param>
            protected override void Update(GameTime gameTime)
            {
                // TODO: Add your update logic here

                base.Update(gameTime);
            }

            /// <summary>
            /// This is called when the game should draw itself.
            /// </summary>
            /// <param name="gameTime">Provides a snapshot of timing values.</param>
            protected override void Draw(GameTime gameTime)
            {
                base.Draw(gameTime);

                // Apply device changes
                if (_applyDeviceChanges)
                {
                    _graphicsDeviceManager.ApplyChanges();
                    _applyDeviceChanges = false;
                }
            }
            
            protected override void OnActivated(object sender, EventArgs args)
            {
                base.OnActivated(sender, args);
                IsAppActive = true;
            }

            protected override void OnDeactivated(object sender, EventArgs args)
            {
                base.OnDeactivated(sender, args);
                IsAppActive = false;
            }
    }
}
