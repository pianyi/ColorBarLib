using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace ColorBarLib
{
    /// <summary>
    /// RGBカラーとHSLカラーを制御します
    /// </summary>
    public class HslColor : IEquatable<HslColor?>
    {
        /// <summary>
        /// 色相 (Hue)
        /// </summary>
        public float H { get; private set; }

        /// <summary>
        /// 彩度 (Saturation)
        /// </summary>
        public float S { get; private set; }

        /// <summary>
        /// 輝度 (Lightness)
        /// </summary>
        public float L { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="hue">色相</param>
        /// <param name="saturation">彩度</param>
        /// <param name="lightness">輝度</param>
        /// <exception cref="ArgumentException">引数値が範囲外</exception>
        private HslColor(float hue, float saturation, float lightness)
        {
            if (hue < 0f || 360f <= hue)
            {
                throw new ArgumentException("hueは0以上360未満の値です。", nameof(hue));
            }

            if (saturation < 0f || 1f < saturation)
            {
                throw new ArgumentException("saturationは0以上1以下の値です。", nameof(saturation));
            }

            if (lightness < 0f || 1f < lightness)
            {
                throw new ArgumentException("lightnessは0以上1以下の値です。", nameof(lightness));
            }

            H = hue;
            S = saturation;
            L = lightness;
        }

        /// <summary>
        /// #RGBの文字列配列からHSLColorの配列に変換します
        /// </summary>
        /// <param name="rgbList">#XXXXXX 形式のRGB色文字列配列</param>
        /// <returns>HSLColorクラスのリスト</returns>
        public static List<HslColor> GetHslList(List<string> rgbList)
        {
            List<HslColor> colorHslList = new List<HslColor>();

            if (rgbList == null)
            {
                return colorHslList;
            }

            foreach (var color in rgbList)
            {
                try
                {
                    var trimmedColor = color.Trim();
                    if (string.IsNullOrWhiteSpace(trimmedColor))
                    {
                        // RGBが空白の場合は処理しない
                        continue;
                    }
                    if ((trimmedColor.StartsWith("#") && 7 != trimmedColor.Length) ||
                        (!trimmedColor.StartsWith("#") && 6 != trimmedColor.Length))
                    {
                        //  RGB(#RGB) の桁数ではない場合処理しない
                        continue;
                    }

                    var startIndex = 0;
                    if (trimmedColor.StartsWith("#"))
                    {
                        startIndex = 1;
                    }

                    var red16 = trimmedColor.Substring(startIndex, 2);
                    var green16 = trimmedColor.Substring(startIndex + 2, 2);
                    var blue16 = trimmedColor.Substring(startIndex + 4, 2);

                    colorHslList.Add(FromRgb(Convert.ToInt32(red16, 16), Convert.ToInt32(green16, 16), Convert.ToInt32(blue16, 16)));
                }
                catch (FormatException ex)
                {
                    // 16心数変換失敗、または色変換に失敗した時に来る
                    Debug.Write(ex.Message);
                    Debug.Write(ex.StackTrace);
                }
            }

            return colorHslList;
        }

        /// <summary>
        /// 指定したRGBColorからHSLColorを作成する
        /// </summary>
        /// <param name="red">赤</param>
        /// <param name="green">緑</param>
        /// <param name="blue">青</param>
        /// <returns>HslColor</returns>
        public static HslColor FromRgb(int red, int green, int blue)
        {
            return FromRgb(Color.FromArgb(red, green, blue));
        }

        /// <summary>
        /// 指定したRGBColorからHSLColorを作成する
        /// </summary>
        /// <param name="rgb">RGBColor</param>
        /// <returns>HslColor</returns>
        public static HslColor FromRgb(Color rgb)
        {
            float r = rgb.R / 255f;
            float g = rgb.G / 255f;
            float b = rgb.B / 255f;

            float max = Math.Max(r, Math.Max(g, b));
            float min = Math.Min(r, Math.Min(g, b));

            float lightness = (max + min) / 2f;
            float hue = 0;
            float saturation = 0;

            if (max != min)
            {
                float c = max - min;

                if (max == r)
                {
                    hue = (g - b) / c;
                }
                else if (max == g)
                {
                    hue = (b - r) / c + 2f;
                }
                else
                {
                    hue = (r - g) / c + 4f;
                }
                hue *= 60f;
                if (hue < 0f)
                {
                    hue += 360f;
                }

                if (lightness < 0.5f)
                {
                    saturation = c / (max + min);
                }
                else
                {
                    saturation = c / (2f - max - min);
                }
            }

            return new HslColor(hue, saturation, lightness);
        }

        /// <summary>
        /// 指定したHSLColorからRGBColorを作成する
        /// </summary>
        /// <param name="hue">色相</param>
        /// <param name="saturation">彩度</param>
        /// <param name="lightness">輝度</param>
        /// <returns>RGBColor</returns>
        public static Color ToRgb(float hue, float saturation, float lightness)
        {
            return ToRgb(new HslColor(hue, saturation, lightness));
        }

        /// <summary>
        /// 指定したHSLColorからRGBColorを作成する
        /// </summary>
        /// <param name="hsl">HSLColor</param>
        /// <returns>RGBColor</returns>
        public static Color ToRgb(HslColor hsl)
        {
            float s = hsl.S;
            float l = hsl.L;

            float r1 = l;
            float g1 = l;
            float b1 = l;

            if (s != 0)
            {
                float h = hsl.H / 60f;
                int i = (int)Math.Floor(h);
                float f = h - i;
                float c;

                if (l < 0.5f)
                {
                    c = 2f * s * l;
                }
                else
                {
                    c = 2f * s * (1f - l);
                }

                float m = l - c / 2f;
                float p = c + m;
                float q;
                if (i % 2 == 0)
                {
                    q = l + c * (f - 0.5f);
                }
                else
                {
                    q = l - c * (f - 0.5f);
                }

                switch (i)
                {
                    case 0:
                        r1 = p;
                        g1 = q;
                        b1 = m;
                        break;

                    case 1:
                        r1 = q;
                        g1 = p;
                        b1 = m;
                        break;

                    case 2:
                        r1 = m;
                        g1 = p;
                        b1 = q;
                        break;

                    case 3:
                        r1 = m;
                        g1 = q;
                        b1 = p;
                        break;

                    case 4:
                        r1 = q;
                        g1 = m;
                        b1 = p;
                        break;

                    case 5:
                        r1 = p;
                        g1 = m;
                        b1 = q;
                        break;

                    default:
                        throw new ArgumentException("色相の値が不正です。", nameof(hsl));
                }
            }

            return Color.FromArgb(GetIntValue(r1 * 255f),
                                  GetIntValue(g1 * 255f),
                                  GetIntValue(b1 * 255f));
        }

        /// <summary>
        /// 小数点以下を全て四捨五入し正数を取得します。
        /// </summary>
        /// <param name="value">正数への変換元データ</param>
        /// <returns>四捨五入した正数値</returns>
        public static int GetIntValue(double value)
        {
            return GetIntValue(Convert.ToDecimal(value));
        }

        /// <summary>
        /// 小数点以下を全て四捨五入し正数を取得します。
        /// </summary>
        /// <param name="value">正数への変換元データ</param>
        /// <returns>四捨五入した正数値</returns>
        public static int GetIntValue(decimal value)
        {
            int length = GetDecimalLength(value);

            decimal result = value;
            for (int i = length; 0 <= i; i--)
            {
                result = Math.Round(result, MidpointRounding.AwayFromZero);
            }

            return Convert.ToInt32(result);
        }

        /// <summary>
        /// 小数点以下の桁数を取得するFunction
        /// </summary>
        /// <param name="value">小数値</param>
        /// <returns>桁数</returns>
        public static int GetDecimalLength(decimal value)
        {
            string valStr = value.ToString()!.TrimEnd('0');
            int idx = valStr.IndexOf('.');

            int result = 0;
            if (idx != -1)
            {
                result = valStr.Substring(idx + 1).Length;
            }

            return result;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return Equals(obj as HslColor);
        }

        /// <inheritdoc/>
        public bool Equals(HslColor? other)
        {
            return other is not null &&
                   H == other.H &&
                   S == other.S &&
                   L == other.L;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(H, S, L);
        }

        /// <inheritdoc/>
        public static bool operator ==(HslColor? left, HslColor? right)
        {
            return EqualityComparer<HslColor>.Default.Equals(left, right);
        }

        /// <inheritdoc/>
        public static bool operator !=(HslColor? left, HslColor? right)
        {
            return !(left == right);
        }
    }
}
