using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsProfiler
{
    public partial class Form1 : Form
    {
        WindowsController wc = new WindowsController();
        public Form1()
        {
            InitializeComponent();
            this.Text = "Window Profiler";
            listBox1.SelectedIndexChanged += ProfileSelected;
            listBox2.SelectedIndexChanged += WindowSelected;
            notifyIcon1.Click += delegate { this.Show(); notifyIcon1.Visible = false; };
            GlobalHotkeying();
            LoadSettings();
            ReloadProfiles();
            this.FormClosing += Form1_FormClosing;

            // Profile removal context menu
            ContextMenu profileMenu = new ContextMenu();
            profileMenu.MenuItems.Add(new MenuItem("Remove selected profile", delegate { RemoveSelectedProfile(); }));
            this.listBox1.ContextMenu = profileMenu;
            // Window Removal context menu
            ContextMenu windowMenu = new ContextMenu();
            windowMenu.MenuItems.Add(new MenuItem("Remove selected window", delegate { RemoveSelectedWindow(); }));
            listBox2.ContextMenu = windowMenu;

            Console.WriteLine(System.IO.Directory.GetCurrentDirectory());
        }

        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            notifyIcon1.Visible = false;
            RemoveHotkeys();
        }




        
        //
        // MAIN FUNCTIONS
        //
        private void PlaceWindows(string pro="")
        {
            List<string> WindowsPlaced = new List<string>();
            if (pro == "")
            {
                foreach (Profile _profile in _ProfileManager.profiles)
                {
                    foreach (Window _Window in _profile.Windows)
                    {
                        WindowsPlaced.Add(_Window.Title);
                        wc.SetWindowPosition(
                            _Window.Title,
                            _Window.PositionX,
                            _Window.PositionY,
                            _Window.Width,
                            _Window.Height
                            );
                    }
                }
            }
            else
            {
                foreach (Profile _profile in _ProfileManager.profiles)
                {
                    if (pro == _profile.ProfileName)
                    {
                        foreach (Window _Window in _profile.Windows)
                        {
                            WindowsPlaced.Add(_Window.Title);
                            wc.SetWindowPosition(
                                _Window.Title,
                                _Window.PositionX,
                                _Window.PositionY,
                                _Window.Width,
                                _Window.Height
                                );
                        }
                    }
                }
            }
            wc.MinimizeAllExcept(WindowsPlaced);

        }
        // 
        // P R O F I L E        S T U F F
        //
        ProfileManager _ProfileManager = new ProfileManager();

        // Profile selected from the list
        void ProfileSelected(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                LoadProfileData(listBox1.SelectedItem.ToString());
            }
            
        }
        void RemoveSelectedProfile()
        {
            if (listBox1.SelectedItem != null)
            {
                foreach (Profile _Profile in _ProfileManager.profiles.ToList<Profile>())
                {
                    if (_Profile.ProfileName == listBox1.SelectedItem.ToString())
                    {
                        _ProfileManager.profiles.Remove(_Profile);
                        _ProfileManager.SaveProfiles();
                        ReloadProfiles();
                    }
                }
            }
        }
        // Loading Profile
        void LoadProfileData(string ProfileName)
        {
            foreach (Profile _Profile in _ProfileManager.profiles)
            {
                if (_Profile.ProfileName == ProfileName)
                {
                    listBox2.Items.Clear();
                    foreach (Window _Window in _Profile.Windows)
                    {
                        listBox2.Items.Add(_Window.Title);
                    }
                }
            }
        }
        // Adding profile
        private void button1_Click(object sender, EventArgs e)
        {
            if (!listBox1.Items.Contains(textBox1.Text))
            {
                _ProfileManager.AddProfile(textBox1.Text);
                ReloadProfiles();
            }
            else
            {
                MessageBox.Show("Profile with this name already exists, pick different name for your profile");
            }
        }
        // Reloading profiles
        private void ReloadProfiles()
        {
            Console.WriteLine("RELOADIN");
            string previous ="";
            if (listBox1.SelectedItem != null)
            {
                previous = listBox1.SelectedItem.ToString();
            }
            listBox1.Items.Clear();
            _ProfileManager.LoadProfiles();
            foreach(Profile _profile in _ProfileManager.profiles)
            {
                listBox1.Items.Add(_profile.ProfileName);
                if (_profile.ProfileName == previous)
                {
                    listBox1.SetSelected(listBox1.Items.IndexOf(_profile.ProfileName), true);
                }
            }
        }
        //
        // A D D I N G  // E D I T I N G WINDOWS
        //
        void WindowSelected(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                if (listBox2.SelectedItem != null)
                {
                    foreach (Profile _profile in _ProfileManager.profiles)
                    {
                        if (_profile.ProfileName == listBox1.SelectedItem.ToString())
                        {
                            foreach (Window _window in _profile.Windows)
                            {
                                if (_window.Title == listBox2.SelectedItem.ToString())
                                {
                                    Console.WriteLine("FOUND : " + _window.Title);
                                    Console.WriteLine("GridBased : " + _window.GridBased);
                                    textBox3.Text = _window.Title;
                                    if (_window.GridBased)
                                    {
                                        checkBox2.Checked = true;
                                        // Total columns combobox value
                                        comboBox2.SelectedIndex = comboBox2.Items.IndexOf(_window._ColumnsTotal.ToString());
                                        // Total rows combobox value
                                        comboBox3.SelectedIndex = comboBox3.Items.IndexOf(_window._RowsTotal.ToString());
                                        // Columns skipped value
                                        comboBox4.SelectedIndex = comboBox4.Items.IndexOf(_window._ColumnsSkipped.ToString());
                                        // Rows Skipped value
                                        comboBox5.SelectedIndex = comboBox5.Items.IndexOf(_window._RowsSkipped.ToString());
                                        // Columns wide value
                                        comboBox6.SelectedIndex = comboBox6.Items.IndexOf(_window._ColumnsWidth.ToString());
                                        // Rows high value
                                        comboBox7.SelectedIndex = comboBox7.Items.IndexOf(_window._RowsHigh.ToString());
                                        // Padding
                                        comboBox8.SelectedIndex = comboBox8.Items.IndexOf(_window._Padding.ToString());
                                    }
                                    else
                                    {
                                        checkBox1.Checked = true;
                                        textBox7.Text = _window.PositionY.ToString();
                                        textBox6.Text = _window.PositionX.ToString();
                                        textBox5.Text = _window.Width.ToString();
                                        textBox4.Text = _window.Height.ToString();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        void RemoveSelectedWindow()
        {
            if (listBox1.SelectedItem != null)
            {
                if (listBox2.SelectedItem != null)
                {
                    foreach (Profile _profile in _ProfileManager.profiles.ToList<Profile>())
                    {
                        if (_profile.ProfileName == listBox1.SelectedItem.ToString())
                        {
                            foreach (Window _window in _profile.Windows.ToList<Window>())
                            {
                                if (_window.Title == listBox2.SelectedItem.ToString())
                                {
                                    _ProfileManager.profiles[_ProfileManager.profiles.IndexOf(_profile)].Windows.Remove(_window);
                                    _ProfileManager.SaveProfiles();
                                    LoadProfileData(_profile.ProfileName);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }
        // Refresh window list
        private void button3_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            comboBox1.Items.Add("");
            foreach (WindowData _Window in wc.GetAllWindows())
            {
                comboBox1.Items.Add(_Window.Title);
            }
        }
        // Add window to list
        private void button2_Click(object sender, EventArgs e)
        {
            string WindowTitle;
            // Add window title from the list
            if (comboBox1.SelectedItem != null && comboBox1.SelectedItem.ToString() != "")
            {
                WindowTitle = comboBox1.SelectedItem.ToString();
            }
            // Add window title from the text
            else
            {
                WindowTitle = textBox2.Text;
            }
            textBox3.Text = WindowTitle;
        }
        // CHECKBOX BEHAVIOUR FOR MANUAL POSITIONING
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            groupBox4.Enabled = checkBox1.Checked;
            groupBox5.Enabled = !checkBox1.Checked;

            checkBox2.Checked = !checkBox1.Checked;
        }
        // CHECKBOX BEHAVIOUS FOR GRID BASED POSITIONING
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            groupBox4.Enabled = !checkBox2.Checked;
            groupBox5.Enabled = checkBox2.Checked;

            checkBox1.Checked = !checkBox2.Checked;
        }
        // get position of matched window
        private void button4_Click(object sender, EventArgs e)
        {
            if(textBox3.Text != ""){
                WindowData window = wc.FindWindow(textBox3.Text);
                WindowsController.RECT r= wc.GetWindowPos(window);
                // Textbox 7 - TOP
                textBox7.Text = r.Top.ToString();
                // Textbox 6 - LEFT
                textBox6.Text = r.Left.ToString();
                // Textbox 5 - Width
                textBox5.Text = (r.Right - r.Left).ToString();
                // Textbox 4 - Height
                textBox4.Text = (r.Bottom - r.Top).ToString();
            }
            else{
                MessageBox.Show("You must match window by something... ");
                return;
            }
            
        }
        // ADD WINDOW TO PROFILE
        private void button5_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
            {
                MessageBox.Show("You must select profile before adding window to it");
                return;
            }
            if (textBox3.Text == "" || textBox3.Text == null)
            {
                MessageBox.Show("You must match window by something..");
                return;
            }
            if (!checkBox1.Checked && !checkBox2.Checked)
            {
                MessageBox.Show("You must select whether you want to position manually or by the grid!");
                return;
            }
            // Position window manually.
            if (checkBox1.Checked)
            {
                int Top;
                int Left;
                int Width;
                int Height;
                if (!int.TryParse(textBox7.Text, out Top))
                {
                    MessageBox.Show("Top value is not numeric");
                    return;
                }
                if (!int.TryParse(textBox6.Text, out Left))
                {
                    MessageBox.Show("Left value is not numeric");
                    return;
                }
                if (!int.TryParse(textBox5.Text, out Width))
                {
                    MessageBox.Show("Width value is not numeric");
                    return;
                }
                if (!int.TryParse(textBox4.Text, out Height))
                {
                    MessageBox.Show("Height value is not numeric");
                    return;
                }
                Window _Window = new Window(textBox3.Text);
                _Window.SetWindowRectangle(Left, Top, Width, Height);
                _ProfileManager.AddToProfile(listBox1.SelectedItem.ToString(), _Window);
                
                ProfileSelected(null, null);
            }
            else
            {
                if (comboBox2.SelectedItem == null || comboBox2.SelectedItem.ToString() == "")
                {
                    MessageBox.Show("You must select how many columns you want to split your screen into!");
                    return;
                }
                if (comboBox3.SelectedItem == null || comboBox3.SelectedItem.ToString() == "")
                {
                    MessageBox.Show("You must select how many rows you want to split your screen into!");
                    return;
                }

                if (comboBox4.SelectedItem == null || comboBox4.SelectedItem.ToString() == "")
                {
                    MessageBox.Show("You must select how many coumns you want window to skip.");
                    return;
                }
                if (comboBox5.SelectedItem == null || comboBox5.SelectedItem.ToString() == "")
                {
                    MessageBox.Show("You must select how many rows you want window to skip.");
                    return;
                }
                if (comboBox6.SelectedItem == null || comboBox6.SelectedItem.ToString() == "")
                {
                    MessageBox.Show("You must select how many columns wide the window should be.");
                    return;
                }
                if (comboBox7.SelectedItem == null || comboBox7.SelectedItem.ToString() == "")
                {
                    MessageBox.Show("You must select how many rows high the window should be.");
                    return;
                }
                if (comboBox8.SelectedItem == null || comboBox8.SelectedItem.ToString() == "")
                {
                    MessageBox.Show("You must select how many pixels of padding the window should have.");
                    return;
                }

                int ColumnsTotal = int.Parse(comboBox2.SelectedItem.ToString());
                int ColumnsSkipped = int.Parse(comboBox4.SelectedItem.ToString());
                int ColumnsWide = int.Parse(comboBox6.SelectedItem.ToString());

                int RowsTotal = int.Parse(comboBox3.SelectedItem.ToString());
                int RowsSkipped = int.Parse(comboBox5.SelectedItem.ToString());
                int RowsHigh = int.Parse(comboBox7.SelectedItem.ToString());

                int Padding = int.Parse(comboBox8.SelectedItem.ToString());

                Window _Window = new Window(textBox3.Text);
                _Window.SetWindowRectangleTable(ColumnsWide, ColumnsTotal, ColumnsSkipped, RowsHigh, RowsTotal, RowsSkipped, Padding);
                _ProfileManager.AddToProfile(listBox1.SelectedItem.ToString(), _Window);
                ReloadProfiles();
                ProfileSelected(null,null);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //wc.MinimizeAll();
            PlaceWindows();
        }
        // Select profile window
        private void button7_Click(object sender, EventArgs e)
        {
            Form form = new Form();
            form.BackColor = Color.FromArgb(64,64,64);
            form.TopMost = true;
            
            // Close form button
            Button close = new Button();
            close.Text = "X";
            
            Font fff = (Font)close.Font.Clone();
            Font ffff = new Font(fff.FontFamily,13f,FontStyle.Bold);
            close.Font = ffff;
            close.Click += delegate { form.Close(); };
            close.Size = new System.Drawing.Size(30, 30);
            close.ForeColor = Color.White;
            close.BackColor = Color.FromArgb(64, 255, 255, 255);
            close.FlatStyle = FlatStyle.Flat;
            close.FlatAppearance.BorderSize = 0;
            close.FlatAppearance.BorderColor = close.BackColor;
            close.Location = new Point(260, 10);
            form.Controls.Add(close);
            //
            int margin = 25;
            int counter = 0;
            
            foreach (Profile _profile in _ProfileManager.profiles)
            {
                Panel p = new Panel();
                p.BackColor = Color.FromArgb(64,255,255,255);
                p.Size = new System.Drawing.Size(300, 50);
                p.Location = new Point(0, 50 + (counter*75));
                
                p.Click += delegate
                {
                    Console.WriteLine(_profile.ProfileName);
                    PlaceWindows(_profile.ProfileName);
                    form.Close();
                };
                p.MouseEnter += delegate
                {
                    p.BackColor = Color.FromArgb(96, 255, 255, 255);
                };
                p.MouseLeave += delegate
                {
                    p.BackColor = Color.FromArgb(64, 255, 255, 255);
                };
                p.Paint += delegate(object s, PaintEventArgs ex)
                {
                    Font font = new Font("Segoe UI", 24.0f);
                    SizeF measure = ex.Graphics.MeasureString(_profile.ProfileName, font);
                    ex.Graphics.DrawString(_profile.ProfileName, font, new SolidBrush(Color.White), new PointF((300-measure.Width)/2, 0));

                };
                form.Controls.Add(p);
                counter++;
            }
            form.FormBorderStyle = FormBorderStyle.None;
            form.Size = new System.Drawing.Size(300, (counter*75)+margin*2);
            // Center form on screen
            int position_x = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Left + (System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width - form.Size.Width)/2;
            int position_y = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Top + (System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height - form.Size.Height) / 2;
            Console.WriteLine(position_x);
            Console.WriteLine(position_y);
            
            form.Show();
            form.Location = new Point(position_x, position_y);
        }
        //
        //  SETTINGS STUFF
        //
        public class MySettings
        {
            public Boolean StartWithWindows;
            public Boolean StartMinimized;
        }
        void LoadSettings()
        {
            
            checkBox3.Checked = WindowsProfiler.Properties.Settings.Default.StartWithWindows;
            if (WindowsProfiler.Properties.Settings.Default.StartMinimized)
            {
                checkBox4.Checked = WindowsProfiler.Properties.Settings.Default.StartMinimized;
                this.Shown += delegate { HideInTray(); };
                
                

            }
        }
        // move to tray button
        private void button8_Click(object sender, EventArgs e)
        {
            HideInTray();
        }
        private void HideInTray()
        {
            this.Hide();
            notifyIcon1.ShowBalloonTip(0);
            notifyIcon1.Visible = true;
        }
        // Run with windows
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            RegistryKey regkey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            string publisherName = Application.CompanyName;
            string productName = Application.ProductName;
            string allProgramsPath = Environment.GetFolderPath(Environment.SpecialFolder.Programs);
            string shortcutPath = Path.Combine(allProgramsPath, publisherName);
            shortcutPath = Path.Combine(shortcutPath, productName) + ".appref-ms";
            regkey.DeleteSubKey(productName, false);//delete old key if exists
            if (checkBox3.Checked)
            {
                regkey.SetValue(productName, shortcutPath);
            }
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            WindowsProfiler.Properties.Settings.Default.StartMinimized = checkBox4.Checked;
            WindowsProfiler.Properties.Settings.Default.Save();
        }
        // GLOBAL HOTKEYING
        // alt+space
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        void GlobalHotkeying()
        {
            RegisterHotKey(this.Handle, 0, 3, Keys.Space.GetHashCode());       // Register alt+spaceas global hotkey. 

        }
        protected override void WndProc(ref Message m)
        {
           
            base.WndProc(ref m);

            if (m.Msg == 0x0312)
            {
                Console.WriteLine("MEH");
                button7_Click(null, null);
                /* Note that the three lines below are not needed if you only want to register one hotkey.
                 * The below lines are useful in case you want to register multiple keys, which you can use a switch with the id as argument, or if you want to know which key/modifier was pressed for some particular reason. */

                
                // do something
            }
        }
        void RemoveHotkeys()
        {
            UnregisterHotKey(this.Handle, 0); 
        }
        // Import profiles
        private void button6_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Data files (*.dat)|*.dat|All files (*.*)|*.*";
            if (DialogResult.OK == ofd.ShowDialog())
            {
                System.IO.File.Copy(ofd.FileName, System.IO.Directory.GetCurrentDirectory() + "\\Profiles.dat",true);
                ReloadProfiles();
            }
            else
            {
                MessageBox.Show("Failed to import profiles");
            }

        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.CheckPathExists = true;
            sfd.FileName = "Profiles.dat";
            if (DialogResult.OK == sfd.ShowDialog())
            {
                System.IO.File.Copy(System.IO.Directory.GetCurrentDirectory() + "\\Profiles.dat", sfd.FileName,true);
                ReloadProfiles();
            }
            else
            {
                MessageBox.Show("Failed to export profiles.");
            }
        }
    }
}
