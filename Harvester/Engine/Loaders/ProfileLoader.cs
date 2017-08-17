using Harvester.Engine.Loaders.Profile;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ZzukBot.Objects;

namespace Harvester.Engine.Loaders
{
    public class ProfileLoader
    {
        private Loader Loader { get; }
        public ProfileData ProfileData { get; internal set; }
        public List<Location> hotspots { get { return ProfileData.Profile.Hotspots.Select(x => x.Location).ToList(); } }
        public List<Location> vendor { get { return ProfileData.Profile.VendorHotspots.Select(x => x.Location).ToList(); } }
        public List<Location> repair { get { return ProfileData.Profile.Repair.Select(x => x.Location).ToList(); } }
        public int index = 0;

        public ProfileLoader(Loader loader)
        {
            Loader = loader;
        }

        public void LoadProfile(OpenFileDialog dialog)
        {
            DialogResult result = dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                ProfileData = Loader.LoadProfile(
                    dialog.FileName,
                    dialog.SafeFileName.Replace(".xml", "").Replace(".json", ""),
                    dialog.SafeFileName.EndsWith(".xml") ? ProfileExtension.XML : ProfileExtension.JSON,
                    ProfileType.Travel
                );
                index = 0;
            }
        }
    }
}