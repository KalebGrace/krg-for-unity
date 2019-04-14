namespace KRG
{
    /// <summary>
    /// I end.
    /// </summary>
    public interface IEnd
    {
        End end { get; }

        #region SAMPLE IMPLEMENTATION

        /*

        End my_end = new End();

        public End end { get { return my_end; } }

        void OnDestroy()
        {
            my_end.Invoke();
        }

        */

        #endregion

    }
}
