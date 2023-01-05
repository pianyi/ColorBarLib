namespace PrismExpansion.Services.Dialogs
{
    /// <summary>
    /// ダイアログ画面へのパラメータ
    /// </summary>
    public static class DialogParams
    {
        /// <summary>
        /// ダイアログを指定するキー
        /// </summary>
        public const string Key = "Key";
        /// <summary>
        /// タイトルメッセージ
        /// </summary>
        public const string Title = "Title";
        /// <summary>
        /// <para>初期ウィンドウの左の位置を指定します</para>
        /// <para>prism:Dialog.WindowStartupLocationがManual指定時に動作します</para>
        /// </summary>
        public const string Left = "Left";
        /// <summary>
        /// <para>初期ウィンドウの上の位置を指定します</para>
        /// <para>prism:Dialog.WindowStartupLocationがManual指定時に動作します</para>
        /// </summary>
        public const string Top = "Top";

        /// <summary>
        /// 表示するウィンドウのオーナーウィンドウの指定を拒否します
        /// </summary>
        public const string UnsetOwner = "UnsetOwner";

        /// <summary>
        /// OKボタン表示
        /// </summary>
        public const string IsShowOk = "IsShowOk";
        /// <summary>
        /// キャンセルボタン表示
        /// </summary>
        public const string IsShowCancel = "IsShowCancel";
    }
}
