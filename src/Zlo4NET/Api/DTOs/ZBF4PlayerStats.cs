using System.Collections.Generic;

using Zlo4NET.Core.Data.Attributes;

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable StringLiteralTypo

#pragma warning disable 1591

namespace Zlo4NET.Api.DTOs
{
    /// <inheritdoc />
    public class ZBF4PlayerStats : ZPlayerStatsBase
    {
        #region Ctor

        public ZBF4PlayerStats(IDictionary<string, float> statsDictionary) : base(statsDictionary) { }

        #endregion

        #region Auto-mapped properties

        [ZStatsMapper(MapFromDictionaryKey = "rank")]
        public float Rank { get; set; }
        [ZStatsMapper(MapFromDictionaryKey = "sc_rank")]
        public float RankCurrentScore { get; set; }
        [ZStatsMapper(MapFromDictionaryKey = "skill")]
        public float Skill { get; set; }
        [ZStatsMapper(MapFromDictionaryKey = "c___k_g")]
        public float Kills { get; set; }
        [ZStatsMapper(MapFromDictionaryKey = "c___d_g")]
        public float Deaths { get; set; }
        [ZStatsMapper(MapFromDictionaryKey = "c_mwin__roo_g")]
        public float Wins { get; set; }
        [ZStatsMapper(MapFromDictionaryKey = "c_mlos__roo_g")]
        public float Losses { get; set; }
        [ZStatsMapper(MapFromDictionaryKey = "c___sfw_g")]
        public float Shots { get; set; }
        [ZStatsMapper(MapFromDictionaryKey = "c___shw_g")]
        public float Hits { get; set; }
        [ZStatsMapper(MapFromDictionaryKey = "c___hsh_g")]
        public float Headshots { get; set; }
        [ZStatsMapper(MapFromDictionaryKey = "c___hsd_ghva")]
        public float LongestHeadshot { get; set; }
        [ZStatsMapper(MapFromDictionaryKey = "c___sa_g")]
        public float TimeSeconds { get; set; }
        //[ZStatsMapper(MapFromDictionaryKey = "elo_games")]
        //public float Rounds { get; set; }
        [ZStatsMapper(MapFromDictionaryKey = "c_vA__de_g")]
        public float VehicleDestroyed { get; set; }
        [ZStatsMapper(MapFromDictionaryKey = "c___dt_g")]
        public float Dogtags { get; set; }
        [ZStatsMapper(MapFromDictionaryKey = "c___nk_ghva")]
        public float NemesisStreak { get; set; }
        [ZStatsMapper(MapFromDictionaryKey = "c___k_ghvs")]
        public float HighestKillStreak { get; set; }

        // Kits

        [ZStatsMapper(MapFromDictionaryKey = "sc_assault")]
        public float AssaultCurrentScore { get; set; }
        [ZStatsMapper(MapFromDictionaryKey = "ssclbas_00")]
        public float AssaultStartCount { get; set; }

        [ZStatsMapper(MapFromDictionaryKey = "sc_engineer")]
        public float EngineerCurrentScore { get; set; }
        [ZStatsMapper(MapFromDictionaryKey = "ssclbe_00")]
        public float EngineerStartCount { get; set; }

        [ZStatsMapper(MapFromDictionaryKey = "sc_recon")]
        public float ReconCurrentScore { get; set; }
        [ZStatsMapper(MapFromDictionaryKey = "ssclbr_00")]
        public float ReconStartCount { get; set; }

        [ZStatsMapper(MapFromDictionaryKey = "sc_support")]
        public float SupportCurrentScore { get; set; }
        [ZStatsMapper(MapFromDictionaryKey = "ssclbsu_00")]
        public float SupportStartCount { get; set; }

        [ZStatsMapper(MapFromDictionaryKey = "sc_commander")]
        public float CommanderCurrentScore { get; set; }
        [ZStatsMapper(MapFromDictionaryKey = "ssclco_00")]
        public float CommanderStartCount { get; set; }

        #endregion

        #region Calculated properies

        //public double RankShortScore { get; set; }
        public double ScoreToRankUp { get; set; }
        public float ScoreToRankUpPercent { get; set; }
        public float Accuracy { get; set; }
        public float WL { get; set; }
        public float KD { get; set; }
        public float AssaultStarProgressPercent { get; set; }
        public float EngineerStarProgressPercent { get; set; }
        public float ReconStarProgressPercent { get; set; }
        public float SupportStarProgressPercent { get; set; }
        public float CommanderStarProgressPercent { get; set; }
        public float TimePlayedHours { get; set; }
        public float Rounds { get; set; }

        #endregion

        #region Manually assigned properties

        public string RankName { get; set; }
        public float RankMaxScore { get; set; }
        public double RankMaxTotalScore { get; set; }

        #endregion

        #region Predefined properties

        public float AssaultMaxScore { get; } = 155000f;
        public float EngineerMaxScore { get; } = 131000f;
        public float ReconMaxScore { get; } = 134000f;
        public float SupportMaxScore { get; } = 104000f;
        public float CommanderMaxScore { get; } = 20000f;

        #endregion
    }
}