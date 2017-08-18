using Harvester.Engine.Loaders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace Harvester.GUI
{
    public partial class CMD : Form
    {
        private ProfileLoader ProfileLoader { get; }

        public CMD(ProfileLoader profileLoader)
        {
            ProfileLoader = profileLoader;
            InitializeComponent();
        }

        public List<string> herbCheckedBoxes = new List<string> { };
        public List<string> mineCheckedBoxes = new List<string> { };

        private void LoadProfileButton_Click(object sender, EventArgs e)
            => ProfileLoader.LoadProfile(LoadProfileOFD);

        private void LoadProfileOFD_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void HerbCheckListBox_MouseLeave(object sender, EventArgs e)
        {
            herbCheckedBoxes.Clear();

            foreach (var item in Herbs.Items)
            {
                if (Herbs.GetItemCheckState(Herbs.Items.IndexOf(item)) == CheckState.Checked)
                    herbCheckedBoxes.Add(item.ToString());
            }
        }

        private void MineCheckListBox_MouseLeave(object sender, EventArgs e)
        {
            mineCheckedBoxes.Clear();

            foreach (var item in Mines.CheckedItems)
            {
                if (Mines.GetItemCheckState(Mines.Items.IndexOf(item)) == CheckState.Checked)
                    mineCheckedBoxes.Add(item.ToString());
            }
        }
    }
}