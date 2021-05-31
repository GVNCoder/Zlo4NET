using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Reflection;

using Zlo4NET.Api.DTOs;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Core.Data.Attributes;
using Zlo4NET.Core.Extensions;
using Zlo4NET.Core.Services;
using Zlo4NET.Core.ZClientAPI;

namespace Zlo4NET.Core.Data.Parsers
{
    internal class ZInstalledGamesParser : IZInstalledGamesParser
    {
        private readonly IDictionary<string, ZGame> _supportedGamesMetadata;

        #region Ctor

        public ZInstalledGamesParser()
        {
            _supportedGamesMetadata = new Dictionary<string, ZGame>();

            // cache supported games
            var enumFields = typeof(ZGame).GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (var field in enumFields)
            {
                var metadata = field.GetCustomAttribute<ZGameEnumMetadataAttribute>(false);
                if (metadata != null)
                {
                    _supportedGamesMetadata.Add(metadata.InternalName, (ZGame) field.GetRawConstantValue());
                }
            }
        }

        #endregion

        public ZGameCollection Parse(ZPacket packet)
        {
            ZGameCollection gameCollection;

            using (var memoryStream = new MemoryStream(packet.Payload, false))
            using (var binaryReader = new BinaryReader(memoryStream, Encoding.ASCII))
            {
                var isX64OperatingSystem = binaryReader.ReadBoolean();
                var gamesCount = binaryReader.ReadZUInt32();
                var games = new List<ZInstalledGame>((int) gamesCount);

                for (var i = 0; i < gamesCount; i++)
                {
                    var item = new ZInstalledGame
                    {
                        RunnableName = binaryReader.ReadZString(),
                        InternalName = binaryReader.ReadZString(),
                        ReadableName = binaryReader.ReadZString()
                    };

                    item.Game = _GetZGameByInternalName(item.InternalName);
                    item.Architecture = _GetZGameArchitectureByRunnableName(item.RunnableName);

                    games.Add(item);
                }

                gameCollection = new ZGameCollection
                {
                    IsX64OperatingSystem = isX64OperatingSystem,
                    Games = games
                        .Where(i => i.Game != ZGame.None)
                        .ToArray()
                };
            }

            return gameCollection;
        }

        #region Private methods

        private ZGame _GetZGameByInternalName(string internalName)
        {
            return _supportedGamesMetadata.TryGetValue(internalName, out var game) ? game : ZGame.None;
        }

        private static ZGameArchitecture _GetZGameArchitectureByRunnableName(string runnableGame)
        {
            return runnableGame.EndsWith(ZGameArchitecture.x64.ToString())
                ? ZGameArchitecture.x64
                : ZGameArchitecture.x32;
        }

        #endregion
    }
}