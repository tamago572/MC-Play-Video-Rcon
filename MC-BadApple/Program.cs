using MinecraftConnection;
using OpenCvSharp;
using System.Diagnostics;


string address = "localhost"; // サーバーのアドレス
ushort port = 25575; // RCONのポート番号
string pswd = "minecraft"; // RCONのパスワード
MinecraftCommands command = new MinecraftCommands(address, port, pswd);

string videoPath = "C:\\Users\\7f7fn\\Downloads\\badapple-48x-8fps.mp4"; // 再生する動画のパス

// titleコマンドを使ってタイトルを表示
command.SetSubTitle("feat. nomico");
command.DisplayTitle("Bad Apple!!");

// プレイヤーの位置を取得
string playerName = "dorayaki9999"; // プレイヤー名
Position pos = command.GetPlayerData(playerName).Position;

// ここから動画を再生
using(var capture = new VideoCapture(videoPath))
{
    if(!capture.IsOpened())
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

    while(true)
    {
        if (capture.Read(img))
        {
            //Cv2.ImShow("Bad Apple!!", img);

            
            // 画像からピクセルごとの色を取得
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var color = img.At<Vec3b>(y, x);
                    int r = color.Item2;
                    int g = color.Item1;
                    int b = color.Item0;

                    // ブロックの色を設定
                    if (r < 32)
                    {
                        command.SetBlock((int)pos.X-height/2+x, (int)pos.Y + height/2 - y, (int)pos.Z+20 , "black_concrete");
                    } else
                    {
                        command.SetBlock((int)pos.X-height/2+x, (int)pos.Y+height/2 - y, (int)pos.Z+20, "white_concrete");
                    }
                }
            }

            Cv2.WaitKey(1000 / 8); // 1フレームあたりの時間 (30fps)

        } else {
            break; // 動画の最後まで再生したら終了 (フレームが空のため終了する)
        }
    }
}

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

