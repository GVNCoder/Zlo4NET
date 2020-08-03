using System.Threading.Tasks;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Core.Data.Parsers;
using Zlo4NET.Core.Services;
using Zlo4NET.Core.ZClient.Data;
using Zlo4NET.Core.ZClient.Services;

namespace Zlo4NET.Core.Data
{
    internal class ZUserService : IZUserService
    {
        private readonly IZClientService _clientService;
        private readonly IZUserInfoParser _userParser;
        private readonly ZLogger _logger;

        public ZUserService(IZClientService clientService)
        {
            _clientService = clientService;
            _userParser = ZParsersFactory.BuildUserInfoParser();
            _logger = ZLogger.Instance;
        }

        public async Task<ZUser> GetAuthorizedUserAsync()
        {
            ZUser user = null;
            var response = await _clientService.SendUserInfoRequestAsync();

            if (response.Status != ZResponseStatusCode.Ok)
            {
                _logger.Error($"Received response by id: {response.Request.Id} with {response.Status}.");
            }
            else
            {
                user = _userParser.Parse(response.Packets);
            }

            AuthorizedUser = user;
            return user;
        }

        public ZUser AuthorizedUser { get; private set; }
    }
}