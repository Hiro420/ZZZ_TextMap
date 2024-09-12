using Newtonsoft.Json;
using System.Reflection;
using ZZZ_TextMap.Defs;
using Google.FlatBuffers;

public static class DictionaryExtensions
{
    public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> target, IEnumerable<KeyValuePair<TKey, TValue>> source)
    {
        if (target == null)
            throw new ArgumentNullException(nameof(target));
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        foreach (var element in source)
            target.Add(element.Key, element.Value);
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        if (args.Length < 2 || !File.Exists(args[0]))
        {
            throw new ArgumentException("Please enter a valid location of a binary file (TextMap_ENTemplateTb) and the name of the output JSON file");
        }
        string inputDir = args[0];
        string outputDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!, "output");
        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }
        string outputFilePath = Path.Combine(outputDirectory, args[1]);
        string textmap_output = ParseTextmapFile(inputDir);
        File.WriteAllText(outputFilePath, textmap_output);
        Console.WriteLine("Done!");

    }

    public static string ParseTextmapFile(string filepath)
    {
        Dictionary<string, string> result = new Dictionary<string, string>();

        byte[] buffer = [];
        using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
        {
            buffer = new byte[fs.Length];
            fs.Read(buffer, 0, buffer.Length);
        }
        TextMapTemplateT textmap_template = new();
        var data = TextMapTemplate.GetRootAsTextMapTemplate(new ByteBuffer(buffer)).UnPack();
        foreach (var entry in data.Data)
        {
            result[entry.Key] = entry.Text;
        }

        return DataToJson(result);
    }

    private static string DataToJson<T>(T data)
    {
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
        };

        return JsonConvert.SerializeObject(data, settings);
    }
}
