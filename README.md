# SpotiListener

A personal listening status page that displays information about me and
the music currently playing in Spotify.

## Features

-  Current track, artist and album information.
- Album artwork retrieval via MPRIS.
- Automatic status updates.
- Responsive Blazor UI.

## Architecture

The application is divided into independent .NET projects:

-   **SpotiListener.Core** - shared models and interfaces.
-   **SpotiListener.Mpris** - retrieves playback information through
    D-Bus and MPRIS.
-   **SpotiListener.Agent** - exposes the current track through a
    local HTTP API.
-   **SpotiListener.Web** - Blazor Web App that displays the listening
    status.

## Technologies

-   C#
-   .NET 8
-   ASP.NET Core
-   Blazor
-   D-Bus / MPRIS
-   HTML, CSS

## Requirements

-   Linux with a running Spotify desktop client.
-   .NET 8 SDK.
-   A session D-Bus environment.

## Running

Start the Agent:

``` bash
dotnet run --project src/SpotiListener.Agent
```

Start the Web application in another terminal:

``` bash
dotnet run --project src/SpotiListener.Web
```

Make sure the Agent URL in the Web application's configuration matches
the address and port on which the Agent is listening.

------------------------------------------------------------------------
