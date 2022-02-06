using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;
using System.IO;
using System;

using Zlo4NET.Api.Models.Server;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Core.Extensions;
using Zlo4NET.Core.Services;
using Zlo4NET.Core.Helpers;
using Zlo4NET.Core.ZClientAPI;

namespace Zlo4NET.Core.Data.Parsers
{
    internal class ZServersListParser : IZServersListParser
    {
        private ZAttributesBase BuildAttributes(IDictionary<string, string> attributes, ZGame game)
        {
            ZAttributesBase attributesModel = null;
            switch (game)
            {
                case ZGame.BF3: attributesModel = new ZBF3Attributes(attributes); break;
                case ZGame.BF4: attributesModel = new ZBF4Attributes(attributes); break;
                case ZGame.BFH: attributesModel = new ZBFHAttributes(attributes); break;
            }

            return attributesModel;
        }

        public ZServerBase BuildServerModel(ZGame game, uint serverId)
        {
            ZServerBase model = null;

            switch (game)
            {
                case ZGame.BF3: model = new ZBF3Server() {Id = serverId}; break;
                case ZGame.BF4: model = new ZBF4Server() { Id = serverId }; break;
                case ZGame.BFH: model = new ZBFHServer() { Id = serverId }; break;
            }

            model.Game = game;

            return model;
        }

        public void ParsePlayers(uint id, ZServerBase model, BinaryReader reader)
        {
            //model.Id = reader.ReadZUInt32();

            var playerList = new ObservableCollection<ZPlayer>();
            var arrLen = reader.ReadByte();

            for (byte i = 0; i < arrLen; i++)
            {
                var player = new ZPlayer
                {
                    Slot = reader.ReadByte(),
                    Id = reader.ReadZUInt32(),
                    Name = reader.ReadZString(),
                    Role = ZPlayerRole.Other
                };

                playerList.Add(player);
            }

            var myPlayer = playerList.FirstOrDefault(p => p.Id == id);
            if (myPlayer != null)
            {
                myPlayer.Role = ZPlayerRole.IAm;
            }

            model.Players = playerList;
            model.CurrentPlayersNumber = (byte) playerList.Count;
        }

        public void ParseIntoServerModel(ZServerBase model, BinaryReader reader)
        {
            // at first parse bytes from reader
            // parse some basic info
            model.Ip = UIntToIPAddress.Convert(reader.ReadZUInt32());
            model.Port = reader.ReadZUInt16();

            // skip block
            reader.SkipBytes(6); // skip 6 bytes [ INIP=4byte; INPORT=2byte; ]

            // parse attributes
            var attributeCount = reader.ReadByte();
            var attributeDictionary = new Dictionary<string, string>(attributeCount);

            for (var i = 0; i < attributeCount; ++i)
            {
                var key = reader.ReadZString().ToLowerInvariant();
                var value = reader.ReadZString();

                attributeDictionary.Add(key, value);
            }

            // parse some basic info
            model.Name = reader.ReadZString();
            model.Players = new ObservableCollection<ZPlayer>();

            // skip block
            reader.SkipBytes(17); // skip 17 bytes [ GameSet=4bytes; ServerState=1byte; IGNO=1byte; MaxPlayers=1byte; NNAT=8bytes; NRES=1byte; NTOP=1byte; ]
            reader.SkipZString(); // skip string [ PGID=String; ]
            reader.SkipBytes(6); // skip 6 bytes [ PRES=1byte; SlotCapacity=1byte; SEED=4bytes; ]
            reader.SkipZString(); // skip string [ UUID=String; ]
            reader.SkipBytes(1); // skip 1 byte [ VOIP=1byte; ]
            reader.SkipZString(); // skip string [ VSTR=String; ]

            // parse game-specific data
            switch (model.Game)
            {
                case ZGame.BF3:
                    // parse specific data
                    model.PlayersCapacity = reader.ReadByte();

                    // skip block
                    reader.SkipBytes(4); // skip 4 bytes [TOTAL_SLOTS_NUMBER=4bytes]

                    break;

                case ZGame.BF4:
                case ZGame.BFH:
                    // skip block
                    reader.SkipBytes(4);

                    // parse specific data
                    var fullPlayersCapacity = reader.ReadByte();

                    // skip block
                    reader.SkipBytes(1); // skip 1 byte [ PRIVATE_SLOTS=1byte; ]

                    // parse specific data
                    model.SpectatorsCapacity = reader.ReadByte();
                    model.PlayersCapacity = (byte) (fullPlayersCapacity - model.SpectatorsCapacity);

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

                    break;

                case ZGame.None:
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // parse already received data
            // normalize the obtained attributes, because some keys can be split into several parts
            // like mapsinfo1 ... and mapsinfo2 ... mapsinfoN ...
            var normalizedAttributes = _NormalizeAttributes(attributeDictionary);

            // build server attributes from normalized dictionary
            model.Attributes = BuildAttributes(normalizedAttributes, model.Game);

            // build server settings from normalized dictionary
            model.Settings = BuildSettings(normalizedAttributes);

            // build map rotation from normalized dictionary
            model.MapRotation = BuildMapRotation(model.Game, normalizedAttributes);

            model.Ping = ZPingService.GetPing(model.Ip);
        }

        private IDictionary<string, string> BuildSettings(IDictionary<string, string> attributes)
        {
            var value = attributes["settings"]
                .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries))
                .ToDictionary(s => s.First(), s => s.Last());

            return value;
        }

        private ZMapRotation BuildMapRotation(ZGame game, IDictionary<string, string> attributes)
        {
            var rotation = new ZMapRotation(null);

            // parse available map list
            var mapList = _ParseMapList(attributes["maps"], game);

            // get current and next maps
            var mapRotationIndexes = _ParseMapIndexes(attributes.ContainsKey("mapsinfo") ? attributes["mapsinfo"] : string.Empty);
            var currentMap = new ZMap
            {
                Name = attributes.ContainsKey("level")
                    ? _mapConverter.GetMapNameByKey(game, attributes["level"])
                    : ZStringConstants.NotAvailable,
                GameModeName = attributes.ContainsKey("levellocation")
                    ? _gameModeConverter.GetGameModeNameByKey(game, attributes["levellocation"])
                    : ZStringConstants.NotAvailable,
                Role = ZMapRole.Current
            };

            rotation.Current = currentMap;

            // check if we can get access to next map in maps list rotation
            if (mapRotationIndexes != null)
            {
                var nextMapIndex = mapRotationIndexes.Last();
                if (nextMapIndex <= mapList.Count - 1)
                {
                    var nextMap = mapList[nextMapIndex];

                    nextMap.Role = ZMapRole.Next;
                    rotation.Next = nextMap;
                }
            }

            // check if we can get access to current map in maps list rotation
            var map = mapList.FirstOrDefault(
                m => m.Name == currentMap.Name && m.GameModeName == currentMap.GameModeName);

            if (map == null)
            {
                mapList.Add(currentMap);
            }
            else
            {
                map.Role = ZMapRole.Current;
            }

            rotation.Rotation = new ObservableCollection<ZMap>(mapList);

            // remove used keys
            attributes.Remove("maps");
            attributes.Remove("mapsinfo");
            attributes.Remove("level");
            attributes.Remove("levellocation");

            return rotation;
        }

        private int[] _ParseMapIndexes(string mapsInfoString)
        {
            return string.IsNullOrEmpty(mapsInfoString)
                ? null
                : mapsInfoString.Split(';')
                    .Last()
                    .Split(',')
                    .Select(int.Parse)
                    .ToArray();
        }

        private List<ZMap> _ParseMapList(string mapString, ZGame game)
        {
            // parse map list rotation
            var maps = mapString.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries) // get map name and gameMode string like key,value
                .Select(str => str.Split(',')) // get arrays where 0 index as Key + 1 index as Value
                .Where(a => a.Length > 1) // drop not full pairs
                .Select(a => new ZMap
                {
                    Name = _mapConverter.GetMapNameByKey(game, a[__mapNameIndex]),
                    GameModeName = _gameModeConverter.GetGameModeNameByKey(game, a[__gameModeIndex]),
                    Role = ZMapRole.Other
                }); // select maps

            return maps.ToList();
        }

        private IDictionary<string, string> _NormalizeAttributes(IDictionary<string, string> rawAttributes)
        {
            var keyGroups = rawAttributes.Keys
                .Where(key => key.Any(char.IsDigit))
                .GroupBy(key => key.Substring(0, key.Length - 1))
                .ToArray();

            foreach (var keyGroup in keyGroups)
            {
                var value = keyGroup.Aggregate(string.Empty, (current, key) => current + rawAttributes[key]);
                keyGroup
                    .ToList()
                    .ForEach(k => rawAttributes.Remove(k));
                rawAttributes.Add(keyGroup.Key, value);
            }

            return rawAttributes;
        }

        private const int __mapNameIndex = 0;
        private const int __gameModeIndex = 1;
        private const string __threadName = "sl_parse"; // server list parse thread

        private readonly object _sync = new object();
        private readonly object _threadInstanceLock = new object();
        private readonly IEnumerable<ZPacket> _emptyEnumerable = CollectionHelper.GetEmptyEnumerable<ZPacket>();

        private readonly uint _myId;
        private readonly ZGame _gameContext;
        private readonly ZMapNameConverter _mapConverter;
        private readonly ZGameModesConverter _gameModeConverter;
        private readonly Queue<ZPacket[]> _cache;
        private readonly ZLogger _logger;

        private Thread _thread;

        public ZServersListParser(uint myId, ZGame gameContext, ZLogger logger)
        {
            _cache = new Queue<ZPacket[]>(5);
            _mapConverter = new ZMapNameConverter();
            _gameModeConverter = new ZGameModesConverter();

            _myId = myId;
            _gameContext = gameContext;
            _logger = logger;
        }

        private void _parseLoop()
        {
            // tread loop
            while (true)
            {
                // gets data for parse
                var fParse = _emptyEnumerable;
                lock (_sync)
                {
                    if (_cache.Any()) fParse = _cache.Dequeue();
                }

                // parse all extracted packets
                foreach (var packet in fParse)
                {
                    using (var memStream = new MemoryStream(packet.Payload, false))
                    using (var reader = new BinaryReader(memStream, Encoding.ASCII))
                    {
                        var action = (ZServerParserAction) reader.ReadByte();
                        var game = (ZGame) reader.ReadByte();
                        var serverId = reader.ReadZUInt32();
                        var serverModel = BuildServerModel(game, serverId);

                        if (_gameContext != game)
                        {
                            ResultCallback?.Invoke(null, ZServerParserAction.Ignore);
                            continue;
                        }

                        switch (action)
                        {
                            case ZServerParserAction.Add:
                                ParseIntoServerModel(serverModel, reader);

                                break;
                            case ZServerParserAction.PlayersList:
                                ParsePlayers(_myId, serverModel, reader);

                                break;
                            case ZServerParserAction.Remove: break;
                        }

                        // handle parsed data
                        ResultCallback?.Invoke(serverModel, action);
                    }
                }

                Thread.Sleep(100);
            }
        }

        public Action<ZServerBase, ZServerParserAction> ResultCallback { get; set; }

        public void ParseAsync(ZPacket[] packets)
        {
            lock (_threadInstanceLock)
            {
                if (_thread == null || ! _thread.IsAlive)
                {
                    _thread = new Thread(new ThreadStart(_parseLoop)) { IsBackground = true, Name = __threadName };
                    _thread.Start();

                    _logger.Debug("Created a thread for parsing the server list");
                }
            }

            lock (_sync)
            {
                _cache.Enqueue(packets);

                _logger.Debug($"Incoming packets count: {packets.Length}");
            }
        }

        public void Close()
        {
            _thread?.Abort();
            
            _logger.Debug("Thread for the server list parsing was aborted");
        }
    }
}