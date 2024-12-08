namespace SmdConverter;

public class S2VANMResults
{
    public List<string> m_boneNames { get; set; } = new();
    public List<List<int>> m_children{ get; set; } = new();
    public List<int> m_parents{ get; set; } = new();
    public List<List<float>> m_modelSpaceTransforms{ get; set; } = new();
    public List<List<float>> m_localSpaceTransforms { get; set; }= new();

    public S2VANMResults(List<Bone> bones)
    {
        List<float> randomList = new();
        foreach (var bone in bones)
        {
            m_boneNames.Add(bone.Name);
            m_parents.Add(bone.Parent);
            m_children.Add(bone.Children);
            m_modelSpaceTransforms.Add(new List<float> {0,0,0,1,1,0,0,0});
            m_localSpaceTransforms.Add(new List<float> {0,0,0,1,1,0,0,0});
        }
    }
}