using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace UnitTestFramework
{
    /// <summary>
    /// A static class with custom functions that are useful in unit tests with lists.
    /// </summary>
    public static class TestHelperFunctions
    {
        /// <summary>
        /// Checks that the two inputted lists have the same objects in them.
        /// Assumes the elements are ordered the same, so if they are the same, they will be in the same order.
        /// Will fail even if the lists contain the same elements, but are in a different order.
        /// Much quicker than the unordered check.
        /// </summary>
        /// <param name="expected">Our expected list of objects</param>
        /// <param name="actual">Our actual list of objects</param>
        public static bool CheckOrderedListsEqual<T>(List<T> expected, List<T> actual)
        {
            // Check sizes are the same
            Assert.AreEqual(expected.Count, actual.Count);

            if (expected.Count != actual.Count)
            {
                Assert.Fail("Lists have different number of elements");
                return false;
            }

            for (int i = 0; i < expected.Count; i++)
            {
                bool result = expected[i].Equals(actual[i]);
                Assert.IsTrue(result);

                if (!result)
                {
                    // Return at the first sign of inconsistency - saves us time doing further checks on an already failed test
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks whether two lists have the same elements regardless of ordering.
        /// Slower than the ordered check so use that wherever possible.
        /// </summary>
        /// <param name="expected">The expected list of objects</param>
        /// <param name="actual">The actual list of objects</param>
        public static bool CheckUnorderedListsEqual<T>(List<T> expected, List<T> actual)
        {
            // Check sizes are the same
            Assert.AreEqual(expected.Count, actual.Count);

            if (expected.Count != actual.Count)
            {
                Assert.Fail("Lists have different number of elements");
                return false;
            }

            for (int i = 0; i < expected.Count; i++)
            {
                bool result = actual.Exists(x => x.GetType().Equals(expected[i]));
                Assert.IsTrue(result);

                if (!result)
                {
                    // Return at the first sign of inconsistency - saves us time doing further checks on an already failed test
                    return false;
                }
            }

            return true;
        }
    }
}
