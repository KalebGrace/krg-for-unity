using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG
{

    /// <summary>
    /// Allows for the label text to be overriden.
    /// Only works when paired with certain other objects/attributes (e.g. BoolObject/BoolObjectDisableAttribute).
    /// Last Refactor: 0.05.002 / 2018-05-05
    /// </summary>
    public class LabelTextAttribute : PropertyAttribute
    {

        #region properties

        public string labelText { get; private set; }

        #endregion

        #region constructors

        /// <summary>
        /// Allows for the label text to be overriden.
        /// Only works when paired with certain other objects/attributes (e.g. BoolObject/BoolObjectDisableAttribute).
        /// </summary>
        /// <param name="labelText">Label text.</param>
        public LabelTextAttribute(string labelText)
        {
            this.labelText = labelText;
        }

        #endregion

    }
}