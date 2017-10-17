using System;
using System.Diagnostics;
using System.Threading;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace HelloUnlocker.Controls
{
    public sealed partial class Lock : UserControl
    {
        private static Timer timer;

        public string Message { get; set; }

        public string Title
        {
            get
            {
                return this.bigTitle.Text;
            }
            set
            {
                this.bigTitle.Text = value;
            }
        }

        private int timerInterval;
        public int TimerInterval
        {
            get { return timerInterval; }
            set
            {
                if (value >= 5000)
                {
                    clearTimer();
                    timerInterval = value;

                    timer = new Timer(async (s) =>
                    {
                        var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;

                        await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                        {
                            this.Visibility = Visibility.Visible;
                            this.Focus(FocusState.Programmatic);
                            CoreWindow.GetForCurrentThread().CharacterReceived -= TimeoutBlock_CharacterReceived;
                            CoreWindow.GetForCurrentThread().PointerEntered -= TimeoutBlock_pointer;
                            CoreWindow.GetForCurrentThread().PointerMoved -= TimeoutBlock_pointer;

                            var dlg = new AdviceDialog(this.Message)
                            {
                                Title = this.Title,
                            };
                            var result = await dlg.ShowAsync();

                            if (dlg.Success == true)
                            {
                                this.Visibility = Visibility.Collapsed;
                                CoreWindow.GetForCurrentThread().CharacterReceived += TimeoutBlock_CharacterReceived;
                                CoreWindow.GetForCurrentThread().PointerEntered += TimeoutBlock_pointer;
                                CoreWindow.GetForCurrentThread().PointerMoved += TimeoutBlock_pointer;
                                restartTimer();
                            }
                        });
                    }, null, value, Timeout.Infinite);

                    CoreWindow wnd = CoreWindow.GetForCurrentThread();

                    if (wnd != null)
                    {
                        CoreWindow.GetForCurrentThread().CharacterReceived += TimeoutBlock_CharacterReceived;
                        CoreWindow.GetForCurrentThread().PointerEntered += TimeoutBlock_pointer;
                        CoreWindow.GetForCurrentThread().PointerMoved += TimeoutBlock_pointer;
                    }
                }
                else
                {
                    throw new ArgumentException();
                }
            }
        }

        public Lock()
        {
            this.InitializeComponent();
            this.TimerInterval = 5000;
            this.Title = "Application Locked";
            this.Message = "Login with Windows Hello to successfully unlock the app";
        }

        private void TimeoutBlock_pointer(CoreWindow sender, PointerEventArgs args)
        {
            restartTimer();
        }

        private void TimeoutBlock_CharacterReceived(CoreWindow sender, CharacterReceivedEventArgs args)
        {
            restartTimer();
        }

        private void restartTimer()
        {
            timer.Change(this.TimerInterval, Timeout.Infinite);
            Debug.WriteLine("Timer restarted, " + DateTime.Now.ToString());
        }

        private void clearTimer()
        {
            if (timer != null)
            {
                timer.Change(Timeout.Infinite, Timeout.Infinite);
                timer.Dispose();
                timer = null;
            }
        }
    }
}