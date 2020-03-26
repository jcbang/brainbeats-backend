using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace brainbeats_backend.Controllers
{
// Path: api/user
[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly Database db;
    private readonly IConfiguration Configuration;

    public UsersController(IConfiguration configuration)
    {
        // Initialize Configuration File
        Configuration = configuration;

        // Initialize Database
        string endpoint = Configuration["Database:Endpoint"];
        string key = Configuration["Database:Key"];
        db = new CosmosClient(endpoint, key).GetDatabase("BrainBeats");
    }

    [Route("register")]
    [HttpPost]
    public async Task<IActionResult> RegisterUser(User user)
    {
        // Verify the request is well formed
        if (user.firstName == null || user.lastName == null || user.email == null)
        {
            return BadRequest("Malformed Request");
        }

        // Generate a new unique primary key
        user.id = Guid.NewGuid().ToString();

        string queryString = "SELECT * FROM u WHERE u.email = @email";
        QueryDefinition queryDefinition = new QueryDefinition(queryString).WithParameter("@email", user.email);

        FeedIterator<User> feedIterator = db.GetContainer("Users").GetItemQueryIterator<User>(queryDefinition);

        // Check to see if this email already exists
        while (feedIterator.HasMoreResults)
        {
            FeedResponse<User> response = await feedIterator.ReadNextAsync();
            if (response.Count > 0)
            {
                return BadRequest("Email already in use");
            }
        }

        ItemResponse<User> res = await db.GetContainer("Users").CreateItemAsync(user, new PartitionKey(user.email));

        if (res.StatusCode == HttpStatusCode.Created)
            return Ok();
        else
            return BadRequest("Something went wrong");
    }
}
}