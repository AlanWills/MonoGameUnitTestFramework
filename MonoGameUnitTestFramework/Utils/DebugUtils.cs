﻿using System.Diagnostics;

namespace MonoGameUnitTestFramework
{
    public static class DebugUtils
    {
        [Conditional("DEBUG")]
        public static void AssertNotNull(object nullableObject)
        {
            Debug.Assert(nullableObject != null);
        }

        [Conditional("DEBUG")]
        public static void AssertNull(object nullableObject)
        {
            Debug.Assert(nullableObject == null);
        }
    }
}
