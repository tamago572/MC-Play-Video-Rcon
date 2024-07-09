using MinecraftConnection;
using OpenCvSharp;
using System.Diagnostics;

class Program
{
    static void Main()
    {
        string address = "localhost"; // サーバーのアドレス
        ushort port = 25575; // RCONのポート番号
        string pswd = "minecraft"; // RCONのパスワード
        MinecraftCommands command = new MinecraftCommands(address, port, pswd);

        string videoPath = "C:\\Users\\7f7fn\\Downloads\\badapple-48x-8fps.mp4"; // 再生する動画のパス
        //string videoPath = "C:\\Users\\7f7fn\\ikisugi48.mp4"; // 再生する動画のパス


        // titleコマンドを使ってタイトルを表示
        //command.SetSubTitle("feat. nomico");
        //command.DisplayTitle("Bad Apple!!");
        command.DisplayMessage(videoPath);

        // プレイヤーの位置を取得
        string playerName = "dorayaki9999"; // プレイヤー名
        Position pos = command.GetPlayerData(playerName).Position;

        // すべての種類のコンクリートブロック
        // string[] concreteBlocks = ["white_concrete", "orange_concrete", "magenta_concrete", "light_blue_concrete", "yellow_concrete", "lime_concrete", "pink_concrete", "gray_concrete", "light_gray_concrete", "cyan_concrete", "purple_concrete", "blue_concrete", "brown_concrete", "green_concrete", "red_concrete", "black_concrete"];

        // ---------------------
        // 色の判定用辞書

        string[] concreteBlocks = {
            "white_concrete", "orange_concrete", "magenta_concrete", "light_blue_concrete",
            "yellow_concrete", "lime_concrete", "pink_concrete", "gray_concrete",
            "light_gray_concrete", "cyan_concrete", "purple_concrete", "blue_concrete",
            "brown_concrete", "green_concrete", "red_concrete", "black_concrete"
        };

        Dictionary<string, (int R, int G, int B)> concreteColors = new Dictionary<string, (int, int, int)>
        {
            {"white_concrete", (207, 213, 214)},
            {"orange_concrete", (224, 97, 0)},
            {"magenta_concrete", (169, 48, 159)},
            {"light_blue_concrete", (36, 137, 199)},
            {"yellow_concrete", (241, 175, 21)},
            {"lime_concrete", (94, 168, 24)},
            {"pink_concrete", (214, 101, 143)},
            {"gray_concrete", (54, 57, 61)},
            {"light_gray_concrete", (135, 138, 143)},
            {"cyan_concrete", (21, 119, 136)},
            {"purple_concrete", (100, 32, 156)},
            {"blue_concrete", (44, 46, 143)},
            {"brown_concrete", (96, 59, 31)},
            {"green_concrete", (73, 91, 36)},
            {"red_concrete", (142, 32, 32)},
            {"black_concrete", (8, 10, 15)}
        };

        




        // ここから動画を再生
        using (var capture = new VideoCapture(videoPath))
        {
            if (!capture.IsOpened())
            {
                Console.WriteLine("動画ファイルが開けませんでした");
                return;
            }

            var img = new Mat();

            int width = capture.FrameWidth;
            int height = capture.FrameHeight;
            Console.WriteLine($"width: {width}, height: {height}");

            Cv2.WaitKey(2000); // 2秒待機

            // 音声の再生
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = "C:\\ffmpeg\\bin\\ffplay.exe";
            processStartInfo.Arguments = $"-nodisp -autoexit {videoPath}";

            Process.Start(processStartInfo);

            while (true)
            {
                if (capture.Read(img))
                {
                    //Cv2.ImShow("Bad Apple!!", img);
                    // 左右反転
                    Cv2.Flip(img, img, FlipMode.Y);

                    // 画像からピクセルごとの色を取得
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            var color = img.At<Vec3b>(y, x);
                            int r = color.Item2;
                            int g = color.Item1;
                            int b = color.Item0;

                            command.SetBlock((int)pos.X - width / 2 + x, (int)pos.Y + height / 2 - y, (int)pos.Z + 20, GetClosestConcreteBlock(concreteColors, r, g, b));
                            // Example usage
                            //var closestBlock = GetClosestConcreteBlock(concreteColors, 200, 150, 100);
                            //Console.WriteLine($"The closest concrete block is: {closestBlock}");
                        }
                    }

                    Console.WriteLine("frame ended");
                    // 1フレームあたりの時間を入力 (これは8fpsの場合)
                    PreciseWait(125);
                }
                else
                {
                    command.DisplayMessage("Complete!!!");
                    break; // 動画の最後まで再生したら終了 (フレームが空のため終了する)
                }
            }
        }
        
    }

    // 以下の関数は、RGBの値から、何色のコンクリートブロックを設置するかを判定し、そのブロック名を返す関数です
    static string GetClosestConcreteBlock(Dictionary<string, (int R, int G, int B)> concreteColors, int r, int g, int b)
    {
        string closestBlock = null;
        double closestDistance = double.MaxValue;

        foreach (var block in concreteColors)
        {
            double distance = GetColorDistance(block.Value.R, block.Value.G, block.Value.B, r, g, b);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestBlock = block.Key;
            }
        }

        return closestBlock;
    }

    static double GetColorDistance(int r1, int g1, int b1, int r2, int g2, int b2)
    {
        return Math.Sqrt(Math.Pow(r1 - r2, 2) + Math.Pow(g1 - g2, 2) + Math.Pow(b1 - b2, 2));
    }


    // タイミング制御
    static void PreciseWait(int milliseconds)
    {
        // ストップウォッチを使って待機時間を計測
        Stopwatch stopwatch = Stopwatch.StartNew();
        long initialTicks = stopwatch.ElapsedTicks;
        long targetTicks = milliseconds * Stopwatch.Frequency / 1000;

        // 指定された時間が経過するまでループ
        while (stopwatch.ElapsedTicks - initialTicks < targetTicks)
        {
            // スピンウェイトを使ってCPU使用率を高くする
            Thread.SpinWait(1);
        }
        stopwatch.Stop();
    }
}

