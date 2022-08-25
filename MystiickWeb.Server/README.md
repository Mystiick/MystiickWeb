# MystiickWeb.Server
This application project is responsible for hosting both the Web API for MystiickWeb, as well as hosting the Blazor WASM project that is compiled in [MystiickWeb.Wasm](../MystiickWeb.Wasm/). It contains simple controllers that pass data to the proper [service class(es)](../MystiickWeb.Core/Services/) to validate and return any relevant data.

The output `MystiickWeb.Server.dll` from this project is the entry point for the entire application. It is responsible for standing up the Kestrel web server to host the controllers and .wasm files.