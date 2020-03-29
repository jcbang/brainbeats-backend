using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace brainbeats_backend.Controllers
{
public class DatabaseContext
{
    public Database db {
        get;
    }

    public DatabaseContext(IConfiguration configuration)
    {
        CosmosClient client;

        // Connect to CosmosDB Client
        if (configuration["Environment"] == "Prod")
        {
            client = new CosmosClient(configuration["Database:ConnectionStrings:Prod"]);
        }
        else
        {
            client = new CosmosClient(configuration["Database:ConnectionStrings:Dev"]);
        }

        // Initialize Database
        client.CreateDatabaseIfNotExistsAsync(configuration["Database:DatabaseName"]).Wait();
        db = client.GetDatabase(configuration["Database:DatabaseName"]);

        // Create Containers
        db.CreateContainerIfNotExistsAsync("Users", "/email").Wait();
        db.CreateContainerIfNotExistsAsync("Beats", "/id").Wait();
        db.CreateContainerIfNotExistsAsync("Playlists", "/id").Wait();
        db.CreateContainerIfNotExistsAsync("Samples", "/id").Wait();
    }
}
}
