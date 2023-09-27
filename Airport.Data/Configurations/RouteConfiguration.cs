using Airport.Models.Entities;
using Airport.Models.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Airport.Data.Configurations
{
    internal class RouteConfiguration
    {
        public async Task ConfigureAsync(IMongoClient client, IAirportDbConfiguration dbSettings)
        {
            var routesCollection = client
                .GetDatabase(dbSettings.DatabaseName)
                .GetCollection<Route>(dbSettings.RoutesCollectionName);
            var data = new List<Route>
            {
                new Route
                {
                    RouteId = new ObjectId("650abb1ee574435a814d7ec0"),
                    RouteName = "Landing",
                    Directions = new List<Direction>
                    {
                        new Direction
                        {
                            From = new ObjectId("000000000000000000000001"),
                            To = new ObjectId("000000000000000000000002"),
                        },
                        new Direction
                        {
                            From = new ObjectId("000000000000000000000002"),
                            To = new ObjectId("000000000000000000000003"),
                        },
                        new Direction
                        {
                            From = new ObjectId("000000000000000000000003"),
                            To = new ObjectId("000000000000000000000004"),
                        },
                        new Direction
                        {
                            From = new ObjectId("000000000000000000000004"),
                            To = new ObjectId("000000000000000000000005"),
                        },
                        new Direction
                        {
                            From = new ObjectId("000000000000000000000005"),
                            To = new ObjectId("000000000000000000000006"),
                        },
                        new Direction
                        {
                            From = new ObjectId("000000000000000000000005"),
                            To = new ObjectId("000000000000000000000007"),
                        },
                    }
                },
                new Route
                {
                    RouteId = new ObjectId("650abb1ee574435a814d7ec1"),
                    RouteName = "Departure",
                    Directions = new List<Direction>
                    {
                        new Direction
                        {
                            From = new ObjectId("000000000000000000000006"),
                            To = new ObjectId("000000000000000000000008")
                        },
                        new Direction
                        {
                            From = new ObjectId("000000000000000000000007"),
                            To = new ObjectId("000000000000000000000008")
                        },
                        new Direction
                        {
                            From = new ObjectId("000000000000000000000008"),
                            To = new ObjectId("000000000000000000000004")
                        },
                        new Direction
                        {
                            From = new ObjectId("000000000000000000000004"),
                            To = new ObjectId("000000000000000000000009"),
                        },
                    }
                }
            };
            try
            {
                await routesCollection.InsertManyAsync(data);
            }
            catch (Exception e)
            {
                await Task.FromException(e);
            }
        }
    }
}