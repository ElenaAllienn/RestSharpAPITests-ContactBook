using System.Text.Json.Serialization;


namespace RestSharpAPITests
{
    public class Contact
    {
        public int id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string dateCreated { get; set; }
        public string comments { get; set; }
    }
}