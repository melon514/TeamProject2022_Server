using System.Linq;
using System.Threading.Tasks;
using MagicOnion.Server.Hubs;
using TeamProject2022.Shared.Hub;
using TeamProject2022.Shared.MessagePackObject;
using UnityEngine;

namespace TeamProject2022.Hubs
{
    public class GamingHub : StreamingHubBase<IGamingHub, IGamingHubReceiver>, IGamingHub
    {
        IGroup _room;
        Player _self;
        Player _player;
        //IInMemoryStorage<DebugPlayer> _strage;
        /*
         * @val     _strage
         * @brief   それぞれのプレイヤーを格納している変数
         *          配列のように _strage.AllValues.Countのように扱える？
         */
        IInMemoryStorage<Player> _strage;

        //public async Task<DebugPlayer[]> JoinAsync(string RoomName, string UserName,
        //    Vector3 Position, Quaternion Rotation)
        //{
        //    _self = new DebugPlayer { Name = UserName, Position = Position, Rotation = Rotation };
        //    (_room, _strage) = await Group.AddAsync(RoomName, _self);
        //    BroadcastExceptSelf(_room).OnJoin(_self);
        //    BroadcastExceptSelf(_room).OnJoin(_player);

        //    return _strage.AllValues.ToArray();
        //}
        public async Task<Player[]> JoinAsync(string RoomName, string UserName,
            Vector3 Position, Quaternion Rotation)
        {
            int a = _strage.AllValues.Count;
            _self = new Player { Name = UserName, Position = Position, Rotation = Rotation };
            (_room, _strage) = await Group.AddAsync(RoomName, _self);
            //BroadcastExceptSelf(_room).OnJoin(_self);
            BroadcastExceptSelf(_room).OnJoin(_player,_self.Position);

            return _strage.AllValues.ToArray();
        }

        public async Task LeaveAsync()
        {
            Broadcast(_room).OnLeave(_self);
            await _room.RemoveAsync(Context);
        }

        public async Task MoveAsynk(Vector3 pos, Quaternion rot)
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