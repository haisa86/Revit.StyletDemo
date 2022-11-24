using Revit.StyletDemo.Model;
using Stylet;
using System.Windows;

namespace Revit.StyletDemo.ViewModel
{
    public class AddChildViewModel: VMBase
    {
        string _text;
        public string Text
        {
            get => _text;
            set => SetAndNotify(ref _text, value);
        }

        public AddChildViewModel(UIElement view) : base(view) { }

        public void Confirm()
        {
            string text = _text?.Trim();
            if(string.IsNullOrEmpty(text))
            {
                MessageBox.Show("请输入内容！");
                return;
            }

            RequestClose(true);
            //发送消息通知MainWindow中添加子节点
            EventAggregator.Publish(new TreeNodeInfo { Text = text });
            //在UI线程中发送消息
            //EventAggregator.PublishOnUIThread(new TreeNodeInfo { Text = text });
        }
    }
}
