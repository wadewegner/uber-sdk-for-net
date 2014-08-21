# Uber SDK for .NET

This SDK provides an easy way for .NET developers to build C# or VB applications that interact with the [Uber API](https://developer.uber.com/).

This SDK is built using the Async/Await pattern for asynchronous development and .NET portable class libraries, making it easy to target multiple Microsoft platforms, including .NET 4.5, Windows Phone 8, Windows 8/8.1, and iOS/Android using Xamarin and Mono.NET.

## NuGet Package

You can try the SDK immmediately by installing the [WadeWegner.Uber](https://www.nuget.org/packages/WadeWegner.Uber/) NuGet package.

    Install-Package WadeWegner.Uber
  
## Sample

You can try an ASP.NET MVC sample that demonstrates the OAuth 2.0 flow for acquiring a user access token. You can find this sample here: [https://github.com/wadewegner/uber-sdk-for-net/tree/master/samples/Web](https://github.com/wadewegner/uber-sdk-for-net/tree/master/samples/Web).

## Operations

### Server Token Authentication

Many of the APIs all you to simply use your server token for authentication. You accomlish this by initializing the `UberClient` with your server token: 

    var client = new UberClient("YOURSERVERTOKEN");

### Access Token Authentication (OAuth 2.0)

For full details, review the [Uber authentication documentation](https://developer.uber.com/v1/auth/#oauth-2-0).

To initial the OAuth 2.0 flow, you will need to redirect the user to login through Uber. You can generate the URL and redirector (in ASP.NET MVC) using the following code:

    var url = Common.FormatAuthorizeUrl(ResponseTypes.Code, "YOURCLIENTID", HttpUtility.UrlEncode("YOURCALLBACKURL"));
    return Redirect(url);

Once the user logs in and allows access, Uber will issue an HTTP 302 redirect back to your callback URL. Grab the authorization code and then get an access token using the following code:

    var auth = new AuthenticationClient();
    await auth.WebServerAsync(_clientId, _clientSecret, _callbackUrl, code);

Now you can initialize the client, specifying that you're using an access token (not a server token):

    var client = new UberClient(TokenTypes.Access, auth.AccessToken);

### Product Types

    var latitude = 37.5F;
    var longitude = -122.2F;

You can use the server token ...

    var client = new UberClient("YOURSERVERTOKEN");
    var results = await client.ProductsAsync(latitude, longitude);

... or user token:

    var client = new UberClient(TokenTypes.Access, auth.AccessToken);
    var results = await client.ProductsAsync(latitude, longitude);

### Price Estimates

    var latitude = 37.5F;
    var longitude = -122.2F;

You can use the server token ...

    var client = new UberClient("YOURSERVERTOKEN");
    var results = await client.PriceEstimateAsync(latitude, longitude, latitude + 0.3F, longitude - 0.3F);

... or user token:

    var client = new UberClient(TokenTypes.Access, auth.AccessToken);
    var results = await client.PriceEstimateAsync(latitude, longitude, latitude + 0.3F, longitude - 0.3F);

### Time Estimates

    var latitude = 37.5F;
    var longitude = -122.2F;

You can use the server token ...

    var client = new UberClient("YOURSERVERTOKEN");
    var results = await client.TimeEstimateAsync(latitude, longitude);

... or user token:

    var client = new UberClient(TokenTypes.Access, auth.AccessToken);
    var results = await client.TimeEstimateAsync(latitude, longitude);

### User Activity

    var client = new UberClient(TokenTypes.Access, auth.AccessToken);
    var userActivity = await client.UserActivityAsync();

### User Profile

    var client = new UberClient(TokenTypes.Access, auth.AccessToken);
    var user = await client.UserAsync();

## Contributing to the Repository ###

If you find any issues or opportunities for improving this respository, fix them!  Feel free to contribute to this project by [forking](http://help.github.com/fork-a-repo/) this repository and make changes to the content.  Once you've made your changes, share them back with the community by sending a pull request. Please see [How to send pull requests](http://help.github.com/send-pull-requests/) for more information about contributing to Github projects.

## Reporting Issues ###

If you find any issues with this demo that you can't fix, feel free to report them in the [issues](https://github.com/wadewegner/uber-sdk-for-net/issues) section of this repository.
