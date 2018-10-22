using ProtoBuf;

namespace Rpc.Demo
{
    [ProtoContract]
    public class UserModel
    {
        [ProtoMember(1)] public string Name { get; set; }

        [ProtoMember(2)] public int Age { get; set; }
        
        [ProtoMember(3)] public string Content { get; set; }
    }
}