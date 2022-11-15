using Example.Core.Mvvm;
using Example.Services;
using Prism.Regions;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Diagnostics;
using System.Reactive.Linq;
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

            Picture = CreateImageService.Picture
                                        .ToReactiveProperty()
                                        .AddTo(Disposables);

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

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
        }
    }
}
