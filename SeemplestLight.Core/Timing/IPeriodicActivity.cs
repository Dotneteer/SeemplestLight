namespace SeemplestLight.Core.Timing
{
    /// <summary>
    /// This interface signs that its implementing object carries out a periodic 
    /// activity
    /// </summary>
    public interface IPeriodicActivity
    {
        /// <summary>
        /// This activity is carried out periodically
        /// </summary>
        void OnPeriodicActivity();
    }
}