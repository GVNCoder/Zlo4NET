// ReSharper disable InconsistentNaming

using Zlo4NET.Api.Models.Shared;

namespace Zlo4NET.Api.DTOs
{
    /// <inheritdoc />
    public class ZBF4Server : ZServerBase
    {
        #region Ctor

        /// <summary>
        /// 
        /// </summary>
        public ZBF4Server()
        {
            TargetGame = ZGame.BF4;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public int SpectatorsCapacity { get; set; }
    }
}