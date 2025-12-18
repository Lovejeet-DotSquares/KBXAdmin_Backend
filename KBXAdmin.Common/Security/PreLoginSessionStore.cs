public static class PreLoginSessionStore
{
    private static readonly Dictionary<string, PreLoginSession> _store = new();

    public static string Create(PreLoginSession session)
    {
        string id = Guid.NewGuid().ToString();
        _store[id] = session;
        return id;
    }

    public static PreLoginSession? Get(string id)
    {
        return _store.TryGetValue(id, out var session) ? session : null;
    }
}

public class PreLoginSession
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public string Role { get; set; } = default!;
}
