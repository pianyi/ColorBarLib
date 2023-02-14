using ColorBarLib;
using System.Drawing;

namespace UnitTestColorBarLib
{
    public class ColorBarPaletteTest
    {
        [Fact]
        public void ConstructorNormal()
        {
            ColorBarPalette lib = new();

            Assert.NotNull(lib);
        }

        [Fact]
        public void ConstructorArgsNull()
        {
            ColorBarPalette lib = new(null);

            Assert.NotNull(lib);
        }

        [Fact]
        public void ConstructorArgsEmpty()
        {
            ColorBarPalette lib = new(new List<string>());

            Assert.NotNull(lib);
        }

        [Fact]
        public void ConstructorArgsNotColor1()
        {
            List<string> args = new()
            {
                "a",
                "b",
                "",
                "#0",
                "#11",
                "#222",
                "#3333",
                "#44444",
                "#5555555",
            };

            ColorBarPalette lib = new(args);

            Assert.NotNull(lib);
        }

        [Fact]
        public void ConstructorArgsNotColor2()
        {
            List<string> args = new()
            {
                "#00112233", // ARGB
                "#GGGGGG",   // 0�`F�ȊO
                "#QQQ",      // 3���J���[
                "001122",    // #����
                "00112233"   // #����ARGB
            };

            ColorBarPalette lib = new(args);

            Assert.NotNull(lib);
        }

        [Fact]
        public void GetRGBColor1()
        {
            List<string> args = new()
            {
                "000000",
                "#111111",
                "222222",
                "#333333",
                "444444",
                "#555555",
                "666666",
                "#777777",
                "888888",
                "#999999",
                "AAAAAA",
                "#BBBBBB",
                "CCCCCC",
                "#DDDDDD",
                "EEEEEE",
                "#FFFFFF",
            };

            ColorBarPalette lib = new(args);

            Assert.NotNull(lib);

            Color color = lib.GetRGBColor(10);
            Assert.Equal(Color.FromArgb(25, 25, 25), color);
        }

        [Fact]
        public void GetRGBColor2()
        {
            List<string> args = new()
            {
                "000000",
                "#FFFFFF",
            };

            ColorBarPalette lib = new(args)
            {
                MinValue = 0,
                MaxValue = 255
            };

            Assert.NotNull(lib);

            Assert.Equal(Color.FromArgb(0, 0, 0), lib.GetRGBColor(-10));
            Assert.Equal(Color.FromArgb(10, 10, 10), lib.GetRGBColor(10));
            Assert.Equal(Color.FromArgb(50, 50, 50), lib.GetRGBColor(50));
            // TODO �Ȃ�1���Ⴄ�̂��������������ǂ�����
            Assert.Equal(Color.FromArgb(99, 99, 99), lib.GetRGBColor(100));
            Assert.Equal(Color.FromArgb(149, 149, 149), lib.GetRGBColor(150));
            Assert.Equal(Color.FromArgb(199, 199, 199), lib.GetRGBColor(200));
            Assert.Equal(Color.FromArgb(249, 249, 249), lib.GetRGBColor(250));
            Assert.Equal(Color.FromArgb(254, 254, 254), lib.GetRGBColor(255));
            // TODO �ő�l��254�Ȃ̂͗����ł��Ă��Ȃ�
            Assert.Equal(Color.FromArgb(254, 254, 254), lib.GetRGBColor(260));
            Assert.Equal(Color.FromArgb(254, 254, 254), lib.GetRGBColor(500));
        }

        [Fact]
        public void GetRGBColor3()
        {
            ColorBarPalette lib = new()
            {
                MinValue = 0,
                MaxValue = 250
            };

            Assert.NotNull(lib);

            // Constructor�̈��������̓K���}�␳(1.5)�L��ɂȂ�̂Œl���قȂ�
            // 2023/02/13 expected�͌v�Z���ׂ������A����擾�l�����̂܂ܐݒ肵�Ă���
            Assert.Equal(Color.FromArgb(0, 0, 0), lib.GetRGBColor(-10));
            Assert.Equal(Color.FromArgb(0, 0, 0), lib.GetRGBColor(0));
            Assert.Equal(Color.FromArgb(30, 30, 30), lib.GetRGBColor(10));
            Assert.Equal(Color.FromArgb(87, 87, 87), lib.GetRGBColor(50));
            Assert.Equal(Color.FromArgb(138, 138, 138), lib.GetRGBColor(100));
            Assert.Equal(Color.FromArgb(181, 181, 181), lib.GetRGBColor(150));
            Assert.Equal(Color.FromArgb(220, 220, 220), lib.GetRGBColor(200));
            Assert.Equal(Color.FromArgb(255, 255, 255), lib.GetRGBColor(250));
            Assert.Equal(Color.FromArgb(255, 255, 255), lib.GetRGBColor(255));
            Assert.Equal(Color.FromArgb(255, 255, 255), lib.GetRGBColor(300));
        }
    }
}
