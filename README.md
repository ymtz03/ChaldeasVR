# ChaldeasVR
Molecular orbital viewer with VR

分子軌道データの可視化アプリケーションです。  
量子化学計算ソフトで計算された fchk や cube 形式の分子軌道情報ファイルを読み込み、表示します。

## [ダウンロード (バイナリ)](https://github.com/ymtz03/ChaldeasVR/releases/tag/v1.0)
MacOS用の .app 形式と Android用 .apk 形式の実行ファイルをダウンロードできます。  
（ビルドに使用したUnityのバージョンは`2017.2.17f1`です）

[fchk / cube サンプルファイル](https://drive.google.com/open?id=1f0nKwczLRPjSbt8lK1Rdeg1ZXNr4vaRg)

## 使用方法

### 起動

可視化したいfchkまたはcube形式のファイルをpersisitent datapath直下に置いて起動してください。persistent datapathはOSによって異なり、MacOSとAndroidの場合はそれぞれ以下の場所です。

MacOS : `~/Library/Application Support/ymtz03/ChaldeasVR/`  
Android : `/storage/emulated/0/Android/data/tk.ymtz03.ChaldeasVR/files/`

（詳しくは[公式ドキュメント](https://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html)を参考にしてください。）

### 操作
VRデバイスを使う場合は機器の向きで操作します。  
PCの場合はキーボードで操作できます。  

|VRデバイス|キー||
|:--|:--|:--|
|上を向く|`I`|回転（上）|
|下を向く|`K`|回転（下）|
|左を向く|`J`|回転（左）|
|右を向く|`L`|回転（右）|
|左に首をかしげる|`U`|拡大|
|右に首をかしげる|`O`|縮小|

<img src="https://user-images.githubusercontent.com/33852087/53624972-34566d00-3c45-11e9-9c99-bf0f3bcbb423.png" width=450px>

左側のパネルに persistent datapath 直下に置いたファイルの一覧が表示されます。
パネルに視点を合わせることでファイルをロードできます。

ファイルをロードすると、右側のパネルに描画できる分子軌道の一覧が表示され、
パネルに視点を合わせると分子軌道が描画されます。
それぞれの分子軌道がロード済みかどうかはパネルの色で表されています。

|パネル|ロード状況|
|:--|:--|
|灰色|未ロード|
|白色|ロード済|
|緑色|ロード済（現在表示中）|

<img src="https://user-images.githubusercontent.com/33852087/54669036-b397dc00-4b33-11e9-9a05-6fed722eee5a.gif" width=450px>

## ダウンロード (ソースコード)

`git clone https://github.com/ymtz03/ChaldeasVR.git`

シーンは Assets/Scenes/Scene です

プロジェクトの中で[Oculus Integration](https://assetstore.unity.com/packages/tools/integration/oculus-integration-82022)のアセットを使用しているので、ビルドする際はインポートしてもらう必要があります（Oculus Integration ver 1.34 で動作確認しました）。
インポートする必要があるのはOculus/VR以下だけです。

#### Macの場合  
`Oculus/VR/Scripts/OVRPlugin.cs`がコンパイルエラーになる場合があるらしいので[こちら](https://qiita.com/Sam/items/d050db69b5e2a4929672)などを参考にスクリプトを書き換えてください。
