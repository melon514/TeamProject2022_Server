using System;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
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

        public async UnaryResult<bool> AsyncAddScore(string name, int score)
        {
            ServerInfo.GetServerInfo().ScoreList[name] += score;
            return true;
        }

        public async UnaryResult<List<int>> AsyncGetScores()
        {
            return ServerInfo.GetServerInfo().ScoreList.Values.ToList();
        }
        
        public async UnaryResult<int> GetConnectCount(string name)
        {
            //あるかどうかを確認してなかった場合は登録してから返すようにする
            if (ServerInfo.GetServerInfo().Players.ContainsKey(name))
            {
                return ServerInfo.GetServerInfo().Players[name];
            }
            else
            {
                
                ServerInfo.GetServerInfo().Players.Add(name,ServerInfo.GetServerInfo().Players.Count);
                return ServerInfo.GetServerInfo().Players[name];
            }
        }

        public async UnaryResult<List<string>> GetOtherPlayers(string name)
        {
            List<string> Others = new List<string>(ServerInfo.GetServerInfo().Players.Keys);
            Others.Remove(name);
            return Others;

        }


    }
}