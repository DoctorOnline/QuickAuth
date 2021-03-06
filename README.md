# QuickAuth
![](https://github.com/alexeychuvagin/QuickAuth/workflows/.NET%20Core/badge.svg)

The CLI to quickly get an access token for a specific client.

## The CLI configuration
appsettings.json

```
{
  "CommonSettings": {
    "ClientConfigurationsPath": "%userprofile%\documents\QuickAuth",
    "CopyAccessTokenToClipboard": true
  }
}
```

## A client configuration
**Default location:** %userprofile%\documents\QuickAuth  
**File name:** %client%.json  

```
{
  "username": "alex",
  "password": "StrongPassword",
  "authUrl": "https://api.domain.com/auth/token",
  "headers": {
    "Connection": "keep-alive"
  }
}
```

## How to get an access token
```
start quickauth.exe "-c clientname"
```

## In addition
You can add CLI path to your system environment variables and use it directly:

```
quickauth -c clientname
```

```
PS C:\Users\Alex> quickauth -c clientname
[17:23:55 INF] Start processing HTTP request POST https://api.domain.com/auth/token
[17:23:55 INF] Sending HTTP request POST https://api.domain.com/auth/token
[17:23:56 INF] Received HTTP response after 1372.7802ms - OK
[17:23:56 INF] End processing HTTP request after 1430.1583ms - OK
[17:23:56 INF] Token: TokenResponse {AccessToken="someaccesstoken", RefreshToken="somerefreshtoken", ExpireDate=08/19/2020 17:53:55}
[17:23:56 INF] Token has been copied to clipboard.
```
