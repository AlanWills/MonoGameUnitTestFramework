using System;
using System.Collections;
using System.Collections.Generic;

namespace UnitTestFramework
{
    /// <summary>
    /// An attribute we use to indicate all of the inputs which should pass the unit test
    /// </summary>
    public class TestValidInputs : Attribute, IEnumerable
    {
        #region Properties and Fields

        private List<object> ValidInputs { get; set; }

        #endregion

        public TestValidInputs(params object[] validInputs)
        {
            ValidInputs = new List<object>();

            foreach (object obj in validInputs)
            {
                ValidInputs.Add(obj);
            }
        }

        public IEnumerator GetEnumerator()
        {
            return ValidInputs.GetEnumerator();
        }
    }
}
