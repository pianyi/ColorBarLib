using Prism.Events;

namespace Example.Modules.ColorBarPalettesDialog.Events
{
    /// <summary>
    /// 選択された色を通知します
    /// </summary>
    public class SelectedColorEvent : PubSubEvent<string>
    {
    }
}
