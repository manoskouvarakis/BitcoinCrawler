# BitcoinCrawler

The project is a .net Core 2 Console Application that fetches bitcoin price from Bitstamp and Gdax exchange service using the respective API.

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes

### Prerequisites

In order to run the application locally, you will need [Microsoft Visual Studio 2017 Community](https://www.visualstudio.com/vs/community/) (or higher). 
All required parameters are configurable through appsettings.json

### Installing

In order to run the project in Visual Studio 2017:

```
- Press F5 to run
```

In order to close the application, press **CTRL + C** in order to send a signal to stop every working task when it finishes.

### Running the tests

In order to run the tests in Visual Studio 2017:

```
- Test -> Run -> All Tests
```

### Implementation 

The base action of the application is **HarvestTask** that consist of pairs of [exchange service, currency] in order to fetch the specific currency pair from an exchange service. 

**HarvestingOrchestratorService** creates such tasks for workers and adds them in a concurrent collection every x seconds (by default set to 30). Those tasks are configured in appsettings.json.

**HarvestingWorkerService** checks periodically (default every 10 seconds) the concurrent collection for new tasks and if it finds one, it handles it through the respective exchange service created by **HarvestingServiceFactory**.
Every new price is stored in memory by **RepositoryService** that implements a concurrent dictionary with key the currency pair and values of concurrent queues of prices (the size of that queue is configurable and default is set to 10).

**PrintoutService** wakes up every x seconds (default is 10) and prints statistics about new prices with aggregated data fetched from **RepositoryService**.

Every parameter for the application to execute is configured through **appsettings.json**.