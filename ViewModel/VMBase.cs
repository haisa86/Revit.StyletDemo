using Stylet;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Revit.StyletDemo.ViewModel
{
    public class VMBase : Screen, IBusyable, IDisposable
    {
        #region Stylet
        static readonly Lazy<EventAggregator> _eventAggregator = new Lazy<EventAggregator>(() =>
        {
            return new EventAggregator();
        }, true);
        /// <summary>
        /// 获取一个默认事件分发器
        /// </summary>
        public static EventAggregator EventAggregator
        {
            get { return _eventAggregator.Value; }
        }
        #endregion Stylet

        #region IDisposable实现
        bool disposed = false;
        /// <summary>
        /// 是否已被释放
        /// </summary>
        public bool IsDisposed => disposed;


        ~VMBase() { Dispose(false); }


        public void Dispose()
        {
            Dispose(true);
            //通知垃圾回收器不再调用终结器
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }
            //清理托管资源
            if (disposing)
            {
                if (View is FrameworkElement fe)
                {
                    fe.Unloaded -= Fe_Unloaded;

                    if (this is IHandle handle)
                    {
                        EventAggregator.Unsubscribe(handle);
                    }
                }
            }
            //清理非托管资源

            OnDispose(disposing);

            //告诉自己已经被释放
            disposed = true;
        }

        protected virtual void OnDispose(bool disposing)
        {

        }
        #endregion IDisposable实现

        bool _isFirstLoaded = true;
        bool _isFirstUnLoaded = true;
        bool _isBusy = false;

        /// <summary>
        /// 指示视图是否正在忙
        /// </summary>
        public bool IsBusy
        {
            get => _isBusy;
            set => SetAndNotify(ref _isBusy, value);
        }

        protected VMBase(UIElement view)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));

            if (view is FrameworkElement fe)
            {
                fe.DataContext = this;
            }
            Stylet.Xaml.View.SetActionTarget(view, this);
            if (this is IViewAware aware)
            {
                aware.AttachView(view);
            }

            if (this is IHandle handle)
                EventAggregator.Subscribe(handle);
        }

        protected override void OnViewLoaded()
        {
            OnViewLoaded(_isFirstLoaded);
            if (_isFirstLoaded)
            {
                _isFirstLoaded = false;
                if (View is FrameworkElement fe)
                {
                    fe.Unloaded -= Fe_Unloaded;
                    fe.Unloaded += Fe_Unloaded;
                }

                Window win = null;
                if (View is Window)
                {
                    win = View as Window;
                }
                else
                {
                    win = Window.GetWindow(View);
                }


                if (win != null)
                {
                    win.Closing -= Win_Closing;
                    win.Closing += Win_Closing;
                    win.Closed -= Win_Closed;
                    win.Closed += Win_Closed;
                }

            }
        }

        private async void Win_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !(await CanCloseAsync());
            if (e.Cancel == false)
            {
                Window window = sender as Window;
                if (window != null)
                    window.Closing -= Win_Closing;
            }
        }

        public override Task<bool> CanCloseAsync()
        {
            return Task.FromResult(CanClose());
        }

        [Obsolete("This method is deprecated, please use CanCloseAsync() instead")]
        protected override bool CanClose()
        {
            return true;
        }

        public override void RequestClose(bool? dialogResult = null)
        {
            if (View is Window win)
            {
                if (dialogResult == null)
                {
                    win.Close();
                    return;
                }
                win.DialogResult = dialogResult;
            }
        }

        private void Win_Closed(object sender, EventArgs e)
        {
            Window window = sender as Window;
            if (window != null)
                window.Closed -= Win_Closed;
            OnClose();
            Dispose();
        }

        private void Fe_Unloaded(object sender, RoutedEventArgs e)
        {
            OnViewUnLoaded(_isFirstUnLoaded);
            if (_isFirstUnLoaded)
                _isFirstUnLoaded = false;
        }

        /// <summary>
        /// 视图触发Loaded事件时执行的回调
        /// </summary>
        /// <param name="isFirstLoaded">视图是否第一次加载</param>
        protected virtual void OnViewLoaded(bool isFirstLoaded)
        {
        }

        /// <summary>
        /// 视图触发Unloaded事件时执行的回调
        /// </summary>
        /// <param name="_isFirstUnLoaded">视图是否第一次移除</param>
        protected virtual void OnViewUnLoaded(bool _isFirstUnLoaded)
        {
        }

        /// <summary>
        /// 在视图关联的视图线程中同步执行委托
        /// </summary>
        /// <param name="action"></param>
        public void InvokeOnUI(Action action)
        {
            var dispatcher = View.Dispatcher;
            if (dispatcher == null) return;
            if (dispatcher.Thread.ManagedThreadId == Thread.CurrentThread.ManagedThreadId)
                action();
            else
                dispatcher.Invoke(action);
        }


        /// <summary>
        /// 在视图关联的视图线程中异步执行委托
        /// </summary>
        /// <param name="action"></param>
        /// <param name="priority"></param>
        public void BeginInvokeOnUI(Action action, DispatcherPriority priority = DispatcherPriority.Background)
        {
            var dispatcher = View.Dispatcher;
            if (dispatcher == null) return;
            dispatcher.BeginInvoke(action, priority);
        }


    }

    public interface IBusyable
    {
        /// <summary>
        /// 指示视图是否正在忙
        /// </summary>
        bool IsBusy { get; set; }
    }
}

