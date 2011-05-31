using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace Nvelope
{
    /// <summary>
    /// Extentions on ASP web forms and other such things
    /// </summary>
    public static class AspExtensions
    {
        public static IEnumerable<ListItem> ToListItems(this ListItemCollection coll)
        {
            foreach (var item in coll)
                yield return item as ListItem;
        }
    }
}
