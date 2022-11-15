using ColorBarLib;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using Reactive.Bindings;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Media.Imaging;

namespace Example.Services
{
    /// <summary>
    /// カラーバーライブラリを使って、画像を作成する
    /// </summary>
    public class CreateImageService
    {
        /// <summary>
        /// 出力画像の横幅
        /// </summary>
        private const int ImageWidth = 50;
        /// <summary>
        /// 出力画像の高さ
        /// </summary>
        private const int ImageHeight = 250;

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
        private Vec3b Black = new() { Item0 = 0, Item1 = 0, Item2 = 0 };

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
            MaxValue = new ReactivePropertySlim<double>(100);
            TicksValue = new ReactivePropertySlim<int>(9);
            Picture = new ReactivePropertySlim<BitmapSource?>();
        }

        /// <summary>
        /// イメージの作成
        /// </summary>
        public void DoCreateImage()
        {
            ColorPalette colorPalette;
            if (string.IsNullOrEmpty(RGBStringList.Value))
            {
                // グレースケール画像作成
                colorPalette = new();
            }
            else
            {
                // カラー画像作成
                colorPalette = new(RGBStringList.Value.Split(",").ToList());
            }

            // 背景白で作成する
            Mat image = new(Margin + ImageHeight + Margin,
                            Margin + ImageWidth + Margin + ColorBarAreaWidth + Margin,
                            MatType.CV_8UC3,
                            new Scalar(255, 255, 255));

            // カラーバーの表示位置を決定
            var minX = Margin + ImageWidth + Margin;
            var minY = Margin;
            var maxX = minX + ImageWidth / 5;
            var maxY = minY + ImageHeight;

            // 枠の表示
            DrawFrame(image, colorPalette, minX, minY, maxX, maxY);

            // ラベル表示
            DrawColorBarLabel(image, minX, minY, maxX, maxY, MinValue.Value, MaxValue.Value);

            // Bitmapにして画面表示
            using var tmp = image.Clone();
            Cv2.Flip(image, tmp, FlipMode.X);
            Picture.Value = BitmapSourceConverter.ToBitmapSource(tmp);
        }

        /// <summary>
        /// カラーバーの中身を作成します
        /// </summary>
        /// <param name="colorImage">画像オブジェクト</param>
        /// <param name="minX">X座標の最小値</param>
        /// <param name="minY">Y座標の最小値</param>
        /// <param name="maxX">X座標の最大値</param>
        /// <param name="maxY">Y座標の最大値</param>
        private void DrawFrame(Mat colorImage, ColorPalette colorPalette, int minX, int minY, int maxX, int maxY)
        {
            for (int y = minY; y < maxY + 1; y++)
            {
                Color color = colorPalette.GetRGBColor(y, minY, maxY);

                for (int x = minX; x < maxX + 1; x++)
                {
                    Vec3b setColor = new() { Item0 = color.B, Item1 = color.G, Item2 = color.R };
                    if (x == minX || x == maxX ||
                        y == minY || y == maxY)
                    {
                        // 枠は黒で表示
                        setColor = Black;
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
                            Math.Round(maxValue - (labelValue * i), 1).ToString(),
                            new OpenCvSharp.Point(maxX + 5, maxY - (labelHeight * i) - 5),
                            HersheyFonts.HersheyDuplex,
                            0.6,
                            Scalar.Black,
                            1,
                            LineTypes.Link8,
                            true);
            }
        }
    }
}
