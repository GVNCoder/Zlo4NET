using System.Collections.Generic;

using Zlo4NET.Data.Attributes;

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable StringLiteralTypo

#pragma warning disable 1591

namespace Zlo4NET.Api.DTOs
{
    /// <inheritdoc />
    public class ZBF3PlayerStats : ZPlayerStatsBase
    {
        #region Ctor

        public ZBF3PlayerStats(IDictionary<string, float> statsDictionary) : base(statsDictionary) { }

        #endregion

        #region Auto-mapped properties

        [ZStatsMapper(MapFromDictionaryKey = "rank", TargetType = typeof(int))]
        public int Rank { get; set; }
        [ZStatsMapper(MapFromDictionaryKey = "c___k_g", TargetType = typeof(int))]
        public int Kills { get; set; }
        [ZStatsMapper(MapFromDictionaryKey = "c___d_g", TargetType = typeof(int))]
        public int Deaths { get; set; }
        [ZStatsMapper(MapFromDictionaryKey = "c_mwin__roo_g", TargetType = typeof(int))]
        public int Wins { get; set; }
        [ZStatsMapper(MapFromDictionaryKey = "c_mlos__roo_g", TargetType = typeof(int))]
        public int Losses { get; set; }
        [ZStatsMapper(MapFromDictionaryKey = "c___sfw_g", TargetType = typeof(int))]
        public int Shots { get; set; }
        [ZStatsMapper(MapFromDictionaryKey = "c___shw_g", TargetType = typeof(int))]
        public int Hits { get; set; }
        [ZStatsMapper(MapFromDictionaryKey = "c___hsh_g", TargetType = typeof(int))]
        public int Headshots { get; set; }
        [ZStatsMapper(MapFromDictionaryKey = "c___hsd_ghva")]
        public float LongestHeadshot { get; set; }
        [ZStatsMapper(MapFromDictionaryKey = "c___sa_g")]
        public float TimeSeconds { get; set; }
        [ZStatsMapper(MapFromDictionaryKey = "elo_games", TargetType = typeof(int))]
        public int Rounds { get; set; }
        [ZStatsMapper(MapFromDictionaryKey = "c_vA__de_g", TargetType = typeof(int))]
        public int VehicleDestroyed { get; set; }
        [ZStatsMapper(MapFromDictionaryKey = "c___dt_g", TargetType = typeof(int))]
        public int Dogtags { get; set; }
        [ZStatsMapper(MapFromDictionaryKey = "c___nk_ghva", TargetType = typeof(int))]
        public int NemesisStreak { get; set; }
        [ZStatsMapper(MapFromDictionaryKey = "c___k_ghvs", TargetType = typeof(int))]
        public int HighestKillStreak { get; set; }

        // Kits

        [ZStatsMapper(MapFromDictionaryKey = "sc_assault")]
        public double AssaultCurrentLongScore { get; set; }
        [ZStatsMapper(MapFromDictionaryKey = "ssclbas_00", TargetType = typeof(int))]
        public int AssaultStartCount { get; set; }

        [ZStatsMapper(MapFromDictionaryKey = "sc_engineer")]
        public double EngineerCurrentLongScore { get; set; }
        [ZStatsMapper(MapFromDictionaryKey = "ssclbe_00", TargetType = typeof(int))]
        public int EngineerStartCount { get; set; }

        [ZStatsMapper(MapFromDictionaryKey = "sc_recon")]
        public double ReconCurrentLongScore { get; set; }
        [ZStatsMapper(MapFromDictionaryKey = "ssclbr_00", TargetType = typeof(int))]
        public int ReconStartCount { get; set; }

        [ZStatsMapper(MapFromDictionaryKey = "sc_support")]
        public double SupportCurrentLongScore { get; set; }
        [ZStatsMapper(MapFromDictionaryKey = "ssclbsu_00", TargetType = typeof(int))]
        public int SupportStartCount { get; set; }

        #endregion

        #region Calculated properies

        public string RankName { get; set; }
        public double RankMaxRelativeScore { get; set; }
        public double RankMaxLongScore { get; set; }
        public double RankCurrentLongScore { get; set; }
        public double RankCurrentRelativeScore { get; set; }
        public float Skill { get; set; }
        public double ScoreToRankUp { get; set; }
        public float ScoreToRankUpPercent { get; set; }
        public float Accuracy { get; set; }
        public float WL { get; set; }
        public float KD { get; set; }
        public float AssaultCurrentRelativeScore { get; set; }
        public float AssaultStarProgressPercent { get; set; }
        public float EngineerCurrentRelativeScore { get; set; }
        public float EngineerStarProgressPercent { get; set; }
        public float ReconCurrentRelativeScore { get; set; }
        public float ReconStarProgressPercent { get; set; }
        public float SupportCurrentRelativeScore { get; set; }
        public float SupportStarProgressPercent { get; set; }
        public float TimePlayedHours { get; set; }

        #endregion

        #region Predefined properties

        public float AssaultMaxRelativeScore { get; } = 220000f;
        public float EngineerMaxRelativeScore { get; } = 145000f;
        public float ReconMaxRelativeScore { get; } = 195000f;
        public float SupportMaxRelativeScore { get; } = 170000f;

        #endregion
    }
}