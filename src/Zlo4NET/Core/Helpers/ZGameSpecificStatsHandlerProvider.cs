using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Newtonsoft.Json.Linq;

using Zlo4NET.Api.DTOs;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Core.Data;
using Zlo4NET.Core.Data.Attributes;

// ReSharper disable InconsistentNaming
// ReSharper disable StringLiteralTypo

namespace Zlo4NET.Core.Helpers
{
    internal static class ZGameSpecificStatsHandlerProvider
    {
        #region Constants

        private const int BF3_MAX_RANK = 145;
        private const int BF4_MAX_RANK = 141;

        #endregion

        public static ZPlayerStatsBase BF3StatsHandler(IDictionary<string, float> statsDictionary)
        {
            var statsObject = new ZBF3PlayerStats(statsDictionary);
            var ranksDetails = _LoadJsonByGame(ZGame.BF3);

            _MapAutoMapperProperties(statsObject, statsDictionary);

            // set manually assigned properties
            var currentRank = ranksDetails[statsObject.Rank];

            statsObject.RankName = currentRank["rankName"].Value<string>();
            statsObject.RankMaxRelativeScore = currentRank["xpRelative"].Value<float>();
            statsObject.RankMaxLongScore = currentRank["xpTotal"].Value<double>();

            // calculate calculated properties
            statsObject.RankCurrentLongScore = _SumKeys(statsDictionary,
                "sc_specialkit", "sc_unlock", "sc_vehiclembt", "sc_vehicleifv", "sc_vehicleaa", "sc_vehicleah", "sc_vehiclesh", "sc_vehiclejet", "sc_vehiclelbt", "sc_vehicleart", "sc_award", "sc_support", "sc_assault", "sc_engineer", "sc_recon");
            statsObject.RankCurrentRelativeScore = statsObject.RankCurrentLongScore - statsObject.RankMaxLongScore;
            statsObject.ScoreToRankUp = statsObject.RankMaxRelativeScore - statsObject.RankCurrentRelativeScore;
            statsObject.Accuracy = statsObject.Hits * 1f / statsObject.Shots * 100; // * 1f it is divide result conversion to float
            statsObject.WL = statsObject.Wins * 1f / statsObject.Losses; // * 1f it is divide result conversion to float
            statsObject.KD = statsObject.Kills * 1f / statsObject.Deaths; // * 1f it is divide result conversion to float
            statsObject.AssaultCurrentRelativeScore = (float) statsObject.AssaultCurrentLongScore -
                                                      statsObject.AssaultMaxRelativeScore *
                                                      statsObject.AssaultStartCount;
            statsObject.AssaultStarProgressPercent = statsObject.AssaultCurrentRelativeScore / statsObject.AssaultMaxRelativeScore * 100;
            statsObject.EngineerCurrentRelativeScore = (float) statsObject.EngineerCurrentLongScore -
                                                      statsObject.EngineerMaxRelativeScore *
                                                      statsObject.EngineerStartCount;
            statsObject.EngineerStarProgressPercent = statsObject.EngineerCurrentRelativeScore / statsObject.EngineerMaxRelativeScore * 100;
            statsObject.ReconCurrentRelativeScore = (float) statsObject.ReconCurrentLongScore -
                                                      statsObject.ReconMaxRelativeScore *
                                                      statsObject.ReconStartCount;
            statsObject.ReconStarProgressPercent = statsObject.ReconCurrentRelativeScore / statsObject.ReconMaxRelativeScore * 100;
            statsObject.SupportCurrentRelativeScore = (float) statsObject.SupportCurrentLongScore -
                                                      statsObject.SupportMaxRelativeScore *
                                                      statsObject.SupportStartCount;
            statsObject.SupportStarProgressPercent = statsObject.SupportCurrentRelativeScore / statsObject.SupportMaxRelativeScore * 100;
            statsObject.TimePlayedHours = statsObject.TimeSeconds / 60 / 60;
            statsObject.Skill = (statsObject.WL + statsObject.KD) / 2;

            // cuz in the case of rank 145, we get division by 0
            if (statsObject.Rank != BF3_MAX_RANK)
            {
                statsObject.ScoreToRankUpPercent = (float) (statsObject.RankCurrentRelativeScore / statsObject.RankMaxRelativeScore * 100);
            }

            return statsObject;
        }

        public static ZPlayerStatsBase BF4StatsHandler(IDictionary<string, float> statsDictionary)
        {
            var statsObject = new ZBF4PlayerStats(statsDictionary);
            var ranksDetails = _LoadJsonByGame(ZGame.BF4);

            _MapAutoMapperProperties(statsObject, statsDictionary);

            // set manually assigned properties
            var currentRank = ranksDetails[(int)statsObject.Rank];

            statsObject.RankName = currentRank["rankName"].Value<string>();
            statsObject.RankMaxRelativeScore = currentRank["xpRelative"].Value<float>();
            statsObject.RankMaxLongScore = currentRank["xpTotal"].Value<double>();

            // calculate calculated properties
            statsObject.RankCurrentRelativeScore = statsObject.RankCurrentLongScore - statsObject.RankMaxLongScore;
            statsObject.ScoreToRankUp = statsObject.RankMaxRelativeScore - statsObject.RankCurrentRelativeScore;
            statsObject.Accuracy = statsObject.Hits * 1f / statsObject.Shots * 100; // * 1f it is divide result conversion to float
            statsObject.WL = statsObject.Wins * 1f / statsObject.Losses; // * 1f it is divide result conversion to float
            statsObject.KD = statsObject.Kills * 1f / statsObject.Deaths; // * 1f it is divide result conversion to float
            statsObject.AssaultCurrentRelativeScore = (float) statsObject.AssaultCurrentLongScore -
                                                      statsObject.AssaultMaxRelativeScore *
                                                      statsObject.AssaultStartCount;
            statsObject.AssaultStarProgressPercent = statsObject.AssaultCurrentRelativeScore / statsObject.AssaultMaxRelativeScore * 100;
            statsObject.EngineerCurrentRelativeScore = (float) statsObject.EngineerCurrentLongScore -
                                                      statsObject.EngineerMaxRelativeScore *
                                                      statsObject.EngineerStartCount;
            statsObject.EngineerStarProgressPercent = statsObject.EngineerCurrentRelativeScore / statsObject.EngineerMaxRelativeScore * 100;
            statsObject.ReconCurrentRelativeScore = (float) statsObject.ReconCurrentLongScore -
                                                      statsObject.ReconMaxRelativeScore *
                                                      statsObject.ReconStartCount;
            statsObject.ReconStarProgressPercent = statsObject.ReconCurrentRelativeScore / statsObject.ReconMaxRelativeScore * 100;
            statsObject.SupportCurrentRelativeScore = (float) statsObject.SupportCurrentLongScore -
                                                      statsObject.SupportMaxRelativeScore *
                                                      statsObject.SupportStartCount;
            statsObject.SupportStarProgressPercent = statsObject.SupportCurrentRelativeScore / statsObject.SupportMaxRelativeScore * 100;
            statsObject.CommanderCurrentRelativeScore = (float) statsObject.CommanderCurrentLongScore -
                                                      statsObject.CommanderMaxRelativeScore *
                                                      statsObject.CommanderStartCount;
            statsObject.CommanderStarProgressPercent = statsObject.CommanderCurrentRelativeScore / statsObject.CommanderMaxRelativeScore * 100;
            statsObject.TimePlayedHours = statsObject.TimeSeconds / 60 / 60;
            statsObject.Rounds = statsObject.Wins + statsObject.Losses;

            return statsObject;
        }

        #region Private helpers

        private static JArray _LoadJsonByGame(ZGame game)
        {
            JArray jObject;

            // convert game to resource key
            var resourceKey = game.ToString().ToLowerInvariant();

            // get resource by resource key
            using (var streamReader = new StreamReader(ZInternalResource.GetResourceStream($"stats.{resourceKey}_rankDetails.json")))
            {
                // load all json content
                var jsonContent = streamReader.ReadToEnd();

                // parse into jObject
                jObject = JArray.Parse(jsonContent);
            }

            return jObject;
        }

        private static void _MapAutoMapperProperties(ZPlayerStatsBase statsInstance, IDictionary<string, float> statsDictionary)
        {
            var type = statsInstance.GetType();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var autoMapperProperties = properties.Where(p => p.GetCustomAttributes<ZStatsMapperAttribute>(false).Count() != 0);

            foreach (var autoMapperProperty in autoMapperProperties)
            {
                var attribute = autoMapperProperty.GetCustomAttribute<ZStatsMapperAttribute>(false);
                var mapperKey = attribute.MapFromDictionaryKey;
                var targetType = attribute.TargetType ?? typeof(float);
                var value = statsDictionary[mapperKey];

                autoMapperProperty.SetValue(statsInstance, Convert.ChangeType(value, targetType));
            }
        }

        private static double _SumKeys(IDictionary<string, float> statsDictionary, params string[] keys)
        {
            return keys.Aggregate(.0d, (v, k) => v + statsDictionary[k]);
        }

        #endregion
    }
}