using UnityEngine;

[System.Serializable]
public struct NetworkVector3
{
    public float x;
    public float y;
    public float z;

    public NetworkVector3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public NetworkVector3(Vector3 vector)
    {
        this.x = vector.x;
        this.y = vector.y;
        this.z = vector.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }

    public override bool Equals(object obj)
    {
        if (!(obj is NetworkVector3)) return false;
        NetworkVector3 other = (NetworkVector3)obj;
        return this.x == other.x && this.y == other.y && this.z == other.z;
    }

    public static bool operator ==(NetworkVector3 a, NetworkVector3 b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(NetworkVector3 a, NetworkVector3 b)
    {
        return !(a == b);
    }
}
