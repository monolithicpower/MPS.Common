using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace MPS.Common.Model
{
    [Serializable]
    public class GroupItemInfo : ObservableObject
    {
        private string _groupItemName;
        [XmlAttribute]
        public string GroupItemName
        {
            get
            {
                return this._groupItemName;
            }
            set
            {
                this._groupItemName = value;
                //RaisePropertyChanged("GroupItemName");
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
        private ObservableCollection<Register> _registers;
        [XmlArrayItem]
        public ObservableCollection<Register> Registers
        {
            get { return _registers; }
            set
            {
                _registers = value;
                //RaisePropertyChanged("Registers");
            }
        }



        private ObservableCollection<dynamic> _regmapRegisers;
        [XmlIgnore]
        public ObservableCollection<dynamic> RegmapRegisers
        {
            get
            {
                if (_regmapRegisers == null)
                {
                    _regmapRegisers = new ObservableCollection<dynamic>();

                }
                if (Registers != null)
                {

                    foreach (var reg in Registers)
                    {

                        if (reg.Right != EnumCommandRight.R.ToString() && !_regmapRegisers.Contains(reg))
                        {
                            _regmapRegisers.Add(reg);
                        }
                    }
                }
                if (ParamterRegisters != null)
                {
                    foreach (var item in ParamterRegisters)
                    {
                        if (!_regmapRegisers.Contains(item))
                            _regmapRegisers.Add(item);
                    }

                }
                return this._regmapRegisers;
            }
            set
            {
                _regmapRegisers = value;
            }

        }

        private ObservableCollection<ParameterRegister> _paramterRegisters;
        [XmlArrayItem]
        public ObservableCollection<ParameterRegister> ParamterRegisters
        {
            get
            {
                return this._paramterRegisters;
            }
            set
            {
                this._paramterRegisters = value;
                //RaisePropertyChanged("ParamterGroupRegisters");
            }
        }

    }
}
