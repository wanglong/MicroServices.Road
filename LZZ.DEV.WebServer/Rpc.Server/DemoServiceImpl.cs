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
        public Task<string> TestInt(int id)
        {
            throw new NotImplementedException();
        }

        public Task<int> TestString(string str)
        {
            throw new NotImplementedException();
        }

        public Task<bool> TestBool(bool log)
        {
            throw new NotImplementedException();
        }

        public Task<DateTime> TestDateTime(DateTime dateTime)
        {
            throw new NotImplementedException();
        }

        public Task<byte> TestByte(byte @by)
        {
            throw new NotImplementedException();
        }

        public Task<UserModel> TestLong(UserModel userModel)
        {
            throw new NotImplementedException();
        }

        public Task<List<string>> TestDictionary(List<string> dictionary)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<string, string>> TestDictionary(Dictionary<string, string> dictionary)
        {
            throw new NotImplementedException();
        }

        public Task<List<Dictionary<string, string>>> TestCollections(List<Dictionary<string, string>> collection)
        {
            throw new NotImplementedException();
        }

        public Task Try()
        {
            throw new NotImplementedException();
        }
    }
}