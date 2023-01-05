using Example.Core.Behaviors.Interfaces;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using PrismExpansion.Services.Dialogs;
using Unity;

namespace Example.ViewModels
{
    public class MainWindowViewModel : BindableBase, IClosing
    {
        private string _title = "Color Bar Library Example Window";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        /// <summary>
        /// ダイアログ表示サービスを表します。
        /// </summary>
        [Dependency]
        internal readonly IDialogService DialogService;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="dialogService"></param>
        public MainWindowViewModel(IDialogService dialogService)
        {
            DialogService = dialogService;
        }

        /// <summary>
        /// 閉じてよいかの判断
        /// </summary>
        /// <returns></returns>
        public bool OnClosing()
        {
            if (DialogService is CustomizeDialogService dialogService)
            {
                // ダイアログをすべて閉じる
                dialogService.CloseAll();
            }

            return true;
        }

        /// <summary>
        /// 閉じた後の処理
        /// </summary>
        public void OnClosed()
        {
        }
    }
}
