using Example.Core.Mvvm;
using Example.Core.Utils;
using Example.Services;
using Prism.Regions;
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
    public class ViewAViewModel : RegionViewModelBase
    {
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
        /// <param name="createImage"></param>
        public ViewAViewModel(IRegionManager regionManager, CreateImageService createImage) :
            base(regionManager)
        {
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

            // ランダムデータ作成ボタン
            ChangeSampleCommand = new ReactiveCommand().AddTo(Disposables);
            ChangeSampleCommand.Subscribe(ChangeSample);

            //　CSVファイル選択ボタン
            BrowseCommand = new ReactiveCommand().AddTo(Disposables);
            BrowseCommand.Subscribe(BrowseCSVFile);

            // CSVファイル読み込みボタン
            ChangeCSVCommand = new ReactiveCommand().AddTo(Disposables);
            ChangeCSVCommand.Subscribe(ReadCsvFile);

            // 最初の1個目を動作させない
            RGBStringList.Skip(1).Subscribe(v => CreateImage());
            MinValue.Skip(1).Subscribe(v => CreateImage());
            MaxValue.Skip(1).Subscribe(v => CreateImage());
            TicksValue.Skip(1).Subscribe(v => CreateImage());

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
            var filePath = FileOpenProcess.GetFilePath(CsvFilePath.Value);

            if (File.Exists(filePath))
            {
                CsvFilePath.Value = filePath;
            }
        }

        /// <summary>
        /// CSVファイルを読み込みます
        /// </summary>
        private void ReadCsvFile()
        {
            string filePath = CsvFilePath.Value;

            try
            {
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

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
        }
    }
}
