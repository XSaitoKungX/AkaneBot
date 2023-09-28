using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Akane.config
{
    internal class JSONReader
    {
        // Declare our Token & Prefix properties of this class
        public string token { get; set; }
        public string prefix { get; set; }

        public async Task ReadJSON() // This method must be run asynchronously
        {
            using (StreamReader sr = new StreamReader("config.json", new UTF8Encoding(false)))
            {
                // Read and then De-Serealize the config.json File
                string json = await sr.ReadToEndAsync();
                JSONStructure data = JsonConvert.DeserializeObject<JSONStructure>(json);

                // Set our properties
                this.token = data.token;
                this.prefix = data.prefix;
            }
        }
    }

    internal sealed class JSONStructure
    {
        public string token { get; set; }
        public string prefix { get; set; }
    }
}
