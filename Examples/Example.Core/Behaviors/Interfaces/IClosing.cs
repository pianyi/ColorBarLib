namespace Example.Core.Behaviors.Interfaces
{
    /// <summary>
    /// メインウィンドウのクローズ処理
    /// </summary>
    public interface IClosing
    {
        /// <summary>
        /// 画面が閉じてよいかどうかを判断します
        /// </summary>
        /// <returns>true:閉じる、false:閉じない</returns>
        bool OnClosing();

        /// <summary>
        /// 画面が閉じた後に動作します
        /// </summary>
        void OnClosed();
    }
}
