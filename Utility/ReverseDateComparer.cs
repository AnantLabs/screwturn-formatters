using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Keeper.Garrett.ScrewTurn.Utility
{
    public class ReverseDateComparer : IComparer<DateTime>
    {
        public int Compare(DateTime x, DateTime y)
        {
            return -1 * DateTime.Compare(x, y);
        }
    }
}
