using Newtonsoft.Json;

namespace Executables.Check
{
    public class Publisher
    {
        public string Name { get; set; }
        public IEnumerable<File> Files { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
