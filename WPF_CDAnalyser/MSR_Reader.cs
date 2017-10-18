using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WPF_CDAnalyser
{/// <summary>
 /// класс для чтения msr файла и сбора данных из него
 /// </summary>
    class MSR_Reader : IDisposable
    {
        private FileStream _file;
        private StreamReader _streamReader;

        private Dictionary<string, string> _header = new Dictionary<string, string>();
        private Dictionary<string, int> _sourseDataDic = new Dictionary<string, int>();
        private List<double> _meansArray = new List<double>();
        private List<string> _mpNamesList = new List<string>();

        public Dictionary<string, string> Header { get => _header; private set => _header = value; }
        public Dictionary<string, int> SourseDataDic { get => _sourseDataDic; private set => _sourseDataDic = value; }
        public List<double> MeansArray { get => _meansArray; private set => _meansArray = value; }
        public List<string> MpNamesList { get => _mpNamesList; private set => _mpNamesList = value; }

        public MSR_Reader(string path)
        {
            try
            {
                this._file = new FileStream(path, FileMode.Open, FileAccess.Read);
                this._streamReader = new StreamReader(_file);
            }
            catch (Exception e)
            {
                Console.WriteLine("!Error File! " + e.Message);
            }

            ReadHead();
            CollectionAllmp();

            CollectionOfSourceData();

            Dispose();
        }

        public void ReadHead()
        {
            Console.WriteLine("+Считывание хэдера+");
            List<string> headerList = new List<string>();

            //reading file
            string temp = "";
            for (int i = 0; i < 11; i++)
            {
                temp = _streamReader.ReadLine();
                headerList.Add(temp);
            }

            String pattern = @"\S+";

            //String pattern = "(\".*?\")";
            List<string> matches = new List<string>();

            foreach (var expression in headerList)
                foreach (Match m in Regex.Matches(expression, pattern))
                    matches.Add(m.ToString().Trim().Replace("/", string.Empty));


            matches.Remove(">ver");
            matches.Remove("MF01");
            matches.Remove("00.00");
            try
            {
                for (int i = 0; i < matches.Count; i += 2)
                {
                    Header.Add(matches[i], matches[i + 1]);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("HEADER ERROR");
            }

            headerList = null;

        }

        public void CollectionOfSourceData()
        {
            Console.WriteLine("+Сбор исходнных данных+");


            string line;
            string typeOfData = "";
            //нахождение исходных данных
            while ((line = _streamReader.ReadLine()) != null)
            {
                if (line.Contains("@no_of_mp"))
                {
                    line = line.Replace("  ", string.Empty).Trim();
                    Char delimetr = ' ';
                    string[] substring = line.Split(delimetr);

                    this.SourseDataDic.Add("no_of_mp", Convert.ToInt32(substring[1]));

                }

                if (line.Contains("@no_of_sequence"))
                {
                    line = line.Trim();
                    Char delimetr = ' ';
                    string[] substring = line.Split(delimetr);
                    this.SourseDataDic.Add("no_of_sequence", Convert.ToInt32(substring[1]));

                }

                if (line.Contains("@no_of_chip"))
                {
                    line = line.Replace("  ", string.Empty).Trim();
                    Char delimetr = ' ';
                    string[] substring = line.Split(delimetr);
                    this.SourseDataDic.Add("no_of_chip", Convert.ToInt32(substring[1]));

                }

                if (line.Contains("@slot_no"))
                {
                    line = line.Replace("       ", string.Empty).Trim();
                    Char delimetr = ' ';
                    string[] substring = line.Split(delimetr);
                    this.SourseDataDic.Add("slot_no", Convert.ToInt32(substring[1]));

                }

                if (line.Contains("1 : Data") || line.Contains("2 : Data"))
                {
                    line = line.Replace(" ", string.Empty).Replace("nm", string.Empty).Replace(".", ",").Trim();
                    Char delimetr = ':';
                    string[] substring = line.Split(delimetr);

                    typeOfData = substring[2];

                    break;
                }
            }


            if (typeOfData == "Mean'")
                //сбор всех стредних штрих
                while ((line = _streamReader.ReadLine()) != null)
                {

                    bool rulle_Mean = line.Contains("Mean'") && !line.Contains("Data");
                    if (rulle_Mean)
                    {

                        line = line.Replace(" ", string.Empty).Replace("nm", string.Empty).Replace(".", ",").Trim();

                        Char delimetr = ':';
                        string[] substring = line.Split(delimetr);
                        this.MeansArray.Add(Convert.ToDouble(substring[2]));
                        // Console.WriteLine(Convert.ToDouble(substring[2]));
                    }

                }
            //сбор всех диаметров
            else
                while ((line = _streamReader.ReadLine()) != null)
                {

                    bool rulle_Mean = line.Contains("Diameter") && !line.Contains("Data") && !line.Contains("X") && !line.Contains("Y") && !line.Contains("Object") && !line.Contains("Measurement");
                    if (rulle_Mean)
                    {

                        line = line.Replace(" ", string.Empty).Replace("nm", string.Empty).Replace(".", ",").Trim();

                        Char delimetr = ':';
                        string[] substring = line.Split(delimetr);
                        this.MeansArray.Add(Convert.ToDouble(substring[2]));

                    }

                }

            //проверка колличества данных и sequence
            if (SourseDataDic["no_of_sequence"] != MeansArray.Count)
            {
                Console.WriteLine("Колличество данных и количество sequence'ов не совпадает! возможны ошибки !");
            }

        }

        public void CollectionAllmp()
        {
            Console.WriteLine("+Считывание всех имен групп из MSR+");
            List<string> mpList = new List<string>();
            int total_of_mp_name = 0;
            string line;

            while ((line = _streamReader.ReadLine()) != null)
            {
                string pattern = "~total_of_mp_name";

                if (line.Contains(pattern))
                {
                    // Console.WriteLine(line);
                    string[] temp = line.Trim().Split(' ');
                    Int32.TryParse(temp[temp.Length - 1], out total_of_mp_name);
                    break;
                }
            }

            int counter = 0;

            while ((line = _streamReader.ReadLine()) != null)
            {
                //Console.WriteLine(line);

                string patternMp = "~mp_name";

                if (line.Contains(patternMp))
                {

                    mpList.Add(line);
                }

                if (counter == total_of_mp_name) break;

                counter++;
            }

            //foreach (var elem in mpList) Console.WriteLine(elem);



            string re1 = ".*?"; // Non-greedy match on filler
            string re2 = "(\".*?\")";   // Double Quote String 1

            Regex r = new Regex(re1 + re2, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            foreach (var expression in mpList)
            {
                Match m = r.Match(expression);
                if (m.Success)
                {
                    String string1 = m.Groups[1].ToString();
                    string1 = string1.Replace('"', ' ');
                    Console.WriteLine(string1);
                    _mpNamesList.Add(string1.ToString());
                }
            }

        }

        public void Dispose()
        {
            _streamReader.Close();
            _streamReader = null;
            _file.Close();
            _file = null;
            //throw new NotImplementedException();
        }
    }
}
