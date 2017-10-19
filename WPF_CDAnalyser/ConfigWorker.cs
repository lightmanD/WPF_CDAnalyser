using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace WPF_CDAnalyser
{/// <summary>
/// Класс для работы с config.xml, конфигурацией рецептов
/// </summary>
    class ConfigWorker : IDisposable
    {
        private XDocument _xDoc;
       
        private string _recipeName;
        private List<string> _mpList;

        public int _groupNumber { get; private set; }
        public List<double> _ctrlValues { get; private set; }
        public List<string> _listGroups { get; private set; }
        public double _sigmaSpacing { get; private set; }

        public ConfigWorker(string inputRecipeName, List<string> mpList)
        {
            _xDoc = new XDocument();
            _xDoc = XDocument.Load("RecipeData.xml");

            _recipeName = inputRecipeName;
            _mpList = mpList;

            if (isRecipe(_recipeName))
            {
                ReadRecipe(_recipeName);
            }
            else
            {
                WriteNewRecipe(_mpList);
                ReadRecipe(_recipeName);
            }
            Dispose();
        }
        public bool isRecipe(string recipeName)
        {
            Console.WriteLine("+Проверка наличия рецепта+");


            foreach (XElement recipeElem in _xDoc.Element("recipes").Elements("recipe"))
            {
                XAttribute nameAttribute = recipeElem.Attribute("name");
                if (nameAttribute.Value.ToString() == recipeName)
                {

                    Console.WriteLine("+Пройдена+");
                    return true;
                }

            }


            Console.WriteLine("+Непройдена+");
            return false;
        }
        public void WriteNewRecipe(List<string> allMP_Points)
        {
            Console.WriteLine("Запись новых данных для рецепта+");

            var mpList = FilteringMPNames(allMP_Points);
            var ctrlValues = WriteCtrlValue();
            var sigma = WriteSigma();

            foreach (var elem in mpList)
            {
                Console.WriteLine(elem);
            }

            XElement root = _xDoc.Element("recipes");

            root.Add(new XElement("recipe",
                new XAttribute("name", _recipeName),
                new XElement("group_number", _groupNumber),
                new XElement("groups", mpList),
                new XElement("ctrl_value", ctrlValues),
                new XElement("sigma_spacing", sigma)));

            _xDoc.Save("RecipeData.xml");
        }
        public List<string> FilteringMPNames(List<string> AllmpNames)
        {

            foreach (var elem in AllmpNames) Console.WriteLine(elem);

            //ввод колличества групп
            WriteGroupNumber();

            List<string> groupNames = new List<string>();

            for (int i = 0; i < _groupNumber; i++)
            {
                string temp = AllmpNames[i].ToString();
                char delim = '-';
                int indexDelim = temp.IndexOf(delim);
                temp = temp.Substring(indexDelim + 1);
                groupNames.Add(temp);
            }
            return groupNames;
        }
        public void WriteGroupNumber()
        {
            Console.WriteLine("+Сбор колличества групп+");
            
            int INT = 0;
            while (true)
            {
                Console.WriteLine("Введите колличество групп: ");
                var fileNumberRead = Console.ReadLine();
                Int32.TryParse(fileNumberRead, out int group_number);
                _groupNumber = group_number;
                if (group_number.GetType().ToString() == INT.GetType().ToString())
                    break;

            }
        }
        private string WriteCtrlValue()
        {
            Console.WriteLine("Введите значения границ(формат Target Deviation МaxSigma  0 0) ");
            string input = Console.ReadLine();

            return input;
        }
        private string WriteSigma()
        {
            Console.WriteLine("Введите интервал (от 1 до 3х сигм)");
            string input = Console.ReadLine();

            return input;
        }
        private void ReadRecipe(string recipeName)
        {
            foreach (XElement recipeElem in _xDoc.Element("recipes").Elements("recipe"))
            {
                XAttribute nameAttribute = recipeElem.Attribute("name");
                XElement groupsNumElement = recipeElem.Element("group_number");
                XElement groupsElement = recipeElem.Element("groups");
                XElement ctrlValuesElement = recipeElem.Element("ctrl_value");
                XElement sigmaSpacingElement = recipeElem.Element("sigma_spacing");

                if (nameAttribute.Value.ToString() == _recipeName)
                {
                    Int32.TryParse(groupsNumElement.Value.ToString(), out int group_number);
                    _groupNumber = group_number;

                    _ctrlValues = ReadCtrlValues(ctrlValuesElement.Value.ToString());

                    var groups = groupsElement.Value;
                    _listGroups = groups.Split(' ').ToList<string>();

                    Double.TryParse(sigmaSpacingElement.Value.ToString(), out double sigmaSpacing);
                    _sigmaSpacing = sigmaSpacing;
                }
            }
        }
        private List<double> ReadCtrlValues(string inputString)
        {
            List<double> ctrlValues = new List<double>();

            string[] stringSplit = inputString.Split(' ');

            foreach (var elem in stringSplit)
            {
                ctrlValues.Add(Double.Parse(elem));
            }

            return ctrlValues;
        }





        public void Dispose()
        {
            _xDoc.Save("RecipeData.xml");
            _xDoc = null;
            //throw new NotImplementedException();
        }
    }
}
