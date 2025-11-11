using System;
using GeopyTestConsoleApp.DataModels;

namespace GeopyTestConsoleApp
{
    public class ResultModel
    {
        public string DivisionName { get; set; }
        public string WellName { get; set; }
        public MeasurementType MeasurementType { get; set; }
        public DateTime Date { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public int Count { get; set; }
    }
}