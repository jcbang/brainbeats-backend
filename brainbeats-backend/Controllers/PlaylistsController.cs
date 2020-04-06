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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Runtime.Remoting;

namespace brainbeats_backend.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class PlaylistsController : ControllerBase
  {
    private readonly Database db;

    public PlaylistsController(IConfiguration configuration) {
      db = new DatabaseContext(configuration).db;
    }

    // Initializes a new playlist given a userId and a beatId
    [Route("create")]
    [HttpPost]
    public async Task<IActionResult> createPlaylist(dynamic req) {
      var body = JsonConvert.DeserializeObject<dynamic>(req.ToString());

      // Required fields: userId and beatId
      if (body.email == null || body.beatId == null || body.name == null) {
        return BadRequest("Malformed Request");
      }

      // Get matching user object from the database
      string queryString = "SELECT * FROM u WHERE u.email = @email";
      QueryDefinition queryDefinition =
          new QueryDefinition(queryString).WithParameter("@email", body.email);

      FeedIterator<User> feedIterator =
          db.GetContainer("Users").GetItemQueryIterator<User>(queryDefinition);

      List<User> users = new List<User>();

      // Get the matching Sample with our ID
      while (feedIterator.HasMoreResults) {
        FeedResponse<User> response = await feedIterator.ReadNextAsync();

        foreach (User u in response) {
          users.Add(u);
        }
      }

      // users[0] holds our returned user object
      User user = users[0];

      // New ID for our playlist
      string newPlaylistId = Guid.NewGuid().ToString();

      if (user.savedPlaylists == null) {
        // Initialize their saved playlists with our new playlist
        user.savedPlaylists = new string[1];

        user.savedPlaylists[0] = newPlaylistId;
      } else {
        // Copy over the pre-existing data into a new array
        string[] newSavedPlaylists = new string[user.savedPlaylists.Length + 1];

        for (int i = 0; i < user.savedPlaylists.Length; i++) {
          newSavedPlaylists[i] = user.savedPlaylists[i];
        }

        // Add on the new playlist to the end of the array
        newSavedPlaylists[newSavedPlaylists.Length - 1] = newPlaylistId;

        user.savedPlaylists = newSavedPlaylists;
      }

      Playlist newPlaylistObject = new Playlist(newPlaylistId);
      newPlaylistObject.beatList = new string[1];
      newPlaylistObject.beatList[0] = body.beatId;

      // Update user object in the database
      ItemResponse<User> userRes = await db.GetContainer("Users").ReplaceItemAsync(user,
          user.id, new PartitionKey(user.email));

      if (userRes.StatusCode != HttpStatusCode.OK) {
        return BadRequest("Something went wrong");
      }

      // Upsert new playlist object into the database
      ItemResponse<Playlist> playlistRes = await db.GetContainer("Playlists").UpsertItemAsync(
        newPlaylistObject, new PartitionKey(newPlaylistObject.id));

      if (playlistRes.StatusCode == HttpStatusCode.Created) {
        return Ok();
      } else {
        return BadRequest("Something went wrong");
      }
    }
  }
}