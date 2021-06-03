using Zlo4NET.Api.Models.Shared;

namespace Zlo4NET.Api.DTOs
{
    /// <inheritdoc />
    public class ZServerBFHL : ZServerBase
    {
        #region Ctor

        /// <summary>
        /// 
        /// </summary>
        public ZServerBFHL()
        {
            TargetGame = ZGame.BFHL;
        }

        #endregion
    }
}