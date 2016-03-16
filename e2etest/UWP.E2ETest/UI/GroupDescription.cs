using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.WindowsAzure.MobileServices.Test
{
    /// <summary>
    /// UI Model for a test group
    /// </summary>
    class GroupDescription:ObservableCollection<TestDescription>
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
            }
        }

        public GroupDescription()
        {
        }
    }
}
