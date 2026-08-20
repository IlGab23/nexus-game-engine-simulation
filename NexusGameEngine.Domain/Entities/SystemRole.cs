using System.Text.RegularExpressions;
using NexusGameEngine.Domain.ResultPattern;

namespace NexusGameEngine.Domain.Entities;

public sealed partial class SystemRole
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public bool AdminRole { get; init; }
    public bool IsActive { get; private set; }

    private SystemRole(Guid id, string name, string description, bool adminRole, bool isActive)
    {
        Id = id;
        Name = name;
        Description = description;
        AdminRole = adminRole;
        IsActive = isActive;
    }

    public static Result<SystemRole> Create(string name, string description, bool adminRole, bool isActive = true)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Error.Validation("SystemRole.NameEmpty", "The role name cannot be empty or whitespace.");

        if (name.Length is < 2 or > 50)
            return Error.Validation("SystemRole.NameLength", "The role name must be between 2 and 50 characters.");

        // Assicura che il nome contenga solo lettere, numeri, trattini o underscore (niente caratteri strani o SQL injection mascherate)
        if (!RoleRegex().IsMatch(name))
            return Error.Validation("SystemRole.NameFormat", "The role name can only contain letters, numbers, dashes, and underscores.");

        if (string.IsNullOrWhiteSpace(description))
            return Error.Validation("SystemRole.DescriptionEmpty", "The role description cannot be empty.");

        if (description.Length > 255)
            return Error.Validation("SystemRole.DescriptionLength", "The role description cannot exceed 255 characters.");

        return new SystemRole(Guid.NewGuid(), name, description, adminRole, isActive);
    }

    public void ToggleRoleActive()
    {
        IsActive = !IsActive;
    }

    [System.Text.RegularExpressions.GeneratedRegex(@"^[a-zA-Z0-9\-_]+$")]
    private static partial Regex RoleRegex();

}
