using System;
using Newtonsoft.Json.Linq;

namespace Zlo4NET.Api.Models.Shared
{
    /// <summary>
    /// Defines the Battlefield 4 stats
    /// </summary>
    public class ZBF4Stats : ZStatsBase
    {
        #region Private fields

        private readonly JObject _player;
        private readonly JObject _stats;
        private readonly JObject _rank;
        private readonly JObject _kits;

        #endregion // Private fields

        #region Ctor

        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="raw">The raw stats content</param>
        public ZBF4Stats(JObject raw)
        {
            // null check
            _ = raw ?? throw new ArgumentNullException(nameof(raw));

            // extract needed parts
            _player = (JObject)raw["player"];
            _stats = (JObject)raw["stats"];
            _rank = (JObject)raw["player"]["rank"];
            _kits = (JObject)raw["stats"]["kits"];
        }

        #endregion // Ctor

        #region Public properties

        public override byte Rank => _rank["nr"].ToObject<byte>();
        public string RankName => _rank["name"].ToObject<string>();
        public float ShortXp => _rank["Short XP"].ToObject<float>();
        public float MaxXp => _rank["Max XP"].ToObject<float>();
        public float Skill => _stats["skill"].ToObject<float>();
        public float Kills => _stats["kills"].ToObject<float>();
        public float Deaths => _stats["deaths"].ToObject<float>();
        public float HeadShots => _stats["headshots"].ToObject<float>();
        public float Shots => _stats["shotsFired"].ToObject<float>();
        public float Hits => _stats["shotsHit"].ToObject<float>();
        public float Suppression => _stats["suppressionAssists"].ToObject<float>();
        public float AvengerKills => _stats["avengerKills"].ToObject<float>();
        public float SaviorKills => _stats["saviorKills"].ToObject<float>();
        public float NemesisKills => _stats["nemesisKills"].ToObject<float>();
        public float Wins => _stats["numWins"].ToObject<float>();
        public float Losses => _stats["numLosses"].ToObject<float>();
        public float KillStreakBonus => _stats["killStreakBonus"].ToObject<float>();
        public float NemesisStreak => _stats["nemesisStreak"].ToObject<float>();
        public float MComDefKills => _stats["mcomDefendKills"].ToObject<float>();
        public float Resupplies => _stats["resupplies"].ToObject<float>();
        public float Repairs => _stats["repairs"].ToObject<float>();
        public float Heals => _stats["heals"].ToObject<float>();
        public float Revives => _stats["revives"].ToObject<float>();
        public float LongestHeadShot => _stats["longestHeadshot"].ToObject<float>();
        public float FlagDef => _stats["flagDefend"].ToObject<float>();
        public float FlagCaps => _stats["flagCaptures"].ToObject<float>();
        public float KillAssists => _stats["killAssists"].ToObject<float>();
        public float VehicleDestroyed => _stats["vehiclesDestroyed"].ToObject<float>();
        public float DogTags => _stats["dogtagsTaken"].ToObject<float>();
        public short GamesPlayed => (short)Math.Floor(Wins + Losses);


        public float WL => Wins / Losses;
        public float KD => Kills / Deaths;
        public float UntilRankUp => MaxXp - ShortXp;
        public float Accuracy => Hits / Shots;

        public short Time
        {
            get
            {
                // sec in game
                var rawTime = _player["timePlayed"].ToObject<double>();

                return (short)Math.Floor((rawTime / 60) / 60);
            }
        }

        public byte CurrentProgressPercent => (byte)Math.Floor((ShortXp * 100) / MaxXp);

        #region Assault

        public byte AssaultStarsCount => _kits["assault"]["stars"]["count"].ToObject<byte>();
        public float AssaultScoreMax => _kits["assault"]["stars"]["Max"].ToObject<float>();
        public float AssaultCurrentScore => _kits["assault"]["stars"]["curr"].ToObject<float>() - (AssaultScoreMax * AssaultStarsCount);
        public byte AssaultStarProgressPercent => (byte)Math.Floor((AssaultCurrentScore * 100) / AssaultScoreMax);

        #endregion // Assault

        #region Engineer

        public byte EngineerStarsCount => _kits["engineer"]["stars"]["count"].ToObject<byte>();
        public float EngineerScoreMax => _kits["engineer"]["stars"]["Max"].ToObject<float>();
        public float EngineerCurrentScore => _kits["engineer"]["stars"]["curr"].ToObject<float>() - (EngineerScoreMax * EngineerStarsCount);
        public byte EngineerStarProgressPercent => (byte)Math.Floor((EngineerCurrentScore * 100) / EngineerScoreMax);

        #endregion // Engineer

        #region Recon

        public byte ReconStarsCount => _kits["recon"]["stars"]["count"].ToObject<byte>();
        public float ReconScoreMax => _kits["recon"]["stars"]["Max"].ToObject<float>();
        public float ReconCurrentScore => _kits["recon"]["stars"]["curr"].ToObject<float>() - (ReconScoreMax * ReconStarsCount);
        public byte ReconStarProgressPercent => (byte)Math.Floor((ReconCurrentScore * 100) / ReconScoreMax);

        #endregion // Recon

        #region Support

        public byte SupportStarsCount => _kits["support"]["stars"]["count"].ToObject<byte>();
        public float SupportScoreMax => _kits["support"]["stars"]["Max"].ToObject<float>();
        public float SupportCurrentScore => _kits["support"]["stars"]["curr"].ToObject<float>() - (SupportScoreMax * SupportStarsCount);
        public byte SupportStarProgressPercent => (byte)Math.Floor((SupportCurrentScore * 100) / SupportScoreMax);

        #endregion // Support

        #endregion // Public properties
    }
}