using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TrackStudyProgress
{
    class DBExecutor
    {
        const string path = @"DB.txt";

        static public void FillTheListFromDB(ref ListBox listBox, ref List<ListEl> listOfListEls)
        {
            using (StreamReader streamReader = new StreamReader(path))
            {
                string line = "";
                while ((line = streamReader.ReadLine()) != null)
                {
                    ListEl listEl = new ListEl(listBox.ActualWidth, JsonConvert.DeserializeObject<ListElData>(line));
                    listOfListEls.Add(listEl);
                    listBox.Items.Add(listEl.BaseGrid);
                }
            }
        }
        static public void ModifyDBEntry(string oldData, string newData)
        {
            string[] lines = File.ReadAllLines(path).ToArray();
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i] == oldData)
                {
                    lines[i] = newData;
                    break;
                }
            }
            File.WriteAllLines(path, lines);
        }
        static public void WriteToDB(string data)
        {
            using (StreamWriter streamWriter = new StreamWriter(path, true))
            {
                streamWriter.WriteLine(data);
            }
        }
        static public List<string> ReadFromDB()
        {
            List<string> result = new List<string>();
            using (StreamReader streamReader = new StreamReader(path))
            {
                string line = "";
                while ((line = streamReader.ReadLine()) != null)
                {
                    result.Add(line);
                }
            }
            return result;
        }
        static public void DeleteDBEntry(string data)
        {
            string[] lines = File.ReadAllLines(path).Where(line => line.Trim() != data).ToArray();
            File.WriteAllLines(path, lines);
        }
    }
}
