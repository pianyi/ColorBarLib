using ColorBarLib;

namespace UnitTestColorBarLib
{
    public class HslColorTest
    {
        [Fact]
        public void GetHslListArgsNull()
        {
            var colorList = HslColor.GetHslList(null);

            Assert.NotNull(colorList);
            Assert.Empty(colorList);
        }

        [Fact]
        public void GetHslListArgsEmpty()
        {
            var colorList = HslColor.GetHslList(new List<string>());

            Assert.NotNull(colorList);
            Assert.Empty(colorList);
        }

        [Fact]
        public void GetHslListArgsNotColor1()
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

            var colorList = HslColor.GetHslList(args);

            Assert.NotNull(colorList);
            Assert.Empty(colorList);
        }

        [Fact]
        public void GetHslListArgsNotColor2()
        {
            List<string> args = new()
            {
                "#00112233", // ARGB
                "#GGGGGG",   // 0～F以外
                "#QQQ",      // 3桁カラー
                "00112",     // #無し桁不足
                "0011223",   // #無し桁オーバー
                "00112233"   // #無しARGB
            };

            var colorList = HslColor.GetHslList(args);

            Assert.NotNull(colorList);
            Assert.Empty(colorList);
        }

        [Fact]
        public void GetHslListNormal1Color()
        {
            List<string> args = new List<string>()
            {
                "#000000",
            };
            var colorList = HslColor.GetHslList(args);

            Assert.NotNull(colorList);
            Assert.NotEmpty(colorList);
            Assert.Single(colorList);
        }

        [Fact]
        public void GetHslListNormal2Colors()
        {
            List<string> args = new List<string>()
            {
                "#000000",
                "FFFFFF",
            };
            var colorList = HslColor.GetHslList(args);

            Assert.NotNull(colorList);
            Assert.NotEmpty(colorList);
            Assert.Equal(2, colorList.Count);
        }

        [Fact]
        public void GetHslListNormalAnyColors()
        {
            List<string> args = new List<string>()
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
            var colorList = HslColor.GetHslList(args);

            Assert.NotNull(colorList);
            Assert.NotEmpty(colorList);
            Assert.Equal(args.Count, colorList.Count);
        }

        [Fact]
        public void FromRgb1()
        {
            HslColor color = HslColor.FromRgb(0, 0, 0);

            Assert.NotNull(color);
            Assert.Equal(0, color.H);
            Assert.Equal(0, color.S);
            Assert.Equal(0, color.L);
        }

        [Fact]
        public void FromRgb2()
        {
            HslColor color = HslColor.FromRgb(Convert.ToInt32("AB", 16), Convert.ToInt32("CD", 16), Convert.ToInt32("EF", 16));

            Assert.NotNull(color);
            Assert.Equal(210, Math.Round(color.H, 0));
            Assert.Equal(68, Math.Round(color.S * 100, 0)); // 0～1 なので%変換後に正数変換し、大体の値でチェックする
            Assert.Equal(80, Math.Round(color.L * 100, 0));
        }

        [Fact]
        public void FromRgb3()
        {
            HslColor color = HslColor.FromRgb(Convert.ToInt32("98", 16), Convert.ToInt32("76", 16), Convert.ToInt32("54", 16));

            Assert.NotNull(color);
            Assert.Equal(30, Math.Round(color.H, 0));
            Assert.Equal(29, Math.Round(color.S * 100, 0)); // 0～1 なので%変換後に正数変換し、大体の値でチェックする
            Assert.Equal(46, Math.Round(color.L * 100, 0));
        }
    }
}
