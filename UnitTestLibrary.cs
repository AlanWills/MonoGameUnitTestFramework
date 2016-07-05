using System.Collections.Generic;

namespace UnitTestFramework
{
    /// <summary>
    /// A static class which contains a copy of every UnitTest class that we will write.
    /// </summary>
    public static class UnitTestLibrary
    {
        #region Properties and Fields

        /// <summary>
        /// A static list of all the unit tests that have been registered
        /// </summary>
        public static List<UnitTest> RegisteredUnitTests { get; set; }

        #endregion
    }
}
