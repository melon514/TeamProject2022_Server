using System;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using MagicOnion;
using MagicOnion.Server;
using Server;
using TeamProject2022.Shared.Services;
using TeamProject2022.Shared.MessagePacks;


namespace Client.Services
{
    public class MyFirstService : ServiceBase<WorldInformation>, WorldInformation
    {
        public async UnaryResult<int> SumAsync(int x, int y)
        {
            //Console.WriteLine($"Received:{x} {y}");
            return x + y;
            
        }
        public async UnaryResult<float> AsyncTimeSet(float time)
        {
            
            return ServerInfo.GetServerInfo().TimeLimit;
        }

        public async UnaryResult<List<Targets>> AsyncTargets()
        {
            return ServerInfo.GetServerInfo().targets;
        }

    }
}