using System;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MagicOnion;
using MagicOnion.Server;
using Server;
using TeamProject2022.Shared.Services;
using TeamProject2022.Shared.MessagePacks;
using System.Text.RegularExpressions;

namespace Client.Services
{
    public class MyFirstService : ServiceBase<WorldInformation>, WorldInformation
    {

        public async UnaryResult<float> AsyncTimeSet(float time)
        {

            return ServerInfo.GetServerInfo().TimeLimit;
        }        
        
        public async UnaryResult<float> AsyncTimeSet_room(string room,float time)
        {
            return Room.GetRoomInfo().getServerInfos(room).TimeLimit;
        }

        public async UnaryResult<List<Targets>> AsyncTargets()
        {
            return ServerInfo.GetServerInfo().targets;
        }
        public async UnaryResult<List<Targets>> AsyncTargets_room(string room)
        {
            return Room.GetRoomInfo().getServerInfos(room).targets;
        }

        public async UnaryResult<bool> AsyncAddScore(string name, int score)
        {
            ServerInfo.GetServerInfo().ScoreList[name] += score;
            return true;
        }
        public async UnaryResult<bool> AsyncAddScore_room(string room,string name, int score)
        {
            Room.GetRoomInfo().getServerInfos(room).ScoreList[name] += score;
            return true;
        }

        public async UnaryResult<bool> AsyncAddScore_room_WhenDestroy(string room, string ForAddScorePlayerName, int score)
        {
            Room.GetRoomInfo().getServerInfos(room).ScoreList[ForAddScorePlayerName] += score;
            return true;
        }

        public async UnaryResult<int> AsyncGetScore_room_Revision(string room ,string name)
        {
            int RevisionScore = Room.GetRoomInfo().getServerInfos(room).ScoreList[name];
            return RevisionScore;
        }

        public async UnaryResult<List<int>> AsyncGetScores()
        {
            return ServerInfo.GetServerInfo().ScoreList.Values.ToList();
        }

        public async UnaryResult<List<int>> AsyncGetScores_room(string room)
        {
            return Room.GetRoomInfo().getServerInfos(room).ScoreList.Values.ToList();
            //return ServerInfo.GetServerInfo().ScoreList.Values.ToList();
        }

        public async UnaryResult<Dictionary<string, int>> AsyncGetNameAndScore()
        {

            return ServerInfo.GetServerInfo().ScoreList;
        }

        public async UnaryResult<Dictionary<string, int>> AsyncGetNameAndScore_room(string room)
        {

            return Room.GetRoomInfo().getServerInfos(room).ScoreList;
            //return ServerInfo.GetServerInfo().ScoreList;
        }

        public async UnaryResult<int> GetMaxPlayerCount()
        {
            return ServerInfo.GetServerInfo().MaxPlayerCount;
        }

        public async UnaryResult<int> GetMaxPlayerCount_room(string room)
        {
            return Room.GetRoomInfo().getServerInfos(room).MaxPlayerCount;
            //return ServerInfo.GetServerInfo().MaxPlayerCount;
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
                return count;
            }
        }

        public async UnaryResult<int> GetConnectCount_room(string room,string name)
        {
            //if (Room.GetRoomInfo().getServerInfos(room)!=null)
            //{
            //    //ある場合
            //    return Room.GetRoomInfo().getServerInfos(room).Players[name];
            //}
            //else
            {
                //ない場合なので諸々登録してから自分の番号を返却
                Player pl = new Player();
                pl.Name = name;
                pl.hp = 100;

                //登録された時についでにリストに追加
                if (Room.GetRoomInfo().getServerInfos(room) == null)
                {
                    Room.GetRoomInfo().AddServerInfo(room);
                }

                Room.GetRoomInfo().getServerInfos(room).PlayerReady.Add(name, false);
                var count = Room.GetRoomInfo().getServerInfos(room).Players.Count;
                Room.GetRoomInfo().getServerInfos(room).Players.Add(name, count);
                Room.GetRoomInfo().getServerInfos(room).PlayerList.Add(name, pl);
                return count;
            }
        }
        public async UnaryResult<int> GetConnectCount_room_member(string room, string name, int maxmember)
        {
            //if (Room.GetRoomInfo().getServerInfos(room)!=null)
            //{
            //    //ある場合
            //    return Room.GetRoomInfo().getServerInfos(room).Players[name];
            //}
            //else
            {
                //ない場合なので諸々登録してから自分の番号を返却
                Player pl = new Player();
                pl.Name = name;
                pl.hp = 100;

                //登録された時についでにリストに追加
                if (Room.GetRoomInfo().getServerInfos(room) == null)
                {
                    Room.GetRoomInfo().AddServerInfo(room);
                    if (maxmember != -1)
                    {
                        Room.GetRoomInfo().getServerInfos(room).MaxPlayerCount = maxmember;
                        Console.WriteLine("Configlate_MAXMember:" +
                                          Room.GetRoomInfo().getServerInfos(room).MaxPlayerCount);

                    }
                }
                Room.GetRoomInfo().getServerInfos(room).PlayerReady.Add(name, false);
                var count = Room.GetRoomInfo().getServerInfos(room).Players.Count;
                Room.GetRoomInfo().getServerInfos(room).Players.Add(name, count);
                Room.GetRoomInfo().getServerInfos(room).PlayerList.Add(name, pl);
                return count;
            }
        }


        public async UnaryResult<List<string>> GetOtherPlayers(string name)
        {
            List<string> Others = new List<string>(ServerInfo.GetServerInfo().Players.Keys);
            Others.Remove(name);
            return Others;
        }
        public async UnaryResult<List<string>> GetOtherPlayers_room(string room,string name)
        {
            //List<string> Others = new List<string>(ServerInfo.GetServerInfo().Players.Keys);
            List<string> Others = new List<string>(Room.GetRoomInfo().getServerInfos(room).Players.Keys);
            Others.Remove(name);
            return Others;
        }

        public async UnaryResult<bool> OnReady(string name, bool ready)
        {
            Console.WriteLine(ServerInfo.GetServerInfo().PlayerReady.Count);
            //そもそも4人集まってなかったら始めないようにする、デフォルト4人
            //todo(melon):  ルーム作成の処理を創ったらここをそのルームのプレイヤーの限界数に変更
            if (ServerInfo.GetServerInfo().PlayerReady.Count < ServerInfo.GetServerInfo().MaxPlayerCount)
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

            //タイムスパンの取得と制限時間の開始
            //ここで人数確認して最後の人が入ってきたらこのスレッドを開始する
            if (ServerInfo.GetServerInfo().InPlayerCount == ServerInfo.GetServerInfo().MaxPlayerCount - 1)
            {
                ServerInfo.GetServerInfo().ThreadLife = true;
                ServerInfo.GetServerInfo().clock = new Thread(new ThreadStart(ServerInfo.GetServerInfo().AsyncClock));
                ServerInfo.GetServerInfo().clock.Start();

                ServerInfo.GetServerInfo().InPlayerCount++;
            }
            else
            {
                ServerInfo.GetServerInfo().InPlayerCount++;
            }
            //全員準備完了
            return true;
        }

        public async UnaryResult<bool> OnReady_room(string room,string name, bool ready)
        {
            Console.WriteLine(Room.GetRoomInfo().getServerInfos(room).PlayerReady.Count);
            //そもそも人数人集まってなかったら始めないようにする
            //todo(melon):  ルーム作成の処理を創ったらここをそのルームのプレイヤーの限界数に変更
            if (Room.GetRoomInfo().getServerInfos(room).PlayerReady.Count < Room.GetRoomInfo().getServerInfos(room).MaxPlayerCount)
            {
                return false;
            }

            //値を入れる
            Room.GetRoomInfo().getServerInfos(room).PlayerReady[name] = ready;

            foreach (var playerstate in Room.GetRoomInfo().getServerInfos(room).PlayerReady)
            {
                Console.WriteLine(playerstate.Key + ":" + playerstate.Value);
                //一人でも準備してなかったらfalseを返す
                if (!playerstate.Value)
                {
                    return false;
                }
            }

            Console.WriteLine("all player ready");

            //タイムスパンの取得と制限時間の開始
            //ここで人数確認して最後の人が入ってきたらこのスレッドを開始する
            if (Room.GetRoomInfo().getServerInfos(room).InPlayerCount == Room.GetRoomInfo().getServerInfos(room).MaxPlayerCount - 1)
            {
                Room.GetRoomInfo().getServerInfos(room).SetUpTimeSpan();
                Room.GetRoomInfo().getServerInfos(room).ThreadLife = true;
                Room.GetRoomInfo().getServerInfos(room).clock =
                    new Thread(new ThreadStart(Room.GetRoomInfo().getServerInfos(room).AsyncClock));
                Room.GetRoomInfo().getServerInfos(room).clock.Start();

                Room.GetRoomInfo().getServerInfos(room).InPlayerCount++;
            }
            else
            {
                Room.GetRoomInfo().getServerInfos(room).InPlayerCount++;
            }
            //全員準備完了
            return true;
        }





        public async UnaryResult<bool> InitializeServerConfig()
        {
            ServerInfo.GetServerInfo().InPlayerCount--;
            //退出したプレイヤーの数がリスト数に満たない場合はまだルームの掃除をしない
            if(ServerInfo.GetServerInfo().InPlayerCount != 0)
            {
                return false;
            }

            ServerInfo.GetServerInfo().PlayerReady.Clear();
            ServerInfo.GetServerInfo().PlayerList.Clear();
            ServerInfo.GetServerInfo().TimeLimit = ServerInfo.GetServerInfo().TimeLimit_Default;
            ServerInfo.GetServerInfo().Players.Clear();
            ServerInfo.GetServerInfo().targets.Clear();
            ServerInfo.GetServerInfo().ThreadLife = false;
            ServerInfo.GetServerInfo().ScoreList.Clear();
            ServerInfo.GetServerInfo().InPlayerCount = 0;

            return true;
        }

        public async UnaryResult<bool> InitializeServerConfig_room(string room, string name)
        {
            //Room.GetRoomInfo().getServerInfos(room).InPlayerCount--;
            //退出したプレイヤーの数がリスト数に満たない場合はまだルームの掃除をしない
            //if(Room.GetRoomInfo().getServerInfos(room).InPlayerCount != 0)
            //{
            //    return false;
            //}

            //Room.GetRoomInfo().getServerInfos(room).PlayerReady.Clear();
            //Room.GetRoomInfo().getServerInfos(room).PlayerList.Clear();
            //Room.GetRoomInfo().getServerInfos(room).TimeLimit = Room.GetRoomInfo().getServerInfos(room).TimeLimit_Default;
            //Room.GetRoomInfo().getServerInfos(room).Players.Clear();
            //Room.GetRoomInfo().getServerInfos(room).targets.Clear();
            //Room.GetRoomInfo().getServerInfos(room).ThreadLife = false;
            //Room.GetRoomInfo().getServerInfos(room).ScoreList.Clear();
            //Room.GetRoomInfo().getServerInfos(room).InPlayerCount = 0;
            //Room.GetRoomInfo().serverInfos.Remove(room);

            //ルームに登録されてる人の情報の削除
            Room.GetRoomInfo().getServerInfos(room).PlayerReady.Remove(name);
            Room.GetRoomInfo().getServerInfos(room).PlayerList.Remove(name);
            Room.GetRoomInfo().getServerInfos(room).TimeLimit = Room.GetRoomInfo().getServerInfos(room).TimeLimit_Default;
            Room.GetRoomInfo().getServerInfos(room).Players.Remove(name);
            Room.GetRoomInfo().getServerInfos(room).ScoreList.Remove(name);
            Room.GetRoomInfo().getServerInfos(room).InPlayerCount--;

            //ルーム内にいるプレイヤーが0人でルームを削除
            if (Room.GetRoomInfo().getServerInfos(room).InPlayerCount == 0)
            {
                Room.GetRoomInfo().getServerInfos(room).targets.Clear();
                Room.GetRoomInfo().getServerInfos(room).ThreadLife = false;
                Room.GetRoomInfo().serverInfos.Remove(room);
            }
            //Room.GetRoomInfo().getServerInfos(room).targets.Remove(name);
            //Room.GetRoomInfo().getServerInfos(room).ThreadLife = false;
            //Room.GetRoomInfo().serverInfos.Remove(room);

            return true;
        }
    }
}