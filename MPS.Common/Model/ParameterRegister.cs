using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MPS.Common.Model
{
    public class ParameterRegister : IDataErrorInfo
    {
        [XmlAttribute]
        public string ParameterName { get; set; }
        [XmlAttribute]
        public double ParameterValue { get; set; }
        [XmlAttribute]
        public double MinValue { get; set; }
        [XmlAttribute]
        public double MaxValue { get; set; }

        public string Error
        {
            get
            {
                return _validateResult;
            }
        }
        private string _validateResult;
        public string this[string columnName]
        {
            get
            {
                string result = string.Empty;

                if (columnName == "ParameterValue")
                {
                    if (ParameterValue < MinValue || ParameterValue > MaxValue)
                    {
                        result = string.Format("The value must be in the range:{0}--{1}",MinValue, MaxValue);
                    }
                    if(Double.IsNaN(ParameterValue))
                    {
                        result = "The value is not effective";
                    }
                }
                _validateResult = result;
                return result;
            }

        }
    }
}

