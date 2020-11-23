using System;
using Newtonsoft.Json.Linq;

namespace Zlo4NET.Api.Models.Shared
{
    /// <summary>
    /// Defines the Battlefield 3 stats
    /// </summary>
    public class ZBF3Stats : ZStatsBase
    {
        #region Private fields

        private readonly JObject _stats;
        private readonly JObject _global;
        private readonly JObject _scores;
        private readonly JObject _kits;

        #endregion // Private fields

        #region Ctor

        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="raw">The raw stats content</param>
        public ZBF3Stats(JObject raw)
        {
            // null check
            _ = raw ?? throw new ArgumentNullException(nameof(raw));

            // extract needed parts
            _stats  = (JObject)raw["stats"];
            _global = (JObject)raw["stats"]["global"];
            _scores = (JObject)raw["stats"]["scores"];
            _kits   = (JObject)raw["stats"]["kits"];
        }

        #endregion // Ctor

        #region Public properties

        public override byte Rank         => _stats["rank"].ToObject<byte>();
        public string RankName            => _stats["rankname"].ToObject<string>();
        public float ShortXp              => _scores["shortxp"].ToObject<float>();
        public float MaxXp                => _scores["maxxp"].ToObject<float>();
        public float Kills                => _global["kills"].ToObject<float>();
        public float Deaths               => _global["deaths"].ToObject<float>();
        public float Wins                 => _global["wins"].ToObject<float>();
        public float Losses               => _global["losses"].ToObject<float>();
        public float Shots                => _global["shots"].ToObject<float>();
        public float Hits                 => _global["hits"].ToObject<float>();
        public float HeadShots            => _global["headshots"].ToObject<float>();
        public float LongestHeadShot      => _global["longesths"].ToObject<float>();
        public float VehicleKills         => _global["vehiclekills"].ToObject<float>();
        public float Revives              => _global["revives"].ToObject<float>();
        public float KillAssists          => _global["killassists"].ToObject<float>();
        public float Resupplies           => _global["resupplies"].ToObject<float>();
        public float Heals                => _global["heals"].ToObject<float>();
        public float Repairs              => _global["repairs"].ToObject<float>();
        public float EloGames             => _global["elo_games"].ToObject<float>();
        public float KillStreakBonus      => _global["killstreakbonus"].ToObject<float>();
        public float VehicleDestroyAssist => _global["vehicledestroyassist"].ToObject<float>();
        public float VehicleDestroyed     => _global["vehicledestroyed"].ToObject<float>();
        public float DogTags              => _global["dogtags"].ToObject<float>();
        public float AvengerKills         => _global["avengerkills"].ToObject<float>();
        public float SaviorKills          => _global["saviorkills"].ToObject<float>();
        public float Suppression          => _global["suppression"].ToObject<float>();
        public float NemesisStreak        => _global["nemesisstreak"].ToObject<float>();
        public float NemesisKills         => _global["nemesiskills"].ToObject<float>();
        public float MComDestroyed        => _global["mcomdest"].ToObject<float>();
        public float MComDefKills         => _global["mcomdefkills"].ToObject<float>();
        public float FlagCaps             => _global["flagcaps"].ToObject<float>();
        public float FlagDef              => _global["flagdef"].ToObject<float>();

        public float WL          => Wins / Losses;
        public float KD          => Kills / Deaths;
        public float UntilRankUp => MaxXp - ShortXp;
        public float Accuracy    => Hits / Shots;
        public short Time
        {
            get
            {
                // sec in game
                var rawTime = _global["time"].ToObject<double>();

                return (short)Math.Floor((rawTime / 60) / 60);
            }
        }

        public byte CurrentProgressPercent => (byte)Math.Floor((ShortXp * 100) / MaxXp);

        #region Assault

        public byte AssaultStarsCount          => _kits["assault"]["star"]["count"].ToObject<byte>();
        public float AssaultScoreMax           => _kits["assault"]["star"]["needed"].ToObject<float>();
        public float AssaultCurrentScore       => _kits["assault"]["star"]["curr"].ToObject<float>() - (AssaultScoreMax * AssaultStarsCount);
        public byte AssaultStarProgressPercent => (byte)Math.Floor((AssaultCurrentScore * 100) / AssaultScoreMax);

        #endregion // Assault

        #region Engineer

        public byte EngineerStarsCount          => _kits["engineer"]["star"]["count"].ToObject<byte>();
        public float EngineerScoreMax           => _kits["engineer"]["star"]["needed"].ToObject<float>();
        public float EngineerCurrentScore       => _kits["engineer"]["star"]["curr"].ToObject<float>() - (EngineerScoreMax * EngineerStarsCount);
        public byte EngineerStarProgressPercent => (byte)Math.Floor((EngineerCurrentScore * 100) / EngineerScoreMax);

        #endregion // Engineer

        #region Recon

        public byte ReconStarsCount          => _kits["recon"]["star"]["count"].ToObject<byte>();
        public float ReconScoreMax           => _kits["recon"]["star"]["needed"].ToObject<float>();
        public float ReconCurrentScore       => _kits["recon"]["star"]["curr"].ToObject<float>() - (ReconScoreMax * ReconStarsCount);
        public byte ReconStarProgressPercent => (byte)Math.Floor((ReconCurrentScore * 100) / ReconScoreMax);

        #endregion // Recon

        #region Support

        public byte SupportStarsCount          => _kits["support"]["star"]["count"].ToObject<byte>();
        public float SupportScoreMax           => _kits["support"]["star"]["needed"].ToObject<float>();
        public float SupportCurrentScore       => _kits["support"]["star"]["curr"].ToObject<float>() - (SupportScoreMax * SupportStarsCount);
        public byte SupportStarProgressPercent => (byte)Math.Floor((SupportCurrentScore * 100) / SupportScoreMax);

        #endregion // Support

        #endregion // Public properties
    }
}