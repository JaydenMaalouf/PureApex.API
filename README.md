# PureApexAPI - C# Apex Legends API
A .NET API Wrapper for the Apex Legends API
Documentation is found below!

## Installation
Our stable build is available from NuGet through the ApexAPI metapackage:
- [PureApexAPI](https://www.nuget.org/packages/PureApexAPI/)

## Getting Started
Once you have added the NuGet Package to your Project, you will need to add the `using Pure.Apex.API;` to your class header.
Then simply instance the ApexAPI class with your Origin Email and Password, like so:
```csharp
var API = new ApexAPI("example@email.com", "password");
```
Now you can easily make calls to the API.

## GetUserAsync()
If you already know a user's Guid or Username, you can use the `GetUserAsync()` method to return an `ApexUser` object.
- Username:
```csharp
var user = API.GetUserAsync("username");
```

## GetUsersAsync()
Same as `GetUserAsync()` but allows to search by generic terms.
- This will return any users who's username starts with `user`:
```csharp
var users = API.GetUsersAsync("user");
```
  
## GetStatsAsync()
If you're wanting to get a user's stats, you can simply use `.GetStatsAsync();` and it will return the requested stats.
```csharp
var user = API.GetUserAsync("username");
var stats = await user.GetStatsAsync();
```

**NOTE: THE API WILL ONLY RETURN VALID DATA FOR THE CURRENT ACTIVE LEGEND FOR THE USER**

Thanks for using my wrapper <3 By Kanga#8041