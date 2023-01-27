# MystiickWeb.Clients
This library is responsible for all of the integration logic. This includes Database integrations, Web/API integrations, and file system integrations.

All classes in this project should be pretty minimal when it comes to business logic, and the **must** implement an interface from [MystiickWeb.Core](../MystiickWeb.Core/Interfaces/Clients) if they are to be injected into a servcie. Supporting classes or models that do not need to be shared between other projects can live within this project with discretion.