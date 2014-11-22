using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurneroGraficos
{
    public static class Data
    {
        public static List<Measurement> GetData()
        {
            List<Measurement> res = new List<Measurement>();
            res.Add(new Measurement { DetectorId = 1, Value = 25, DateTime = new DateTime(2014, 09, 13, 8, 29, 0) });
            res.Add(new Measurement { DetectorId = 1, Value = 20, DateTime = new DateTime(2014, 09, 13, 8, 45, 0) });
            res.Add(new Measurement { DetectorId = 2, Value = 22, DateTime = new DateTime(2014, 09, 13, 8, 56, 0) });
            res.Add(new Measurement { DetectorId = 2, Value = 28, DateTime = new DateTime(2014, 09, 13, 8, 33, 0) });
            res.Add(new Measurement { DetectorId = 3, Value = 23, DateTime = new DateTime(2014, 09, 13, 8, 20, 0) });
            res.Add(new Measurement { DetectorId = 3, Value = 26, DateTime = new DateTime(2014, 09, 13, 8, 15, 0) });
            res.Add(new Measurement { DetectorId = 3, Value = 25, DateTime = new DateTime(2014, 09, 13, 8, 40, 0) });
            res.Add(new Measurement { DetectorId = 3, Value = 23, DateTime = new DateTime(2014, 09, 13, 8, 09, 0) });
            res.Add(new Measurement { DetectorId = 4, Value = 29, DateTime = new DateTime(2014, 09, 13, 8, 12, 0) });
            res.Add(new Measurement { DetectorId = 4, Value = 24, DateTime = new DateTime(2014, 09, 13, 8, 24, 0) });
            return res;
        }
    }

    public class Measurement
    {
        public long DetectorId {get;set;}
        public int Value {get;set;}
        public DateTime DateTime {get;set;}
    }
}
