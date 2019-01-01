namespace mgs2_v_s_fix
{
    public class UpdateSoftwareSheet
    {
        public string VERSION { get; set; }

        public string ReleaseName { get; set; }

        public string ReleaseURL { get; set; }

        public string CreatedAt { get; set; }

        public string LatestZipName { get; set; }

        public string LatestZipURL { get; set; }

        public string SizeInByte { get; set; }

        public string Changelog { get; set; }


        public UpdateSoftwareSheet()
        {
            VERSION = "-1";
            ReleaseName = "-1";
            ReleaseURL = "-1";
            CreatedAt = "-1";
            LatestZipName = "-1";
            LatestZipURL = "-1";
            SizeInByte = "-1";
            Changelog = "-1";
        }
    } // End class
}