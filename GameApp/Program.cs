using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace BunkerSurvival
{
    class Program
    {
        // ===== C 游戏内核接口（P/Invoke）=====
        [DllImport("GameCore.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Game_Init();

        [DllImport("GameCore.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Game_NextDay();

        [DllImport("GameCore.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Game_GetDay();

        [DllImport("GameCore.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Game_GetHealth();

        [DllImport("GameCore.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Game_GetFood();

        [DllImport("GameCore.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Game_GetWater();

        [DllImport("GameCore.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Game_IsAlive();

        // ===== 静态随机对象 =====
        static Random rnd = new Random();

        // ===== 打字机打印函数 =====
        static void TypewriterPrint(string text, int speed = 40)
        {
            foreach (char c in text)
            {
                Console.Write(c);
                Thread.Sleep(speed);
            }
            Console.WriteLine();
        }

        // ===== ASCII Logo =====
        static void PrintAsciiLogo()
        {
            string[] logo =
            {
                " ____  _   _ _   _ _  __    ___   ____  ____  ",
                "| __ )| | | | \\ | | |/ /   / _ \\ / ___|| __ ) ",
                "|  _ \\| | | |  \\| | ' /   | | | | |  _ |  _ \\ ",
                "| |_) | |_| | |\\  | . \\   | |_| | |_| || |_) |",
                "|____/ \\___/|_| \\_|_|\\_\\   \\___/ \\____||____/ "
            };

            Console.ForegroundColor = ConsoleColor.Gray;
            foreach (var line in logo)
            {
                TypewriterPrint(line, 25);
            }
            Console.ResetColor();
        }

        // ===== 随机末日文本（UI 层，不影响数值）=====
        static string GetRandomBunkerEvent()
        {
            string[] events =
            {
                "地堡大门传来沉重的撞击声，通风系统发出了刺耳的磨损声。",
                "警报灯短暂闪烁，备用电源自动接管。",
                "墙体传来低沉回响，像是远方的爆炸。",
                "空气过滤系统发出异常噪声，但仍在运行。",
                "广播里只有白噪声，你的心跳格外清晰。"
            };

            return events[rnd.Next(events.Length)];
        }

        static void Main(string[] args)
        {
            Console.Title = "BUNKER-OS - 末日地堡生存模拟";

            // 1. 启动画面
            PrintAsciiLogo();

            Console.ForegroundColor = ConsoleColor.Gray;
            TypewriterPrint("正在链接深层地堡服务器...");
            TypewriterPrint("正在扫描生命体征...");
            Console.ResetColor();

            // 2. 初始化 C 游戏内核
            bool kernelReady = false;
            int simulatedDay = 0;  // 模拟模式下使用
            try
            {
                TypewriterPrint("\n正在尝试链接底层内核...");
                Game_Init();
                kernelReady = true;
                TypewriterPrint("内核链接成功。");
            }
            catch (EntryPointNotFoundException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                TypewriterPrint("\n[系统错误] 内核函数导出失败。目前处于“模拟演示模式”。");
                TypewriterPrint("[建议] 请检查 C 语言函数前是否加了 __declspec(dllexport)。");
                Console.ResetColor();
            }

            // 3. 初始状态展示（保护调用）
            int initHealth = kernelReady ? Game_GetHealth() : 100;
            Console.ForegroundColor = ConsoleColor.Green;
            TypewriterPrint($"[状态] 初始生命值: {initHealth}");
            Console.ResetColor();

            // ===== 玩家命令循环 =====
            while (true)
            {
                bool alive = kernelReady ? Game_IsAlive() != 0 : initHealth > 0;
                if (!alive)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    TypewriterPrint("生命体征消失……地堡归于沉寂。");
                    Console.ResetColor();
                    break;
                }

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("\n> ");
                Console.ResetColor();

                string? inputRaw = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(inputRaw))
                    continue;

                string input = inputRaw.Trim().ToLower();

                if (input == "exit")
                {
                    TypewriterPrint("正在断开与地堡系统的连接……");
                    break;
                }
                else if (input == "status")
                {
                    int day = kernelReady ? Game_GetDay() : simulatedDay;
                    int hp = kernelReady ? Game_GetHealth() : initHealth;
                    int food = kernelReady ? Game_GetFood() : 10;
                    int water = kernelReady ? Game_GetWater() : 10;

                    Console.ForegroundColor = ConsoleColor.Green;
                    TypewriterPrint($"第 {day} 天");
                    TypewriterPrint($"生命: {hp}");
                    TypewriterPrint($"食物: {food}");
                    TypewriterPrint($"水源: {water}");
                    Console.ResetColor();
                }
                else if (input == "next")
                {
                    if (kernelReady)
                        Game_NextDay();
                    else
                        simulatedDay++;  // DLL 未加载时自增天数

                    int day = kernelReady ? Game_GetDay() : 1;
                    int hp = kernelReady ? Game_GetHealth() : initHealth;
                    int food = kernelReady ? Game_GetFood() : 10;
                    int water = kernelReady ? Game_GetWater() : 10;

                    Console.ForegroundColor = ConsoleColor.Gray;
                    TypewriterPrint($"—— 第 {day} 天 ——");
                    TypewriterPrint(GetRandomBunkerEvent());
                    Console.ResetColor();

                    Console.ForegroundColor = ConsoleColor.Green;
                    TypewriterPrint($"生命: {hp}  食物: {food}  水源: {water}");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    TypewriterPrint("未知命令。可用命令: status, next, exit");
                    Console.ResetColor();
                }
            }
        }
    }
}
