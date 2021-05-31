namespace Zlo4NET.Api.Models.Shared
{
    /// <inheritdoc />
    public class ZMultiLaunchParameters : ZBaseLaunchParameters
    {
        /// <summary>
        /// Gets or sets server ZloID. Require
        /// </summary>
        public uint ServerId { get; set; }
        /// <summary>
        /// Gets or sets player role
        /// </summary>
        public ZRole Role { get; set; } = ZRole.Soldier;
    }
}