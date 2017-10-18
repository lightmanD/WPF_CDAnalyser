using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPF_CDAnalyser
{/// <summary>
/// класс для храения результата по одной пластине
/// </summary>
    class ResultObject
    {
        public Dictionary<string, string> Header { set; get; }
        public Dictionary<string, int> SourceInfo { set; get; }
        public int GroupNumber { set; get; }
        public List<string> GroupNames { set; get; }
        public double SigmaSpacing { set; get; }
        public List<double> CtrlValues { set; get; }
        public List<List<double>> MeansByGroup { set; get; }
        public List<List<double>> ResultData { set; get; }




    }
}
