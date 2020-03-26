namespace brainbeats_backend.Controllers
{
public class User
{
    public string id
    {
        get;
        set;
    }
    public string email
    {
        get;
        set;
    }
    public string firstName
    {
        get;
        set;
    }
    public string lastName
    {
        get;
        set;
    }
    public string[] savedBeats
    {
        get;
        set;
    }
    public string[] savedPlaylists
    {
        get;
        set;
    }
}
}
