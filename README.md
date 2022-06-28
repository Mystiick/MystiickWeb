# MystiickWeb Dashboard
Repository for the Server and Client for {URL}. This website contains some small projects of mine built in Blazor
 - Minecraft server status
    - The server is currently down since no one plays on it anymore. Maybe someday I'll stand up a mocked UI.
 - Photo Viewer
    - Module to view and download some photos taken by me. They are uploaded and tagged by an internal tool [ImageProcessor](https://github.com/Mystiick/ImageProcessor)

## Projects
 - [./MystiickWeb.Client](./MystiickWeb.Client) project for the web application that is run in the user's browser.
 - [./MystiickWeb.Server](./MystiickWeb.Server) project for the server application that contains the web APIs that the client will call.
 - [./MystiickWeb.Server.Clients](./MystiickWeb.Server.Clients) project containing File/Data clients that integrate with files, databases, and other implementation heavy classes.
 - [./MystiickWeb.Shared](./MystiickWeb.Shared) contains models that are shared between projects, namely Client/Server models and configuration models.

## Quickstart
### Dev
To stand up a development environment, you can run `./quickstart-dev.sh` in [quickstart](./quickstart/). This will stand up a mysql database container (port 3306) and adminer container (http://localhost:8080). It will also create an `appsettings.dev.json` file for you that can be modified as needed for local development.

Since adminer is hosted inside the same docker-compose file, you'll need to access it using the server `quickstart-database` instead of `localhost`, which is what your appsettings will need to use. This is due to the network adapter for the containers using Docker's network, not the parent system's.

### Demo
To startup a demo application with some sample seeded data, you can run `./quickstart.sh` in [quickstart](./quickstart/), and stop and clean up with `./quickstop.sh`.

Alternatively if you don't wish to run it yourself, you can view it in action at the URL at the top.