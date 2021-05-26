using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Zlo4NET.Api.DTOs;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Core.Data;
using Zlo4NET.Core.Data.Attributes;

// ReSharper disable StringLiteralTypo

namespace Zlo4NET.Core.Helpers
{
    internal static class ZGameSpecificStatsHandlerProvider
    {
        public static ZPlayerStatsBase BF3StatsHandler(IDictionary<string, float> statsDictionary)
        {
            var statsObject = new ZBF3PlayerStats();
            var ranksDetails = _LoadJsonByGame(ZGame.BF3);

            //int index = 0;
            //foreach (var ranksDetailKeyValue in ranksDetails)
            //{
            //    var ranksDetail = ranksDetailKeyValue.Value;
            //    var sum = 0;

            //    for (int i = 0; i < index + 1; i++)
            //    {
            //        var rank = ranksDetails[i.ToString()];

            //        sum += rank["xpRelative"].ToObject<int>();
            //    }

            //    ranksDetail["xpTotal"] = sum == 0 ? ranksDetail["xpRelative"] : sum;
            //    index++;

            //    //float finalsum = 0;
            //    //for (int i = 0; i < index; ++i)
            //    //{
            //    //    //finalsum += GetRankMaxScore(i);
            //    //}
            //}

            //File.WriteAllText(@"someJson.json", ranksDetails.ToString());

            _MapAutoMapperProperties(statsObject, statsDictionary);

            // set some rank properties
            var currentRank = ranksDetails[statsObject.Rank.ToString(CultureInfo.InvariantCulture)];

            statsObject.RankName = currentRank["rankName"].ToObject<string>();
            statsObject.MaxScore = currentRank["xpTotal"].ToObject<float>();
            statsObject.ShortScore = currentRank["xpRelative"].ToObject<float>();

            // calculate some fields
            statsObject.CurrentScore = _SumKeys(statsDictionary,
                "sc_specialkit", "sc_vehiclembt", "sc_vehicleaa", "sc_vehicleah", "sc_vehiclesh", "sc_vehiclejet", "sc_vehiclelbt", "sc_vehicleart", "sc_award", "sc_support", "sc_assault", "sc_engineer", "sc_recon");
            statsObject.ScoreToRankUp = statsObject.MaxScore - statsObject.ShortScore;
            statsObject.ScoreToRankUpPercent = statsObject.MaxScore / statsObject.ShortScore * 100;
            statsObject.Accuracy = statsObject.Hits / statsObject.Shots * 100;
            statsObject.WL = statsObject.Wins / statsObject.Losses;
            statsObject.KD = statsObject.Kills / statsObject.Deaths;
            statsObject.AssaultStarProgressPercent = statsObject.AssaultMaxScore / statsObject.AssaultCurrentScore * 100;
            statsObject.EngineerStarProgressPercent = statsObject.EngineerMaxScore / statsObject.EngineerCurrentScore * 100;
            statsObject.ReconStarProgressPercent = statsObject.ReconMaxScore / statsObject.ReconCurrentScore * 100;
            statsObject.SupportStarProgressPercent = statsObject.SupportMaxScore / statsObject.SupportCurrentScore * 100;
            statsObject.TimePlayedHours = statsObject.TimeSeconds / 60 / 60;

            return statsObject;
        }

        public static ZPlayerStatsBase BF4StatsHandler(IDictionary<string, float> statsDictionary)
        {
            //var statsObject = new ZBF4PlayerStats();

            return null;
        }

        #region Private helpers

        private static JObject _LoadJsonByGame(ZGame game)
        {
            JObject jObject;

            // convert game to resource key
            var resourceKey = game.ToString().ToLowerInvariant();

            // get resource by resource key
            using (var streamReader = new StreamReader(ZInternalResource.GetResourceStream($"stats.{resourceKey}_rankDetails.json")))
            {
                // load all json content
                var jsonContent = streamReader.ReadToEnd();

                // parse into jObject
                jObject = JObject.Parse(jsonContent);
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
                var value = statsDictionary[mapperKey];

                autoMapperProperty.SetValue(statsInstance, value);
            }
        }

        private static double _SumKeys(IDictionary<string, float> statsDictionary, params string[] keys)
        {
            return keys.Aggregate(.0d, (v, k) => v += statsDictionary[k]);
        }

        #endregion
    }
}