namespace Zlo4NET.Api.DTOs
{
    /// <summary>
    /// Represents an authorized in ZClient user DTO (Data Transfer Object)
    /// </summary>
    public class ZUser
    {
        /// <summary>
        /// Gets user name that authorized in ZClient
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Gets user id that authorized in ZClient
        /// </summary>
        public uint UserId { get; set; }
    }
}