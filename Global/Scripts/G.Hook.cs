using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG
{
    partial class G
    {
        public static class Hook
        {
            /// ----------
            ///    CALL
            /// ----------

            public static void Call(ID hookID, ref string s)
            {
                switch (hookID)
                {
                    case ID.FungusSayOnEnterDisplayText:
                        var e = FungusSayOnEnterDisplayText;
                        if (e != null) e(ref s);
                        break;
                    default:
                        G.U.Unsupported(null, hookID);
                        break;
                }
            }


            /// ----------
            ///   EVENT!
            /// ----------

            public static event Delegate.RefString FungusSayOnEnterDisplayText;


            /// ----------
            ///  DELEGATE
            /// ----------

            public static class Delegate
            {
                public delegate void RefString(ref string s);
            }


            /// ----------
            ///     ID
            /// ----------

            public enum ID
            {
                //f -> 6 * 3 = 18, u -> 21 * 3 = 63, & then 5000
                FungusSayOnEnterDisplayText = 18635000,
            }
        }
    }
}
