namespace UnitTestFramework
{
    // Only supports instance functions for now
    public class FunctionSuccess : TestPassIf
    {
        #region Properties and Fields

        /// <summary>
        /// The name of the function we will be applying to the object
        /// </summary>
        private string FunctionName { get; set; }

        #endregion

        public FunctionSuccess(string functionName, params object[] parametersForFunction) :
            base(null, parametersForFunction)
        {
            FunctionName = functionName;
        }
    }
}
