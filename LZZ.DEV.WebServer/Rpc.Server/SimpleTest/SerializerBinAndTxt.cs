using System;
using System.IO;
using Newtonsoft.Json;
using ProtoBuf;
using Rpc.Demo;

namespace Rpc.Server.SimpleTest
{
    public static class SerializerBinAndTxt
    {
        public static void Test()
        {
            for (var i = 0; i < 5; i++)
            {
                TestSerializer();
                Console.WriteLine();
            }
        }

        private static void TestSerializer()
        {
            var userModel = new UserModel
            {
                Name = "aaaaaaa",
                Age = 30000000,
                Content = "一、Google官方版本:https://github.com/google/protobuf/tree/master/csharp(谷歌官方开发、比较晦涩,主库名字:Google.ProtoBuf.dll) 二、.Net社区版本:https:..."
            };

            using (var fs = File.Create(AppDomain.CurrentDomain.BaseDirectory + "/user.bin"))
            {
                var dateBegin = DateTime.Now;
                Serializer.Serialize(fs, userModel);
                Console.WriteLine($"Serializer to bin duration : {(DateTime.Now - dateBegin).TotalMilliseconds}ms, {fs.Length} byte");
            }

            {
                File.Create(AppDomain.CurrentDomain.BaseDirectory + "/user.txt").Close();
                var dateBegin = DateTime.Now;
                var str = JsonConvert.SerializeObject(userModel);
                Console.WriteLine($"Serializer to txt duration : {(DateTime.Now - dateBegin).TotalMilliseconds}ms, {System.Text.Encoding.Default.GetBytes(str).Length} byte");
                using (var sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "/user.txt"))
                {
                    sw.Write(str);
                    sw.Flush();
                }
            }
        }
    }
}