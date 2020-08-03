## Описание

**Zlo4NET API** это проект API для разработки лаунчеров игр серии _Battlefield_ для проекта **ZLOEmu**. Был создан с целью сделать разработку лаунчеров с использованием технологий **WinForms** и **WPF** более гибкой и удобной.

## Установка и зависимости

Для использования **Zlo4NET API** в вашем проекте, можно использовать один из ниже описанных способов его подключения:

* _Использование пакетного менеджера **NuGet**_

Откройте пакетный менеджер NuGet и установите следующие сборки в свой проект

1. Newtonsoft.Json
2. Zlo4NET

* _Использование оффлайн сборки_

Добавьте следующие сборки в свой проект через References

1. Newtonsoft.Json.dll
2. Zlo4NET.dll

## Доступные возможности
* асинхронно подключаться к _ZClient_ и отслеживать состояние подлючения
* асинхронно получать список серверов и его обновлений
* получать _debug_ информацию
* асинхронно запускать игры без использования _ZLOrigin_
* получать информацию с _pipe_ игры
* безопасно инжектить _dll_ в процесс игры **(не читы)**
* асинхронно получать статистику игрока BF3, BF4

Что бы знать, что планируется сделать и что делается уже сейчас, посетите [**Trello**](https://trello.com/b/soZz4WYB).

## С чего стоит начать
#### Обращение к API
API использует паттерн _Singleton_, поэтому, вам всегда будет доступен один и тот же обьект API `var apiClient = ZApi.Instance;`. Обьект будет создан при первом обращении к свойству `Instance`.
#### Конфирурация API
В начале вашей работы с API, вам нужно его сконфигурировать путем вызова матода `Configure(ZConfiguration config)`. Так как, некоторые операции должны выполняться в контексте _UI_ потока, в конфигурацию следует передать обьект контекста вашего _UI_  `SynchronizationContext.Current`. `SynchronizationContext.Current` следует вызывыть в вашем _UI_ потоке, иначе будет захвачен не тот контекст.

**Пример:**
```csharp
var apiClient = ZApi.Instance;
var configuration = new ZConfiguration
{
	SynchronizationContext = SynchronizationContext.Current
};
apiClient.Configure(configuration);
```
Метод `Configure(...)` можно вызвать только один раз.

#### Подключение к ZClient
Для работы API, вам нужно установить соединение с _ZClient_. Для подключения, вам нужно получить сервис `IZConnection` и вызвать у него метод `Connect()`. Далее отслеживайте событие `ConnectionChanged`, что бы знать, когда соединение сменило свое состояние. Так же, вы можете досрочно разорвать соединение, вызвав метод `Disconnect()`. Цыклы подлючений и отключений не ограничены.

**Пример:**
```csharp
var apiClient = ZApi.Instance;
var connection = apiClient.Connection;

connection.ConnectionChanged += ApiConnectionChangedHandler;
connection.Connect();

void ApiConnectionChangedHandler(object sender, ZConnectionChangedArgs e) {
	var connectionState = e.IsConnected;
	var authUser = e.AuthorizedUser;
	// do something
}

connection.Disconnect();
```
#### Получение списка серверов
Для получения списка серверов, вам нужно создать специальный сервис, вызвав метод `CreateServersListService(...)` передав в него игру, которая вам требуется. Результатом выполнения будет `IZServersListService`. 

Как будете готовы начать заполнение и обновление списка, вызовите метод `StartReceiving()`. Что бы получить доступ к списку серверов, обратитесь к свойству `ServersCollection`.

**Пример:**
```csharp
var apiClient = ZApi.Instance;
var serversService = apiClient.CreateServersListService(ZGame.BF3);
m_bindableServersList = serversService.ServersCollection;
serversService.StartReceiving();
```
API не высчитывает значения _ping_ для каждого сервера, но предоставляет событие, которое поможет вам посчитать _ping_ одним махом. После получения полного списка серверов, будет иницировано событие `InitialSizeReached`. Данное событие инициируеться только один раз.
Обновления количества серверов, можно отслежывать через событие `CollectionChanged` у обьекта списка.

**Пример:**
```csharp
var serversService = apiClient.CreateServersListService(ZGame.BF3);
var collection = serversService.ServersCollection;

serversService.InitialSizeReached += ServersInitialSizeReachedHanlder;
collection.CollectionChanged += ServersListChangedHandler;

void ServersInitialSizeReachedHanlder(object sender, EventArgs e)
{
	// do something
}

void ServersListChangedHandler(object sender, NotifyCollectionChangedEventArgs e)
{
	// do something
}
```

Обратите внимание, вы можете обратится к последнему созданному такому сервису, через свойство API `ServersListService`. 
Если вам уже не нужен данный сервис, вызовите метод `StopReceiving()`. Он освободит ресурсы, и закроет обьект для подальшего использования. Что бы проверить, доступен ли обьект для использования через свойство API `ServersListService`, проверьте свойство сервиса `CanUse`.

**Пример:**
```csharp
var apiClient = ZApi.Instance;
var serversService = apiClient.ServersListService;

if (serversService.CanUse)
{
	serversService.StopReceiving();
}
```

#### Создание и запуск игры
Что бы запустить игру, нужно ее создать через фабрику игр `IZGameFactory`.

**Пример:**
```csharp
var apiClient = ZApi.Instance;
var game = await apiClient.GameFactory.CreateSingleAsync(new ZSingleParams 
{
	Game = ZGame.BF4,
	PreferredArchitecture = ZGameArchitecture.x64
});
var runResult = await game.RunAsync();
```

#### Injector
Позволяет загружать код указанных сборок (решейды) в процесс игры, без боязни быть забаненым.

**Пример:**
```csharp
var apiClient = ZApi.Instance;
var dlls = new { "C:\\bf3_disableBlueShit.dll" };
apiClient.Inject(ZGame.BF3, dlls);
```
#### Получение статитстики игрока
Получение статистики игрока доступно прямо из обьекта API. Просто вызовите метод `GetStatsAsync(...)` передав в параметры игру, которая вам нужна. Будет возвращен абстрактный клас `ZStatsBase`, который нужно привести к нужному для вас типу статистики `ZBF3Stats` для _Battlefield 3_ и `ZBF4Stats` для _Battlefield 4_.

**Пример:**
```csharp
var apiClient = ZApi.Instance;
var stats = (ZBF3Stats) await apiClient.GetStatsAsync(ZGame.BF3);
```
#### Получение информации об ошибках
Вы можете получить доступ к отладочной информации API через событие `OnMessage`.

**Пример:**
```csharp
var apiClient = ZApi.Instance;
var logger = apiClient.Logger;
logger.OnMessage += ApiLoggerMessageHandler;

void ApiLoggerMessage(object sender, ZLogMessageArgs e) {
	// do something
}
```

## Список изменений
**v0.1.8.0 alpha**

* реализована xml документация

## Связь
Если у вас есть вопросы, пишите мне сюда Discord(XrIStOs#6552).

## Благодарности
* **bigworld12** (примеры и мотивация)
* **ZLOFENIX** (отвечал на все вопросы, железное терпение)
* **Администрация проекта ZLOEMU** (понимание и содействие)
