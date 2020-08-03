# Описание публичных типов

## Enums
#### ZGame
Defines the Battlefield games

| Value      | Description                 |
|------------|-----------------------------|
| BF3 = 0    |The Battlefield 3 game       |
| BF4        |The Battlefield 4 game       |
| BFH        |The Battlefield Hardline game|
| None = 255 |Заглушка                     |

#### ZCoopDifficulty
Defines CoOp difficulty level

|Value|Description|
|-----|-----------|
|Easy = 0|Easy difficulty|
|Normal|Normal difficulty|
|Hard|Hard difficulty|

#### ZCoopLevels
Defines CoOp level codes

|Value|Description|
|-----|-----------|
|COOP_007 = 0|Operation Exodus|
|COOP_006|Fire from the Sky|
|COOP_009|Exfiltration|
|COOP_002|Hit and Run|
|COOP_003|Drop 'Em Like Liquid|
|COOP_010|The Eleventh Hour|

#### ZGameArchitecture
Defines game architecture

|Value|Description|
|-|-|
|x64 = 0|x64 architecture|
|x32|x86 architecture|

#### ZRunResult
Defines ZClient game run result

|Value|Description|
|-|-|
|Success = 0|Success|
|NotFound|Target game not found|
|Error|Some error|
|None|Stub|

#### ZRole
Defines player roles

|Value|Description|
|-|-|
|Soldier = 0|Soldier role. Supports on All games|
|Commander|Commander role. Supports on Battlefield 4 \*only (\*Hardline maybe to)|
|Spectator|Spectator role. Supports on Battlefield 4 \*only (\*Hardline maybe to)|

#### ZPlayMode
Defines play modes

|Value|Description|
|-|-|
|Singleplayer|Singleplayer|
|Multiplayer|Multiplayer|
|CooperativeHost|Cooperative host|
|CooperativeClient|Cooperative client|
|TestRange|Test range|

#### ZLogLevel
Defines log level

|Value|Description|
|-|-|
|Info|Info|
|Debug|Debug|
|Warning|Warning|
|Error|Error|

## Stats models
#### ZStatsBase, ZBF3Stats and ZBF4Stats 
Defines base stats. Property name as documentation

## Run game params models
#### ZSingleParams
Defines launch options for a singleplayer

|Property|Get\Set|Description|
|-|-|-|
|ZGame Game|++|Gets or sets the target game. Require|
|ZGameArchitecture? PreferredArchitecture|++|Gets or sets preferred game architecture. Optional. Default value is `null`|

#### ZMultiParams
Defines launch options for a multiplayer

|Property|Get\Set|Description|
|-|-|-|
|ZGame Game|++|Gets or sets the target game. Require|
|ZGameArchitecture? PreferredArchitecture|++|Gets or sets preferred game architecture. Optional. Default value is `null`|
|uint ServerId|++|Gets or sets server ZloID. Require|
|ZRole Role|++|Gets or sets player role. Default value is `ZRole.Soldier`|

#### ZTestRangeParams
Defines launch options for a test range (playground). В параметр `Game` следует передавать игру `ZGame.BF4`. Так как, пока только _Battlefield 4_ может похвататся его работой.

|Property|Get\Set|Description|
|-|-|-|
|ZGame Game|++|Gets or sets the target game. Require|
|ZGameArchitecture? PreferredArchitecture|++|Gets or sets preferred game architecture. Optional. Default value is `null`|

#### ZCoopParams
Defines launch options for a cooperative

|Property|Get\Set|Description|
|-|-|-|
|ZPlayMode Mode|++|Gets or sets play mode. Allow values is `ZPlayMode.CooperativeClient` or `ZPlayMode.CooperativeHost`. Required|
|ZGameArchitecture? PreferredArchitecture|++|Gets or sets preferred game architecture. Optional. Default value is `null`|
|ZCoopLevels? Level|++|Gets or sets level (mission) code. Required for `ZPlayMode.CooperativeHost` play mode. Default value is `null`|
|ZCoopDifficulty? Difficulty|++|Gets or sets difficulty level. Required for `ZPlayMode.CooperativeHost` play mode. Default value is `null`|
|uint? FriendId|++|Gets or sets cooperative host (friend) ZloID. Required for `ZPlayMode.CooperativeClient` play mode. Default value is `null`|

## Event args models
#### ZGamePipeArgs : EventArgs
Defines game pipe event args

|Property|Get\Set|Description|
|-|-|-|
|string FullMessage|+ -|Gets full pipe message|
|string FirstPart|+ -|Gets only first part of pipe message|
|string SecondPart|+ -|Gets only second part of pipe message|

#### ZConnectionChangedArgs : EventArgs
Defines connection changed event args

|Property|Get\Set|Description|
|-|-|-|
|bool IsConnected|+ -|Gets connection state|
|ZUser AuthorizedUser|+ -|Gets authorized user|

#### ZLogMessageArgs : EventArgs
Defines log event args

|Property|Get\Set|Description|
|-|-|-|
|ZLogLevel Level|+ -|Gets message level|
|string Message|+ -|Gets message|

## Other
#### ZConfiguration
Defines api configuration

|Property|Get\Set|Description|
|-|-|-|
|SynchronizationContext SynchronizationContext|++|Gets or sets UI synchronization context|

#### ZUser
Defines Zlo user

|Property|Get\Set|Description|
|-|-|-|
|uint Id|++|Gets ZloID|
|string Name|++|Gets ZloName|

## Server models

#### abstract ZServerBase : ZObservableObject

Defines base server

|Property|Get\Set|Description|IsObservable|
|-|-|-|-|
|uint Id|++|Gets the ZloID|-|
|string Name|++|Gets name|-|
|IPAddress Ip|++|Gets ip address|-|
|uint Port|++|Gets port number|-|
|int Ping|++|Gets signal delay in ms. By default it is not calculated. This field is observable|+|
|ZGame Game|++|Gets target game|-|
|abstract byte SpectatorsCapacity|++|Gets capacity of spectators|-|
|byte PlayersCapacity|++|Gets capacity of players. This field is observable|+|
|byte CurrentPlayersNumber|++|Gets number of current players. This field is observable|+|
|ZMap CurrentMap|++|Gets current map|+-|
|ObservableCollection<ZPlayer> Players|++|Gets list of players|+-|
|IEnumerable<ZMap> SupportedMaps|++|Gets list of supported maps|-|
|ZAttributesBase Attributes|++|Gets attributes|-|
|IDictionary<string, string> Settings|++|Gets settings|-|

#### ZBF3Server : ZServerBase

Defines the Battlefield 3 server

|Property|Get\Set|Description|IsObservable|
|-|-|-|-|
|override byte SpectatorsCapacity|throw NotSupportedException|This property not supported on Battlefield 3|-|

#### ZBF4Server : ZServerBase

Defines the Battlefield 4 server

|Property|Get\Set|Description|IsObservable|
|-|-|-|-|
|override byte SpectatorsCapacity|++|Gets capacity of spectators|-|

#### ZBFHServer : ZServerBase

Defines the Battlefield Hardline server

|Property|Get\Set|Description|IsObservable|
|-|-|-|-|
|override byte SpectatorsCapacity|++|Gets capacity of spectators|-|

#### abstract class ZAttributesBase

Defines basic server attributes

|Property|Get\Set|Description|
|-|-|-|
|string BannerUrl|+ -|Gets the banner url|
|string Country|+ -|Gets country code|
|string Message|+ -|Gets server message|
|string Mod|+ -|Gets ???|
|string Preset|+ -|Gets preset name|
|string PunkBusterVersion|+ -|Gets PunkBuster version|
|string Region|+ -|Gets region code|
|string Description|+ -|Gets description|
|abstract string FairFight|+ -|Gets an indication of the presence of a FairFight|
|abstract string ServerType|+ -|Gets type of server|
|abstract string TickRate|+ -|Gets tick rate|

#### ZBF3Attributes : ZAttributesBase

Defines the Battlefield 3 server attributes

|Property|Get\Set|Description|
|-|-|-|
|override string ServerType|throw NotSupportedException|The Battlefield 3 not supported this attribute|
|override string FairFight|throw NotSupportedException|The Battlefield 3 not supported this attribute|
|override string TickRate|throw NotSupportedException|The Battlefield 3 not supported this attribute|

#### ZBF4Attributes : ZAttributesBase

Defines the Battlefield 4 server attributes

|Property|Get\Set|Description|
|-|-|-|
|override string ServerType|+ -|Gets type of server|
|override string FairFight|+ -|Gets an indication of the presence of a FairFight|
|override string TickRate|+ -|Gets tick rate|

#### ZBFHAttributes : ZAttributesBase

Defines the Battlefield Hardline server attributes

|Property|Get\Set|Description|
|-|-|-|
|override string ServerType|+ -|Gets type of server|
|override string FairFight|+ -|Gets an indication of the presence of a FairFight|
|override string TickRate|+ -|Gets tick rate|

#### ZPlayer

Defines player

|Property|Get\Set|Description|
|-|-|-|
|byte Slot|++|Gets player slot number|
|uint Id|++|Gets player ZloID|
|string Name|++|Gets name of player|

#### ZMap : ZObservableObject

Defines server map

|Property|Get\Set|Description|IsObservable|
|-|-|-|-|
|string Name|++|Gets name of map|+|
|string GameModeName|++|Gets name of game mode|+|

## Helpers

#### ZResource

Provides some resources for general use

|Method|Description|Params|Exceptions|Return|
|-|-|-|-|-|
|`static string[] GetBF3MapNames()`|Gets Battlefield 3 maps array|-|-|String array of map names|
|`static string[] GetBF4MapNames()`|Gets Battlefield 4 maps array|-|-|String array of map names|
|`static string[] GetBFHMapNames()`|Gets Battlefield Hardline maps array|-|-|String array of map names|
|`static string[] GetBF3GameModeNames()`|Gets Battlefield 3 game modes array|-|-|String array of game mode names|
|`static string[] GetBF4GameModeNames()`|Gets Battlefield 4 game modes array|-|-|String array of game mode names|
|`static string[] GetBFHGameModeNames()`|Gets Battlefield Hardline game modes array|-|-|String array of game mode names|

#### ZPingService

Helps calculate roundtrip delay of the server

|Method|Description|Params|Exceptions|Return|
|-|-|-|-|-|
|`static int GetPing(IPAddress address)`|Calculate roundtrip delay of the `address`|`address` - IP address for which the calculation will be executed|-|Calculated delay time. If is over 999, then return 999|

## Service

#### interface IZApi

Defines the ZloApi

##### Methods

|Method|Description|Params|Exceptions|Return|
|-|-|-|-|-|
|`Task<ZStatsBase> GetStatsAsync(ZGame game)`|Makes an asynchronous request to get current soldier statistics|`game` - Game context|`NotSupportedException` - Occurs when specifying the Battlefield Hardline parameter; `InvalidOperationException` - Occurs when Api is not connected|A task that represents the asynchronous get soldier statistics operation|
|`IZServersListService CreateServersListService(ZGame game)`|Creates and returns an implementation of `IZServersListService`|`game` - Game context|`InvalidOperationException` - Occurs when trying to create a service in an non-configured Api or Api is not connected; `InvalidEnumArgumentException` - Occurs when a parameter is set to an invalid value|Implementation of `IZServersListService`|
|`void InjectDll(ZGame game, string[] paths)`|Makes asynchronous requests to inject specified dlls into the game process|`game` - Game context; `paths` - An array of dll paths for injection|`InvalidOperationException` - Occurs when Api is not connected|-|
|`void Configure(ZConfiguration config)`|Configures api|`config` - Configuration instance|`InvalidOperationException` - Occurs when this method is called more than once; `ArgumentNullException` - Occurs when `config` is null; `ArgumentException` - Occurs when `ZConfiguration.SynchronizationContext` is not specified|-|

##### Properties

|Property|Get\Set|Description|
|-|-|-|
|`IZGameFactory GameFactory`|+ -|Gets game factory|
|`IZConnection Connection`|+ -|Gets API connection|
|`IZLogger Logger`|+ -|Gets API logger|





