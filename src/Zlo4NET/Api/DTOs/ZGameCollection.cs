namespace Zlo4NET.Api.DTOs
{
    /// <summary>
    /// 
    /// </summary>
    public class ZGameCollection
    {
        /// <summary>
        /// 
        /// </summary>
        public bool IsX64OperatingSystem { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ZInstalledGame[] Games { get; set; }
    }
}