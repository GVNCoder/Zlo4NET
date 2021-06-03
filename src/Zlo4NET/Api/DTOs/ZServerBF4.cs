// ReSharper disable InconsistentNaming

using Zlo4NET.Api.Models.Shared;

namespace Zlo4NET.Api.DTOs
{
    /// <inheritdoc />
    public class ZServerBF4 : ZServerBase
    {
        #region Ctor

        /// <summary>
        /// 
        /// </summary>
        public ZServerBF4()
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