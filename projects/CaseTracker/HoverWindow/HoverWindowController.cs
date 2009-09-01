using System;
using FogBugzNet;
using System.Windows.Forms;
using System.Drawing;
using System.Configuration;

namespace FogBugzCaseTracker
{
    public partial class HoverWindow
    {
        private FogBugz _fb;

        private bool _resizing = false;
        private AutoUpdater _autoUpdate;

        private bool _dragging = false;
        private DateTime _startDragTime;
        private int _mouseDownX;

        private int _mouseDownY;
        private int _dragDistance = 0;

        private int _gripStartX;


        public HoverWindow()
        {
            InitializeComponent();



            _baseSearch = ConfigurationManager.AppSettings["BaseSearch"];
            _ignoreBaseSearch = bool.Parse(ConfigurationManager.AppSettings["IgnoreBaseSearch"]);
            _autoUpdate = new AutoUpdater(ConfigurationManager.AppSettings["AutoUpdateURL"],
                                            new TimeSpan(int.Parse(ConfigurationManager.AppSettings["VersionUpdateCheckIntervalHours"]), 0, 0));

            loadSettings();

            _autoUpdate.Run();
        }

        private void startDragging(MouseEventArgs e)
        {
            _startDragTime = DateTime.Now;

            _dragDistance = 0;
            _mouseDownX = e.X;
            _mouseDownY = e.Y;
            _dragging = true;
        }
        private bool atScreenEdge(Point p)
        {
            return (p.X <= 0) || (p.Y <= 0);

        }

        private void moveWindow(Point p)
        {

            Rectangle screen = Screen.PrimaryScreen.WorkingArea;
            if (Screen.AllScreens.Length == 1)
            {
                if (p.X < 5)
                    p.X = 0;
                if (p.X + Width > screen.Width - 5)
                    p.X = screen.Width - Width;
            }
            if (p.Y < 5)
                p.Y = 0;
            if (p.Y + Height > screen.Height - 5)
                p.Y = screen.Height - Height;

            Location = p;
        }

        private void dragWindow(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point p = new Point();
                // Measure drag distance
                _dragDistance += (int)Math.Sqrt((e.X - _mouseDownX) * (e.X - _mouseDownX) + (e.Y - _mouseDownY) * (e.Y - _mouseDownY));

                // Measure drag speed
                long ticks = DateTime.Now.Subtract(_startDragTime).Milliseconds;
                if (ticks > 0)
                {
                    double speed = (double)_dragDistance / ticks;

                    // Not doing anything with the drag speed for now
                    // Might use it to implement mouse gestures
                }
                _dragDistance = 0;
                _startDragTime = DateTime.Now;


                p.X = Location.X + (e.X - _mouseDownX);
                p.Y = Location.Y + (e.Y - _mouseDownY);
                moveWindow(p);
            }

        }

        private void MoveWindowToCenter()
        {
            Point p = new Point();
            p.X = (Screen.PrimaryScreen.WorkingArea.Width - Width) / 2;
            p.Y = 0;
            Location = p;
        }

        private void ResizeWidth()
        {
            Width += Cursor.Position.X - _gripStartX;
            _gripStartX = Cursor.Position.X;
        }

        private void CloseApplication()
        {
            try
            {
                if (_switchToNothinUponClosing)
                    _fb.StopWorking();
                saveSettings();
            }
            catch (System.Exception x)
            {
                Utils.LogError(x.ToString());

            }
        }
        
    } // class HoverWindow
} // ns FogBugzCaseTracker
