# Pusher .NET HTTP API library

[![Build Status](https://travis-ci.org/pusher/pusher-http-dotnet.svg?branch=master)](https://travis-ci.org/pusher/pusher-http-dotnet)

This is a .NET library for interacting with the Pusher HTTP API.

Registering at <http://pusher.com> and use the application credentials within your app as shown below.

Comprehensive documenation can be found at <http://pusher.com/docs/>.

## Installation

### NuGet Package
```
Install-Package PusherServer
```

## How to use

### Constructor

```
var Pusher = new Pusher(APP_ID, APP_KEY, APP_SECRET);
```

If you created your app in a different cluster to the default cluster, specify this as follows:

```cs

var options = new PusherOptions();
options.Cluster = "eu";

var Pusher = new Pusher(APP_ID, APP_KEY, APP_SECRET, options);
```

*Please Note: the `Cluster` option is overridden by `HostName` option. So, if `HostName` is set then `Cluster` will be ignored.*

### Publishing/Triggering events

To trigger an event on one or more channels use the trigger function.

#### A single channel

```
var result = pusher.Trigger( "channel-1", "test_event", new { message = "hello world" } );
```

#### Multiple channels

```
var result = pusher.Trigger( new string[]{ "channel-1", "channel-2" ], "test_event", new { message: "hello world" } );
```

### Excluding event recipients

In order to avoid the person that triggered the event also receiving it the `trigger` function can take an optional `ITriggerOptions` parameter which has a `SocketId` property. For more informaiton see: <http://pusher.com/docs/publisher_api_guide/publisher_excluding_recipients>.

```
var result = pusher.Trigger(channel, event, data, new TriggerOptions() { SocketId = "1234.56" } );
```

### Authenticating Private channels

To authorise your users to access private channels on Pusher, you can use the `Authenticate` function:

```
var auth = pusher.Authenticate( channelName, socketId );
var json = auth.ToJson();
```

The `json` can then be returned to the client which will then use it for validation of the subscription with Pusher.

For more information see: <http://pusher.com/docs/authenticating_users>

### Authenticating Presence channels

Using presence channels is similar to private channels, but you can specify extra data to identify that particular user:

```
var channelData = new PresenceChannelData() {
	user_id: "unique_user_id",
	user_info: new {
	  name = "Phil Leggetter"
	  twitter_id = "@leggetter"
	}
};
var auth = pusher.Authenticate( channelName, socketId, channelData );
var json = auth.ToJson();
```

The `json` can then be returned to the client which will then use it for validation of the subscription with Pusher.

For more information see: <http://pusher.com/docs/authenticating_users>

### Application State

It is possible to query the state of your Pusher application using the generic `Pusher.Get( resource )` method and overloads.

For full details see: <http://pusher.com/docs/rest_api>

#### List channels

You can get a list of channels that are present within your application:

```
IGetResult<ChannelsList> result = pusher.Get<ChannelsList>("/channels");
```

or

```
IGetResult<ChannelsList> result = pusher.FetchStateForChannels<ChannelsList>();
```

You can provide additional parameters to filter the list of channels that is returned.

```
IGetResult<ChannelsList> result = pusher.Get<ChannelsList>("/channels", new { filter_by_prefix = "presence-" } );
```

or

```
IGetResult<ChannelsList> result = pusher.FetchStateForChannels<ChannelsList>(new { filter_by_prefix = "presence-" } );
```

There is also an asynchronous variation

```
pusher.FetchStateForChannelsAsync<ChannelsList>((IGetResult<ChannelsList> result) =>
{
});
```

#### Fetch channel information

Retrieve information about a single channel:

```
IGetResult<object> result = pusher.Get<object>("/channels/my_channel" );
```

or

```
IGetResult<object> result = pusher.FetchStateForChannel<object>("my_channel");
```

There is also an asynchronous variation

```
pusher.FetchStateForChannelAsync<object>("my_channel", (ITriggerResult result) =>
{
});
```

Retrieve information about multiple channels:

```
IGetResult<object> result = pusher.FetchStateForChannels<object>();
```

There is also an asynchronous variation

```
pusher.FetchStateForChannelsAsync<object>((ITriggerResult result) =>
{
});
```

*Note: `object` has been used above because as yet there isn't a defined class that the information can be serialized on to*

#### Fetch a list of users on a presence channel

Retrieve a list of users that are on a presence channel:

```
IGetResult<object> result = pusher.FetchUsersFromPresence<object>("/channels/presence-channel/users" );
```

or

```
IGetResult<object> result = pusher.FetchUsersFromPresenceChannel<object>("my_channel");
```

There is also an asynchronous variation

```
pusher.FetchUsersFromPresenceChannelAsync<object>("my_channel", (ITriggerResult result) =>
{
});
```

*Note: `object` has been used above because as yet there isn't a defined class that the information can be serialized on to*

### WebHooks

Pusher will trigger WebHooks based on the settings you have for your application. You can consume these and use them
within your application as follows.

For more information see <https://pusher.com/docs/webhooks>.

```
// How you get these depends on the framework you're using

// HTTP_X_PUSHER_SIGNATURE from HTTP Header
var receivedSignature = "value";

// Body of HTTP request
var receivedBody = "value;

var pusher = new Pusher(...);
var webHook = pusher.ProcessWebHook(receivedSignature, receivedBody);
if(webHook.IsValid)
{
  // The WebHook validated
  // Dictionary<string,string>[]
  var events = webHook.Events;

  foreach(var webHookEvent in webHook.Events)
  {
    var eventType = webHookEvent["name"];
    var channelName = webHookEvent["channel"];

    // depending on the type of event (eventType)
    // there may be other values in the Dictionary<string,string>
  }

}
else {
  // Log the validation errors to work out what the problem is
  // webHook.ValidationErrors
}
```

## Development Notes

* Developed using Visual Studio Community 2013
* The NUnit test framework is used for testing, your copy of Visual Studio needs the "NUnit test adapter" installed from Tools -> Extensions and Updates if you wish to run the test from the IDE.
* PusherServer acceptance tests depends on [PusherClient](https://github.com/pusher-community/pusher-websocket-dotnet).

### Alternative environments

The solution can be opened and compiled in Xamarin Studio on OSX.

Alternatively, the solution can be built from the command line if Mono is installed.  First of all, open up a terminal and navigate to the root directory of the solution. The second step is to restore the Nuget packages, which can be done with this command

```
nuget restore pusher-dotnet-server.sln
```

and finally build the solution, now that the packages have been restored

```
xbuild pusher-dotnet-server.sln
```

During the build, there will be a warning about a section called TestCaseManagementSettings in the GlobalSection.  Please ignore this, as it is a Visual Studio specific setting.

## Publish to NuGet

You should be familiar with [creating and publishing NuGet packages](http://docs.nuget.org/docs/creating-packages/creating-and-publishing-a-package).

From the `pusher-dotnet-server` directory:

1. Update `PusherServer/Properties/AssemblyInfo.cs` with new version number.
2. Check and change any info required in `PusherServer/PusherServer.nuspec`.
3. Run `package.cmd` to pack a package to deploy to NuGet.
3. Run `tools/nuget.exe push PusherServer.{VERSION}.nupkg'.

## License

This code is free to use under the terms of the MIT license.
