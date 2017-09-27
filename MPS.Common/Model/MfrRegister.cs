using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace MPS.Common.Model
{
    [Serializable]
    public class MfrRegister : ObservableObject
    {
        [XmlAttribute]
        public string RegisterName { get; set; }
        [XmlAttribute]
        public string RegisterRight { get; set; }
        [XmlAttribute]
        public string RegisterDataType { get; set; }
        private int _byteValue;
        [XmlAttribute]
        public int ByteValue
        {
            get
            {
                return _byteValue;
            }
            set
            {
                _byteValue = value;
                RaisePropertyChanged("ByteValue");

            }
        }
        
        [XmlAttribute]
        public byte Address { get; set; }
        [XmlArrayItem]
        public ObservableCollection<BitControl> BitControls { get; set; }


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




    }
}
