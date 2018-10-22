using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rpc.Demo;

namespace Rpc.Server
{
    /// <summary>
    /// DemoService µœ÷¿‡
    /// </summary>
    public class DemoServiceImpl : IDemoService
    {
        // ReSharper disable once RedundantAssignment
        public Task<string> TestInt(int id) => Task.FromResult((id++).ToString());

        public Task<int> TestString(string str) => Task.FromResult(int.Parse(str));

        public Task<bool> TestBool(bool log) => Task.FromResult(!log);

        public Task<DateTime> TestDateTime(DateTime dateTime) => Task.FromResult(dateTime.AddDays(1));

        public Task<byte> TestByte(byte @by) => Task.FromResult<byte>(245);

        public Task<UserModel> TestModel(UserModel userModel) => Task.FromResult(userModel);

        public Task<List<string>> TestDictionary(List<string> dictionary) => Task.FromResult(dictionary);

        public Task<Dictionary<string, string>> TestDictionary(Dictionary<string, string> dictionary) => Task.FromResult(dictionary);

        public Task<List<Dictionary<string, string>>> TestCollections(List<Dictionary<string, string>> collection) => Task.FromResult(collection);
    }
}