using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace WindowsProfiler
{
    class ProfileManager
    {
        public List<Profile> profiles = new List<Profile>();
        public ProfileManager()
        {
            LoadProfiles();
        }
        public void AddProfile(string ProfileName)
        {
            Profile Profile = new Profile(ProfileName);
            profiles.Add(Profile);
            SaveProfiles();
        }
        public void AddToProfile(string ProfileName, Window _Window)
        {
            Console.WriteLine("ADDING PROFILE FOR : " + ProfileName + " WITH WINDOW : " + _Window.Title);
            foreach (Profile _profile in profiles)
            {
                if (_profile.ProfileName == ProfileName)
                {
                    foreach (Window _win in _profile.Windows.ToList<Window>())
                    {
                        if (_win.Title == _Window.Title)
                        {
                            _profile.Windows.Remove(_win);
                        }
                    }
                    _profile.Windows.Add(_Window);
                }
            }
            Console.WriteLine("Total profiles : " + profiles.Count);
            int ccc = 0;
            Console.WriteLine("First profile windows : " + profiles[0].Windows.Count);
            SaveProfiles();
        }
        public void LoadProfiles()
        {
            
            string location = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (!location.EndsWith("\\"))
            {
                location += "\\";
            }
            if (!System.IO.File.Exists(location + "Profiles.dat"))
            {

                Console.WriteLine(location + "Profiles.dat" + " DOES NOT EXISTS!");
                SaveProfiles();
                
            }
            Console.WriteLine("LOADING");
            FileStream fs = new FileStream(System.IO.Directory.GetCurrentDirectory()+"\\Profiles.dat", FileMode.Open);
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                profiles = (List<Profile>)formatter.Deserialize(fs);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }
            foreach (Profile p in profiles)
            {
                Console.WriteLine("LOADED : " + p.ProfileName + " With "+ p.Windows.Count +" Windows.");
                foreach (Window _W in p.Windows)
                {
                    Console.WriteLine(_W.Title);
                }
            }
        }
        public void SaveProfiles()
        {
            Console.WriteLine("SAVING");
            FileStream fs = new FileStream(System.IO.Directory.GetCurrentDirectory() + "\\Profiles.dat", FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(fs, profiles);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }
        }
    }
    [Serializable]
    class Profile{
        public string ProfileName;
        public List<Window>Windows = new List<Window>();
        public Profile(string _ProfileName){
            ProfileName = _ProfileName;
        }
        public void AddWindow(string WindowTitle){
            Window Window = new Window(WindowTitle);
            Windows.Add(Window);
        }
    }
    [Serializable]
    public class Window
    {
        public string Title;

        public int PositionX;
        public int PositionY;
        public int Width;
        public int Height;

        public Window(string _Title)
        {
            Title = _Title;
        }
        public void SetWindowRectangle(int _PositionX,int _PositionY, int _Width, int _Height)
        {
            PositionX = _PositionX;
            PositionY = _PositionY;
            Width = _Width;
            Height = _Height;
        }
        public Boolean GridBased = false;
        public int _ColumnsWidth;
        public int _ColumnsTotal;
        public int _ColumnsSkipped;

        public int _RowsHigh;
        public int _RowsTotal;
        public int _RowsSkipped;

        public int _Padding;
        public void SetWindowRectangleTable(int ColumnsWidth, int ColumnsTotal,int ColumnsSkipped, int RowsHeight, int RowsTotal, int RowsSkipped, int Margin)
        {
            
            GridBased = true;
            _ColumnsWidth = ColumnsWidth;
            _ColumnsTotal = ColumnsTotal;
            _ColumnsSkipped = ColumnsSkipped;
            _RowsHigh = RowsHeight;
            _RowsTotal = RowsTotal;
            _RowsSkipped = RowsSkipped;
            _Padding = Margin;

            Screen monitor = new Screen(1);
            PositionX = monitor.PushLeft()  + ( monitor.ColumnWidth(ColumnsTotal) * ColumnsSkipped);
            PositionX += Margin / 2;
            PositionY = monitor.PushTop()   + ( monitor.RowHeight(RowsTotal) * RowsSkipped);
            PositionY += Margin / 2;
            Width = monitor.ColumnWidth(ColumnsTotal) * ColumnsWidth;
            Width -= Margin;
            Height = monitor.RowHeight(RowsTotal) * RowsHeight;
            Height -= Margin;
            
            Console.WriteLine(PositionX);
            Console.WriteLine(PositionY);
            Console.WriteLine(Width);
            Console.WriteLine(Height);
        }
        public Rectangle GetWindowRectangle()
        {
            return new Rectangle(PositionX, PositionY, Width, Height);
        }

    }
}
