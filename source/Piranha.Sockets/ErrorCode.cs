namespace Piranha.Sockets;

public readonly struct ErrorCode
{
    private readonly string? _name;
    private readonly string? _description;

    public readonly int Id { get; }
    public readonly string Name => _name ?? "";
    public readonly string Description => _description ?? "";

    public ErrorCode(int id) : this(id, "UNKNOWN", "Unrecognized error")
    {
    }

    public ErrorCode(int id, string name, string description)
    {
        Id = id;
        _name = name;
        _description = description;
    }

    public override string ToString() => $"{Name}({Id}) {Description}";
}
