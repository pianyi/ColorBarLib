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

            Assert.Equal(Color.FromArgb(0, 0, 0), lib.GetRGBColor(lib.MinValue));
            Assert.Equal(Color.FromArgb(26, 26, 26), lib.GetRGBColor(10));
            Assert.Equal(Color.FromArgb(255, 255, 255), lib.GetRGBColor(lib.MaxValue));
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

            Assert.Equal(Color.FromArgb(0, 0, 0), lib.GetRGBColor(-100));
            Assert.Equal(Color.FromArgb(0, 0, 0), lib.GetRGBColor(-1));
            Assert.Equal(Color.FromArgb(0, 0, 0), lib.GetRGBColor(0));
            Assert.Equal(Color.FromArgb(10, 10, 10), lib.GetRGBColor(10));
            Assert.Equal(Color.FromArgb(50, 50, 50), lib.GetRGBColor(50));
            Assert.Equal(Color.FromArgb(100, 100, 100), lib.GetRGBColor(100));
            Assert.Equal(Color.FromArgb(150, 150, 150), lib.GetRGBColor(150));
            Assert.Equal(Color.FromArgb(200, 200, 200), lib.GetRGBColor(200));
            Assert.Equal(Color.FromArgb(250, 250, 250), lib.GetRGBColor(250));
            Assert.Equal(Color.FromArgb(255, 255, 255), lib.GetRGBColor(255));
            Assert.Equal(Color.FromArgb(255, 255, 255), lib.GetRGBColor(256));
            Assert.Equal(Color.FromArgb(255, 255, 255), lib.GetRGBColor(300));
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

        [Fact]
        public void GetRGBColor4()
        {
            List<string> args = new()
            {
                "#FF0000",
                "#0000FF",
            };
            ColorBarPalette lib = new(args);

            Assert.NotNull(lib);

            Assert.Equal(Color.FromArgb(255, 0, 0), lib.GetRGBColor(lib.MinValue));
            Assert.Equal(Color.FromArgb(0, 255, 0), lib.GetRGBColor(lib.MaxValue / 2));
            Assert.Equal(Color.FromArgb(0, 0, 255), lib.GetRGBColor(lib.MaxValue));
        }
    }
}
