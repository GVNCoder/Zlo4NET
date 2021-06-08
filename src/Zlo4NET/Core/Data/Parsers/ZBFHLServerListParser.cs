using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Collections.Generic;

using Zlo4NET.Api.DTOs;
using Zlo4NET.Api.Shared;
using Zlo4NET.Core.Extensions;
using Zlo4NET.Core.Helpers;

// ReSharper disable UseObjectOrCollectionInitializer
// ReSharper disable InconsistentNaming

namespace Zlo4NET.Core.Data.Parsers
{
    internal class ZBFHLServerListParser : ZServerListParserBase
    {
        #region Ctor

        public ZBFHLServerListParser(uint currentUserId) : base(currentUserId, ZGame.BFHL)
        {
        }

        #endregion

        #region Abstract methods

        protected override ZServerBase ParseServerModel(BinaryReader binaryReader)
        {
            var model = new ZBFHLServer();

            model.Id = binaryReader.ReadZUInt32();

            // parse the underlying data first
            _ParseServerIps(model, binaryReader);
            _ParseServerAttributes(model, binaryReader);

            model.Name = binaryReader.ReadZString();

            // skip data block
            binaryReader.SkipBytes(17); // skip 17 bytes [ GameSet=4bytes; ServerState=1byte; IGNO=1byte; MaxPlayers=1byte; NNAT=8bytes; NRES=1byte; NTOP=1byte; ]
            binaryReader.SkipZString(); // skip string [ PGID=String; ]
            binaryReader.SkipBytes(6); // skip 6 bytes [ PRES=1byte; SlotCapacity=1byte; SEED=4bytes; ]
            binaryReader.SkipZString(); // skip string [ UUID=String; ]
            binaryReader.SkipBytes(1); // skip 1 byte [ VOIP=1byte; ]
            binaryReader.SkipZString(); // skip string [ VSTR=String; ]

            // skip block
            binaryReader.SkipBytes(4);

            model.PlayersCapacity = binaryReader.ReadByte();

            // skip block
            binaryReader.SkipBytes(1); // skip 1 byte [ PRIVATE_SLOTS=1byte; ]

            //model.SpectatorsCapacity = binaryReader.ReadByte();

            // skip block
            //reader.SkipBytes(2);  // skip 1 byte [ PRIVATE_SPEC_SLOTS=1byte; GMRG=1byte; ]
            //var skipLength = reader.ReadByte(); // skip more data [ RNFO=bigData; ]
            //for (var i = 0; i < skipLength; ++i)
            //{
            //    reader.SkipZString();
            //    reader.SkipBytes(4);

            //    var skipLength2 = reader.ReadByte();
            //    for (var j = 0; j < skipLength2; j++)
            //    {
            //        reader.SkipZString();
            //        reader.SkipZString();
            //    }
            //}
            //reader.SkipZString();

            return model;
        }

        protected override ZServerBase ParseServerPlayers(BinaryReader binaryReader)
        {
            var model = new ZBFHLServer();

            // begin parse
            model.Id = binaryReader.ReadZUInt32();

            var playersList = new List<ZPlayer>();
            var countOfPlayers = binaryReader.ReadByte();

            for (var i = 0; i < countOfPlayers; i++)
            {
                var playerSlot = binaryReader.ReadByte();
                var playerId = binaryReader.ReadZUInt32();
                var playerName = binaryReader.ReadZString();

                var player = new ZPlayer
                {
                    Slot = playerSlot,
                    Id = playerId,
                    Name = playerName,
                    Role = ZPlayerRole.InServerPlayer
                };

                playersList.Add(player);
            }

            // set authorized player
            var authorizedPlayer = playersList.FirstOrDefault(p => p.Id == _currentUserId);
            if (authorizedPlayer != null)
            {
                authorizedPlayer.Role = ZPlayerRole.AuthorizedPlayer;
            }

            model.PlayersList = playersList;
            model.CurrentPlayersCount = playersList.Count;

            return model;
        }

        protected override ZServerBase ParseRemovedServerModel(BinaryReader binaryReader)
        {
            var model = new ZBFHLServer();

            // begin parse
            model.Id = binaryReader.ReadZUInt32();

            return model;
        }

        #endregion

        #region Private helpers

        private static int[] _ParseMapRotationIndexes(string mapsInfoString)
        {
            return string.IsNullOrEmpty(mapsInfoString)
                ? null
                : mapsInfoString.Split(';')
                    .Last()
                    .Split(',')
                    .Select(int.Parse)
                    .ToArray();
        }
        private List<ZMap> _ParseMapList(string mapsAttributeValue)
        {
            ZMap ParseMap_local(string[] keyValue)
            {
                string mapName = null;
                string gameModeName = null;

                // key - value
                if (keyValue.Length == 2)
                {
                    mapName = _mapNameConverter.GetMapNameByKey(keyValue.First());
                    gameModeName = _gameModeConverter.GetGameModeNameByKey(keyValue.Last());
                }
                else if (keyValue.Length == 0 || keyValue.Length > 2)
                {
                    _logger.Warning($"The string containing the map and game mode data is not in the correct format: {keyValue.Aggregate((stringSum, i) => stringSum + i)}");
                }
                else // so, then we have one element
                {
                    // if it is the map name, then Ok
                    // if no, then wtf ?
                    mapName = _mapNameConverter.GetMapNameByKey(keyValue.Single());
                }

                var mapModel = new ZMap
                {
                    Name = mapName,
                    GameModeName = gameModeName,
                    InRotationPosition = ZMapInMapRotation.InRotation,
                    RawKeys = keyValue
                };

                return mapModel;
            }

            // parse map list rotation
            // get map name and gameMode strings like key-value
            var maps = mapsAttributeValue.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(str => str.Split(','))
                .Select(ParseMap_local)
                .Where(i => i != null)
                .ToList();

            return maps;
        }
        private ZMapRotation _CreateMapRotation(IDictionary<string, string> attributes)
        {
            var mapList = _ParseMapList(attributes["maps"]);
            var mapRotationIndexes = _ParseMapRotationIndexes(attributes.ContainsKey("mapsinfo") ? attributes["mapsinfo"] : string.Empty);
            var rotation = new ZMapRotation { Rotation = mapList };

            // try find out Current and Next maps in map rotation
            if (mapRotationIndexes == null || mapRotationIndexes.Length != 2)
            {
                return rotation;
            }

            // find current map
            var currentMapIndex = mapRotationIndexes.First();
            var currentMapModel = mapList.ElementAtOrDefault(currentMapIndex);

            if (currentMapModel == null)
            {
                var mapName = _mapNameConverter.GetMapNameByKey(attributes["level"]);
                var gameModeName = _gameModeConverter.GetGameModeNameByKey(attributes["levellocation"]);

                currentMapModel = new ZMap
                {
                    Name = mapName,
                    GameModeName = gameModeName,
                    RawKeys = new[] { attributes["level"], attributes["levellocation"] }
                };

                mapList.Add(currentMapModel);
            }

            currentMapModel.InRotationPosition = ZMapInMapRotation.CurrentInRotation;
            rotation.Current = currentMapModel;

            // find next map
            var nextMapIndex = mapRotationIndexes.Last();
            var nextMapModel = mapList.ElementAtOrDefault(nextMapIndex);

            // ReSharper disable once InvertIf
            if (nextMapModel != null)
            {
                nextMapModel.InRotationPosition = ZMapInMapRotation.NextInRotation;
                rotation.Next = nextMapModel;
            }

            return rotation;
        }
        private static IDictionary<string, string> _CreateSettings(IDictionary<string, string> attributes)
        {
            var value = attributes["settings"]
                .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries))
                .ToDictionary(s => s.First(), s => s.Last());

            return value;
        }
        private static ZServerAttributes _CreateAndMapAttributes(IDictionary<string, string> attributes)
        {
            var serverAttributes = new ZServerAttributes();
            var properties = typeof(ZServerAttributes)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public);

            // map attribute key-value to property
            foreach (var property in properties)
            {
                var propertyKey = property.Name.ToLowerInvariant();

                if (attributes.TryGetValue(propertyKey, out var attributeValue))
                {
                    property.SetValue(serverAttributes, attributeValue);
                }
            }

            return serverAttributes;
        }
        private static IDictionary<string, string> _NormalizeAttributes(IDictionary<string, string> attributes)
        {
            var keyGroups = attributes.Keys
                .Where(key => key.Any(char.IsDigit))
                .GroupBy(key => key.Substring(0, key.Length - 1));

            foreach (var keyGroup in keyGroups)
            {
                var value = keyGroup.Aggregate(string.Empty, (current, key) => current + attributes[key]);

                keyGroup.ToList()
                    .ForEach(k => attributes.Remove(k));

                attributes.Add(keyGroup.Key, value);
            }

            return attributes;
        }
        private void _ParseServerAttributes(ZServerBase model, BinaryReader binaryReader)
        {
            var attributeCount = binaryReader.ReadByte();
            var attributeDictionary = new Dictionary<string, string>(attributeCount);

            for (var i = 0; i < attributeCount; i++)
            {
                var key = binaryReader.ReadZString().ToLowerInvariant();
                var value = binaryReader.ReadZString();

                attributeDictionary.Add(key, value);
            }

            // normalize the obtained attributes, because some keys can be split into several parts
            // like mapsinfo1 ... mapsinfo2 ... mapsinfoN ...
            var normalizedAttributes = _NormalizeAttributes(attributeDictionary);

            model.RawServerAttributesDictionary = normalizedAttributes;
            model.Attributes = _CreateAndMapAttributes(normalizedAttributes);
            model.Settings = _CreateSettings(normalizedAttributes);
            model.MapRotation = _CreateMapRotation(normalizedAttributes);
        }
        private static void _ParseServerIps(ZServerBase model, BinaryReader binaryReader)
        {
            var ip = ZUIntToIpAddress.Convert(binaryReader.ReadZUInt32());
            var port = binaryReader.ReadZUInt16();

            model.ServerEndPoint = new IPEndPoint(ip, port);

            var inIp = ZUIntToIpAddress.Convert(binaryReader.ReadZUInt32()); // INIP
            var inPort = binaryReader.ReadZUInt16(); // INPORT

            model.ServerInEndPoint = new IPEndPoint(inIp, inPort);
        }

        #endregion
    }
}