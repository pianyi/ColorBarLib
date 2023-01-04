using Example.Core.Mvvm;
using Example.Core.Utils;
using Example.Modules.ColorBarPalettesDialog.Events;
using Example.Services;
using Prism.Events;
using Prism.Regions;
using Prism.Services.Dialogs;
using PrismExpansion.Services.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Diagnostics;
using System.IO;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Unity;

namespace Example.Modules.ModuleName.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public class ViewAViewModel : RegionViewModelBase
    {
        /// <summary>
        /// イベント制御
        /// </summary>
        [Dependency]
        internal IEventAggregator EventAggregator { get; set; }

        /// <summary>
        /// ダイアログ表示サービスを表します。
        /// </summary>
        [Dependency]
        internal readonly IDialogService DialogService;

        /// <summary>
        /// カラーバー作成
        /// </summary>
        [Dependency]
        internal CreateImageService CreateImageService { get; set; }

        /// <summary>
        /// RGB文字列
        /// </summary>
        public ReactiveProperty<string> RGBStringList { get; set; }

        /// <summary>
        /// 表示データの最小値
        /// </summary>
        public ReactiveProperty<double> MinValue { get; set; }

        /// <summary>
        /// 表示データの最大値
        /// </summary>
        public ReactiveProperty<double> MaxValue { get; set; }

        /// <summary>
        /// メモリの個数
        /// </summary>
        public ReactiveProperty<int> TicksValue { get; set; }

        /// <summary>
        /// 現在表示されているサンプルデータの範囲
        /// </summary>
        public ReactiveProperty<string> SampleData { get; set; }

        /// <summary>
        /// CSVファイルのパス
        /// </summary>
        public ReactiveProperty<string> CsvFilePath { get; set; }

        /// <summary>
        /// 色選択ダイアログ
        /// </summary>
        public ReactiveCommand SelectColorCommand { get; set; }

        /// <summary>
        /// サンプルデータの変更
        /// </summary>
        public ReactiveCommand ChangeSampleCommand { get; set; }

        /// <summary>
        /// ファイル選択ダイアログ表示
        /// </summary>
        public ReactiveCommand BrowseCommand { get; set; }

        /// <summary>
        /// CSVファイルを読み込みます
        /// </summary>
        public ReactiveCommand ChangeCSVCommand { get; set; }

        /// <summary>
        /// 作成した画像の表示
        /// </summary>
        public ReactiveProperty<BitmapSource?> Picture { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="regionManager"></param>
        /// <param name="eventAggregator"></param>
        /// <param name="dialogService"></param>
        /// <param name="createImage"></param>
        public ViewAViewModel(IRegionManager regionManager,
                              IEventAggregator eventAggregator,
                              IDialogService dialogService,
                              CreateImageService createImage) :
            base(regionManager)
        {
            EventAggregator = eventAggregator;
            DialogService = dialogService;
            CreateImageService = createImage;

            RGBStringList = CreateImageService.RGBStringList
                                              .ToReactivePropertyAsSynchronized(e => e.Value)
                                              .AddTo(Disposables);

            MinValue = CreateImageService.MinValue
                                         .ToReactivePropertyAsSynchronized(e => e.Value)
                                         .AddTo(Disposables);
            MaxValue = CreateImageService.MaxValue
                                         .ToReactivePropertyAsSynchronized(e => e.Value)
                                         .AddTo(Disposables);
            TicksValue = CreateImageService.TicksValue
                                           .ToReactivePropertyAsSynchronized(e => e.Value)
                                           .AddTo(Disposables);

            SampleData = CreateImageService.SampleData
                                           .ToReactivePropertyAsSynchronized(e => e.Value)
                                           .AddTo(Disposables);

            CsvFilePath = new ReactiveProperty<string>(@"SampleFile\neco.csv").AddTo(Disposables);

            Picture = CreateImageService.Picture
                                        .ToReactiveProperty()
                                        .AddTo(Disposables);

            // 色選択ダイアログ
            SelectColorCommand = new ReactiveCommand().WithSubscribe(ShowColorDialog)
                                                      .AddTo(Disposables);

            // ランダムデータ作成ボタン
            ChangeSampleCommand = new ReactiveCommand().WithSubscribe(ChangeSample)
                                                       .AddTo(Disposables);

            //　CSVファイル選択ボタン
            BrowseCommand = new ReactiveCommand().WithSubscribe(BrowseCSVFile)
                                                 .AddTo(Disposables);

            // CSVファイル読み込みボタン
            ChangeCSVCommand = new ReactiveCommand().WithSubscribe(ReadCsvFile)
                                                    .AddTo(Disposables);

            // 最初の1個目を動作させない
            RGBStringList.Skip(1).Subscribe(v => CreateImage());
            MinValue.Skip(1).Subscribe(v => CreateImage());
            MaxValue.Skip(1).Subscribe(v => CreateImage());
            TicksValue.Skip(1).Subscribe(v => CreateImage());

            // 色追加
            EventAggregator.GetEvent<SelectedColorEvent>().Subscribe(AddRgbString);

            // 最初の1個目を作成
            CreateImage();
        }

        /// <summary>
        /// 画像作成の開始
        /// </summary>
        private void CreateImage()
        {
            try
            {
                CreateImageService.DoCreateImage();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.StackTrace);
            }
        }

        /// <summary>
        /// RGBの色を追加します
        /// </summary>
        /// <param name="rgbString">#付きの色データ(nullは初期化)</param>
        private void AddRgbString(string rgbString)
        {
            try
            {
                if (rgbString == null)
                {
                    CreateImageService.RGBStringList.Value = string.Empty;
                }
                else
                {
                    string? tmp = CreateImageService.RGBStringList.Value?.Trim();
                    if (!string.IsNullOrWhiteSpace(tmp) && tmp.LastIndexOf(",") != tmp.Length - 1)
                    {
                        tmp += ", ";
                    }

                    CreateImageService.RGBStringList.Value = tmp + rgbString;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.StackTrace);
            }
        }

        /// <summary>
        /// カラーパレットダイアログを表示
        /// </summary>
        private void ShowColorDialog()
        {
            try
            {
                var mainWindow = Application.Current.MainWindow;
                // ダイアログに渡すパラメータ
                var param = new DialogParameters()
                {
                    { DialogParams.Key,   nameof(ColorBarPalettesDialog.Views.ViewColorBarPalettes) },
                    { DialogParams.Title, "Append Colors" },
                    { DialogParams.Top,   mainWindow.Top },
                    { DialogParams.Left,  mainWindow.Left +  mainWindow.Width - 100} // ボタンが隠れるぐらい左に移動する
                };

                // 押下されたボタン区分
                var ret = ButtonResult.No;
                // ダイアログからの戻りパラメータ
                IDialogParameters? resultParam = null;

                DialogService.Show(nameof(ColorBarPalettesDialog.Views.ViewColorBarPalettes),
                                   param, r => { ret = r.Result; resultParam = r.Parameters; },
                                   nameof(ColorBarPalettesDialog.Views.ViewColorBarPalettes));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.StackTrace);
            }
        }

        /// <summary>
        /// サンプルデータを変更します
        /// </summary>
        private void ChangeSample()
        {
            try
            {
                CreateImageService.CreateSampleImageData();
                CreateImage();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.StackTrace);
            }
        }

        /// <summary>
        /// ファイルパスを取得します
        /// </summary>
        private void BrowseCSVFile()
        {
            try
            {
                var filePath = FileOpenProcess.GetFilePath(CsvFilePath.Value);

                if (File.Exists(filePath))
                {
                    CsvFilePath.Value = filePath;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.StackTrace);
            }
        }

        /// <summary>
        /// CSVファイルを読み込みます
        /// </summary>
        private void ReadCsvFile()
        {
            try
            {
                string filePath = CsvFilePath.Value;

                if (File.Exists(filePath))
                {
                    CreateImageService.ReadCSVFile(filePath);
                    CreateImage();
                }
                else
                {
                    MessageBox.Show($"ファイルが存在しません。",
                                    "エラー",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error,
                                    MessageBoxResult.Yes);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.StackTrace);

                MessageBox.Show($"ファイルの読み込みに失敗しました。ファイル形式が正しくありません。{Environment.NewLine}条件：カンマ区切り、1データ=1ピクセル、全行同じ列数、ファイルフォーマットはUTF-8",
                                "エラー",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error,
                                MessageBoxResult.Yes);
            }
        }
    }
}
