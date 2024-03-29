﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagicOnion.Server.Hubs;
using Server;
using TeamProject2022.Shared.Hub;
using TeamProject2022.Shared.MessagePacks;
using UnityEngine;

namespace TeamProject2022.Hubs
{
    public class GamingHub : StreamingHubBase<IGamingHub, IGamingHubReceiver>, IGamingHub
    {
        IGroup _room;
        Player _self;
        IInMemoryStorage<Player> _storage;

        public async Task<Player[]> JoinAsync(string RoomName, string UserName,
            Vector3 Position, Quaternion Rotation)
        {
            //var temp_id = Server.ServerInfo.GetServerInfo().PlayerList.Count;

            //_self = new Player
            //{
            //    Name = UserName,
            //    Position = Position,
            //    Rotation = Rotation,
            //    score = 0,
            //    hp = Server.ServerInfo.GetServerInfo().MaxHp,
            //    id = temp_id,
            //    shotflg = false,
            //    barrierflg = false

            //};
            ////入室してきたときにDictionaryの値を追加
            //Server.ServerInfo.GetServerInfo().ScoreList.Add(UserName, 0);
            ////Server.ServerInfo.GetServerInfo().PlayerList.Add(_self);
            //(_room, _storage) = await Group.AddAsync(RoomName, _self);
            ////BroadcastExceptSelf(_room).OnJoin(_self);

            ////BroadcastExceptSelf(_room).OnJoin(_self);
            //Broadcast(_room).OnJoin(_self);
            ////Broadcast(_room).OnJoin(_self);


            //return _storage.AllValues.ToArray();
            var temp_id = Server.ServerInfo.GetServerInfo().PlayerList.Count;
            _self = new Player
            {
                Name = UserName,
                Position = Position,
                Rotation = Rotation,
                score = 0,
                hp = Server.ServerInfo.GetServerInfo().MaxHp,
                id = temp_id,
                shotflg = false,
                barrierflg = false,
                TargetName = "None"
            };
            //入室してきたときにDictionaryの値を追加
            Server.ServerInfo.GetServerInfo().ScoreList.Add(UserName, 0);
            //Server.ServerInfo.GetServerInfo().PlayerList.Add(_self.Name, _self);
            //Console.WriteLine("ConnectedPlayer:" + Server.ServerInfo.GetServerInfo().PlayerList.Count);
            (_room, _storage) = await Group.AddAsync(RoomName, _self);

            //BroadcastExceptSelf(_room).OnJoin(_self);

            //BroadcastExceptSelf(_room).OnJoin(_self);
            Broadcast(_room).OnJoin(_self);
            //Broadcast(_room).OnJoin(_self);


            return _storage.AllValues.ToArray();

        }

        public async Task<Player[]> JoinAsync_test(string RoomName,
            string UserName, Vector3 Position, Quaternion Rotation)
        {
            //var temp_id = Server.ServerInfo.GetServerInfo().PlayerList.Count;
            int index = 0;
            foreach (var player in Room.GetRoomInfo().getServerInfos(RoomName).PlayerList)
            {
                if (player.Value.Name == UserName)
                {
                  break;  
                }
                ++index;
            }
            _self = new Player
            {
                Name = UserName,
                Position = Position,
                Rotation = Rotation,
                time = 300,
                score = 0,
                //hp = Server.ServerInfo.GetServerInfo().MaxHp,
                hp = 100,
                id = index,
                shotflg = false,
                barrierflg = false,
                TargetName = "None",
                Immolized = false

            };
            //入室してきたときにDictionaryの値を追加
            //Server.ServerInfo.GetServerInfo().ScoreList.Add(UserName, 0);
            Room.GetRoomInfo().getServerInfos(RoomName).ScoreList.Add(UserName, 0);
            //Server.ServerInfo.GetServerInfo().PlayerList.Add(_self.Name, _self);
            //Console.WriteLine("ConnectedPlayer:" + Server.ServerInfo.GetServerInfo().PlayerList.Count);
            (_room, _storage) = await Group.AddAsync(RoomName, _self);
            //BroadcastExceptSelf(_room).OnJoin(_self);

            //BroadcastExceptSelf(_room).OnJoin(_self);
            Broadcast(_room).OnJoin(_self);
            //Broadcast(_room).OnJoin(_self);


            return _storage.AllValues.ToArray();
        }


        public async Task LeaveAsync()
        {
            Broadcast(_room).OnLeave(_self);
            await _room.RemoveAsync(Context);
        }

        public async Task MoveAsync(Vector3 pos, Quaternion rot)
        {
            _self.Position = pos;
            _self.Rotation = rot;
            Broadcast(_room).OnMove(_self);
        }
        public async Task MoveAsync_test(Vector3 pos, Quaternion rot,
            float hp,bool shotflg,bool barrierflg,
            string TargetName,float InterbalShot,bool immolized)
        {
            _self.hp = hp;
            _self.Position = pos;
            _self.Rotation = rot;
            _self.shotflg = shotflg;
            _self.barrierflg = barrierflg;
            if (TargetName == null)
            {
                _self.TargetName = "None";
            }
            else
            {
                _self.TargetName = TargetName;
            }

            _self.InterbalCount = InterbalShot;
            _self.Immolized = immolized;
            Broadcast(_room).OnMove_test(_self);
        }


        protected override async ValueTask OnDisconnected()
        {
            await CompletedTask;
        }


    }
}