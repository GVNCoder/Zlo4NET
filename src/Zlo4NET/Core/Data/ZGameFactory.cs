﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Newtonsoft.Json.Linq;

using Zlo4NET.Api.DTOs;
using Zlo4NET.Api.Service;
using Zlo4NET.Core.Helpers;
using Zlo4NET.Api.Models.Shared;

namespace Zlo4NET.Core.Data
{
    internal class ZGameFactory : IZGameFactory
    {
        #region Constants

        private const string SINGLE_KEY = "single";
        private const string MULTI_KEY = "multi";
        private const string COOP_HOST_KEY = "coopHost";
        private const string COOP_CLIENT_KEY = "coopClient";
        private const string TEST_RANGE_KEY = "testRange";

        #endregion

        private readonly IDictionary<string, string> _argumentsDictionary = new Dictionary<string, string>
        {
            { "[personaRef]", string.Empty },
            { "[gameId]", string.Empty },
            { "[isSpectator]", string.Empty },
            { "[role]", string.Empty },
            { "[friendId]", string.Empty },
            { "[level]", string.Empty },
            { "[difficulty]", string.Empty },
        };

        private readonly IZConnection _connection;
        private readonly JObject _runStrings;

        #region Ctor

        public ZGameFactory(IZConnection connection)
        {
            _connection = connection;

            using (var streamReader = new StreamReader(ZInternalResource.GetResourceStream("run.json")))
            {
                var content = streamReader.ReadToEnd();
                _runStrings = JObject.Parse(content);
            }
        }

        #endregion

        #region Private methods

        private IZGameProcess _createGameProcess(ZInstalledGame targetGame, string commandArguments)
        {
            switch (targetGame.Game)
            {
                case ZGame.BF3: return new ZGameProcess(commandArguments, targetGame, "venice_snowroller", "bf3");
                case ZGame.BF4:
                    return new ZGameProcess(commandArguments, targetGame, "warsaw_snowroller",
                        targetGame.Architecture == ZGameArchitecture.x64 ? "bf4" : "bf4_x86");
                case ZGame.BFHL:
                    return new ZGameProcess(commandArguments, targetGame, "omaha_snowroller","bfh");

                case ZGame.None:
                default: throw new Exception();
            }
        }

        private string _mapPlaceholders(string template)
        {
            var matches = Regex.Matches(template, "\\[\\w+\\]");
            foreach (Match match in matches)
            {
                var placeholderValue = match.Value;

                // process placeholder
                template = template.Replace(placeholderValue, _argumentsDictionary[placeholderValue]);
            }

            return template;
        }

        private static void _ValidateBasicArguments(ZBaseLaunchParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException();
            }

            if (parameters.TargetGame == null)
            {
                throw new ArgumentException($"{nameof(parameters.TargetGame)} is null");
            }
        }

        private static void _ValidateCoopClientArguments(ZCoopClientLaunchParameters parameters)
        {
            _ValidateBasicArguments(parameters);

            if (parameters.TargetGame.Game != ZGame.BF3)
            {
                throw new NotSupportedException($"{parameters.TargetGame.Game} not supported");
            }

            if (parameters.HostId == null)
            {
                throw new ArgumentException($"{nameof(parameters.HostId)} is null");
            }
        }

        private static void _ValidateCoopHostArguments(ZCoopHostLaunchParameters parameters)
        {
            _ValidateBasicArguments(parameters);

            if (parameters.TargetGame.Game != ZGame.BF3)
            {
                throw new NotSupportedException($"{parameters.TargetGame.Game} not supported");
            }

            if (parameters.Level == null)
            {
                throw new ArgumentException($"{nameof(parameters.Level)} is null");
            }

            if (parameters.Difficulty == null)
            {
                throw new ArgumentException($"{nameof(parameters.Difficulty)} is null");
            }
        }

        private static void _ValidateTestRangeArguments(ZTestRangeLaunchParameters parameters)
        {
            _ValidateBasicArguments(parameters);

            if (parameters.TargetGame.Game != ZGame.BF4)
            {
                throw new NotSupportedException($"{parameters.TargetGame.Game} not supported");
            }
        }

        private static void _ValidateMultiArguments(ZMultiLaunchParameters parameters)
        {
            _ValidateBasicArguments(parameters);

            if (parameters.TargetGame.Game == ZGame.BF3 && parameters.Role == ZRole.Spectator && parameters.Role == ZRole.Commander)
            {
                throw new NotSupportedException($"{parameters.TargetGame.Game} doesn't support {parameters.Role} role");
            }

            if (parameters.TargetGame.Game == ZGame.BFHL && parameters.Role == ZRole.Spectator && parameters.Role == ZRole.Commander)
            {
                throw new NotSupportedException($"{parameters.TargetGame.Game} doesn't support {parameters.Role} role");
            }
        }

        #endregion

        public IZGameProcess CreateSingle(ZSingleLaunchParameters parameters)
        {
            ZConnectionHelper.MakeSureConnection();
            _ValidateBasicArguments(parameters);

            var currentUser = _connection.GetCurrentUserInfo();
            var commandArgumentsTemplate = _runStrings[SINGLE_KEY].Value<string>();

            // generate launch arguments
            _argumentsDictionary["[personaRef]"] = currentUser.UserId.ToString();

            // replace placeholders
            var commandParameters = _mapPlaceholders(commandArgumentsTemplate);
            var gameProcess = _createGameProcess(parameters.TargetGame, commandParameters);

            return gameProcess;
        }

        public IZGameProcess CreateCoopClient(ZCoopClientLaunchParameters parameters)
        {
            ZConnectionHelper.MakeSureConnection();
            _ValidateCoopClientArguments(parameters);

            var currentUser = _connection.GetCurrentUserInfo();
            var commandArgumentsTemplate = _runStrings[COOP_CLIENT_KEY].Value<string>();

            // generate launch arguments
            _argumentsDictionary["[personaRef]"] = currentUser.UserId.ToString();
            _argumentsDictionary["[friendId]"] = parameters.HostId.ToString();

            // replace placeholders
            var commandParameters = _mapPlaceholders(commandArgumentsTemplate);
            var gameProcess = _createGameProcess(parameters.TargetGame, commandParameters);

            return gameProcess;
        }

        public IZGameProcess CreateCoopHost(ZCoopHostLaunchParameters parameters)
        {
            ZConnectionHelper.MakeSureConnection();
            _ValidateCoopHostArguments(parameters);

            var currentUser = _connection.GetCurrentUserInfo();
            var commandArgumentsTemplate = _runStrings[COOP_HOST_KEY].Value<string>();

            // generate launch arguments
            _argumentsDictionary["[personaRef]"] = currentUser.UserId.ToString();
            _argumentsDictionary["[level]"] = parameters.Level.ToString();
            _argumentsDictionary["[difficulty]"] = parameters.Difficulty.ToString();

            // replace placeholders
            var commandParameters = _mapPlaceholders(commandArgumentsTemplate);
            var gameProcess = _createGameProcess(parameters.TargetGame, commandParameters);

            return gameProcess;
        }

        public IZGameProcess CreateTestRange(ZTestRangeLaunchParameters parameters)
        {
            ZConnectionHelper.MakeSureConnection();
            _ValidateTestRangeArguments(parameters);

            var currentUser = _connection.GetCurrentUserInfo();
            var commandArgumentsTemplate = _runStrings[TEST_RANGE_KEY].Value<string>();

            // generate launch arguments
            _argumentsDictionary["[personaRef]"] = currentUser.UserId.ToString();

            // replace placeholders
            var commandParameters = _mapPlaceholders(commandArgumentsTemplate);
            var gameProcess = _createGameProcess(parameters.TargetGame, commandParameters);

            return gameProcess;
        }

        public IZGameProcess CreateMulti(ZMultiLaunchParameters parameters)
        {
            ZConnectionHelper.MakeSureConnection();
            _ValidateMultiArguments(parameters);

            var currentUser = _connection.GetCurrentUserInfo();
            var commandArgumentsTemplate = _runStrings[MULTI_KEY].Value<string>();

            // generate launch arguments
            _argumentsDictionary["[personaRef]"] = currentUser.UserId.ToString();
            _argumentsDictionary["[gameId]"] = parameters.ServerId.ToString();
            _argumentsDictionary["[role]"] = parameters.Role.ToString();
            _argumentsDictionary["[isSpectator]"] =
                parameters.Role == ZRole.Spectator ? "isspectator=\\\"true\\\"" : string.Empty;

            // replace placeholders
            var commandParameters = _mapPlaceholders(commandArgumentsTemplate);
            var gameProcess = _createGameProcess(parameters.TargetGame, commandParameters);

            return gameProcess;
        }
    }
}