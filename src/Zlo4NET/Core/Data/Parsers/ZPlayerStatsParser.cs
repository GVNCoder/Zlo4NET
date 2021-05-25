using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Newtonsoft.Json.Linq;

using Zlo4NET.Api.DTOs;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Core.Extensions;
using Zlo4NET.Core.Services;
using Zlo4NET.Core.ZClientAPI;

namespace Zlo4NET.Core.Data.Parsers
{
    internal class ZPlayerStatsParser : IZPlayerStatsParser
    {
        private static int[] maxranks { get; } =
        {
            1000,
            7000,
            10000,
            11000,
            12000,
            13000,13000,
            14000,
            15000,15000,
            19000,
            20000,20000,20000,
            30000,30000,30000,30000,30000,30000,30000,30000,
            40000,40000,40000,40000,40000,40000,40000,
            50000,50000,50000,50000,50000,50000,50000,50000,
            55000,55000,
            60000,60000,60000,60000,60000,
            80000,
            230000
        };

        #region Internal types

        // ReSharper disable once InconsistentNaming
        private class _Stats
        {
            public IDictionary<string, float> Stats;
            public JObject RankInfo;
        }

        #endregion

        private readonly ZLogger _logger;

        #region Ctor

        public ZPlayerStatsParser()
        {
            _logger = ZLogger.Instance;
        }

        #endregion

        #region IZPlayerStatsParser interface

        public ZPlayerBaseStats Parse(ZPacket packet)
        {
            var statsObject = _ParseStatsObject(packet);

            return null;
        }

        #endregion

        #region Private helpers

        private static _Stats _ParseStatsObject(ZPacket packet)
        {
            _Stats stats = null;

            using (var memoryStream = new MemoryStream(packet.Payload, false))
            using (var binaryReader = new BinaryReader(memoryStream, Encoding.ASCII))
            {
                // parse packet payload
                var targetGame = (ZGame) binaryReader.ReadByte();
                var countOfStats = binaryReader.ReadZUInt16();
                var statsDictionary = new Dictionary<string, float>(countOfStats);

                for (ushort i = 0; i < countOfStats; i++)
                {
                    var name = binaryReader.ReadZString();
                    var value = binaryReader.ReadZFloat();

                    statsDictionary.Add(name, value);
                }

                // load json rank info
                var rankInfo = _LoadJsonByGame(targetGame);

                // create internal stats object
                stats = new _Stats
                {
                    Stats = statsDictionary,
                    RankInfo = rankInfo
                };
            }

            return stats;
        }

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

        #endregion

        //public ZBF3Stats ParseBF3Stats(ZPacket[] packets)
        //{
        //    var jStats = _statsTemplates[ZStringConstants.BF3ResourceKey] as JObject;

        //    _assign(statsDictionary, jStats);

        //    var statsToken = jStats["stats"];
        //    var rank = statsToken.Value<int>("rank");

        //    statsToken["rankname"] = GetBF3RankName(rank);

        //    var scoresToken = statsToken["scores"];
        //    var vehiclesScoreValue = SumIfNum(
        //        scoresToken["vehicleaa"],
        //        scoresToken["vehicleah"],
        //        scoresToken["vehicleifv"],
        //        scoresToken["vehiclejet"],
        //        scoresToken["vehiclembt"],
        //        scoresToken["vehiclesh"],
        //        scoresToken["vehiclelbt"],
        //        scoresToken["vehicleart"]);
        //    var combatScore = SumIfNum(
        //                     scoresToken["support"],
        //                     scoresToken["assault"],
        //                     scoresToken["engineer"],
        //                     scoresToken["recon"]) +
        //                 vehiclesScoreValue;
        //    var allScore = combatScore + SumIfNum(scoresToken["unlock"], scoresToken["award"], scoresToken["special"]);

        //    scoresToken["maxxp"] = GetRankMaxScore(rank);
        //    scoresToken["shortxp"] = allScore - Sumfrom0to(rank);
        //    scoresToken["longxp"] = allScore;

        //    var resultStats = new ZBF3Stats(jStats);
        //    return resultStats;
        //}

        //public ZBF4Stats ParseBF4Stats(ZPacket[] packets)
        //{
        //    var packet = packets.First();
        //    var statsDictionary = _parseStatsDictionary(packet);
        //    var jStats = _statsTemplates[ZStringConstants.BF4ResourceKey] as JObject;
        //    var jRanksDetails = _LoadResourceByName("stats.bf4_details.json");

        //    _assign(statsDictionary, jStats);

        //    var scoreValue = jStats["player"]["score"].ToObject<long>();
        //    var jRank = jStats["player"]["rank"] as JObject;
        //    var rankValue = jRank.Value<int>("nr");

        //    var jRankDetail = jRanksDetails[rankValue.ToString()] as JObject;
        //    var jNextRankDetail = jRanksDetails[(rankValue + 1).ToString()] as JObject;

        //    var currentMin = jRankDetail["XP Min Total"].ToObject<long>();
        //    var relativeScore = scoreValue - currentMin;
        //    var currentMax = jNextRankDetail["XP Min Relative"].ToObject<long>();

        //    jRank["name"] = jRankDetail["Rank Title"];
        //    jRank["Unlocks"] = jRankDetail["Unlocks"];
        //    jRank["Short XP"] = relativeScore;
        //    jRank["Long XP"] = scoreValue;
        //    jRank["needed"] = currentMax - relativeScore;
        //    jRank["Max XP"] = currentMax;

        //    //kits
        //    foreach (var jKitProperty in ((JObject)jStats["stats"]["kits"]).Properties())
        //    {
        //        var jKit = jKitProperty.Value as JObject;

        //        var kitScore = jKit["score"].ToObject<double>();
        //        var max = jKit["stars"]["Max"].ToObject<double>();
        //        var kitStars = kitScore / max;

        //        jKit["stars"]["count"] = (int) kitStars;

        //        var shortCurrent = kitScore - ((int) kitStars * max);

        //        jKit["stars"]["shortCurr"] = shortCurrent;
        //        jKit["stars"]["progress"] = shortCurrent / max * 100;
        //    }

        //    var resultStats = new ZBF4Stats(jStats);
        //    return resultStats;
        //}

        #region Private methods



        private void _assign(IDictionary<string, float> statsDictionary, JObject jToken)
        {
            foreach (var item in jToken)
            {
                if (item.Value.Type == JTokenType.String)
                {
                    var oldstr = item.Value.ToObject<string>();
                    if (oldstr.StartsWith("stat."))
                    {
                        statsDictionary.TryGetValue(oldstr.Substring(5), out var val);
                        jToken[item.Key] = val;
                    }
                }
                else if (item.Value.Type == JTokenType.Object)
                {
                    _assign(statsDictionary, (JObject)item.Value);
                }
                else if (item.Value.Type == JTokenType.Array)
                {
                    var JAr = (JArray)item.Value;
                    foreach (var JAri in JAr)
                    {
                        if (JAri.Type == JTokenType.Object)
                        {
                            _assign(statsDictionary, (JObject)JAri);
                        }
                    }
                }
            }
        }

        private double Sumfrom0to(int index)
        {
            float finalsum = 0;
            for (int i = 0; i < index; ++i)
            {
                finalsum += GetRankMaxScore(i);
            }
            return finalsum;
        }

        public static int GetRankMaxScore(int rank)
        {
            if (rank <= 45)
            {
                return maxranks[rank];
            }
            else
            {
                if (rank == 145)
                {
                    return 0;
                }
                return 230000;
            }
        }

        public static double SumIfNum(params JToken[] objects)
        {
            double final = 0;
            foreach (var item in objects)
            {
                if (IsNum(item))
                {
                    final += (double)item;
                }
            }
            return final;
        }

        public static bool IsNum(object obj)
        {
            return double.TryParse(obj.ToString(), out double _);
        }

        #endregion
    }
}