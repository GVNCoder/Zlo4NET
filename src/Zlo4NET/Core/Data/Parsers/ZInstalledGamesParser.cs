using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
            using (var br = new BinaryReader(memory, Encoding.ASCII))
            {
                installedGames.IsX64 = br.ReadBoolean();

                var gamesCount = br.ReadZUInt32();
                games = new List<ZInstalledGame>((int) gamesCount);

                for (var i = 0; i < gamesCount; i++)
                {
                    var item = new ZInstalledGame
                    {
                        RunnableName = br.ReadZString(),
                        ZloName = br.ReadZString(),
                        FriendlyName = br.ReadZString()
                    };
                    item.EnumGame = ZStringToGameConverter.Convert(item.ZloName);

                    games.Add(item);
                }
            }

            installedGames.InstalledGames = games
                .Where(g => g.EnumGame != ZGame.None)
                .ToArray();
            
            return installedGames;
        }
    }
}