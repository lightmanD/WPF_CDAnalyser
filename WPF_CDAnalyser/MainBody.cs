using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPF_CDAnalyser
{/// <summary>
/// Основное рабочее тело программы, на вход подается лишь список путей к файлaм
/// </summary>
    class MainBody
    {
        private string[] _filesPaths;

        ResultObject wrObj;

        private List<ResultObject> resultsList;
        //Dictionary<string, string> headerDic;



        public MainBody(string[] filsePath)
        {
            _filesPaths = filsePath;
            ResultsList = new List<ResultObject>();
        }

        internal List<ResultObject> ResultsList { get => resultsList; set => resultsList = value; }

        public void mainAnalysis()
        {
            int counter = 1;
            foreach (var filePath in _filesPaths)
            {
                Console.WriteLine($"File # {counter}");

                var msrFile = new MSR_Reader(filePath);

                var headerDic = msrFile.Header;
                var meansList = msrFile.MeansArray;
                var sourseInfoDic = msrFile.SourseDataDic;
                var mpAllList = msrFile.MpNamesList;
                
                var ConfigWorker = new ConfigWorker(headerDic["recipe_name"], mpAllList);

                var groupNumber = ConfigWorker._groupNumber;
                var gropsNames = ConfigWorker._listGroups;
                var sigmaSpacing = ConfigWorker._sigmaSpacing;
                var ctrlValues = ConfigWorker._ctrlValues;
                
                var analyser = new Analyser(meansList, sourseInfoDic, groupNumber);

                var meansByGroups = analyser.MeansByWafer;
                var resultData = analyser.resultData;

                wrObj = new ResultObject
                {
                    SourceInfo = sourseInfoDic,
                    Header = headerDic,
                    GroupNumber = groupNumber,
                    GroupNames = gropsNames,
                    SigmaSpacing = sigmaSpacing,
                    CtrlValues = ctrlValues,
                    MeansByGroup = meansByGroups,
                    ResultData = resultData
                };
                ResultsList.Add(wrObj);

                
                counter++;
            }

            var lotAnalysResult = new Analyser(ResultsList);

            var meansByLot = lotAnalysResult.MeansInGroupByLot;
            var resultsByLot = lotAnalysResult.ResultDataByLot;
            
            var lotObj = new LotResultObject { MeansByGroup = meansByLot, ResultData=resultsByLot };

            var excelWriter = new ExcelWorker(ResultsList, lotObj);
            
        }

        
    }
}
