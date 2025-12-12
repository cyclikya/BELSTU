namespace ASPA008_1
{
    public class CelebritiesConfig
    {
        public string PhotosRequestPath { get; set; }
        public string PhotosFolder { get; set; }
        public string ConnectionString { get; set; }
        public string ISO3166alpha2Path { get; set; }
        public CelebritiesConfig() {
            this.PhotosRequestPath = "/Photos";
            this.PhotosFolder = "Photos";
            this.ConnectionString = "Host=localhost;Port=5432;Database=Celebrity;Username=postgres;Password=vivi5567";
            this.ISO3166alpha2Path = "CountryCodes\\Лабораторная_08__iso3166-1-alpha2-country-codes.json";
        }
    }
}