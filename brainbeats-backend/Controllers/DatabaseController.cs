using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace brainbeats_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatabaseController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public DatabaseController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // Recreates the database in the event of data loss;
        // "_IfNotExists" prevents us from overwriting data if there already
        // exists a Database or Container
        [Route("create-database")]
        [HttpPost]
        public async Task<IActionResult> CreateDatabase()
        {
            // Initialize Database
            string env = Configuration["Environment"];
            string dbName = Configuration["Database:DatabaseName"];
            string endpoint = Configuration["Database:" + env + ":Endpoint"];
            string key = Configuration["Database:" + env + ":Key"];

            try
            {
                // Create Database if it does not yet exist
                CosmosClient client = new CosmosClient(endpoint, key);
                await client.CreateDatabaseIfNotExistsAsync(dbName);

                // Create Containers if they do not yet exist
                Database db = client.GetDatabase(dbName);
                await db.CreateContainerIfNotExistsAsync("Users", "/email");
                await db.CreateContainerIfNotExistsAsync("Beats", "/id");
                await db.CreateContainerIfNotExistsAsync("Playlists", "/id");
                await db.CreateContainerIfNotExistsAsync("Samples", "/id");

                return Ok();
            }
            catch (CosmosException e)
            {
                return BadRequest(e);
            }
        }
    }
}