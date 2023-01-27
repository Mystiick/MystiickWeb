# MystiickWeb.Core
This library contains the vast majority of business logic required to properly run the application. It does not do any direct queries nor integrate directly with any APIs. All of the integration code is in [MystiickWeb.Clients](../MystiickWeb.Clients/), using clients injected via interface.

## Interfaces
This project contains both interfaces for Services and the implementing Clients. 
- Services should only contain any business logic or validations needed.
- Clients should only contain the integration code, and keep all business logic left to the services where possible.

MystiickWeb.Core also contains implementations for the services, leaving the implementations for hte Clients up to [MystiickWeb.Clients](../MystiickWeb.Clients/)