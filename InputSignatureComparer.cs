using System.Collections.Generic;

namespace _0G.Legacy
{
    public class InputSignatureComparer : IComparer<InputSignature>
    {
        public int Compare(InputSignature x, InputSignature y)
        {
            int xc = x.complexity;
            int yc = y.complexity;
            //if the complexity differs, sort by complexity descending (higher numbers first)
            //else, sort by key ascending (alphabetically)
            return xc != yc ? yc.CompareTo(xc) : x.key.CompareTo(y.key);
        }
    }
}