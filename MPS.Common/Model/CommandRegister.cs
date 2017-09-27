using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Serialization;


namespace MPS.Common.Model
{
    [Serializable]
    public class CommandRegister : ObservableObject, IDataErrorInfo
    {
        private readonly SolidColorBrush editBrush = new SolidColorBrush(Colors.Yellow);
        private readonly SolidColorBrush newBrush = new SolidColorBrush(Colors.LimeGreen);
        private readonly SolidColorBrush normalBrush = new SolidColorBrush(Colors.White);
        [XmlAttribute]
        public string AssociateGroup { get; set; }
        [XmlAttribute]
        public string RegisterName { get; set; }
        [XmlAttribute]
        public byte CommandAddress { get; set; }
        [XmlAttribute]
        public byte CommandLength { get; set; }
        [XmlAttribute]
        public string CommandPossibleValues { get; set; }
        [XmlAttribute]
        public string CommandRight { get; set; }
        [XmlAttribute]
        public string CommandDataType { get; set; }
        [XmlAttribute]
        public string Unit { get; set; }
        [XmlAttribute]
        public double Lsb { get; set; }

        //[XmlIgnore]
        //public double RealLsb
        //{
        //    get
        //    {
        //        if (ReadLsb != null)
        //            return ReadLsb(RegisterName);
        //        return Lsb;
        //    }
        //}

        private SolidColorBrush _CommandBackColor;
        [XmlIgnore]
        public SolidColorBrush CommandBackColor
        {
            get
            {
                return this._CommandBackColor;
            }
            set
            {
                this._CommandBackColor = value;
                RaisePropertyChanged("CommandBackColor");
            }
        }
        private bool _isSelected;
        [XmlIgnore]
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }

        private string _CommandData;
        [XmlAttribute]
        public string CommandData
        {
            get
            {
                return this._CommandData;
            }
            set
            {

                this._CommandData = value;

                if (CommanSourcedData == null)
                {
                    CommandBackColor = normalBrush;
                    CommanSourcedData = value;
                }
                else
                {
                    if (string.Equals(CommanSourcedData, value))
                    {
                        CommandBackColor = normalBrush;
                    }
                    else
                    {
                        CommandBackColor = newBrush;

                    }
                  
                }                          

                RaisePropertyChanged("CommandData");
            }
        }

        private UInt16 _CommandDacValue;
        [XmlIgnore]
        public UInt16 CommandDacValue
        {
            get
            {
                return this._CommandDacValue;
            }
            set
            {
                this._CommandDacValue = value;
                RaisePropertyChanged("CommandDacValue");
            }
        }

        [XmlIgnore]
        public string CommanSourcedData { get; set; }

        [XmlIgnore]
        public double CommandMin
        {
            get
            {
                return 0;
            }
        }

        //private double _CommandMax;
        [XmlIgnore]
        public double CommandMax
        {
            get
            {
                return (Math.Pow(2, CommandLength) - 1) * Lsb;
            }
        }

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

                if (columnName == "CommandData")
                {
                    if (!string.IsNullOrEmpty(CommanSourcedData) && string.IsNullOrEmpty(CommandPossibleValues))
                    {
                        double rlt;
                        if (!double.TryParse(CommandData, out rlt))
                        {
                            result = "Invalid data type for double";
                        }

                        if (GetLSB != null)
                        {
                            Lsb = GetLSB(RegisterName);
                            // CommandData = (CommandDacValue * Lsb).ToString ();
                        }
                        //else
                        //{
                        //     tempLsb = Lsb;

                        //}
                        
                        if (rlt < 0 || rlt > (Math.Pow(2, CommandLength) - 1) * Lsb)
                        {
                            result = string.Format("The value must be in the range:{0}--{1}, and step is {2}", 0, (Math.Pow(2, CommandLength) - 1) * Lsb, Lsb);
                        }

                    }
                }
                _validateResult = result;
                return result;
            }
        }

        public delegate double ParseLsbDelegate(string regName);
        [XmlIgnore]
        public ParseLsbDelegate GetLSB;
        //[XmlIgnore]
        //public ParseLsbDelegate ReadLsb;
    }
}
