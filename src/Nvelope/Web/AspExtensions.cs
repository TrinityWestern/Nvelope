using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace Nvelope.Web
{
    /// <summary>
    /// Extentions on ASP web forms and other such things
    /// </summary>
    public static class AspExtensions
    {
        public static IEnumerable<ListItem> ToListItems(this ListItemCollection firstColumn)
        {
            foreach (var item in firstColumn)
                yield return item as ListItem;
        }
    }
}
