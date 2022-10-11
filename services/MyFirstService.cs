using System;
using MagicOnion;
using MagicOnion.Server;
using TeamProject2022.Shared.Services;


namespace Client.Services
{
    public class MyFirstService : ServiceBase<IMyFirstService>, IMyFirstService
    {
        public async UnaryResult<int> SumAsync(int x, int y)
        {
            Console.WriteLine($"Received:{x} {y}");
            return x + y;
        }
    }
}