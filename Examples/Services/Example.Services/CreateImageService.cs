using ColorBarLib;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace Example.Services
{
    /// <summary>
    /// カラーバーライブラリを使って、画像を作成する
    /// </summary>
    public class CreateImageService
    {
        /// <summary>
        /// 出力画像の上下左右のマージン
        /// </summary>
        private const int Margin = 10;

        /// <summary>
        /// カラーバーの表示領域
        /// </summary>
        private const int ColorBarAreaWidth = 85;

        /// <summary>
        /// 画像の黒色
        /// </summary>
        private Vec3b _black = new() { Item0 = 0, Item1 = 0, Item2 = 0 };

        /// <summary>
        /// 出力画像の横幅
        /// </summary>
        private int _imageWidth = 150;
        /// <summary>
        /// 出力画像の高さ
        /// </summary>
        private int _imageHeight = 250;

        /// <summary>
        /// イメージサンプルデータ
        /// </summary>
        private readonly List<double> _sampleImageData = new();

        /// <summary>
        /// RGB文字列リスト
        /// </summary>
        public ReactivePropertySlim<string> RGBStringList { get; set; }

        /// <summary>
        /// 表示値の最小値
        /// </summary>
        public ReactivePropertySlim<double> MinValue { get; set; }

        /// <summary>
        /// 表示値の最大値
        /// </summary>
        public ReactivePropertySlim<double> MaxValue { get; set; }

        /// <summary>
        /// メモリの個数
        /// </summary>
        public ReactivePropertySlim<int> TicksValue { get; set; }

        /// <summary>
        /// 現在表示されているサンプルデータの範囲
        /// </summary>
        public ReactivePropertySlim<string> SampleData { get; set; }

        /// <summary>
        /// 出力画像
        /// </summary>
        public ReactivePropertySlim<BitmapSource?> Picture { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CreateImageService()
        {
            RGBStringList = new ReactivePropertySlim<string>("#FF0000, #FFFF00, #00FF00, #00FFFF, #0000FF, #FF00FF, #FF0000");
            MinValue = new ReactivePropertySlim<double>(0);
            MaxValue = new ReactivePropertySlim<double>(255);
            TicksValue = new ReactivePropertySlim<int>(9);
            SampleData = new ReactivePropertySlim<string>(string.Empty);
            Picture = new ReactivePropertySlim<BitmapSource?>();

            CreateSampleImageData();
        }

        /// <summary>
        /// サンプル画像作成
        /// </summary>
        public void CreateSampleImageData()
        {
            _sampleImageData.Clear();

            _imageWidth = 150;
            _imageHeight = 250;

            Random random = new();

            for (int i = 0; i < _imageWidth * _imageHeight; i++)
            {
                var calValue = random.NextDouble();

                // ランダムの0.0～1.0 の値を 画面で指定されているデータ範囲に変換
                var tmp = Math.Round(MinValue.Value - ((calValue - 0.0) / (1.0 - 0.0)) * (MinValue.Value - MaxValue.Value), 3);

                _sampleImageData.Add(tmp);
            }

            // サンプルデータの範囲を表示
            SampleData.Value = $"{_sampleImageData.Min()}～{_sampleImageData.Max()}";
        }

        /// <summary>
        /// 輝度値のCSVファイルを読み込みます
        /// </summary>
        /// <param name="filePath">読み込みCSVファイル</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void ReadCSVFile(string filePath)
        {
            _sampleImageData.Clear();
            using StreamReader fs = new(filePath, Encoding.GetEncoding("UTF-8"));

            _imageWidth = 0;
            _imageHeight = 0;
            while (fs.EndOfStream == false)
            {
                var line = fs.ReadLine();
                if (!string.IsNullOrEmpty(line))
                {
                    var data = line.Split(",").Select(double.Parse).ToList();
                    if (data != null)
                    {
                        _sampleImageData.AddRange(data);
                        if (_imageWidth == 0)
                        {
                            _imageWidth = data.Count;
                        }
                        else if (_imageWidth != data.Count)
                        {
                            throw new ArgumentOutOfRangeException("filePath", "CSVファイルの幅データが異なる行が存在します。");
                        }
                    }
                }

                // 行数をカウント
                _imageHeight++;
            }

            // サンプルデータの範囲を表示
            SampleData.Value = $"{_sampleImageData.Min()}～{_sampleImageData.Max()}";
        }

        /// <summary>
        /// イメージの作成
        /// </summary>
        public void DoCreateImage()
        {
            Stopwatch stopwatch = new();
            stopwatch.Start();

            ColorBarPalette colorPalette;
            if (string.IsNullOrEmpty(RGBStringList.Value))
            {
                // グレースケール画像作成
                colorPalette = new()
                {
                    MinValue = MinValue.Value,
                    MaxValue = MaxValue.Value
                };
            }
            else
            {
                // カラー画像作成
                colorPalette = new(RGBStringList.Value.Split(",").ToList())
                {
                    MinValue = MinValue.Value,
                    MaxValue = MaxValue.Value
                };
            }

            Debug.WriteLine($"カラーパレット作成完了：{stopwatch.ElapsedMilliseconds}");

            // 背景白で作成する
            Mat image = new(Margin + _imageHeight + Margin,
                            Margin + _imageWidth + Margin + ColorBarAreaWidth + Margin,
                            MatType.CV_8UC3,
                            new Scalar(255, 255, 255));

            // 画像の作成
            DrawImage(image, colorPalette);

            // カラーバーの表示位置を決定
            var minX = Margin + _imageWidth + Margin;
            var minY = Margin;
            var maxX = minX + ColorBarAreaWidth / 5;
            var maxY = minY + _imageHeight;

            // カラーバーの表示
            DrawFrame(image, colorPalette, minX, minY, maxX, maxY);

            // ラベル表示
            DrawColorBarLabel(image, minX, minY, maxX, maxY, MinValue.Value, MaxValue.Value);

            Debug.WriteLine($"画像作成完了：{stopwatch.ElapsedMilliseconds}");

            // Bitmapにして画面表示
            using var tmp = image.Clone();
            Picture.Value = BitmapSourceConverter.ToBitmapSource(tmp);
        }

        /// <summary>
        /// 画像を作成します
        /// </summary>
        /// <param name="colorImage">出力先クラス</param>
        /// <param name="colorPalette">カラーバーパレット</param>
        private void DrawImage(Mat colorImage, ColorBarPalette colorPalette)
        {
            // CPU次第だが、大きい画像の場合はParallel.Forを使った方が良い(小さい画像だと逆に遅くなる)
            for (int height = 0; height < _imageHeight; height++)
            {
                for (int width = 0; width < _imageWidth; width++)
                {
                    // 出力対象のデータを取得
                    var value = _sampleImageData[width + height * _imageWidth];

                    // 対応する色を取得
                    var color = colorPalette.GetRGBColor(value);
                    // OpenCVSharp4 に変換
                    Vec3b vec3bColor = new()
                    {
                        Item0 = color.B,
                        Item1 = color.G,
                        Item2 = color.R
                    };

                    int xPos = width + Margin;
                    int yPos = height + Margin;

                    // 指定位置の色を変更
                    colorImage.Set(yPos, xPos, vec3bColor);
                }
            }
        }

        /// <summary>
        /// カラーバーの中身を作成します
        /// </summary>
        /// <param name="colorImage">画像オブジェクト</param>
        /// <param name="minX">X座標の最小値</param>
        /// <param name="minY">Y座標の最小値</param>
        /// <param name="maxX">X座標の最大値</param>
        /// <param name="maxY">Y座標の最大値</param>
        private void DrawFrame(Mat colorImage, ColorBarPalette colorPalette, int minX, int minY, int maxX, int maxY)
        {
            // カラーバー用に最大値・最小値を変更
            colorPalette.MinValue = minY;
            colorPalette.MaxValue = maxY;

            for (int y = minY; y < maxY + 1; y++)
            {
                var rgbColor = colorPalette.GetRGBColor(maxY - y);
                // プロパティを設定しない場合は、こちらでも良い
                // var rgbColor = colorPalette.GetRGBColor(y, minY, maxY); 

                Vec3b vec3BColor = new()
                {
                    Item0 = rgbColor.B,
                    Item1 = rgbColor.G,
                    Item2 = rgbColor.R
                };

                for (int x = minX; x < maxX + 1; x++)
                {
                    Vec3b setColor = vec3BColor;

                    if (x == minX || x == maxX ||
                        y == minY || y == maxY)
                    {
                        // 枠は黒で表示
                        setColor = _black;
                    }

                    colorImage.Set(y, x, setColor);
                }
            }
        }

        /// <summary>
        /// カラーバーのラベルを表示します
        /// </summary>
        /// <param name="colorImage">画像オブジェクト</param>
        /// <param name="minX">X座標の最小値</param>
        /// <param name="minY">Y座標の最小値</param>
        /// <param name="maxX">X座標の最大値</param>
        /// <param name="maxY">Y座標の最大値</param>
        /// <param name="minValue">最小値</param>
        /// <param name="maxValue">最大値</param>
        private void DrawColorBarLabel(Mat colorImage, int minX, int minY, int maxX, int maxY, double minValue, double maxValue)
        {
            var labelHeight = (maxY - minY) / (TicksValue.Value - 1);
            var labelValue = Math.Abs((maxValue - minValue) / (TicksValue.Value - 1));

            for (int i = 0; i < TicksValue.Value; i++)
            {
                Cv2.PutText(colorImage,
                            Math.Round(minValue + (labelValue * i), 1).ToString(),
                            new Point(maxX + 5, maxY - (labelHeight * i) + 5),
                            HersheyFonts.HersheyDuplex,
                            0.6,
                            Scalar.Black,
                            1,
                            LineTypes.Link8,
                            false);
            }
        }
    }
}
