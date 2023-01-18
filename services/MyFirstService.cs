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
                Player pl = new Player();
                pl.Name = name;
                pl.hp = 100;

                //登録された時についでにリストに追加
                ServerInfo.GetServerInfo().PlayerReady.Add(name, false);
                var count = ServerInfo.GetServerInfo().Players.Count;
                ServerInfo.GetServerInfo().Players.Add(name, count);
                ServerInfo.GetServerInfo().PlayerList.Add(name, pl);
                ServerInfo.GetServerInfo().Shotflgs.Add(name, false);
                ServerInfo.GetServerInfo().Barrierflgs.Add(name, false);
                //ServerInfo.GetServerInfo().PlayerList.Add(name,);
                return count;
            }
        }

        public async UnaryResult<List<string>> GetOtherPlayers(string name)
        {
            List<string> Others = new List<string>(ServerInfo.GetServerInfo().Players.Keys);
            Others.Remove(name);
            return Others;

        }

        public async UnaryResult<bool> OnReady(string name, bool ready)
        {
            Console.WriteLine(ServerInfo.GetServerInfo().PlayerReady.Count);
            //そもそも4人集まってなかったら始めないようにする、デフォルト4人
            //todo(melon):  ルーム作成の処理を創ったらここをそのルームのプレイヤーの限界数に変更
            if (ServerInfo.GetServerInfo().PlayerReady.Count < 1)
            {
                return false;
            }

            //値を入れる
            ServerInfo.GetServerInfo().PlayerReady[name] = ready;

            foreach (var playerstate in ServerInfo.GetServerInfo().PlayerReady)
            {
                Console.WriteLine(playerstate.Key + ":" + playerstate.Value);
                //一人でも準備してなかったらfalseを返す
                if (!playerstate.Value)
                {
                    return false;
                }
            }

            Console.WriteLine("all player ready");
            //全員準備完了
            return true;
        }

        public async UnaryResult<bool> HitResult(string from, string to)
        {
            var tempHP = ServerInfo.GetServerInfo().PlayerList[to].hp -= 3;
            if (tempHP <= 0)
            {
                return false;
            }
            Console.WriteLine("Hit_Endpoint->" + to);
            //当たったらhpを減らすけど継続なのか一括で減らすのかわからなかったためこうしてる
            if (ServerInfo.GetServerInfo().PlayerList[to].hp <= 0)
            {
                ServerInfo.GetServerInfo().PlayerList[to].hp = ServerInfo.GetServerInfo().MaxHp;
                ServerInfo.GetServerInfo().ScoreList[from] += 3000;

                return true;
            }

            return false;
        }


        public async UnaryResult<float> SetterHP(string name)
        {
            return ServerInfo.GetServerInfo().PlayerList[name].hp;
        }

        public async UnaryResult<bool> SetMaxHP(float hp)
        {
            ServerInfo.GetServerInfo().MaxHp = hp;
            return true;
        }

        public async UnaryResult<bool> ChangeShotflg(string name,bool flg)
        {
            if (ServerInfo.GetServerInfo().Shotflgs[name] != flg)
            {
                Console.WriteLine("Cahnged_shot");
            }
            return ServerInfo.GetServerInfo().Shotflgs[name] = flg;

            //通知が来たら逆の方を入れる
            //return ServerInfo.GetServerInfo().Shotflgs[name] =
            //    !ServerInfo.GetServerInfo().Shotflgs[name];
        }

        public async UnaryResult<bool> ChangeBarrierflg(string name,bool flg)
        {
            if (ServerInfo.GetServerInfo().Barrierflgs[name] != flg)
            {
                Console.WriteLine("Cahnged_barrier");
            }
            return ServerInfo.GetServerInfo().Barrierflgs[name] = flg;

            //通知が来たら逆の方を入れる
            //return ServerInfo.GetServerInfo().Barrierflgs[name] =
            //    !ServerInfo.GetServerInfo().Barrierflgs[name];
        }

        public async UnaryResult<Dictionary<string, bool>> EnemysShotFlg(string name)
        {
            Dictionary<string, bool> temp_shotflg =
                new Dictionary<string, bool>(ServerInfo.GetServerInfo().Shotflgs);
            temp_shotflg.Remove(name);
            return temp_shotflg;
        }
        public async UnaryResult<Dictionary<string, bool>> EnemysBarrerFlg(string name)
        {
            Dictionary<string, bool> temp_barrerflg =
                new Dictionary<string, bool>(ServerInfo.GetServerInfo().Barrierflgs);
            temp_barrerflg.Remove(name);
            return temp_barrerflg;
        }
    }
}
