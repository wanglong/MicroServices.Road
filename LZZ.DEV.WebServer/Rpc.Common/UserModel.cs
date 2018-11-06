using ProtoBuf;

namespace Rpc.Common
{
    [ProtoContract(DataMemberOffset = 0)]
    public class UserModel
    {
        [ProtoMember(1)] public string Name { get; set; }

        [ProtoMember(2)] public int Age { get; set; }
    }
}