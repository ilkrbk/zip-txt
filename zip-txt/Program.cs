using System;
using System.Collections.Generic;
using System.IO;

namespace zip_txt
{
    class Program
    {
        static void Main(string[] args)
        {
            args = new[] {"--decompress", "text1.txt", "text2.txt"};
            (string, string, string) param = ConvertParam(args);
            switch (param.Item1)
            {
                case "--compress":
                    CompressFile(param);
                    break;
                case "--decompress":
                    DecompressFile(param);
                    break;
            }
            
        }
        static (string, string, string) ConvertParam(string[] args)
        {
            (string, string, string) param = (args[0], args[1], args[2]);
            return param;
        }
        static void CompressFile((string, string, string) param)
        {
            List<string> list = ReadByte(param);
            List<(string, string)> listStep1 = ReplaceRepeat(list);
            List<string> listStep2 = SumOfSingleElem(listStep1);
            WriteByte(listStep2, param);
        }
        static List<string> ReadByte((string, string, string) param)
        {
            List<string> list = new List<string>();
            FileStream fstream = File.OpenRead(param.Item2);
            byte[] array = new byte[fstream.Length];
            fstream.Read(array, 0, array.Length);
            foreach (var item in array)
            {
                list.Add(Convert.ToString(item, 16));
            }
            return list;
        }
        static List<(string, string)> ReplaceRepeat(List<string> list)
        {
            List<(string, string)> listResult = new List<(string, string)>();
            for (int i = 0; i < list.Count; i++)
            {
                int count = 1;
                while (i + count <= list.Count - 1 && list[i] == list[i + count])
                    count++;
                listResult.Add((Convert.ToString(count, 16), list[i]));
                i += count - 1;
            }
            return listResult;
        }
        static List<string> SumOfSingleElem(List<(string, string)> list)
        {
            List<string> listResult = new List<string>();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Item1 == "1")
                {
                    int count = 1;
                    while (list[i].Item1 == "1" && list[i].Item1 == list[i + count].Item1)
                    {
                        count++;
                    }
                    listResult.Add("00");
                    listResult.Add(Convert.ToString(count, 16));
                    for (int j = i; j < i + count; j++)
                    {
                        listResult.Add(list[j].Item2);
                    }
                    i += count - 1;
                }
                else
                {
                    listResult.Add(list[i].Item1);
                    listResult.Add(list[i].Item2);
                }
            }
            return listResult;
        }
        static void WriteByte(List<string> list, (string, string, string) param)
        {
            byte[] byteList = new byte[list.Count];
            for (int i = 0; i < list.Count; i++)
                byteList[i] = Convert.ToByte(list[i], 16);
            File.WriteAllBytes(param.Item3, byteList);
        }
        
        //=========================================================================
        
        static void DecompressFile((string, string, string) param)
        {
            List<string> list = ReadByte(param);
            List<string> listStep1 = UnZipper(list);
            WriteByte(listStep1, param);

        }
        static List<string> UnZipper(List<string> list)
        {
            List<string> listResult = new List<string>();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == "0")
                {
                    for (int j = 0; j < Convert.ToInt32(list[i + 1]); j++)
                    {
                        listResult.Add(list[i + j + 2]);
                    }
                    i += Convert.ToInt32(list[i + 1]) + 1;
                }
                else
                {
                    for (int j = 0; j < Convert.ToInt32(list[i], 16); j++)
                    {
                        listResult.Add(list[i + 1]);
                    }
                    i += 1;
                }
            }
            return listResult;
        }
    }
}