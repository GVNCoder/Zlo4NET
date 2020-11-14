using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;
using System.IO;
using System;

using Zlo4NET.Core.ZClient.Data;
using Zlo4NET.Api.Models.Server;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Core.Extensions;
using Zlo4NET.Core.Services;
using Zlo4NET.Core.Helpers;

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

        public ZServerBase BuildServerModel(ZGame game)
        {
            ZServerBase model = null;

            switch (game)
            {
                case ZGame.BF3: model = new ZBF3Server(); break;
                case ZGame.BF4: model = new ZBF4Server(); break;
                case ZGame.BFH: model = new ZBFHServer(); break;
            }

            model.Game = game;

            return model;
        }

        public void ParsePlayers(uint id, ZServerBase model, BinaryReader reader)
        {
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

        public void ParseBase(ZServerBase model, BinaryReader reader)
        {
            model.Ip = UIntToIPAddress.Convert(reader.ReadZUInt32());
            model.Port = reader.ReadZUInt16();

            reader.SkipBytes(6); // skip 6 bytes [ INIP=4byte; INPORT=2byte; ]

            var attributes = _ParseAttributes(reader);
            var settings = _ParseSettings(attributes);

            _ParseMapRotation(model, attributes);

            model.Attributes = BuildAttributes(attributes, model.Game);
            model.Name = reader.ReadZString();
            model.Settings = settings;
            model.Players = new ObservableCollection<ZPlayer>();

            reader.SkipBytes(17); // skip 17 bytes [ GameSet=4bytes; ServerState=1byte; IGNO=1byte; MaxPlayers=1byte; NNAT=8bytes; NRES=1byte; NTOP=1byte; ]
            reader.SkipZString(); // skip string [ PGID=String; ]
            reader.SkipBytes(6); // skip 6 bytes [ PRES=1byte; SlotCapacity=1byte; SEED=4bytes; ]
            reader.SkipZString(); // skip string [ UUID=String; ]
            reader.SkipBytes(1); // skip 1 byte [ VOIP=1byte; ]
            reader.SkipZString(); // skip string [ VSTR=String; ]
        }

        private IDictionary<string, string> _ParseSettings(IDictionary<string, string> attributes)
        {
            var value = attributes["settings"]
                .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries))
                .ToDictionary(s => s.First(), s => s.Last());

            return value;
        }

        private void _ParseMapRotation(ZServerBase model, IDictionary<string, string> attributes)
        {
            if (!attributes.ContainsKey("maps") || !attributes.ContainsKey("mapsinfo"))
            {
                _logger.Warning("maps or mapsinfo no have attribute key");
            }
            else
            {
                var maps = _ParseMapList(attributes["maps"], model.Game);
                var mapsTuple = _GetMapsTuple(maps, attributes, model.Game);
                var rotation = new ZMapRotation
                {
                    Current = mapsTuple.Item1,
                    Next = mapsTuple.Item2,
                    Rotation = new ObservableCollection<ZMap>(maps)
                };

                model.MapRotation = rotation;

                // remove used keys
                attributes.Remove("maps");
                attributes.Remove("mapsinfo");
                attributes.Remove("level");
                attributes.Remove("levellocation");
            }
        }

        private Tuple<ZMap, ZMap> _GetMapsTuple(ZMap[] maps, IDictionary<string, string> attributes, ZGame game)
        {
            // local helpers
            ZMap l_ParseCurrentMapFromAttributes(IDictionary<string, string> attrs)
            {
                return new ZMap
                {
                    Name = attributes.ContainsKey("level")
                        ? _mapConverter.GetMapNameByKey(game, attributes["level"])
                        : ZStringConstants.NotAvailable,
                    GameModeName = attributes.ContainsKey("levellocation")
                        ? _gameModeConverter.GetGameModeNameByKey(game, attributes["levellocation"])
                        : ZStringConstants.NotAvailable
                };
            }

            var mapIndexes = _ParseMapIndexes(attributes);
            var indexLimit = maps.Length - 1;

            ZMap currentMap, nextMap = null;

            // calculate next map index
            var nextMapIndex = mapIndexes.Last();

            if (nextMapIndex <= indexLimit) _AssignMapModel(out nextMap, maps, nextMapIndex, ZMapRole.Next);

            // calculate current map index
            var currentMapIndex = mapIndexes.First();
            if (currentMapIndex > indexLimit)
            {
                var map = l_ParseCurrentMapFromAttributes(attributes);
                
                map.Role = ZMapRole.Current;
                currentMap = map;
            }
            else
            {
                _AssignMapModel(out currentMap, maps, currentMapIndex, ZMapRole.Current);
            }

            return new Tuple<ZMap, ZMap>(currentMap, nextMap);
        }

        private void _AssignMapModel(out ZMap mapModel, ZMap[] maps, int index, ZMapRole role)
        {
            mapModel = maps[index];
            mapModel.Role = role;
        }

        private int[] _ParseMapIndexes(IDictionary<string, string> attributes)
        {
            var mapsInfo = attributes["mapsinfo"];
            if (string.IsNullOrEmpty(mapsInfo))
                return new[] { 0, 0 };

            var value = attributes["mapsinfo"]
                .Split(';')
                .Last()
                .Split(',')
                .Select(int.Parse)
                .ToArray();
            return value;
        }

        private ZMap[] _ParseMapList(string mapString, ZGame game)
        {
            var value = mapString.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries)
                .Select(str => str.Split(','))
                .Where(a => a.Length > 1)
                .Select(a => new ZMap
                {
                    Name = _mapConverter.GetMapNameByKey(game, a[__mapNameIndex]),
                    GameModeName = _gameModeConverter.GetGameModeNameByKey(game, a[__gameModeIndex]),
                    Role = ZMapRole.Other
                })
                .ToArray();
            return value;
        }

        public void ParseSpecifics(ZServerBase model, BinaryReader reader)
        {
            switch (model.Game)
            {
                case ZGame.BF3:

                    model.PlayersCapacity = reader.ReadByte();
                    reader.SkipBytes(4); // skip 4 bytes [TOTAL_SLOTS_NUMBER=4bytes]

                    break;

                case ZGame.BF4:
                case ZGame.BFH:

                    reader.SkipBytes(4);
                    model.PlayersCapacity = reader.ReadByte();
                    reader.SkipBytes(1); // skip 1 byte [ PRIVATE_SLOTS=1byte; ]
                    model.SpectatorsCapacity = reader.ReadByte();
                    reader.SkipBytes(2);  // skip 1 byte [ PRIVATE_SPEC_SLOTS=1byte; GMRG=1byte; ]

                    var length = reader.ReadByte(); // skip more data [ RNFO=bigData; ]
                    for (var i = 0; i < length; ++i)
                    {
                        reader.SkipZString();
                        reader.SkipBytes(4);

                        var length2 = reader.ReadByte();
                        for (var j = 0; j < length2; j++)
                        {
                            reader.SkipZString();
                            reader.SkipZString();
                        }
                    }
                    reader.SkipZString();

                    break;
            }
        }

        private IDictionary<string, string> _ParseAttributes(BinaryReader reader)
        {
            var numberOfAttributes = reader.ReadByte();
            var attributes = new Dictionary<string, string>(numberOfAttributes);

            for (byte i = 0; i < numberOfAttributes; ++i)
            {
                var key = reader.ReadZString().ToLowerInvariant();
                var value = reader.ReadZString();

                attributes.Add(key, value);
            }

            _NormalizeAttributes(attributes);
            return attributes;
        }

        private void _NormalizeAttributes(IDictionary<string, string> rawAttributes)
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
        }

        private const int __mapNameIndex = 0;
        private const int __gameModeIndex = 1;
        private const string __threadName = "sl_parse";

        private readonly object _sync = new object();
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
                    using (var memStream = new MemoryStream(packet.Content, false))
                    using (var reader = new BinaryReader(memStream, Encoding.ASCII))
                    {
                        var action = (ZServerParserAction) reader.ReadByte();
                        var game = (ZGame) reader.ReadByte();
                        var serverModel = BuildServerModel(game);

                        if (_gameContext != game)
                        {
                            ResultCallback?.Invoke(null, ZServerParserAction.Ignore);
                            continue;
                        }

                        serverModel.Id = reader.ReadZUInt32();

                        switch (action)
                        {
                            case ZServerParserAction.Add:
                                ParseBase(serverModel, reader);
                                ParseSpecifics(serverModel, reader);

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
            if (_thread == null || ! _thread.IsAlive)
            {
                _thread = new Thread(new ThreadStart(_parseLoop)) { IsBackground = true, Name = __threadName };
                _thread.Start();

                _logger.Debug("Created a thread for parsing the server list");
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