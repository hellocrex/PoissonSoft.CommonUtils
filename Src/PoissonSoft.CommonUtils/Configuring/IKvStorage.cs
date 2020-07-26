namespace PoissonSoft.CommonUtils.Configuring
{
    /// <summary>
    /// Key-value container
    /// </summary>
    public interface IKvStorage
    {
        /// <summary>
        /// Read/write access to storage
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string this[string key] { get; set; }

        /// <summary>
        /// Getting all available keys
        /// </summary>
        string[] AllKeys { get; }

        /// <summary>
        /// Remove item from storage
        /// </summary>
        /// <param name="key"></param>
        void RemoveItem(string key);
    }
}
