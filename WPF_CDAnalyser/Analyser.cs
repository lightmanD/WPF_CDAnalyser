using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPF_CDAnalyser
{/// <summary>
/// Полный расчет всех характеристик
/// </summary>
    class Analyser
    {
        private int _no_of_sequence;

        public List<double> MeansList { get; private set; }
        public Dictionary<string, int> SourceDataDic { get; private set; }
        public int GroupNumber { get; private set; }

        public List<List<double>> meansByWafer = new List<List<double>>();
        public List<List<double>> resultData = new List<List<double>>();

        public List<List<double>> meansInGroupByLot = new List<List<double>>();
        public List<List<double>> resultDataByLot = new List<List<double>>();

        public List<List<double>> MeansByWafer { get => meansByWafer; set => meansByWafer = value; }
        public List<List<double>> ResultData { get => resultData; set => resultData = value; }
        public List<List<double>> MeansInGroupByLot { get => meansInGroupByLot; set => meansInGroupByLot = value; }
        public List<List<double>> ResultDataByLot { get => resultDataByLot; set => resultDataByLot = value; }

        public Analyser(List<double> meansList, Dictionary<string, int> sourceDataDic, int groupNumber)
        {
            MeansList = meansList;
            SourceDataDic = sourceDataDic;
            GroupNumber = groupNumber;

            //Int32.TryParse(sourceDataDic["no_of_sequence"].ToString(), out int group_number);
            // _no_of_sequence = group_number;

            _no_of_sequence = sourceDataDic["no_of_sequence"];

            CalculatingByWafer();
        }

        public Analyser(List<ResultObject> waferResults)
        {
            int group_number = waferResults[0].GroupNumber;

            //List<List<List<double>>> waferGroupMean= new List<List<List<double>>>();
            //foreach (var elem in waferResults)
            //{
            //    waferGroupMean.Add(elem.MeansByGroup);
            //}

            CalculatingByLot(waferResults, group_number);
        }

        public double Mean(List<double> inputArray)
        {
            List<double> listWithoutZero = new List<double>();
            foreach (var elem in inputArray)
            {
                if (elem != 0) listWithoutZero.Add(elem);
            }

            return listWithoutZero.Average();
        }

        public double Sigma(List<double> inputArray)
        {
            List<double> listWithoutZero = new List<double>();
            foreach (var elem in inputArray)
            {
                if (elem != 0) listWithoutZero.Add(elem);
            }

            double sigma = 0;

            double mean = Mean(listWithoutZero);

            foreach (double elem in listWithoutZero)
            {
                sigma += Math.Pow(elem - mean, 2);
            }

            sigma = Math.Pow(sigma / (listWithoutZero.Count - 1), 0.5);
            return sigma;
        }

        public double Range(List<double> inputArray)
        {
            List<double> listWithoutZero = new List<double>();
            foreach (var elem in inputArray)
            {
                if (elem != 0) listWithoutZero.Add(elem);
            }

            return listWithoutZero.Max() - listWithoutZero.Min();

        }

        public double Max(List<double> inputArray)
        {
            List<double> listWithoutZero = new List<double>();
            foreach (var elem in inputArray)
            {
                if (elem != 0) listWithoutZero.Add(elem);
            }

            return listWithoutZero.Max();
        }

        public double Min(List<double> inputArray)
        {
            List<double> listWithoutZero = new List<double>();
            foreach (var elem in inputArray)
            {
                if (elem != 0) listWithoutZero.Add(elem);
            }
            return listWithoutZero.Min();
        }

        private void CalculatingByWafer()
        {
            try
            {
                for (int i = 0; i < GroupNumber; i++)
                {
                    meansByWafer.Add(new List<double>());
                    resultData.Add(new List<double>());
                    for (int j = i; j < _no_of_sequence; j += GroupNumber)
                    {
                        meansByWafer[i].Add(MeansList[j]);
                    }

                    resultData[i].Add(Mean(MeansByWafer[i]));
                    resultData[i].Add(Sigma(MeansByWafer[i]));
                    resultData[i].Add(Range(MeansByWafer[i]));
                    resultData[i].Add(Min(MeansByWafer[i]));
                    resultData[i].Add(Max(MeansByWafer[i]));

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("CalcualtingByWafer Error!" + e.Message.ToString());
            }

        }

        private void CalculatingByLot(List<ResultObject> waferResults, int groupNumber)
        {
            //var allMeansByGroups = new List<List<double>>();

            for (int i = 0; i < groupNumber; i++)
            {
                meansInGroupByLot.Add(new List<double>());

                for (int j = 0; j < waferResults.Count; j++)
                {

                    foreach (var elem in waferResults[j].MeansByGroup[i])

                        meansInGroupByLot[i].Add(elem);

                }
            }
            Console.WriteLine($"meansInGroupByLot{meansInGroupByLot.Count}");
            Console.WriteLine($"meansInGroupByLot[0]{meansInGroupByLot[0].Count}");

            int counter = 0;
            foreach (var elem in meansInGroupByLot)
            {
                resultDataByLot.Add(new List<double>());
                resultDataByLot[counter].Add(Mean(elem));
                resultDataByLot[counter].Add(Sigma(elem));
                resultDataByLot[counter].Add(Range(elem));
                resultDataByLot[counter].Add(Min(elem));
                resultDataByLot[counter].Add(Max(elem));


                counter++;
            }

        }
    }
}
