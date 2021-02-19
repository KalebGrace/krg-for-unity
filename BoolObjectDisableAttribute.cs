using UnityEngine;

namespace _0G.Legacy
{
    /// <summary>
    /// Disables the object value when the specified bool value is set.
    /// An optional description can be displayed in place of the disabled object value.
    /// Used with BoolObject and BoolObjectDisableDrawer.
    /// </summary>
    public class BoolObjectDisableAttribute : PropertyAttribute
    {
        #region properties

        public bool boolValue { get; private set; }

        public string disableDescription { get; private set; }

        #endregion

        #region constructors

        /// <summary>
        /// Disables the object value when the specified bool value is set.
        /// An optional description can be displayed in place of the disabled object value.
        /// </summary>
        /// <param name="boolValue">This is the bool value that will disable the object value.</param>
        /// <param name="disableDescription">Description to be displayed in place of the disabled object value.</param>
        public BoolObjectDisableAttribute(bool boolValue, string disableDescription = null)
        {
            this.boolValue = boolValue;
            this.disableDescription = disableDescription;
        }

        #endregion
    }
}