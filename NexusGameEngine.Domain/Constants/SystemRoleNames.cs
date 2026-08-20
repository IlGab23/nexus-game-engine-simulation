namespace NexusGameEngine.Domain.Constants;

public record BaseSystemRole(string Name, string Description, bool IsAdminRole);
public static class SystemRoleNames
{
    public readonly static BaseSystemRole Admin = new("Admin", "A Company member", true);
    public static Guid? AdminId { get; private set; } = null;
    public readonly static BaseSystemRole PlayerPlus = new("PlayerPlus", "A Player with a more expensive edition who play the game", false);
    public static Guid? PlayerPlusId { get; private set; } = null;
    public readonly static BaseSystemRole Player = new("Player", "A Player who play the game", false);
    public static Guid? PlayerId { get; private set; } = null;

    public static List<(string Name, string Description, bool IsAdminRole)> GetAllRoles() => new([(Admin.Name, Admin.Description, Admin.IsAdminRole), (PlayerPlus.Name, PlayerPlus.Description, PlayerPlus.IsAdminRole), (Player.Name, Player.Description, Player.IsAdminRole)]);

    public static void SetRoleId(Guid adminId, Guid playerPlusId, Guid playerId)
    {
        AdminId = adminId;
        PlayerPlusId = playerPlusId;
        PlayerId = playerId;
    }
}
