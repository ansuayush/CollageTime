using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmployeeDocumentScannerHelper
{
    public class MainForm : Form
    {
        private readonly HrNestApiClient _api = new HrNestApiClient();
        private readonly TwainScannerService _twain;

        private TextBox _txtUrl = null!;
        private TextBox _txtUser = null!;
        private TextBox _txtPassword = null!;
        private Button _btnLogin = null!;
        private ComboBox _cboScanners = null!;
        private Button _btnRefreshScanners = null!;
        private Button _btnPickScanner = null!;
        private Button _btnConnect = null!;
        private Button _btnScan = null!;
        private CheckBox _chkShowUi = null!;
        private CheckBox _chkSignAfterUpload = null!;
        private ComboBox _cboSignerRole = null!;
        private TextBox _txtSignatureName = null!;
        private TextBox _txtEmployeeSearch = null!;
        private ListBox _lstEmployees = null!;
        private TextBox _txtDocTitle = null!;
        private FlowLayoutPanel _preview = null!;
        private Button _btnClearPages = null!;
        private Button _btnUpload = null!;
        private Label _lblStatus = null!;
        private Label _lblSelected = null!;

        private EmployeeHit? _selectedEmployee;
        private System.Windows.Forms.Timer? _searchTimer;

        public MainForm()
        {
            _twain = new TwainScannerService(this);
            _twain.StatusChanged += (_, msg) => SetStatus(msg);
            _twain.PagesChanged += (_, __) => RenderPreview();
            _twain.ScanCompleted += (_, __) =>
            {
                SetStatus("Scan complete. Pages: " + _twain.Pages.Count);
                PromptForDocumentNameAfterScan();
            };

            BuildUi();
            Load += MainForm_Load;
            FormClosed += (_, __) =>
            {
                _twain.Dispose();
                _api.Dispose();
            };
        }

        private void MainForm_Load(object? sender, EventArgs e)
        {
            _txtUrl.Text = "http://localhost:51643/";
            try
            {
                _twain.OpenSession();
                RefreshScannerList();
            }
            catch (Exception ex)
            {
                SetStatus("TWAIN open warning: " + ex.Message);
            }
        }

        private void BuildUi()
        {
            var bg = Color.FromArgb(236, 241, 236);
            var ink = Color.FromArgb(28, 42, 34);
            var muted = Color.FromArgb(90, 110, 98);
            var line = Color.FromArgb(210, 222, 214);
            var green = Color.FromArgb(32, 128, 72);
            var greenDark = Color.FromArgb(24, 102, 58);
            var softGreen = Color.FromArgb(232, 245, 236);

            Text = "HRNest · Employee Document Scanner";
            Width = 1100;
            Height = 800;
            MinimumSize = new Size(900, 650);
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 9.25F);
            BackColor = bg;
            ForeColor = ink;

            // Fill content first, then top/bottom chrome — TableLayout avoids SplitContainer size errors
            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = bg,
                Padding = new Padding(8)
            };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 420));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            Controls.Add(root);

            var header = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = green };
            header.Controls.Add(new Label
            {
                Text = "HRNest  ·  Employee Document Scanner",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(16, 0, 0, 0),
                Font = new Font("Segoe UI Semibold", 13F, FontStyle.Bold),
                ForeColor = Color.White
            });
            Controls.Add(header);

            _lblStatus = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 28,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(12, 0, 0, 0),
                BackColor = Color.FromArgb(28, 42, 34),
                ForeColor = Color.FromArgb(210, 230, 218),
                Text = "Ready — scroll the left panel for Employee (3) and Upload (4)."
            };
            Controls.Add(_lblStatus);

            // ===== LEFT: one scrollable column — size each GroupBox to fit its buttons =====
            var scroll = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                AutoScrollMinSize = new Size(0, 900),
                BackColor = bg,
                Padding = new Padding(4),
                Margin = new Padding(0, 0, 8, 0)
            };
            root.Controls.Add(scroll, 0, 0);

            int y = 4;
            int w = 372;

            void FitGroup(GroupBox g, int pad = 14)
            {
                int bottom = 28;
                foreach (Control c in g.Controls)
                    bottom = Math.Max(bottom, c.Bottom);
                g.Height = bottom + pad;
            }

            void AddLabel(Control parent, string text, ref int top, bool bold = false, Color? color = null)
            {
                var lbl = new Label
                {
                    Text = text,
                    Left = 8,
                    Top = top,
                    Width = w,
                    Height = bold ? 22 : 16,
                    Font = bold ? new Font("Segoe UI Semibold", 10F, FontStyle.Bold) : new Font("Segoe UI", 8.25F),
                    ForeColor = color ?? (bold ? ink : muted)
                };
                parent.Controls.Add(lbl);
                top += lbl.Height + 2;
            }

            TextBox AddBox(Control parent, ref int top)
            {
                var box = new TextBox
                {
                    Left = 8,
                    Top = top,
                    Width = w,
                    Height = 24,
                    BorderStyle = BorderStyle.FixedSingle,
                    Font = new Font("Segoe UI", 9.25F)
                };
                parent.Controls.Add(box);
                top += 28;
                return box;
            }

            // --- 1. connection ---
            var gConnect = new GroupBox
            {
                Text = "1. Connect to HRNest",
                Left = 2,
                Top = y,
                Width = 392,
                Height = 220,
                BackColor = Color.White,
                ForeColor = ink,
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold)
            };
            scroll.Controls.Add(gConnect);
            int cy = 22;
            AddLabel(gConnect, "Site URL", ref cy);
            _txtUrl = AddBox(gConnect, ref cy);
            AddLabel(gConnect, "User ID", ref cy);
            _txtUser = AddBox(gConnect, ref cy);
            AddLabel(gConnect, "Password", ref cy);
            _txtPassword = AddBox(gConnect, ref cy);
            _txtPassword.UseSystemPasswordChar = true;
            cy += 4;
            _btnLogin = PrimaryButton("Sign in", 140, 36, green, greenDark);
            _btnLogin.Left = 8;
            _btnLogin.Top = cy;
            _btnLogin.Click += async (_, __) => await LoginAsync();
            gConnect.Controls.Add(_btnLogin);
            FitGroup(gConnect);
            y += gConnect.Height + 10;

            // --- 2. scanner ---
            var gScan = new GroupBox
            {
                Text = "2. Scanner",
                Left = 2,
                Top = y,
                Width = 392,
                Height = 200,
                BackColor = Color.White,
                ForeColor = ink,
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold)
            };
            scroll.Controls.Add(gScan);
            cy = 22;
            _cboScanners = new ComboBox
            {
                Left = 8,
                Top = cy,
                Width = w,
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.System
            };
            gScan.Controls.Add(_cboScanners);
            cy += 30;
            _btnRefreshScanners = SecondaryButton("Refresh", 95, 28, line, ink);
            _btnRefreshScanners.Left = 8;
            _btnRefreshScanners.Top = cy;
            _btnPickScanner = SecondaryButton("TWAIN picker", 110, 28, line, ink);
            _btnPickScanner.Left = 108;
            _btnPickScanner.Top = cy;
            _btnConnect = SecondaryButton("Connect", 95, 28, line, ink);
            _btnConnect.Left = 224;
            _btnConnect.Top = cy;
            _btnRefreshScanners.Click += (_, __) => RefreshScannerList();
            _btnPickScanner.Click += (_, __) =>
            {
                try
                {
                    _twain.ShowSelectSourceDialog();
                    RefreshScannerList();
                    if (!string.IsNullOrEmpty(_twain.CurrentSourceName))
                        _cboScanners.SelectedItem = _twain.CurrentSourceName;
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Scanner"); }
            };
            _btnConnect.Click += (_, __) =>
            {
                try
                {
                    if (_cboScanners.SelectedItem == null) { MessageBox.Show("Select a scanner."); return; }
                    _twain.SelectSourceByName(_cboScanners.SelectedItem.ToString()!);
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Connect"); }
            };
            gScan.Controls.Add(_btnRefreshScanners);
            gScan.Controls.Add(_btnPickScanner);
            gScan.Controls.Add(_btnConnect);
            cy += 34;
            _chkShowUi = new CheckBox
            {
                Text = "Show scanner UI",
                Left = 8,
                Top = cy,
                Width = 180,
                Checked = true,
                ForeColor = muted,
                Font = new Font("Segoe UI", 9F)
            };
            gScan.Controls.Add(_chkShowUi);
            cy += 26;
            _btnScan = PrimaryButton("Scan page(s)", 160, 34, green, greenDark);
            _btnScan.Left = 8;
            _btnScan.Top = cy;
            _btnScan.Click += (_, __) =>
            {
                try { _twain.StartScan(Handle, _chkShowUi.Checked); }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Scan"); }
            };
            gScan.Controls.Add(_btnScan);
            FitGroup(gScan);
            y += gScan.Height + 10;

            // --- 3. employee ---
            var gEmp = new GroupBox
            {
                Text = "3. Employee",
                Left = 2,
                Top = y,
                Width = 392,
                Height = 180,
                BackColor = Color.White,
                ForeColor = ink,
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold)
            };
            scroll.Controls.Add(gEmp);
            cy = 22;
            AddLabel(gEmp, "Search name / file #", ref cy);
            _txtEmployeeSearch = AddBox(gEmp, ref cy);
            _txtEmployeeSearch.TextChanged += EmployeeSearch_TextChanged;
            _lstEmployees = new ListBox
            {
                Left = 8,
                Top = cy,
                Width = w,
                Height = 64,
                BorderStyle = BorderStyle.FixedSingle,
                IntegralHeight = false
            };
            _lstEmployees.SelectedIndexChanged += (_, __) =>
            {
                _selectedEmployee = _lstEmployees.SelectedItem as EmployeeHit;
                _lblSelected.Text = _selectedEmployee == null ? "No employee selected." : ("Selected: " + _selectedEmployee);
                _lblSelected.ForeColor = _selectedEmployee == null ? muted : greenDark;
            };
            gEmp.Controls.Add(_lstEmployees);
            cy += 68;
            _lblSelected = new Label
            {
                Text = "No employee selected.",
                Left = 8,
                Top = cy,
                Width = w,
                Height = 28,
                ForeColor = muted,
                Font = new Font("Segoe UI", 8.5F)
            };
            gEmp.Controls.Add(_lblSelected);
            FitGroup(gEmp);
            y += gEmp.Height + 10;

            // --- 4. upload + optional sign ---
            var gUpload = new GroupBox
            {
                Text = "4. Document name, optional sign & upload",
                Left = 2,
                Top = y,
                Width = 392,
                Height = 280,
                BackColor = softGreen,
                ForeColor = greenDark,
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold)
            };
            scroll.Controls.Add(gUpload);

            int uy = 22;
            AddLabel(gUpload, "Document name (required)", ref uy);
            _txtDocTitle = AddBox(gUpload, ref uy);
            _txtDocTitle.PlaceholderText = "e.g. Offer Letter.pdf";

            _chkSignAfterUpload = new CheckBox
            {
                Text = "Optional: sign as Employee or Admin",
                Left = 8,
                Top = uy,
                Width = w,
                AutoSize = true,
                ForeColor = ink,
                Font = new Font("Segoe UI", 9F)
            };
            gUpload.Controls.Add(_chkSignAfterUpload);
            uy = _chkSignAfterUpload.Bottom + 8;

            var lblSignAs = new Label
            {
                Text = "Sign as",
                Left = 8,
                Top = uy,
                Width = w,
                Height = 16,
                ForeColor = muted,
                Font = new Font("Segoe UI", 8.25F),
                Visible = false
            };
            gUpload.Controls.Add(lblSignAs);

            _cboSignerRole = new ComboBox
            {
                Left = 8,
                Top = uy + 18,
                Width = w,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Enabled = false,
                Visible = false,
                FlatStyle = FlatStyle.System
            };
            _cboSignerRole.Items.Add("Employee");
            _cboSignerRole.Items.Add("Admin");
            _cboSignerRole.SelectedIndex = 0;
            gUpload.Controls.Add(_cboSignerRole);

            var lblSignName = new Label
            {
                Text = "Typed signature name",
                Left = 8,
                Top = uy + 50,
                Width = w,
                Height = 16,
                ForeColor = muted,
                Font = new Font("Segoe UI", 8.25F),
                Visible = false
            };
            gUpload.Controls.Add(lblSignName);

            _txtSignatureName = new TextBox
            {
                Left = 8,
                Top = uy + 68,
                Width = w,
                Height = 24,
                Enabled = false,
                Visible = false,
                BorderStyle = BorderStyle.FixedSingle,
                PlaceholderText = "Full name as signature"
            };
            gUpload.Controls.Add(_txtSignatureName);

            _btnUpload = PrimaryButton("Upload PDF to employee folder", w, 40, green, greenDark);
            _btnUpload.Left = 8;
            _btnUpload.Top = uy + 8;
            _btnUpload.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            _btnUpload.Click += async (_, __) => await UploadAsync();
            gUpload.Controls.Add(_btnUpload);

            void SetSignFieldsVisible(bool on)
            {
                lblSignAs.Visible = on;
                _cboSignerRole.Visible = on;
                _cboSignerRole.Enabled = on;
                lblSignName.Visible = on;
                _txtSignatureName.Visible = on;
                _txtSignatureName.Enabled = on;
                _btnUpload.Top = on ? (_txtSignatureName.Bottom + 10) : (_chkSignAfterUpload.Bottom + 10);
                FitGroup(gUpload);
            }

            _chkSignAfterUpload.CheckedChanged += (_, __) => SetSignFieldsVisible(_chkSignAfterUpload.Checked);
            SetSignFieldsVisible(false);

            // bottom spacer so Upload is reachable via scroll
            scroll.Controls.Add(new Panel
            {
                Left = 0,
                Top = y + gUpload.Height + 20,
                Width = 20,
                Height = 40
            });

            // ===== RIGHT: preview =====
            var right = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(12), BorderStyle = BorderStyle.FixedSingle };
            root.Controls.Add(right, 1, 0);

            var previewTitle = new Label
            {
                Text = "Preview scanned pages  ·  use ↺ Left / ↻ Right under each page",
                Dock = DockStyle.Top,
                Height = 28,
                Font = new Font("Segoe UI Semibold", 10.5F, FontStyle.Bold),
                ForeColor = ink
            };
            right.Controls.Add(previewTitle);

            _btnClearPages = SecondaryButton("Clear pages", 120, 32, line, ink);
            _btnClearPages.Dock = DockStyle.Bottom;
            _btnClearPages.Click += (_, __) => _twain.ClearPages();
            right.Controls.Add(_btnClearPages);

            _preview = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(248, 250, 248),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(8)
            };
            right.Controls.Add(_preview);
            _preview.BringToFront();
        }

        private static Button PrimaryButton(string text, int width, int height, Color green, Color greenDark)
        {
            var btn = new Button
            {
                Text = text,
                Width = width,
                Height = height,
                FlatStyle = FlatStyle.Flat,
                BackColor = green,
                ForeColor = Color.White,
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = greenDark;
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(18, 84, 48);
            return btn;
        }

        private static Button SecondaryButton(string text, int width, int height, Color border, Color ink)
        {
            var btn = new Button
            {
                Text = text,
                Width = width,
                Height = height,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = ink,
                Font = new Font("Segoe UI", 9F),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderColor = border;
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(242, 248, 244);
            return btn;
        }

        private void SetStatus(string message)
        {
            if (InvokeRequired) { BeginInvoke(new Action(() => SetStatus(message))); return; }
            _lblStatus.Text = message;
        }

        private void RefreshScannerList()
        {
            try
            {
                if (!_twain.IsOpen) _twain.OpenSession();
                var names = _twain.GetSourceNames();
                _cboScanners.Items.Clear();
                foreach (var n in names) _cboScanners.Items.Add(n);
                if (_cboScanners.Items.Count > 0) _cboScanners.SelectedIndex = 0;
                SetStatus(names.Count == 0
                    ? "No TWAIN scanners detected. Install drivers / run as 32-bit / use TWAIN picker."
                    : ("Detected " + names.Count + " scanner(s)."));
            }
            catch (Exception ex)
            {
                SetStatus("Detect failed: " + ex.Message);
            }
        }

        private async Task LoginAsync()
        {
            try
            {
                _btnLogin.Enabled = false;
                _api.SetBaseUrl(_txtUrl.Text);
                var (ok, message, session) = await _api.LoginAsync(_txtUser.Text, _txtPassword.Text);
                SetStatus(message);
                if (!ok)
                {
                    MessageBox.Show(message, "Sign in");
                    return;
                }

                ApplyLoggedInEmployee(session);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Sign in");
            }
            finally
            {
                _btnLogin.Enabled = true;
            }
        }

        private void ApplyLoggedInEmployee(SessionInfo? session)
        {
            _lstEmployees.Items.Clear();
            _selectedEmployee = null;
            _lblSelected.Text = "No employee selected.";

            // Default optional signature to Employee (optional for employees)
            if (_cboSignerRole.Items.Count > 0)
            {
                if (session?.IsAdmin == true && session.IsEmployee == false)
                    _cboSignerRole.SelectedItem = "Admin";
                else
                    _cboSignerRole.SelectedItem = "Employee";
            }

            var me = session?.CurrentEmployee;
            if (me != null && me.EmployeeId > 0)
            {
                _txtEmployeeSearch.TextChanged -= EmployeeSearch_TextChanged;
                _txtEmployeeSearch.Text = me.PersonName;
                _txtEmployeeSearch.TextChanged += EmployeeSearch_TextChanged;

                _lstEmployees.Items.Add(me);
                _lstEmployees.SelectedItem = me;
                _selectedEmployee = me;
                _lblSelected.Text = "Selected: " + me;
                if (string.IsNullOrWhiteSpace(_txtSignatureName.Text))
                    _txtSignatureName.Text = me.PersonName;
                SetStatus("Signed in. Employee loaded: " + me.PersonName);
                return;
            }

            _txtEmployeeSearch.TextChanged -= EmployeeSearch_TextChanged;
            _txtEmployeeSearch.Clear();
            _txtEmployeeSearch.TextChanged += EmployeeSearch_TextChanged;
            SetStatus(session?.IsAdmin == true
                ? "Signed in as admin. Search for an employee to upload."
                : "Signed in. Search for an employee to upload.");
        }

        private void EmployeeSearch_TextChanged(object? sender, EventArgs e)
        {
            _searchTimer?.Stop();
            _searchTimer ??= new System.Windows.Forms.Timer { Interval = 350 };
            _searchTimer.Tick -= SearchTick;
            _searchTimer.Tick += SearchTick;
            _searchTimer.Start();
        }

        private async void SearchTick(object? sender, EventArgs e)
        {
            _searchTimer?.Stop();
            var text = _txtEmployeeSearch.Text.Trim();
            if (text.Length < 2 || !_api.IsLoggedIn) return;
            try
            {
                var hits = await _api.SearchEmployeesAsync(text);
                _lstEmployees.Items.Clear();
                foreach (var h in hits) _lstEmployees.Items.Add(h);
                SetStatus("Found " + hits.Count + " employee(s).");
            }
            catch (Exception ex)
            {
                SetStatus("Search error: " + ex.Message);
            }
        }

        private void RenderPreview()
        {
            if (InvokeRequired) { BeginInvoke(new Action(RenderPreview)); return; }
            _preview.Controls.Clear();
            var pages = _twain.Pages.ToList();
            var line = Color.FromArgb(210, 222, 214);
            var ink = Color.FromArgb(28, 42, 34);

            if (pages.Count == 0)
            {
                _preview.Controls.Add(new Label
                {
                    Text = "No pages yet.\nScan with the green Scan button, then rotate if needed.",
                    AutoSize = true,
                    ForeColor = Color.FromArgb(90, 110, 98),
                    Font = new Font("Segoe UI", 10F),
                    Margin = new Padding(16)
                });
                return;
            }

            for (int i = 0; i < pages.Count; i++)
            {
                var page = pages[i];
                int pageIndex = i;

                var wrap = new Panel
                {
                    Width = 168,
                    Height = 248,
                    Margin = new Padding(8),
                    BackColor = Color.White
                };
                wrap.Paint += (_, e) =>
                {
                    using var pen = new Pen(line);
                    e.Graphics.DrawRectangle(pen, 0, 0, wrap.ClientSize.Width - 1, wrap.ClientSize.Height - 1);
                };

                var thumb = new PictureBox
                {
                    Location = new Point(10, 10),
                    Width = 148,
                    Height = 168,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    BorderStyle = BorderStyle.None,
                    Image = new Bitmap(page, new Size(148, 168)),
                    BackColor = Color.FromArgb(248, 250, 248)
                };

                var pageLbl = new Label
                {
                    Text = "Page " + (i + 1),
                    Location = new Point(10, 182),
                    Width = 148,
                    Height = 22,
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = ink,
                    Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold)
                };

                var btnCcw = SecondaryButton("↺ Left", 66, 28, line, ink);
                btnCcw.Location = new Point(14, 208);
                btnCcw.Click += (_, __) => _twain.RotatePage(pageIndex, -90);
                var btnCw = SecondaryButton("↻ Right", 70, 28, line, ink);
                btnCw.Location = new Point(86, 208);
                btnCw.Click += (_, __) => _twain.RotatePage(pageIndex, 90);

                wrap.Controls.Add(thumb);
                wrap.Controls.Add(pageLbl);
                wrap.Controls.Add(btnCcw);
                wrap.Controls.Add(btnCw);
                _preview.Controls.Add(wrap);
            }
        }

        private void PromptForDocumentNameAfterScan()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(PromptForDocumentNameAfterScan));
                return;
            }

            if (_twain.Pages.Count == 0)
                return;

            using var dlg = new Form
            {
                Text = "Document name",
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MinimizeBox = false,
                MaximizeBox = false,
                ClientSize = new Size(420, 140),
                Font = Font
            };
            var lbl = new Label
            {
                Text = "Enter a name for this scanned document:",
                Left = 16,
                Top = 16,
                AutoSize = true
            };
            var box = new TextBox
            {
                Left = 16,
                Top = 44,
                Width = 380,
                Text = string.IsNullOrWhiteSpace(_txtDocTitle.Text)
                    ? ("Scan_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf")
                    : _txtDocTitle.Text
            };
            var ok = new Button { Text = "Save name", DialogResult = DialogResult.OK, Left = 220, Top = 90, Width = 85 };
            var cancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Left = 315, Top = 90, Width = 80 };
            dlg.Controls.AddRange(new Control[] { lbl, box, ok, cancel });
            dlg.AcceptButton = ok;
            dlg.CancelButton = cancel;
            box.SelectAll();
            box.Focus();

            if (dlg.ShowDialog(this) == DialogResult.OK && !string.IsNullOrWhiteSpace(box.Text))
            {
                var name = box.Text.Trim();
                if (!name.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                    name += ".pdf";
                _txtDocTitle.Text = name;
                SetStatus("Document name set: " + name);
            }
            else
            {
                SetStatus("Document name not set. Enter a name before upload.");
                _txtDocTitle.Focus();
            }
        }

        private async Task UploadAsync()
        {
            if (!_api.IsLoggedIn) { MessageBox.Show("Sign in first."); return; }
            if (_selectedEmployee == null) { MessageBox.Show("Select an employee."); return; }
            if (_twain.Pages.Count == 0) { MessageBox.Show("Scan at least one page."); return; }

            if (string.IsNullOrWhiteSpace(_txtDocTitle.Text))
            {
                PromptForDocumentNameAfterScan();
                if (string.IsNullOrWhiteSpace(_txtDocTitle.Text))
                {
                    MessageBox.Show("Document name is required.", "Document name");
                    _txtDocTitle.Focus();
                    return;
                }
            }

            var docName = _txtDocTitle.Text.Trim();
            if (!docName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                docName += ".pdf";
            _txtDocTitle.Text = docName;

            bool signAfter = _chkSignAfterUpload.Checked;
            string signerRole = _cboSignerRole.SelectedItem?.ToString() ?? "Employee";
            string signatureName = (_txtSignatureName.Text ?? "").Trim();
            if (signAfter && string.IsNullOrWhiteSpace(signatureName))
            {
                MessageBox.Show("Enter a typed signature name, or uncheck optional sign.", "Signature");
                _txtSignatureName.Focus();
                return;
            }

            try
            {
                _btnUpload.Enabled = false;
                SetStatus("Uploading as " + docName + "...");
                var pages = _twain.Pages.ToList();
                var (ok, message, documentId) = await _api.UploadPagesAsync(_selectedEmployee.EmployeeId, docName, pages);
                if (ok && signAfter && documentId.HasValue)
                {
                    var (signOk, signMsg) = await _api.SignDocumentAsync(documentId.Value, signerRole, signatureName);
                    message = message + " " + (signOk ? signMsg : ("(sign failed: " + signMsg + ")"));
                }
                SetStatus(message);
                MessageBox.Show(message, ok ? "Upload" : "Upload failed");
                if (ok)
                {
                    _twain.ClearPages();
                    _txtDocTitle.Clear();
                    _chkSignAfterUpload.Checked = false;
                    _txtSignatureName.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Upload");
            }
            finally
            {
                _btnUpload.Enabled = true;
            }
        }
    }
}
