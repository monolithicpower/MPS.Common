using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.DataSources;

namespace MPS.Common.Chart
{
    public class LineGraphViewModel : ViewModelBase
    {
        /// <summary>
        /// Gets or sets the point data source.
        /// </summary>
        /// <value>The point data source.</value>
        public ObservableDataSource<System.Windows.Point> PointDataSource
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name of the line graph.</value>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>The color.</value>
        public System.Windows.Media.Color Color
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the entity id.
        /// </summary>
        /// <value>The entity id.</value>
        public Guid EntityId
        {
            get;
            set;
        }

        public bool LineAndMarker
        {
            get;
            set;
        }

        public int Thickness
        {
            get;
            set;
        }
    }
}
