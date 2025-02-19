using System;
using System.Collections.Generic;
using System.Drawing;

namespace ColorBarLib
{
    /// <summary>
    /// �J���[�o�[�p���b�g�̍쐬�E�擾���s��
    /// </summary>
    public class ColorBarPalette
    {
        /// <summary>
        /// �J���[�\�����̃J���[�p�^�[��
        /// </summary>
        private readonly List<Color> _colorPattern = new List<Color>();

        /// <summary>
        /// �J���[�o�[�p���b�g�ŗ��p����ŏ��l��ݒ肵�܂�
        /// </summary>
        public double MinValue { get; set; } = 0.0;

        /// <summary>
        /// �J���[�o�[�p���b�g�ŗ��p����ő�l��ݒ肵�܂�
        /// </summary>
        public double MaxValue { get; set; } = 100.0;

        /// <summary>
        /// �K���}�␳�l�i�F���w�莞�ɗ��p�j
        /// </summary>
        public double Gamma { get; set; } = 1.5;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public ColorBarPalette()
        {
        }

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        /// <param name="rgbList">#RRGGBB �Ŏw�肳�ꂽ�F�̃��X�g(�z��0�̏ꍇ�̓O���[�X�P�[���ƂȂ�܂�)</param>
        public ColorBarPalette(List<string> rgbList)
            : this()
        {
            // �J���[�p���b�g�̍쐬
            CreateColorBarData(rgbList);
        }

        /// <summary>
        /// �J���[�p���b�g�̍쐬
        /// </summary>
        /// <param name="colorRGBStringList">�J���[�p���b�g�f�[�^</param>
        private void CreateColorBarData(List<string> colorRGBStringList)
        {
            if (colorRGBStringList == null || colorRGBStringList.Count <= 0)
            {
                // �J���[�w�肪�Ȃ��ꍇ�͏������Ȃ�
                return;
            }

            // �������RGB��Hsl���X�g�ɕϊ�����
            List<HslColor> colorHslList = HslColor.GetHslList(colorRGBStringList);
            if (colorHslList == null || colorHslList.Count <= 0)
            {
                // �ϊ���̃f�[�^�������ꍇ�͏������Ȃ�
                return;
            }

            int area = (int)Math.Ceiling(360.0 / (colorHslList.Count - 1));

            int areaIndex = 0;

            float stepHue = 0;
            float stepSaturation = 0;
            float stepLightness = 0;

            // �����l
            float hue = colorHslList[0].H;
            float saturation = colorHslList[0].S;
            float lightness = colorHslList[0].L;

            // HSL �����ɍ��킹��0�`359�񃋁[�v����B
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
                            // �F����0�`359�̉~���̂��߁A0��360 �ƂȂ�B
                            // �����u270�`359�v�ŁA�����u0�`90�v �̏ꍇ�A���E���z�����Ɣ��f���A�Čv�Z����
                            stepHue = (360 + nextHsl.H - nowHsl.H) / area;
                        }
                        else if (nowHsl.H < 90 && 270 < nextHsl.H)
                        {
                            // �����u90�`0�v�ŁA�����u359�`270�v �̏ꍇ�A���E���z�����Ɣ��f���A�Čv�Z����
                            stepHue = (nextHsl.H - (nowHsl.H + 360)) / area;
                        }
                        else
                        {
                            // ��������
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
                            // ���݂̋P�x���ő�̏ꍇ�A�ڕW�l�̐F���͌Œ�ɂ��A�P�x������ω������Ă������ŃL���C�ɕ\������
                            hue = nextHsl.H;
                            stepHue = 0;
                        }
                        else if (nextHsl.L == 1)
                        {
                            // ���̋P�x���ő�̏ꍇ�A�P�x�̂ݕω�������
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

                // �ő�E�ŏ��l�̐ݒ�
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
        /// �v���p�e�B�ɐݒ肳��Ă���ő�l�E�ŏ��l�͈̔͂����ɁA�����ɑΉ�����Color���擾���܂�
        /// </summary>
        /// <param name="value">��������l</param>
        /// <returns>�Y������F</returns>
        public Color GetRGBColor(double value)
        {
            return GetRGBColor(value, MinValue, MaxValue);
        }

        /// <summary>
        /// �����ɑΉ�����Color���擾���܂�
        /// </summary>
        /// <param name="value">��������l</param>
        /// <param name="min">�����l�̍ŏ��l</param>
        /// <param name="max">�����l�̍ő�l</param>
        /// <returns>�Y������F</returns>
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
        /// �l����J���[�F���擾���܂�
        /// </summary>
        /// <param name="value">��������l</param>
        /// <param name="min">�����l�̍ŏ��l</param>
        /// <param name="max">�����l�̍ő�l</param>
        /// <returns>����l�ɊY������F�i�J���[�j</returns>
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

            // �l�� �w��͈͓��ɕϊ�����(�Ō�͔z��̌��Ȃ̂�-1 �����l�ɂ��Ȃ��Ɣ͈͊O�ɂȂ�)
            var index = (int)Math.Round(0.0 - ((calValue - min) / (max - min)) * (0.0 - (_colorPattern.Count - 1)), MidpointRounding.AwayFromZero);

            return _colorPattern[index];
        }

        /// <summary>
        /// �l����O���[�X�P�[���F���擾���܂�
        /// </summary>
        /// <param name="value">��������l</param>
        /// <param name="min">�����l�̍ŏ��l</param>
        /// <param name="max">�����l�̍ő�l</param>
        /// <returns>����l�ɊY������F�i�O���[�X�P�[���j</returns>
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

            // �l�� 0�`1�ɕϊ�����
            calValue = 0.0 - ((calValue - min) / (max - min)) * (0.0 - 1.0);

            double brightness;
            if (Gamma == 1.0)
            {
                // �K���}�␳�����̌v�Z
                brightness = (int)Math.Round(calValue * 255.0, MidpointRounding.AwayFromZero);
            }
            else
            {
                // �K���}�␳�L��̌v�Z(Gnuplot�ł�1.5�ŕ␳���Ă���Ver5.4)
                brightness = (int)Math.Round(255.0 * Math.Pow(calValue, 1.0 / Gamma), MidpointRounding.AwayFromZero);
            }

            return Color.FromArgb(Convert.ToByte(brightness), Convert.ToByte(brightness), Convert.ToByte(brightness));
        }
    }
}
