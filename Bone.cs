using System.Text.RegularExpressions;

namespace SmdConverter;

public class Bone
{
    public string Name;
    public int Parent;
    public int Index;
    public List<int> Children;

    public Bone(int index, string name,int parent)
    {
        Name = Regex.Replace(name, @"[^a-zA-Z0-9_]", "_");
        Name = Regex.Replace(Name, @"valve", "aaaaa", RegexOptions.IgnoreCase);
        Parent = parent;
        Index = index;
        Children = new List<int>();
    }
}