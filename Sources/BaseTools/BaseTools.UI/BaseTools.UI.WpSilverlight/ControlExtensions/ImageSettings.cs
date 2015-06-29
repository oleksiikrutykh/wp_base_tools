namespace BaseTools.UI.ControlExtensions
{
    public class ImageSettings
    {
        public ImageSettings()
        {
            this.LoadingImageStub = ImageLoader.ImageStubPath.OriginalString;
        }

        public double Width { get; set; }

        public double Height { get; set; }

        public string LoadingImageStub { get; set; }
    }
}
