using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagicOnion.Server.Hubs;
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
            _self = new Player { Name = UserName, Position = Position, Rotation = Rotation };
            (_room, _storage) = await Group.AddAsync(RoomName, _self);
            //BroadcastExceptSelf(_room).OnJoin(_self);

            //BroadcastExceptSelf(_room).OnJoin(_self);
            Broadcast(_room).OnJoin(_self);


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


        protected override async ValueTask OnDisconnected()
        {
            await CompletedTask;
        }
        
        
    }
}