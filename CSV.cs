using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using System;

namespace Minerva.Module
{
    public interface IRow : IEnumerable<string>
    {
        string this[string col] { get; set; }
        string Name { get; }
        int Count { get; }

        public static IRow Of(string name, Dictionary<string, string> value)
        {
            return new Row(name, value);
        }


        struct Row : IRow
        {
            string key;
            Dictionary<string, string> value;

            public Row(KeyValuePair<string, Dictionary<string, string>> item)
            {
                this.key = item.Key;
                this.value = item.Value;
            }

            public Row(string Key, Dictionary<string, string> Value)
            {
                this.key = Key;
                this.value = Value;
            }

            public string this[string col] { get => value[col]; set => this.value[col] = value; }

            public string Name => key;

            public int Count => value.Count;

            public IEnumerator<string> GetEnumerator()
            {
                return value.Values.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return value.Values.GetEnumerator();
            }
        }
    }

    public interface ITable : IEnumerable, IEnumerable<IRow>
    {
        string this[string row, string col] { get; set; }
        IRow this[string row] { get; }
        int Count { get; }
        string[] ColumnNames { get; }
        string[] RowNames { get; }


        IRow GetOrCreateRow(string rowName);


        public static Dictionary<string, Dictionary<string, string>> ToDictionaries(ITable table)
        {
            var @this = table;
            Dictionary<string, Dictionary<string, string>> res = new Dictionary<string, Dictionary<string, string>>();
            foreach (var rowName in @this.RowNames)
            {
                Dictionary<string, string> t = new();
                res.Add(rowName, t);
                var row = @this[rowName];
                foreach (var colName in @this.ColumnNames)
                {
                    t.Add(colName, row[colName]);
                }
            }

            return res;
        }

        public static void Convert<TSource, TTarget>(TSource sourceTable, TTarget target) where TSource : ITable where TTarget : ITable
        {
            foreach (var rowName in sourceTable.RowNames)
            {
                IRow targetRow = target.GetOrCreateRow(rowName);
                IRow sourceRow = sourceTable[rowName];
                foreach (var colName in sourceTable.ColumnNames)
                {
                    targetRow[colName] = sourceRow[colName];
                }
            }
        }

        public static TTarget Convert<TSource, TTarget>(TSource sourceTable) where TSource : ITable where TTarget : ITable, new()
        {
            var target = new TTarget();
            Convert(sourceTable, target);
            return target;
        }
    }

    public class CSVFile : ITable
    {
        public Dictionary<string, Dictionary<string, string>> table;
        public string[] cols;
        public string[] rows;

        public CSVFile()
        {
            table = new();
            cols = Array.Empty<string>();
            rows = Array.Empty<string>();
        }

        public IRow this[string row] => throw new System.NotImplementedException();

        public string this[string row, string col] { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public int Count => table.Count;

        public string[] ColumnNames => cols;

        public string[] RowNames => rows;

        public IRow GetOrCreateRow(string rowName)
        {
            if (table.TryGetValue(rowName, out var value))
                return IRow.Of(rowName, value);

            Array.Resize(ref rows, rows.Length + 1);
            rows[^1] = rowName;
            value = new Dictionary<string, string>();
            table.Add(rowName, value);
            return IRow.Of(rowName, value);
        }

        public IEnumerator<IRow> GetEnumerator()
        {
            foreach (var item in table)
            {
                yield return IRow.Of(item.Key, item.Value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    /// <summary>
    /// The CSV Reader/Writer used in lcoalization
    /// </summary>
    public static class CSV
    {
        private const char CSV_SEPARATOR = ',';

        public static string ConvertToCSV(string name, ITable table)
        {
            StringBuilder sb = new(name);
            sb.Append(CSV_SEPARATOR);
            sb.Append(string.Join(CSV_SEPARATOR, table.ColumnNames));
            sb.Append('\n');

            foreach (var item in table)
            {
                sb.Append(item.Name);
                sb.Append(CSV_SEPARATOR);
                var entries = string.Join(
                    CSV_SEPARATOR,
                    item.Select(
                        text => !text.Contains("\"") && !text.Contains(",")
                        ? text
                        : $"\"{text.Replace("\"", "\"\"")}\""
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

            file.cols = entries.Dequeue().Split(CSV_SEPARATOR)[1..].ToArray();
            var rows = new List<string>();

            while (entries.Count != 0)
            {
                var entry = entries.Dequeue();
                if (string.IsNullOrEmpty(entry) || string.IsNullOrWhiteSpace(entry)) continue;
                List<string> words = GetWords(entry);
                while (file.cols.Length > words.Count - 1)
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
                rows.Add(row);

                words.RemoveAt(0);
                var dict = new Dictionary<string, string>();
                for (int i = 0; i < file.cols.Length; i++)
                {
                    dict.Add(file.cols[i], words[i]);
                }
                file.table.Add(row, dict);
            }
            file.rows = rows.ToArray();
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