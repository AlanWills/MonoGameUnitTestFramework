using _2DEngine;
using _2DEngineData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace MonoGameUnitTestFramework
{
    /// <summary>
    /// The only screen for unit testing game dependent classes.  Will run the tests, display the output and then close.
    /// </summary>
    public class UnitTestGameMainMenuScreen : MenuScreen
    {
        #region Properties and Fields

        /// <summary>
        /// The list control for the output strings from our unit tests.
        /// </summary>
        private ListControl OutputLog { get; set; }

        private List<UnitTest> UnitTests { get; set; }

        private string LogFilePath { get; set; }

        #endregion

        public UnitTestGameMainMenuScreen(string mainMenuScreenDataAsset = "Screens\\UnitTestGameMainMenuScreen.xml") :
            base(mainMenuScreenDataAsset)
        {
            UnitTests = new List<UnitTest>();
            LogFilePath = Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\..\\..", "TestResults", "TestResults.txt");

            // Creates and overwrites the text file for our output log
            Directory.CreateDirectory(Path.GetDirectoryName(LogFilePath));
            File.Create(LogFilePath);
        }

        #region Virtual Functions

        protected override void AddInitialUI()
        {
            base.AddInitialUI();

            OutputLog = AddScreenUIObject(new ListControl(ScreenDimensions, ScreenCentre));

            foreach (TypeInfo typeInfo in Assembly.GetEntryAssembly().DefinedTypes)
            {
                if (typeInfo.IsSubclassOf(typeof(UnitTest)))
                {
                    UnitTests.Add((UnitTest)Activator.CreateInstance(typeInfo));
                }
            }
        }

        /// <summary>
        /// Make sure we create the data file if it does not exist so our game does not crash when running from a third party like a build server
        /// </summary>
        /// <returns></returns>
        protected override BaseScreenData LoadScreenData()
        {
            return AssetManager.GetData<BaseScreenData>("Screens\\StartupLogoScreen.xml", true);
        }

        /// <summary>
        /// Run the tests and then add a lifetime module to kill the application after a period of time
        /// </summary>
        public override void Begin()
        {
            base.Begin();

            foreach (UnitTest test in UnitTests)
            {
                LogOutput(test.Run());
            }

            AddModule(new LifeTimeModule(5), true, true);
        }

        /// <summary>
        /// After the tests have run, press escape to close the application
        /// </summary>
        /// <param name="elapsedGameTime"></param>
        /// <param name="mousePosition"></param>
        public override void HandleInput(float elapsedGameTime, Vector2 mousePosition)
        {
            base.HandleInput(elapsedGameTime, mousePosition);

            if (GameKeyboard.IsKeyPressed(Keys.Escape))
            {
                Die();
            }
        }

        /// <summary>
        /// When this screen dies, close the application
        /// </summary>
        public override void Die()
        {
            base.Die();

            ScreenManager.Instance.EndGame();
        }

        #endregion

        #region Logging Functions

        /// <summary>
        /// Adds the inputted text to our Output and uses the flag result to colour it either green (pass) or red (fail)
        /// </summary>
        /// <param name="text"></param>
        private void LogInfo(Tuple<bool, string> result)
        {
            // Add an extra bit of a text clarifying the output of the test
            string logOutput = result.Item2 + (result.Item1 ? " passed" : " failed");
            Label resultText = OutputLog.AddChild(new Label(logOutput, Vector2.Zero), true, true);
            resultText.Colour = result.Item1 ? Color.Green : Color.Red;

            using (StreamWriter writer = File.AppendText(LogFilePath))
            {
                writer.WriteLine(resultText.Text);
            }
        }

        private void LogInfo(bool result, string text)
        {
            LogInfo(new Tuple<bool, string>(result, text));
        }

        /// <summary>
        /// Adds the text in the inputted list to our Output
        /// </summary>
        /// <param name="resultList"></param>
        private void LogOutput(List<Tuple<bool, string>> resultList)
        {
            foreach (Tuple<bool, string> result in resultList)
            {
                LogInfo(result);
            }
        }

        #endregion
    }
}