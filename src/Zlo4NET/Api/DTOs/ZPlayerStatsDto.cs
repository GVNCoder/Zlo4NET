using Zlo4NET.Core.Data.Attributes;

#pragma warning disable 1591

namespace Zlo4NET.Api.DTO
{
    /// <summary>
    /// 
    /// </summary>
    public class ZPlayerStatsDto
    {
        [ZStatsMapper(MapFromDictionaryKey = "rank")]
        public float Rank { get; set; }
    }
}