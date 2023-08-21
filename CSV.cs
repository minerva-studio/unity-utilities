using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using Table = System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, string>>;

namespace Minerva.Module
{

    public class CSVFile
    {
        public Table table;
        public List<string> cols;
        public List<string> rows;

        public CSVFile()
        {
            table = new();
            cols = new();
            rows = new();
        }
    }

    /// <summary>
    /// The CSV Reader/Writer used in lcoalization
    /// </summary>
    public static class CSV
    {
        private const char CSV_SEPARATOR = ',';

        public static string ConvertToCSV(string name, Table table)
        {
            StringBuilder sb = new(name);
            sb.Append(CSV_SEPARATOR);
            sb.Append(string.Join(CSV_SEPARATOR, table.FirstOrDefault().Value.Keys));
            sb.Append('\n');

            foreach (var item in table)
            {
                sb.Append(item.Key);
                sb.Append(CSV_SEPARATOR);
                var entries = string.Join(
                    CSV_SEPARATOR,
                    item.Value.Select(
                        text => !text.Value.Contains("\"") && !text.Value.Contains(",")
                        ? text.Value
                        : $"\"{text.Value.Replace("\"", "\"\"")}\""
                        )
                );
                sb.Append(entries);
                sb.Append("\n");
            }
            return sb.ToString();
        }

        public static CSVFile Import(string path)
        {
            string input = File.ReadAllText(path);

            var file = new CSVFile();
            var entries = new Queue<string>(input.Split('\n'));

            file.cols = entries.Dequeue().Split(CSV_SEPARATOR).ToList();
            file.cols.RemoveAt(0);

            while (entries.Count != 0)
            {
                var entry = entries.Dequeue();
                if (string.IsNullOrEmpty(entry) || string.IsNullOrWhiteSpace(entry)) continue;
                List<string> words = GetWords(entry);
                while (file.cols.Count > words.Count - 1)
                {
                    words.Add(string.Empty);
                    //Debug.LogError(file.cols.Count);
                    //Debug.LogError(words.Count);
                    //foreach (var item in words)
                    //{
                    //    Debug.Log(item);
                    //}
                    //throw new InvalidDataException();
                }
                string row = words[0];
                file.rows.Add(row);
                words.RemoveAt(0);

                var dict = new Dictionary<string, string>();
                for (int i = 0; i < file.cols.Count; i++)
                {
                    dict.Add(file.cols[i], words[i]);
                }
                file.table.Add(row, dict);
            }
            return file;
        }

        private static List<string> GetWords(string entry)
        {
            List<string> words = new();
            StringBuilder stringBuilder = new StringBuilder();
            bool isInQuote = false;
            for (int i = 0; i < entry.Length; i++)
            {
                char c = entry[i];
                switch (c)
                {
                    case ',':
                        // in quote, save
                        if (isInQuote)
                        {
                            stringBuilder.Append(c);
                        }
                        // not in quote, end of word
                        else
                        {
                            words.Add(stringBuilder.ToString());
                            stringBuilder.Clear();
                        }
                        break;
                    case '\"':
                        // start of a new word, start with "
                        if (!isInQuote && stringBuilder.Length == 0)
                        {
                            isInQuote = true;
                            continue;
                        }
                        // a "", means literal "
                        if (i + 1 < entry.Length && entry[i + 1] == '\"')
                        {
                            stringBuilder.Append(c);
                            i++;
                        }
                        // else the end of the word
                        else
                        {
                            isInQuote = false;
                        }
                        break;
                    default:
                        stringBuilder.Append(c);
                        break;
                }
            }
            words.Add(stringBuilder.ToString());
            return words;
        }
    }
}