using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;
namespace MPS.Common.Model
{
    public class BitControl : ObservableObject, IDataErrorInfo
    {

        //[XmlAttribute]
        //public string AssociateAddress { get; set; }
        [XmlAttribute]
        public string BitName { get; set; }
        [XmlAttribute]
        public byte Position { get; set; }
        [XmlAttribute]
        public byte BitLength { get; set; }
        private string _possibleValues;
        [XmlAttribute]
        public string PossibleValues
        {
            get
            {
                return this._possibleValues;
            }
            set
            {
                this._possibleValues = value;
            }
        }

        [XmlIgnore]
        public string SourceBitValue { get; set; }

        private string _currentBitValue;
        [XmlAttribute]
        public string CurrentBitValue
        {
            get
            {
                return _currentBitValue;
            }

            set
            {
                _currentBitValue = value;
                if (SourceBitValue == null)
                {
                    BitBackColor = normalBrush;
                    SourceBitValue = value;
                }
                else
                {
                    if (string.Equals(SourceBitValue, value))
                    {
                        BitBackColor = normalBrush;
                    }
                    else
                    {
                        BitBackColor = newBrush;

                    }
                }
                RaisePropertyChanged("CurrentBitValue");

            }
        }

        private SolidColorBrush _bitBackColor = new SolidColorBrush(Colors.White);
        [XmlIgnore]
        public SolidColorBrush BitBackColor
        {
            get
            {
                return this._bitBackColor;
            }
            set
            {
                this._bitBackColor = value;
                RaisePropertyChanged("BitBackColor");
            }
        }


        [XmlAttribute]
        public double MaxValue { get; set; }
        [XmlAttribute]
        public double MinValue { get; set; }
        [XmlAttribute]
        public double Lsb { get; set; }
        [XmlAttribute]
        public string Unit { get; set; }

        [XmlIgnore]
        public string Error
        {
            get
            {
                return _validateResult;
            }
        }
        private string _validateResult;
        [XmlIgnore]
        public string this[string columnName]
        {
            get
            {
                string result = string.Empty;

                if (columnName == "CurrentBitValue")
                {
                    if (!string.IsNullOrEmpty(SourceBitValue) && string.IsNullOrEmpty(PossibleValues))
                    {
                        double rlt;
                        if (!double.TryParse(CurrentBitValue, out rlt))
                        {
                            result = "Invalid data type for double";
                        }
                        if (GetLSB != null)
                        {
                            Lsb = GetLSB(BitName);
                        }
                        if (MinValue == MaxValue)
                        {

                            if (rlt < 0 || rlt > (Math.Pow(2, BitLength) - 1) * Lsb)
                            {
                                result = string.Format("The value must be in the range:{0}--{1}, and step is {2}", 0, (Math.Pow(2, BitLength) - 1) * Lsb, Lsb);
                            }
                        }
                        else
                        {
                            if (rlt < MinValue || rlt > MaxValue)
                            {
                                result = string.Format("The value must be in the range:{0}--{1}, and step is {2}", MinValue, MaxValue, Lsb);
                            }
                        }


                    }
                }
                _validateResult = result;
                return result;
            }
        }
        private readonly SolidColorBrush editBrush = new SolidColorBrush(Colors.Yellow);
        private readonly SolidColorBrush newBrush = new SolidColorBrush(Colors.LimeGreen);
        private readonly SolidColorBrush normalBrush = new SolidColorBrush(Colors.White);
        public delegate double ParseLsbDelegate(string regName);
        [XmlIgnore]
        public ParseLsbDelegate GetLSB;
    }

}
