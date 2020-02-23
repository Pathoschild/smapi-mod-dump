namespace EiTK.Update
{
    public class contactData
    {
        public string websiteName { get; }
        public string websiteLink { get; }

        public contactData(string websiteName,string websiteLink)
        {
            this.websiteName = websiteName;
            this.websiteLink = websiteLink;
        }
    }
}