using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
namespace MPS.Common.Model
{
    public class LineGraph : ObservableObject
    {
        private string _titleDescription;
        public string TitleDescription
        {

            get
            {
                return this._titleDescription;
            }
            set
            {
                this._titleDescription = value;
                RaisePropertyChanged("TitleDescription");
            }
        }

        private Point _pointData;
        public Point PointData
        {
            get
            {
                return this._pointData;
            }
            set
            {
                this._pointData = value;
                RaisePropertyChanged("PointData");
            }
        }
    }
}
