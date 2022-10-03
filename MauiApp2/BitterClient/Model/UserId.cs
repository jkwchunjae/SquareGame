namespace BitterClient.Model;

public class UserId : IComparable<UserId>, IEquatable<UserId>
{
    public string Id { get; set; }

    public UserId(string id)
    {
        Id = id;
    }

    public int Length => Id.Length;

    public bool Contains(string value) => Id.Contains(value);

    public override string ToString()
    {
        return Id;
    }
    public int CompareTo(UserId? other)
    {
        return Id.CompareTo(other?.Id);
    }
    public static bool operator ==(UserId obj1, UserId obj2)
    {
        if (ReferenceEquals(obj1, obj2))
        {
            return true;
        }
        if (ReferenceEquals(obj1, null))
        {
            return false;
        }
        if (ReferenceEquals(obj2, null))
        {
            return false;
        }

        return obj1.Equals(obj2);
    }
    public static bool operator !=(UserId obj1, UserId obj2)
    {
        return !(obj1 == obj2);
    }
    public static bool operator ==(UserId obj1, string obj2)
    {
        return obj1.Id == obj2;
    }
    public static bool operator !=(UserId obj1, string obj2)
    {
        return !(obj1 == obj2);
    }
    public bool Equals(UserId? other)
    {
        if (ReferenceEquals(other, null))
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Id == other.Id;
    }
    public override bool Equals(object? obj)
    {
        return Equals(obj as UserId);
    }
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
