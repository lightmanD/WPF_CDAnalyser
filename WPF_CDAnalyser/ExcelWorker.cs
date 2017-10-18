using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WPF_CDAnalyser
{/// <summary>
/// Класс для сохранения всех данных в формате xlsx
/// </summary>
    class ExcelWorker
    {
        private FileInfo _file;
        private string _path;


        public List<ResultObject> Wafers { get; private set; }
        public LotResultObject LotResult { get; private set; }


        public ExcelWorker(List<ResultObject> wafers, LotResultObject lotResult)
        {
            Wafers = wafers;
            LotResult = lotResult;

            FileExcelCreator();
            ExcelWriter();

        }

        private void FileExcelCreator()
        {
            string recipeName = Wafers[0].Header["recipe_name"];
            string lotID = Wafers[0].Header["lot_id"];

            _path = Directory.GetCurrentDirectory() + @"\results\";

            _path += "_" + lotID;
            _path += "_" + recipeName+".xlsx";
            _path = _path.Replace('\"', '_');
            Console.WriteLine(_path);
            _file = new FileInfo(_path);
        }

        private void ExcelWriter()
        {
            using (var package = new ExcelPackage(_file))
            {
                foreach (var wafer in Wafers)
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("W " + wafer.SourceInfo["slot_no"].ToString()+DateTime.Now.Millisecond.ToString());

                    FormWriter(worksheet);
                    HeaderWriter(worksheet, wafer);
                    WaferWriter(worksheet, wafer);
                }

                ExcelWorksheet worksheetForLot = package.Workbook.Worksheets.Add("ByLot");
                FormWriter(worksheetForLot);
                HeaderWriter(worksheetForLot, Wafers[0]);
                LotWriter(worksheetForLot, Wafers[0]);

                Stream stream = File.Create(_path);
                package.SaveAs(stream);
                stream.Close();
            }
        }

        private void FormWriter(ExcelWorksheet workSheet)
        {
            workSheet.Cells[19, 3].Value = "Group name";
            workSheet.Cells[20, 3].Value = "Mean";
            workSheet.Cells[21, 3].Value = "Sigma";
            workSheet.Cells[22, 3].Value = "Range";
            workSheet.Cells[23, 3].Value = "Min";
            workSheet.Cells[24, 3].Value = "Max";

            workSheet.Cells[26, 3].Value = "All values";
        }

        private void WaferWriter(ExcelWorksheet workSheet, ResultObject wafer)
        {

            for (int i = 0; i < wafer.GroupNumber; i++)
            {
                int rowCounter = 19;
                // запись имени группы
                workSheet.Cells[rowCounter++, i + 4].Value = wafer.GroupNames[i];
                // запись агрегатных результатов
                foreach (var val in wafer.ResultData[i])
                    workSheet.Cells[rowCounter++, i + 4].Value = val;

                rowCounter++;
                //запись всех значений
                foreach (var val in wafer.MeansByGroup[i])
                    workSheet.Cells[rowCounter++, i + 4].Value = val;
            }
        }

        private void LotWriter(ExcelWorksheet workSheet, ResultObject wafer)
        {
            for (int i = 0; i < LotResult.MeansByGroup.Count; i++)
            {
                int rowCounter = 19;
                workSheet.Cells[rowCounter++, i + 4].Value = wafer.GroupNames[i];
                foreach (var val in LotResult.ResultData[i])
                    workSheet.Cells[rowCounter++, i + 4].Value = val;
                rowCounter++;
                foreach (var val in LotResult.MeansByGroup[i])
                    workSheet.Cells[rowCounter++, i + 4].Value = val;
            }
        }

        private void ExcelSaver()
        {

        }

        private void HeaderWriter(ExcelWorksheet workSheet, ResultObject wafer)
        {
            var rowCounter = 1;

            foreach (var elem in wafer.Header)
            {
                workSheet.Cells[rowCounter, 1].Value = elem.Key;
                workSheet.Cells[rowCounter, 2].Value = elem.Value;
                rowCounter++;
            }

            foreach(var elem in wafer.SourceInfo)
            {
                workSheet.Cells[rowCounter, 1].Value = elem.Key;
                workSheet.Cells[rowCounter, 2].Value = elem.Value;
                rowCounter++;
            }

        }
    }
}
