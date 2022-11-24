using Revit.StyletDemo.Model;
using Revit.StyletDemo.View;
using Stylet;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace Revit.StyletDemo.ViewModel
{
    public class MainViewModel : VMBase, IHandle<TreeNodeInfo>
    {
        TreeNodeInfo _selectedNode;
        /// <summary>
        /// 获取或设置被选中的树节点
        /// </summary>
        public TreeNodeInfo SelectedNode
        {
            get => _selectedNode;
            private set => SetAndNotify(ref _selectedNode, value);
        }

        public ObservableCollection<TreeNodeInfo> TreeNodeCollection { get; } = new ObservableCollection<TreeNodeInfo>();

        public MainViewModel(UIElement view) : base(view) { }

        protected override void OnViewLoaded(bool isFirstLoaded)
        {
            //窗体第一次加载时创建树控件数据源
            if (isFirstLoaded)
            {
                var wallNode = new TreeNodeInfo { Text = "Wall" };
                AddChildren(wallNode, NewChildren(wallNode.Text));

                var beamNode = new TreeNodeInfo { Text = "Beam" };
                AddChildren(beamNode, NewChildren(beamNode.Text));

                TreeNodeCollection.Add(wallNode);
                TreeNodeCollection.Add(beamNode);
            }
        }

        private void AddChildren(TreeNodeInfo parent, IEnumerable<TreeNodeInfo> collection)
        {
            foreach (TreeNodeInfo child in collection)
            {
                parent.Children.Add(child);
            }
        }

        //模拟创建10个子节点
        private IEnumerable<TreeNodeInfo> NewChildren(string text)
        {
            for (int i = 0; i < 10; i++)
            {
                yield return new TreeNodeInfo
                {
                    Text = $"{text}{i}"
                };
            }
        }

        public void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

        }

        //public void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        //{
        //    SelectedNode = e.NewValue as TreeNodeInfo;
        //}

        public void AddChild()
        {
            if (SelectedNode == null)
            {
                MessageBox.Show("请选中节点");
                return;
            }

            var win = new AddChildView();
            win.Owner = View as Window;
            win.ShowDialog();
        }

        public void Handle(TreeNodeInfo message)
        {
            //接收从AddChildView发送的消息，为当前被选中的节点添加一个子节点
            if (SelectedNode != null)
            {
                SelectedNode.Children.Add(message);
            }
        }
    }

}
