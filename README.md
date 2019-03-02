# PureApex.API - C# Apex Legends API
A .NET API Wrapper for the Apex Legends API
Documentation is found below!

## Installation
Our stable build is available from NuGet through the ApexAPI metapackage:
- [PureApex.API](https://www.nuget.org/packages/PureApex.API/)

## Getting Started
Once you have added the NuGet Package to your Project, you will need to add the `using PureApex.API;` to your class header.
Then simply instance the ApexAPI class with your Origin Email and Password, like so:
```csharp
var API = new ApexAPI("example@email.com", "password");
```
Then login to Origin with the `LoginAsync()` call:
```csharp
var result = await API.LoginAsync();
```
The `result` boolean will be set to true or false depending on the login success.

#### Now you can easily make calls to the API!

## GetUserAsync()
If you already know a user's Guid or Username, you can use the `GetUserAsync()` method to return an `ApexUser` object.
- Username:
```csharp
var user = await API.GetUserAsync("username");
```
- UserId:
```csharp
var user = await API.GetUserAsync(userId);
```

## GetUsersAsync()
Same as `GetUserAsync()` but allows to search by generic terms.
- This will return any users who's username starts with `user`:
```csharp
var users = await API.GetUsersAsync("user");
```
  
## GetStatsAsync()
If you're wanting to get a user's stats, you can simply use `.GetStatsAsync();` and it will return the requested stats.
```csharp
var user = API.GetUserAsync("username");
var stats = await user.GetStatsAsync();
```

**NOTE: THE API WILL ONLY RETURN VALID DATA FOR THE CURRENT ACTIVE LEGEND FOR THE USER**

#### Thanks for using my wrapper ❤️ By Kanga#8041.

**Please note: This API wrapper is for educational purposes only. I am not affiliated with Origin or any of it's entities.**
