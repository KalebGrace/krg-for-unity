using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG {

    /// <summary>
    /// Enum attribute.
    /// Last Refactor: 0.05.002 / 2018-05-05
    /// </summary>
    public class EnumAttribute : PropertyAttribute {

#region properties

        public System.Type enumType { get; private set; }

#endregion

#region constructors

        public EnumAttribute(System.Type enumType) {
            this.enumType = enumType;
        }

#endregion

    }
}
