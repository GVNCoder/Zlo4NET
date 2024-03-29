﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Newtonsoft.Json.Linq;

using Zlo4NET.Api.DTOs;
using Zlo4NET.Api.Service;
using Zlo4NET.Helpers;
using Zlo4NET.Api.Shared;

namespace Zlo4NET.Data
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

        #region Internal types

        private static class ParametersValidator
        {
            public static void ThrowIfNotValid(ZBaseLaunchParameters parameters)
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

            public static void ThrowIfNotValid(ZCoopClientLaunchParameters parameters)
            {
                ThrowIfNotValid((ZBaseLaunchParameters) parameters);

                if (parameters.TargetGame.Game != ZGame.BF3)
                {
                    throw new NotSupportedException($"{parameters.TargetGame.Game} not supported");
                }

                if (parameters.HostId == null)
                {
                    throw new ArgumentException($"{nameof(parameters.HostId)} is null");
                }
            }

            public static void ThrowIfNotValid(ZCoopHostLaunchParameters parameters)
            {
                ThrowIfNotValid((ZBaseLaunchParameters) parameters);

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

            public static void ThrowIfNotValid(ZTestRangeLaunchParameters parameters)
            {
                ThrowIfNotValid((ZBaseLaunchParameters) parameters);

                if (parameters.TargetGame.Game != ZGame.BF4)
                {
                    throw new NotSupportedException($"{parameters.TargetGame.Game} not supported");
                }
            }

            public static void ThrowIfNotValid(ZMultiLaunchParameters parameters)
            {
                ThrowIfNotValid((ZBaseLaunchParameters) parameters);

                if (parameters.TargetGame.Game == ZGame.BF3 && parameters.Role == ZJoinPlayerRole.Spectator && parameters.Role == ZJoinPlayerRole.Commander)
                {
                    throw new NotSupportedException($"{parameters.TargetGame.Game} doesn't support {parameters.Role} role");
                }

                if (parameters.TargetGame.Game == ZGame.BFHL && parameters.Role == ZJoinPlayerRole.Spectator && parameters.Role == ZJoinPlayerRole.Commander)
                {
                    throw new NotSupportedException($"{parameters.TargetGame.Game} doesn't support {parameters.Role} role");
                }
            }
        }

        #endregion

        private readonly IDictionary<string, string> _argumentsDictionary = new Dictionary<string, string>
        {
            { "[personaRef]",  string.Empty },
            { "[gameId]",      string.Empty },
            { "[isSpectator]", string.Empty },
            { "[role]",        string.Empty },
            { "[friendId]",    string.Empty },
            { "[level]",       string.Empty },
            { "[difficulty]",  string.Empty },
        };

        private readonly IDictionary<ZGame, Func<ZInstalledGame, string, IZGameProcess>> _gameProcessCreationMethods = new Dictionary<ZGame, Func<ZInstalledGame, string, IZGameProcess>>
        {
            { ZGame.BF3, ZGameProcessCreateMethodsProvider.CreateBF3GameProcess },
            { ZGame.BF4, ZGameProcessCreateMethodsProvider.CreateBF4GameProcess },
            { ZGame.BFHL, ZGameProcessCreateMethodsProvider.CreateBFHLGameProcess },
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

        #endregion

        public IZGameProcess CreateSingle(ZSingleLaunchParameters parameters)
        {
            ZThrowHelper.ThrowIfNotConnected();
            ParametersValidator.ThrowIfNotValid(parameters);

            var targetGame = parameters.TargetGame;
            var currentUser = _connection.GetCurrentUserInfo();
            var commandArgumentsTemplate = _runStrings[SINGLE_KEY].Value<string>();

            // generate launch arguments
            _argumentsDictionary["[personaRef]"] = currentUser.UserId.ToString();

            // replace placeholders
            var commandParameters = _mapPlaceholders(commandArgumentsTemplate);
            var gameProcess = _gameProcessCreationMethods[targetGame.Game].Invoke(targetGame, commandParameters);

            return gameProcess;
        }

        public IZGameProcess CreateCoopClient(ZCoopClientLaunchParameters parameters)
        {
            ZThrowHelper.ThrowIfNotConnected();
            ParametersValidator.ThrowIfNotValid(parameters);

            var targetGame = parameters.TargetGame;
            var currentUser = _connection.GetCurrentUserInfo();
            var commandArgumentsTemplate = _runStrings[COOP_CLIENT_KEY].Value<string>();

            // generate launch arguments
            _argumentsDictionary["[personaRef]"] = currentUser.UserId.ToString();
            _argumentsDictionary["[friendId]"] = parameters.HostId.ToString();

            // replace placeholders
            var commandParameters = _mapPlaceholders(commandArgumentsTemplate);
            var gameProcess = _gameProcessCreationMethods[targetGame.Game].Invoke(targetGame, commandParameters);

            return gameProcess;
        }

        public IZGameProcess CreateCoopHost(ZCoopHostLaunchParameters parameters)
        {
            ZThrowHelper.ThrowIfNotConnected();
            ParametersValidator.ThrowIfNotValid(parameters);

            var targetGame = parameters.TargetGame;
            var currentUser = _connection.GetCurrentUserInfo();
            var commandArgumentsTemplate = _runStrings[COOP_HOST_KEY].Value<string>();

            // generate launch arguments
            _argumentsDictionary["[personaRef]"] = currentUser.UserId.ToString();
            _argumentsDictionary["[level]"] = parameters.Level.ToString();
            _argumentsDictionary["[difficulty]"] = parameters.Difficulty.ToString();

            // replace placeholders
            var commandParameters = _mapPlaceholders(commandArgumentsTemplate);
            var gameProcess = _gameProcessCreationMethods[targetGame.Game].Invoke(targetGame, commandParameters);

            return gameProcess;
        }

        public IZGameProcess CreateTestRange(ZTestRangeLaunchParameters parameters)
        {
            ZThrowHelper.ThrowIfNotConnected();
            ParametersValidator.ThrowIfNotValid(parameters);

            var targetGame = parameters.TargetGame;
            var currentUser = _connection.GetCurrentUserInfo();
            var commandArgumentsTemplate = _runStrings[TEST_RANGE_KEY].Value<string>();

            // generate launch arguments
            _argumentsDictionary["[personaRef]"] = currentUser.UserId.ToString();

            // replace placeholders
            var commandParameters = _mapPlaceholders(commandArgumentsTemplate);
            var gameProcess = _gameProcessCreationMethods[targetGame.Game].Invoke(targetGame, commandParameters);

            return gameProcess;
        }

        public IZGameProcess CreateMulti(ZMultiLaunchParameters parameters)
        {
            ZThrowHelper.ThrowIfNotConnected();
            ParametersValidator.ThrowIfNotValid(parameters);

            var targetGame = parameters.TargetGame;
            var currentUser = _connection.GetCurrentUserInfo();
            var commandArgumentsTemplate = _runStrings[MULTI_KEY].Value<string>();

            // generate launch arguments
            _argumentsDictionary["[personaRef]"] = currentUser.UserId.ToString();
            _argumentsDictionary["[gameId]"] = parameters.ServerId.ToString();
            _argumentsDictionary["[role]"] = parameters.Role.ToString();
            _argumentsDictionary["[isSpectator]"] =
                parameters.Role == ZJoinPlayerRole.Spectator ? "isspectator=\\\"true\\\"" : string.Empty;

            // replace placeholders
            var commandParameters = _mapPlaceholders(commandArgumentsTemplate);
            var gameProcess = _gameProcessCreationMethods[targetGame.Game].Invoke(targetGame, commandParameters);

            return gameProcess;
        }
    }
}