using Example.Modules.ColorBarPalettesDialog.Events;
using Example.Services;
using Prism.Events;
using Prism.Navigation;
using Prism.Services.Dialogs;
using PrismExpansion.Mvvm;
using PrismExpansion.Services.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Diagnostics;
using Unity;

namespace Example.Modules.ColorBarPalettesDialog.ViewModels
{
    /// <summary>
    /// カラーパレットダイアログ
    /// </summary>
    public class ViewColorBarPalettesViewModel : DisposableBindableViewModelBase, ICustomizeDialogAware, IDisposable, IDestructible
    {
        /// <summary>
        /// イベント制御
        /// </summary>
        [Dependency]
        internal IEventAggregator EventAggregator { get; set; }

        /// <summary>
        /// カラーバー作成
        /// </summary>
        [Dependency]
        internal CreateImageService CreateImageService { get; set; }

        /// <summary>
        /// 選択されている色情報
        /// </summary>
        public ReactiveProperty<string> ColorString { get; set; }

        /// <summary>
        /// 色の追加コマンド
        /// </summary>
        public ReactiveCommand AppendCommand { get; }

        /// <summary>
        /// クリアコマンド
        /// </summary>
        public ReactiveCommand ClearCommand { get; }

        /// <summary>
        /// 閉じるボタンのCommandを取得します。
        /// </summary>
        public ReactiveCommand CloseCommand { get; }

        /// <summary>
        /// タイトル
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 親ウィンドウを無しにして、メインウィドウで隠れるようにする
        /// </summary>
        public bool UnsetOwner { get; set; } = true;

        /// <summary>
        /// 左の位置はデフォルト設定
        /// </summary>
        public double Left { get; set; } = double.NaN;

        /// <summary>
        /// 上の位置はデフォルト設定
        /// </summary>
        public double Top { get; set; } = double.NaN;

        /// <summary>
        /// ダイアログを閉じるイベント
        /// </summary>
        public event Action<IDialogResult> RequestClose;

        /// <summary>
        /// ダイアログを閉じ事が出来るかどうか
        /// </summary>
        /// <returns></returns>
        public bool CanCloseDialog()
        {
            return true;
        }

        /// <summary>
        /// ダイアログが閉じた時の処理
        /// </summary>
        public void OnDialogClosed()
        {
            Dispose();
        }

        /// <summary>
        /// ダイアログが表示された時の処理
        /// </summary>
        /// <param name="parameters">IDialogServiceに設定されたパラメータを表すパラメータ</param>
        public void OnDialogOpened(IDialogParameters parameters)
        {
            // レイアウト制御
            SetLayout(parameters);
        }

        /// <summary>
        /// 画面レイアウトを設定します
        /// </summary>
        /// <param name="parameters"></param>
        private void SetLayout(IDialogParameters parameters)
        {
            if (parameters != null)
            {
                if (parameters.TryGetValue(DialogParams.Title, out string title))
                {
                    // タイトル設定
                    Title = title;
                }
                if (parameters.TryGetValue(DialogParams.Top, out int top))
                {
                    Top = top;
                }
                if (parameters.TryGetValue(DialogParams.Left, out int left))
                {
                    Left = left;
                }
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ViewColorBarPalettesViewModel(IEventAggregator eventAggregator,
                                             CreateImageService createImage)
        {
            EventAggregator = eventAggregator;
            CreateImageService = createImage;

            ColorString = new ReactiveProperty<string>("#FF0000").AddTo(Disposables);

            AppendCommand = new ReactiveCommand().WithSubscribe(AppendCommandClick).AddTo(Disposables);
            ClearCommand = new ReactiveCommand().WithSubscribe(ClearCommandClick).AddTo(Disposables);
            CloseCommand = new ReactiveCommand().WithSubscribe(CloseCommandClick).AddTo(Disposables);
        }

        /// <summary>
        /// 追加ボタン押下
        /// </summary>
        private void AppendCommandClick()
        {
            try
            {
                var tmp = ColorString.Value;
                if (7 < tmp.Length)
                {
                    // α値を削除する
                    tmp = $"#{tmp.Substring(3)}";
                }

                // 色選択通知
                EventAggregator.GetEvent<SelectedColorEvent>().Publish(tmp);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.StackTrace);
            }
        }

        /// <summary>
        /// クリアボタン押下
        /// </summary>
        private void ClearCommandClick()
        {
            try
            {
                // 色を全削除指示
                EventAggregator.GetEvent<SelectedColorEvent>().Publish(null);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.StackTrace);
            }
        }

        /// <summary>
        /// 閉じるボタンクリックイベント
        /// </summary>
        private void CloseCommandClick()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
        }

        /// <summary>
        /// 画面破棄の親名称を取得します
        /// </summary>
        /// <returns>nameofの親クラス名称</returns>
        protected override string ParentCloseDialogControl()
        {
            return nameof(ViewColorBarPalettesViewModel);
        }
    }
}
