using Stylet;
using System.Collections.ObjectModel;

namespace Revit.StyletDemo.Model
{
    public class TreeNodeInfo : PropertyChangedBase
    {
        string _text;
        public string Text
        {
            get => _text;
            set => SetAndNotify(ref _text, value);
        }

        ObservableCollection<TreeNodeInfo> _children = new ObservableCollection<TreeNodeInfo>();
        public ObservableCollection<TreeNodeInfo> Children => _children;
    }
}
