# ColorBarLib
C# で カラーバーを作成するためのカラーパレットを作成するライブラリ
一般的に言う HeatMap (ヒートマップ) と同じだと思われます。

## ビルド環境
- Visual Studio 2022 Community
- .net6.0-windows

## 他プロジェクトで利用する場合
１．
- Release でビルドする
- ColorBarLib\bin\Release\net6.0-windows\ColorBarLib.dll を自分の開発環境で読み込む

２．
- 「ColorBarLib」フォルダを自分の開発環境にコピーする
- ColorBarLib.csproj を自分の開発環境に取り込む
- 別プロジェクトからプロジェクト参照する

## 使い方
C#:グレースケール
```
colorPalette = new()
{
    MinValue = 0,
    MaxValue = 255
};
// 上記指定したプロパティ値を利用
Color color = colorPalette.GetRGBColor(value);
// または上記指定したプロパティ値を無視し、最大値・最小値を指定
// Color color = colorPalette.GetRGBColor(value, minValue, maxValue);
```
C#:カラー
```
ColorPalette colorPalette = new("#FF0000, #FFFF00, #00FF00, #00FFFF, #0000FF, #FF00FF, #FF0000")
{
    MinValue = 0,
    MaxValue = 255
};
// 上記指定したプロパティ値を利用
Color color = colorPalette.GetRGBColor(value);
// または上記指定したプロパティ値を無視し、最大値・最小値を指定
// Color color = colorPalette.GetRGBColor(value, minValue, maxValue);
```

## サンプル画像
ColorBarLib\SampleFile\neco.csv を利用

![ねこ_1](https://user-images.githubusercontent.com/4666260/203235016-0f86ad61-db6c-4e03-901f-f9b2ef29fb9f.png)
![ねこ_2](https://user-images.githubusercontent.com/4666260/203235026-e8dc431b-c584-4c95-b0b5-94e6c6563057.png)
![ねこ_3](https://user-images.githubusercontent.com/4666260/203235031-9ffc65ba-4020-49b4-9b07-f413a7fb4b3a.png)
![ねこ_4](https://user-images.githubusercontent.com/4666260/203235041-bae01fef-3c0d-4cbb-8e74-c2f08cac0be6.png)

# 出典
* RGB ←→ HSL 変換機能は 「Dobon.net」さんの変換ソースを利用しています。
https://dobon.net/vb/dotnet/graphics/hsv.html
* サンプル画像のイメージは「いらすとや」さんの「ねこ」を元に作成しています。
https://www.irasutoya.com/2019/06/blog-post_83.html

# License(ライセンス)
[MIT license](https://en.wikipedia.org/wiki/MIT_License).

日本語：(https://licenses.opensource.jp/MIT/MIT.html)

# その他
- .net 6 を利用していますが、.NET Framework などでも動くはずです…
  ソースだけコピーしてください。

