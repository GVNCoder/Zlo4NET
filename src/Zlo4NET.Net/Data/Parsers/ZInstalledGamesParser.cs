using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Reflection;

using Zlo4NET.Api.DTOs;
using Zlo4NET.Api.Shared;
using Zlo4NET.Data.Attributes;
using Zlo4NET.Extensions;
using Zlo4NET.Services;
using Zlo4NET.ZClientAPI;

namespace Zlo4NET.Data.Parsers
{
    internal class ZInstalledGamesParser : IZInstalledGamesParser
    {
        private readonly IList<ZGameEnumMetadataAttribute> _supportedGamesMetadata;

        #region Ctor

        public ZInstalledGamesParser()
        {
            _supportedGamesMetadata = new List<ZGameEnumMetadataAttribute>();

            // cache supported games
            foreach (var field in typeof(ZGame).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                var metadata = field.GetCustomAttribute<ZGameEnumMetadataAttribute>(false);
                if (metadata != null)
                {
                    _supportedGamesMetadata.Add(metadata);
                }
            }
        }

        #endregion

        public ZInstalledGamesCollection Parse(ZPacket packet)
        {
            ZInstalledGamesCollection gameCollection;

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
                    item.Architecture = _GetZGameArchitectureByRunnableName(item.Game, item.RunnableName);

                    games.Add(item);
                }

                gameCollection = new ZInstalledGamesCollection
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
            var metadata = _supportedGamesMetadata.FirstOrDefault(i => i.InternalName == internalName);
            return metadata?.GameReference ?? ZGame.None;
        }

        private ZGameArchitecture _GetZGameArchitectureByRunnableName(ZGame gameReference, string runnableGame)
        {
            var x32 = ZGameArchitecture.x32.ToString();
            var x64 = ZGameArchitecture.x64.ToString();

            if (runnableGame.EndsWith(x32))
            {
                return ZGameArchitecture.x32;
            }
            
            if (runnableGame.EndsWith(x64))
            {
                return ZGameArchitecture.x64;
            }

            var metadata = _supportedGamesMetadata.FirstOrDefault(i => i.GameReference == gameReference);

            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (metadata != null)
            {
                return metadata.DefaultArchitecture;
            }

            return ZGameArchitecture.None;
        }

        #endregion
    }
}