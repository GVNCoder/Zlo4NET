using System.Collections.Generic;

namespace Zlo4NET.Api.DTOs
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class ZPlayerStatsBase
    {
        /// <summary>
        /// Gets or sets player stats dictionary
        /// </summary>
        public IDictionary<string, float> StatsDictionary { get; }

        #region Ctor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="statsDictionary"></param>
        protected ZPlayerStatsBase(IDictionary<string, float> statsDictionary)
        {
            StatsDictionary = statsDictionary;
        }

        #endregion
    }
}