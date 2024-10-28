# Mwf_AnnotationTool

mwf形式の心電図データのアノテーションを行うツールです

## 概要

- mwf形式の心電図データを表示し、10秒ごとのブロックにラベルを付与することができます。
- 付与できるラベルは以下の通りです。
    - Normal：通常の拍動
    - AF：心房細動
    - Noise：データが正しく取れていない箇所
    - SSS：洞不全症候群
    - AVB：房室ブロック
    - Pose：数秒以上の心停止
    - NSVT：非持続性心室頻拍
    - PVC：心室性期外収縮
    - PAC：心房性期外収縮
- 付与したラベルはcsvファイル形式で保存できます
- csvファイルにはNormal以外のラベルの付与状況が以下のような形式で保存されています
    - ID：上の表のAFから順に1から8までのいずれかの数字
    - Start,End：心電図を10秒ごとに分割したブロックの最初を0として、そのラベルが連続で付与されている最初のブロックと最後のブロックのインデックス
    - Label：付与されているラベル名
    - StartTime,EndTime：ラベルが付与されている部分が計測された日時

## 使い方

- 左上の”open file”からmwfファイルを選択し、開きます
    - mwfファイルと同じディレクトリに以前保存されたcsvファイルが存在する場合ラベルの付与状況を反映します
- 画面下部でラベルをクリック、またはテンキーによりラベルを選択できます
- 心電図を右クリックで指定したブロックに選択したラベルを付与できます
    - 右クリックしたままドラッグすることでカーソルの通ったブロックにラベルを付与することもできます
    - また、ダブルクリックによって選択したブロックより前にある同じラベルが付与されているブロックまでのすべてのブロックに同時にラベルを付与することが可能です
- 心電図を左クリックすることで右下にそのブロックの心電図を拡大できます
    - 拡大された心電図の左で数字を入力もしくは矢印ボタンで表示する最大値と最小値を設定できます
- 左下の矢印ボタンでページ移動ができるほか、その下にページ数を入力し”Jump”ボタンを押すことで指定したページに移動できます
- 画面右上で数字を入力もしくは矢印ボタンを押すことで表示する心電図の最大値と最小値を設定できます
- 左上の”save file”から付与したラベルを保存できます
    - csvファイルはmwfファイルと同じディレクトリに、”「指定したmwfファイル名」.csv”の名前で保存されます


## サンプル
mwfファイルのサンプルとして"sample.mwf"、csvファイルのサンプルとして"sample.mwf.csv"を用意しています