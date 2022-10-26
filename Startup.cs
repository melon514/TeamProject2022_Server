﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TeamProject2022.Shared.MessagePacks;
using UnityEngine;


namespace Server
{

    public sealed class ServerInfo
    {
        /*
         * @var     TimeLimit
         * @brief   ゲームの時間制限
         *          これを弄って決めるといいよ
         */
        public float TimeLimit = 300;
        /*
         * @var     span
         * @brief   一秒にかかる時間
         *          この変数をクライアントに渡し続ける
         */
        public float span { get; set; }

        //for debug
        public List<Targets> targets = new List<Targets>();
        private static ServerInfo SERVERINFO = new ServerInfo();

        private ServerInfo() { }

        /*
         * @func     GetServerInfo
         * @brief   シングルトン
         *          主にサーバーの情報を渡し続けるのでこんな感じで書いた
         */
        public static ServerInfo GetServerInfo()
        {
            return SERVERINFO;
        }

        /*
         * @func    SetUpTimeSpan
         * @brief   タイムスパン設定用関数
         *          サーバー側がUnityのTimeを使えないためStartUpで1秒を作る必要があったのでこんな感じにしてる
         *          使ってるのは"System.Threading.Tasks"の"Stopwatch"
         */
        public async void SetUpTimeSpan()
        {
            var sw = new Stopwatch();
            sw.Start();
            await Task.Delay(1000);
            sw.Stop();
            var sp = new TimeSpan();
            sp = sw.Elapsed;
            span = Convert.ToSingle(sp.TotalSeconds);
        }

        public void AsyncClock()
        {
            while (true)
            {
                //note:(melon)  実体に持たせててずっと保存されてるため1ループしたら削除するようにこの処理
                if (targets.Count >= 1)
                {
                    targets.Clear();
                }
                //ミリ秒換算で1秒スリープしてlimitを減らしてる
                System.Threading.Thread.Sleep(1000);
                TimeLimit -= span;

                if (Math.Floor(TimeLimit) % 10 == 0)
                {
                    //スレッドで実行するとだいぶ重くなるので別枠でタスクを走らせる
                    Task.Run(() => logic());

                }
            }
        }

        public async void logic()
        {
            //UnityEngineのRandomが使えませんクソです
            System.Random pos;
            //あった時邪魔なので削除

            pos = new System.Random((int)TimeLimit);
            //NOTE:(melon)  ここはとりあえず4体生成したいのでを入れてる
            for (int i = 0; i < 4; ++i)
            {
                Targets t = new Targets();
                t.id = i;
                t.x = pos.Next(-500, 500);
                t.y = 0.0f;
                t.z = pos.Next(-500, 500);
                //t.pos = new Vector3(pos.Next(-500, 500),
                //    0.0f,
                //    pos.Next(-500, 500));

                //完成品をlistに入れる
                targets.Add(t);
                Console.WriteLine(t.id + ":" + t.x + "_" + t.y + "_" + t.z);
            }
        }
    }


    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();
            services.AddMagicOnion();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //タイムスパン取得用
            ServerInfo.GetServerInfo().SetUpTimeSpan();
            Thread clock = new Thread(new ThreadStart(ServerInfo.GetServerInfo().AsyncClock));
            //Thread logic = new Thread(new ThreadStart(ServerInfo.GetServerInfo().logic));
            clock.Start();
            //logic.Start();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapGrpcService<GreeterService>();
                endpoints.MapMagicOnionService();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }
    }
}
