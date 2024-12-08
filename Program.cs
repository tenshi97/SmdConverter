// See https://aka.ms/new-console-template for more information

using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Xml;

namespace SmdConverter
{
    class Program
    {
        public static List<Bone> Bones = new();
        public static void Main(string[] args)
        {
            Console.WriteLine("SMD Converter v1.0.0 coded by Rainsfield 08/12/2024");
            while (true)
            {
                Console.WriteLine("输入需要转换的smd名(包含后缀),输入all转换当前目录下所有smd,输入exit退出");
                var filename = Console.ReadLine();
                if (filename == "exit")
                {
                    break;
                }
                
                if (filename == "all")
                {
                    string[] smdfiles = Directory.GetFiles(".", "*.smd");
                    foreach (var smdfile in smdfiles)
                    {
                        var truename = smdfile.Replace("./", "");
                        Console.WriteLine(truename);
                        DoWork(truename);
                    }

                    continue;
                }
                
                if (!File.Exists(filename))
                {
                    Console.WriteLine("文件不存在");
                    continue;
                }

                DoWork(filename);
            }
        }

        public static void DoWork(string filename)
        {
            Bones.Clear();
            var content = File.ReadAllText(filename);
            if (string.IsNullOrEmpty(content))
            {
                Console.WriteLine($"文件{filename}读取失败");
                return;
            }

            var lines = content.Split('\n');

            for (var i = 0; i < lines.Length; i++)
            {
                lines[i] = lines[i].Trim();
            }
            var index1 = Array.IndexOf(lines,"nodes");
            var index2 = Array.IndexOf(lines,"end");
            if (index1 < 0 || index2 < 0)
            {
                Console.WriteLine($"文件{filename}格式不正确——没有找到骨骼段 请检查是否有nodes和end行{index1} {index2}");
                return;
            }

            for (var i = index1 + 1; i < index2; i++)
            {
                var line = lines[i];
                var bonestr = line.Split(" ");
                if (bonestr.Length != 3)
                {
                    Console.WriteLine($"文件{filename}格式不正确 第{i}行骨骼语句格式错误");
                    return;
                }

                int id = 0;
                int parentId = 0;
                if (!int.TryParse(bonestr[0], out id))
                {
                    Console.WriteLine($"文件{filename}格式不正确 第{i}行骨骼语句格式错误 id转换失败");
                    return;
                }

                if (!int.TryParse(bonestr[2], out parentId))
                {
                    Console.WriteLine($"文件{filename}格式不正确 第{i}行骨骼语句格式错误 父id转换失败");
                    return;
                }

                var name = bonestr[1].Trim('\"');
                Console.WriteLine($"{id} {name} {parentId}");
                var bone = new Bone(id,name,parentId);
                Bones.Add(bone);
            }

            foreach (var b in Bones)
            {
                if (b.Parent != -1)
                {
                    var parent = Bones.FirstOrDefault(p=>p.Index == b.Parent);
                    if (parent == null)
                    {
                        Console.WriteLine($"文件{filename}格式不正确 骨骼{b.Index}({b.Name})的parent({b.Parent})未找到");
                        return;
                    }
                    parent.Children.Add(b.Index);
                }   
            }
            
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,  // 格式化输出，显示缩进
                Converters = { new JsonFloatConverter(6) } 
            };
            S2VANMResults results = new S2VANMResults(Bones);
            var outputstr = JsonSerializer.Serialize(results, options);
            var ofname = filename.Split('.').First() + ".vanmgrph";
            File.WriteAllText(ofname, outputstr);
            Console.WriteLine($"处理完成,输出到文件{ofname}");
        }
            
    }
    public class JsonFloatConverter : System.Text.Json.Serialization.JsonConverter<float>
    {
        private readonly int _precision;

        public JsonFloatConverter(int precision)
        {
            _precision = precision;
        }

        public override float Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.GetSingle();
        }

        public override void Write(Utf8JsonWriter writer, float value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString($"F{_precision}"));
        }
    }
}