using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG {

    public class InputSignatureComparer : IComparer<InputSignature> {
        
#region IComparer implementation

        public int Compare(InputSignature x, InputSignature y) {
            int xc = x.complexity;
            int yc = y.complexity;
            //if the complexity differs, sort by complexity descending (higher numbers first)
            //else, sort by key ascending (alphabetically)
            return xc != yc ? yc.CompareTo(xc) : x.key.CompareTo(y.key);
        }

#endregion

    }
}
