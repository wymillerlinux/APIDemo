namespace Demo_WebAPI_Weather
{
    public struct LocationZip
    {
        public double Zip { get; set; }

        public LocationZip(double zip)
        {
            this.Zip = zip;
        }
    }
}