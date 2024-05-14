using MinecraftConnection;
using OpenCvSharp;
using System;


string address = "localhost"; // サーバーのアドレス
ushort port = 25575; // RCONのポート番号
string pswd = "minecraft"; // RCONのパスワード
MinecraftCommands command = new MinecraftCommands(address, port, pswd);

string videoPath = "/Users/potesala/badapple-48x.mp4"; // 再生する動画のパス

command.SetSubTitle("feat. nomico");
command.DisplayTitle("Bad Apple!!");

using(var capture = new VideoCapture(videoPath))
{
    if(!capture.IsOpened())
    {
        Console.WriteLine("動画ファイルが開けませんでした");
        return;
    }
    var img = new Mat();
    while(true)
    {
        if (capture.Read(img))
        {
            Cv2.ImShow("Bad Apple!!", img);
            Cv2.WaitKey(1000 / 30); // 1フレームあたりの時間 (30fps)
        } else {
            break; // 動画の最後まで再生したら終了 (フレームが空のため終了する)
        }
    }
}

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

