using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using NTwain;
using NTwain.Data;

namespace EmployeeDocumentScannerHelper
{
    /// <summary>
    /// Thin wrapper around NTwain for detecting sources and acquiring image pages.
    /// Prefer x86 process — most scanner TWAIN drivers are 32-bit.
    /// </summary>
    public sealed class TwainScannerService : IDisposable
    {
        private TwainSession? _session;
        private DataSource? _currentSource;
        private readonly List<Image> _pages = new List<Image>();
        private readonly Control _syncControl;

        public TwainScannerService(Control syncControl)
        {
            _syncControl = syncControl ?? throw new ArgumentNullException(nameof(syncControl));
        }

        public IReadOnlyList<Image> Pages => _pages;

        public event EventHandler? PagesChanged;
        public event EventHandler<string>? StatusChanged;
        public event EventHandler? ScanCompleted;

        public bool IsOpen => _session != null && _session.State >= 3;

        public void OpenSession()
        {
            CloseSession();

            // Prefer legacy DSM when needed for older drivers
            try { PlatformInfo.Current.PreferNewDSM = true; } catch { /* ignore */ }

            var appId = TWIdentity.CreateFromAssembly(DataGroups.Image, Assembly.GetExecutingAssembly());
            _session = new TwainSession(appId);

            _session.TransferReady += (s, e) => RaiseStatus("Transfer ready...");
            _session.DataTransferred += Session_DataTransferred;
            _session.SourceDisabled += (s, e) =>
            {
                RaiseStatus("Scan finished.");
                SafeInvoke(() => ScanCompleted?.Invoke(this, EventArgs.Empty));
            };
            _session.TransferError += (s, e) => RaiseStatus("Transfer error: " + e.Exception?.Message);

            var rc = _session.Open();
            if (rc != ReturnCode.Success)
                throw new InvalidOperationException("Could not open TWAIN session (" + rc + "). Is TWAIN DSM installed?");

            RaiseStatus("TWAIN session open. Sources: " + _session.Count());
        }

        public List<string> GetSourceNames()
        {
            EnsureSession();
            return _session!.Select(ds => ds.Name).Where(n => !string.IsNullOrWhiteSpace(n)).ToList();
        }

        public void SelectSourceByName(string name)
        {
            EnsureSession();
            CloseCurrentSource();

            _currentSource = _session!.FirstOrDefault(ds => string.Equals(ds.Name, name, StringComparison.OrdinalIgnoreCase));
            if (_currentSource == null)
                throw new InvalidOperationException("Scanner not found: " + name);

            var rc = _currentSource.Open();
            if (rc != ReturnCode.Success)
                throw new InvalidOperationException("Could not open scanner (" + rc + ").");

            RaiseStatus("Connected: " + _currentSource.Name);
        }

        public void ShowSelectSourceDialog()
        {
            EnsureSession();
            CloseCurrentSource();
            _currentSource = _session!.ShowSourceSelector();
            if (_currentSource == null)
            {
                RaiseStatus("No scanner selected.");
                return;
            }
            var rc = _currentSource.Open();
            if (rc != ReturnCode.Success)
                throw new InvalidOperationException("Could not open scanner (" + rc + ").");
            RaiseStatus("Connected: " + _currentSource.Name);
        }

        public string? CurrentSourceName => _currentSource?.Name;

        public void ClearPages()
        {
            foreach (var img in _pages) img.Dispose();
            _pages.Clear();
            PagesChanged?.Invoke(this, EventArgs.Empty);
        }

        public void RotatePage(int index, int degrees)
        {
            if (index < 0 || index >= _pages.Count) return;
            degrees = ((degrees % 360) + 360) % 360;
            if (degrees == 0) return;

            var src = _pages[index];
            RotateFlipType flip;
            switch (degrees)
            {
                case 90: flip = RotateFlipType.Rotate90FlipNone; break;
                case 180: flip = RotateFlipType.Rotate180FlipNone; break;
                case 270: flip = RotateFlipType.Rotate270FlipNone; break;
                default: return;
            }

            var copy = new Bitmap(src);
            copy.RotateFlip(flip);
            src.Dispose();
            _pages[index] = copy;
            PagesChanged?.Invoke(this, EventArgs.Empty);
        }

        public void StartScan(IntPtr parentHandle, bool showScannerUi)
        {
            EnsureSession();
            if (_currentSource == null)
                throw new InvalidOperationException("Select a scanner first.");

            if (_currentSource.IsOpen == false)
            {
                var openRc = _currentSource.Open();
                if (openRc != ReturnCode.Success)
                    throw new InvalidOperationException("Could not open scanner (" + openRc + ").");
            }

            RaiseStatus("Starting scan on " + _currentSource.Name + "...");
            var mode = showScannerUi ? SourceEnableMode.ShowUI : SourceEnableMode.NoUI;
            var rc = _currentSource.Enable(mode, true, parentHandle);
            if (rc != ReturnCode.Success)
                throw new InvalidOperationException("Enable/scan failed (" + rc + ").");
        }

        private void Session_DataTransferred(object? sender, DataTransferredEventArgs e)
        {
            try
            {
                Image? image = null;
                if (e.NativeData != IntPtr.Zero)
                {
                    using var stream = e.GetNativeImageStream();
                    if (stream != null)
                    {
                        using var temp = Image.FromStream(stream);
                        image = new Bitmap(temp);
                    }
                }
                else if (!string.IsNullOrEmpty(e.FileDataPath) && File.Exists(e.FileDataPath))
                {
                    using var temp = Image.FromFile(e.FileDataPath);
                    image = new Bitmap(temp);
                }

                if (image != null)
                {
                    SafeInvoke(() =>
                    {
                        _pages.Add(image);
                        PagesChanged?.Invoke(this, EventArgs.Empty);
                        RaiseStatus("Captured page " + _pages.Count);
                    });
                }
            }
            catch (Exception ex)
            {
                RaiseStatus("Page capture error: " + ex.Message);
            }
        }

        private void CloseCurrentSource()
        {
            if (_currentSource != null)
            {
                try { if (_currentSource.IsOpen) _currentSource.Close(); } catch { /* ignore */ }
                _currentSource = null;
            }
        }

        public void CloseSession()
        {
            CloseCurrentSource();
            if (_session != null)
            {
                try { if (_session.State >= 3) _session.Close(); } catch { /* ignore */ }
                _session = null;
            }
        }

        private void EnsureSession()
        {
            if (_session == null || _session.State < 3)
                OpenSession();
        }

        private void RaiseStatus(string message)
        {
            SafeInvoke(() => StatusChanged?.Invoke(this, message));
        }

        private void SafeInvoke(Action action)
        {
            if (_syncControl.IsDisposed) return;
            if (_syncControl.InvokeRequired)
                _syncControl.BeginInvoke(action);
            else
                action();
        }

        public void Dispose()
        {
            ClearPages();
            CloseSession();
        }
    }
}
