using System;
using System.Collections.Generic;
using System.Drawing;

namespace ColorBarLib
{
    /// <summary>
    /// カラーバーパレットの作成・取得を行う
    /// </summary>
    public class ColorBarPalette
    {
        /// <summary>
        /// カラー表示時のカラーパターン
        /// </summary>
        private readonly List<Color> _colorPattern = new List<Color>();

        /// <summary>
        /// カラーバーパレットで利用する最小値を設定します
        /// </summary>
        public double MinValue { get; set; } = 0.0;

        /// <summary>
        /// カラーバーパレットで利用する最大値を設定します
        /// </summary>
        public double MaxValue { get; set; } = 100.0;

        /// <summary>
        /// ガンマ補正値（色未指定時に利用）
        /// </summary>
        public double Gamma { get; set; } = 1.5;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ColorBarPalette()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="rgbList">#RRGGBB で指定された色のリスト(配列0の場合はグレースケールとなります)</param>
        public ColorBarPalette(List<string> rgbList)
            : this()
        {
            // カラーパレットの作成
            CreateColorBarData(rgbList);
        }

        /// <summary>
        /// カラーパレットの作成
        /// </summary>
        /// <param name="colorRGBStringList">カラーパレットデータ</param>
        private void CreateColorBarData(List<string> colorRGBStringList)
        {
            if (colorRGBStringList == null || colorRGBStringList.Count <= 0)
            {
                // カラー指定がない場合は処理しない
                return;
            }

            // 文字列のRGBをHslリストに変換する
            List<HslColor> colorHslList = HslColor.GetHslList(colorRGBStringList);
            if (colorHslList == null || colorHslList.Count <= 0)
            {
                // 変換後のデータが無い場合は処理しない
                return;
            }

            int area = (int)Math.Ceiling(360.0 / (colorHslList.Count - 1));

            int areaIndex = 0;

            float stepHue = 0;
            float stepSaturation = 0;
            float stepLightness = 0;

            // 初期値
            float hue = colorHslList[0].H;
            float saturation = colorHslList[0].S;
            float lightness = colorHslList[0].L;

            // HSL 方式に合わせて0～359回ループする。
            for (int i = 0; i < 361; i++)
            {
                if (area * areaIndex < i)
                {
                    if (areaIndex + 1 < colorHslList.Count)
                    {
                        var nowHsl = colorHslList[areaIndex];
                        var nextHsl = colorHslList[areaIndex + 1];

                        stepHue = Math.Abs(nextHsl.H - nowHsl.H) / area;
                        stepSaturation = Math.Abs(nextHsl.S - nowHsl.S) / area;
                        stepLightness = Math.Abs(nextHsl.L - nowHsl.L) / area;

                        if (270 < nowHsl.H && nextHsl.H < 90)
                        {
                            // 色相は0～359の円情報のため、0＝360 となる。
                            // 今が「270～359」で、次が「0～90」 の場合、境界を越えたと判断し、再計算する
                            stepHue = (360 + nextHsl.H - nowHsl.H) / area;
                        }
                        else if (nowHsl.H < 90 && 270 < nextHsl.H)
                        {
                            // 今が「90～0」で、次が「359～270」 の場合、境界を越えたと判断し、再計算する
                            stepHue = (nextHsl.H - (nowHsl.H + 360)) / area;
                        }
                        else
                        {
                            // 増減判定
                            if (nextHsl.H < nowHsl.H)
                            {
                                stepHue *= -1;
                            }
                            if (nextHsl.S < nowHsl.S)
                            {
                                stepSaturation *= -1;
                            }
                            if (nextHsl.L < nowHsl.L)
                            {
                                stepLightness *= -1;
                            }
                        }

                        if (nowHsl.L == 1)
                        {
                            // 現在の輝度が最大の場合、目標値の色相は固定にし、輝度だけを変化させていく事でキレイに表示する
                            hue = nextHsl.H;
                            stepHue = 0;
                        }
                        else if (nextHsl.L == 1)
                        {
                            // 次の輝度が最大の場合、輝度のみ変化させる
                            stepHue = 0;
                            stepSaturation = 0;
                        }
                    }

                    areaIndex++;
                }

                if (i != 0)
                {
                    if (!float.IsInfinity(stepHue))
                    {
                        hue += stepHue;
                    }
                    if (!float.IsInfinity(stepSaturation))
                    {
                        saturation += stepSaturation;
                    }
                    if (!float.IsInfinity(stepLightness))
                    {
                        lightness += stepLightness;
                    }
                }

                // 最大・最小値の設定
                if (359 < hue)
                {
                    hue -= 359;
                }
                if (hue < 0)
                {
                    hue += 359;
                }

                if (saturation < 0)
                {
                    saturation = 0;
                }
                if (1 < saturation)
                {
                    saturation = 1;
                }

                if (lightness < 0)
                {
                    lightness = 0;
                }
                if (1 < lightness)
                {
                    lightness = 1;
                }

                var nextColor = HslColor.ToRgb(hue, saturation, lightness);

                _colorPattern.Add(nextColor);
            }
        }

        /// <summary>
        /// プロパティに設定されている最大値・最小値の範囲を元に、引数に対応したColorを取得します
        /// </summary>
        /// <param name="value">検索する値</param>
        /// <returns>該当する色</returns>
        public Color GetRGBColor(double value)
        {
            return GetRGBColor(value, MinValue, MaxValue);
        }

        /// <summary>
        /// 引数に対応したColorを取得します
        /// </summary>
        /// <param name="value">検索する値</param>
        /// <param name="min">検索値の最小値</param>
        /// <param name="max">検索値の最大値</param>
        /// <returns>該当する色</returns>
        public Color GetRGBColor(double value, double min, double max)
        {
            Color result;

            if (1 < _colorPattern.Count)
            {
                result = GetRGBScaleData(value, min, max);
            }
            else
            {
                result = GetGrayScaleData(value, min, max);
            }

            return result;
        }

        /// <summary>
        /// 値からカラー色を取得します
        /// </summary>
        /// <param name="value">検索する値</param>
        /// <param name="min">検索値の最小値</param>
        /// <param name="max">検索値の最大値</param>
        /// <returns>判定値に該当する色（カラー）</returns>
        private Color GetRGBScaleData(double value, double min, double max)
        {
            var calValue = value;
            if (calValue < min)
            {
                calValue = min;
            }
            if (max < calValue)
            {
                calValue = max;
            }

            // 値を 指定範囲内に変換する(最後は配列の個数なので-1 した値にしないと範囲外になる)
            var index = (int)Math.Round(0.0 - ((calValue - min) / (max - min)) * (0.0 - (_colorPattern.Count - 1)), MidpointRounding.AwayFromZero);

            return _colorPattern[index];
        }

        /// <summary>
        /// 値からグレースケール色を取得します
        /// </summary>
        /// <param name="value">検索する値</param>
        /// <param name="min">検索値の最小値</param>
        /// <param name="max">検索値の最大値</param>
        /// <returns>判定値に該当する色（グレースケール）</returns>
        private Color GetGrayScaleData(double value, double min, double max)
        {
            var calValue = value;
            if (calValue < min)
            {
                calValue = min;
            }
            if (max < calValue)
            {
                calValue = max;
            }

            // 値を 0～1に変換する
            calValue = 0.0 - ((calValue - min) / (max - min)) * (0.0 - 1.0);

            double brightness;
            if (Gamma == 1.0)
            {
                // ガンマ補正無しの計算
                brightness = (int)Math.Round(calValue * 255.0, MidpointRounding.AwayFromZero);
            }
            else
            {
                // ガンマ補正有りの計算(Gnuplotでは1.5で補正しているVer5.4)
                brightness = (int)Math.Round(255.0 * Math.Pow(calValue, 1.0 / Gamma), MidpointRounding.AwayFromZero);
            }

            return Color.FromArgb(Convert.ToByte(brightness), Convert.ToByte(brightness), Convert.ToByte(brightness));
        }
    }
}
