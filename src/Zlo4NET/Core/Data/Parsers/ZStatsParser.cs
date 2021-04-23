using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;

using Newtonsoft.Json.Linq;

using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Core.Extensions;
using Zlo4NET.Core.Helpers;
using Zlo4NET.Core.Services;
using Zlo4NET.Core.ZClientAPI;

namespace Zlo4NET.Core.Data.Parsers
{
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    internal class ZStatsParser : IZStatsParser
    {
        private readonly JObject _statsTemplates;

        public ZStatsParser()
        {
            _statsTemplates = _LoadResourceByName("stats.template.json");
        }

        public ZBF3Stats ParseBF3Stats(ZPacket[] packets)
        {
            var packet = packets.First();
            var statsDictionary = _parseStatsDictionary(packet);
            var jStats = _statsTemplates[ZStringConstants.BF3ResourceKey] as JObject;

            _assign(statsDictionary, jStats);

            var statsToken = jStats["stats"];
            var rank = statsToken.Value<int>("rank");

            statsToken["rankname"] = GetBF3RankName(rank);

            var scoresToken = statsToken["scores"];
            var vehiclesScoreValue = SumIfNum(
                scoresToken["vehicleaa"],
                scoresToken["vehicleah"],
                scoresToken["vehicleifv"],
                scoresToken["vehiclejet"],
                scoresToken["vehiclembt"],
                scoresToken["vehiclesh"],
                scoresToken["vehiclelbt"],
                scoresToken["vehicleart"]);
            var combatScore = SumIfNum(
                             scoresToken["support"],
                             scoresToken["assault"],
                             scoresToken["engineer"],
                             scoresToken["recon"]) +
                         vehiclesScoreValue;
            var allScore = combatScore + SumIfNum(scoresToken["unlock"], scoresToken["award"], scoresToken["special"]);

            scoresToken["maxxp"] = GetRankMaxScore(rank);
            scoresToken["shortxp"] = allScore - Sumfrom0to(rank);
            scoresToken["longxp"] = allScore;

            var resultStats = new ZBF3Stats(jStats);
            return resultStats;
        }

        public ZBF4Stats ParseBF4Stats(ZPacket[] packets)
        {
            var packet = packets.First();
            var statsDictionary = _parseStatsDictionary(packet);
            var jStats = _statsTemplates[ZStringConstants.BF4ResourceKey] as JObject;
            var jRanksDetails = _LoadResourceByName("stats.bf4_details.json");

            _assign(statsDictionary, jStats);

            var scoreValue = jStats["player"]["score"].ToObject<long>();
            var jRank = jStats["player"]["rank"] as JObject;
            var rankValue = jRank.Value<int>("nr");

            var jRankDetail = jRanksDetails[rankValue.ToString()] as JObject;
            var jNextRankDetail = jRanksDetails[(rankValue + 1).ToString()] as JObject;

            var currentMin = jRankDetail["XP Min Total"].ToObject<long>();
            var relativeScore = scoreValue - currentMin;
            var currentMax = jNextRankDetail["XP Min Relative"].ToObject<long>();

            jRank["name"] = jRankDetail["Rank Title"];
            jRank["Unlocks"] = jRankDetail["Unlocks"];
            jRank["Short XP"] = relativeScore;
            jRank["Long XP"] = scoreValue;
            jRank["needed"] = currentMax - relativeScore;
            jRank["Max XP"] = currentMax;

            //kits
            foreach (var jKitProperty in ((JObject)jStats["stats"]["kits"]).Properties())
            {
                var jKit = jKitProperty.Value as JObject;

                var kitScore = jKit["score"].ToObject<double>();
                var max = jKit["stars"]["Max"].ToObject<double>();
                var kitStars = kitScore / max;

                jKit["stars"]["count"] = (int) kitStars;

                var shortCurrent = kitScore - ((int) kitStars * max);

                jKit["stars"]["shortCurr"] = shortCurrent;
                jKit["stars"]["progress"] = shortCurrent / max * 100;
            }

            var resultStats = new ZBF4Stats(jStats);
            return resultStats;
        }

        #region Private methods

        private JObject _LoadResourceByName(string name)
        {
            JObject jObject;
            using (var sr = new StreamReader(ZInternalResource.GetResourceStream(name)))
            {
                var content = sr.ReadToEnd();
                jObject = JObject.Parse(content);
            }

            return jObject;
        }

        private IDictionary<string, float> _parseStatsDictionary(ZPacket packet)
        {
            IDictionary<string, float> stats;

            using (var memory = new MemoryStream(packet.Payload, false))
            using (var br = new BinaryReader(memory, Encoding.ASCII))
            {
                br.SkipBytes(1); // skip game id

                var count = br.ReadZUInt16();
                stats = new Dictionary<string, float>(count);

                for (ushort i = 0; i < count; i++)
                {
                    var statName = br.ReadZString();
                    var statValue = br.ReadZFloat();

                    stats.Add(statName, statValue);
                }
            }

            return stats;
        }

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

        private static string[] ranknames { get; } =
        {
            "Noob",
            "Private First Class","Private First Class 1 Star","Private First Class 2 Stars","Private First Class 3 Stars",
            "Lance Corporal","Lance Corporal 1 Star","Lance Corporal 2 Stars", "Lance Corporal 3 Stars",
            "Corporal","Corporal 1 Star", "Corporal 2 Stars","Corporal 3 Stars",
            "Sergeant","Sergeant 1 Star","Sergeant 2 Stars","Sergeant 3 Stars",
            "Staff Sergeant","Staff Sergeant 1 Star","Staff Sergeant 2 Stars",
            "Gunnery Sergeant","Gunnery Sergeant 1 Star","Gunnery Sergeant 2 Stars",
            "Master Sergeant","Master Sergeant 1 Star","Master Sergeant 2 Stars",
            "First Sergeant","First Sergeant 1 Star","First Sergeant 2 Stars",
            "Master Gunnery Sergeant","Master Gunnery Sergeant 1 Star","Master Gunnery Sergeant 2 Stars",
            "Sergeant Major","Sergeant Major 1 Star","Sergeant Major 2 Star",
            "Warrant Officer 1","Chief Warrant Officer 2","Chief Warrant Officer 3","Chief Warrant Officer 4","Chief Warrant Officer 5",
            "Second Lieutenant","First Lieutenant",
            "Captain","Major","Lt. Colonel","Colonel"
        };

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

        private double Sumfrom0to(int index)
        {
            float finalsum = 0;
            for (int i = 0; i < index; ++i)
            {
                finalsum += GetRankMaxScore(i);
            }
            return finalsum;
        }

        public static string GetBF3RankName(int rank)
        {
            if (rank <= 45)
            {
                return ranknames[rank];
            }
            else
            {
                return $"Colonel Service Star {rank - 45}";
            }
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