# MC-BadApple

MinecraftでBad Apple!!のPVをブロックで再現するスクリプト

サーバーのRCON機能を活用する

OpenCVで画素の色情報を取得し、色によってMinecraftにSetBlockコマンドを送信する

実際実行するとクソ重いので編集で倍速するといい感じになるかもしれません。

原因としては1pxずつ/setblockで置いてるので/fillとかにすれば軽くなるかも？
