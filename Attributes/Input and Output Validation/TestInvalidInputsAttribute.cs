using System;
using System.Collections;
using System.Collections.Generic;

namespace UnitTestFramework
{
    /// <summary>
    /// An attribute we use to indicate all of the inputs which should not pass the unit test.
    /// </summary>
    public class TestInvalidInputs : Attribute, IEnumerable
    {
        #region Properties and Fields

        private List<object> InvalidInputs { get; set; }

        #endregion

        public TestInvalidInputs(params object[] invalidInputs)
        {
            InvalidInputs = new List<object>();

            foreach (object obj in invalidInputs)
            {
                InvalidInputs.Add(obj);
            }
        }

        public IEnumerator GetEnumerator()
        {
            return InvalidInputs.GetEnumerator();
        }
    }
}
