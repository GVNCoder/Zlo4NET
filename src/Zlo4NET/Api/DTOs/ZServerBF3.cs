using Zlo4NET.Api.Models.Shared;

namespace Zlo4NET.Api.DTOs
{
    /// <inheritdoc />
    public class ZServerBF3 : ZServerBase
    {
        #region Ctor

        /// <summary>
        /// 
        /// </summary>
        public ZServerBF3()
        {
            TargetGame = ZGame.BF3;
        }

        #endregion
    }
}