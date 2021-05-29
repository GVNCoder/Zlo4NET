using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Zlo4NET.Api.DTOs;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Core.Extensions;
using Zlo4NET.Core.Helpers;
using Zlo4NET.Core.Services;
using Zlo4NET.Core.ZClientAPI;

namespace Zlo4NET.Core.Data.Parsers
{
    internal class ZInstalledGamesParser : IZInstalledGamesParser
    {
        public ZInstalledGames Parse(ZPacket packet)
        {
            var installedGames = new ZInstalledGames();

            List<ZInstalledGame> games;

            using (var memory = new MemoryStream(packet.Payload, false))
            using (var binaryReader = new BinaryReader(memory, Encoding.ASCII))
            {
                installedGames.IsX64OperatingSystem = binaryReader.ReadBoolean();

                var gamesCount = binaryReader.ReadZUInt32();
                games = new List<ZInstalledGame>((int) gamesCount);

                for (var i = 0; i < gamesCount; i++)
                {
                    var item = new ZInstalledGame
                    {
                        RunnableName = binaryReader.ReadZString(),
                        InternalName = binaryReader.ReadZString(),
                        ReadableName = binaryReader.ReadZString()
                    };

                    item.Game = ZStringToGameConverter.Convert(item.InternalName);

                    games.Add(item);
                }
            }

            installedGames.Games = games
                .Where(g => g.Game != ZGame.None)
                .ToArray();
            
            return installedGames;
        }
    }
}