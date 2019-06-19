using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG
{
    public static class IEndEM
    {
        public static void Dispose<T>(this T me) where T : MonoBehaviour, IEnd
        {
            if (me == null)
            {
                G.U.Warning("The IEnd MonoBehaviour you wish to dispose of is null.");
                return;
            }
            if (!me.end.wasInvoked)
            {
                G.U.Err("{0} has been destroyed without calling my_end.Invoke() in the OnDestroy method.", me.ToString());
            }
            Object.Destroy(me);
        }
    }
}
