using System.Linq;
using System.Web.UI.WebControls;
using NUnit.Framework;
using Nvelope.Web;

namespace Nvelope.Tests.Web
{
    [TestFixture]
    class AspExtensionTests
    {
        [Test]
        public void ListItems()
        {
            var list = new CheckBoxList();
            list.Items.Add(new ListItem("A deer, a female deer",      "Do"));
            list.Items.Add(new ListItem("A drop of golden sun",       "Re"));
            list.Items.Add(new ListItem("A name, I call myself",      "Me"));
            list.Items.Add(new ListItem("A long, long way to go",     "Fa"));
            list.Items.Add(new ListItem("A needle pulling thread",    "Sol"));
            list.Items.Add(new ListItem("A note to follow Sol",       "La"));
            list.Items.Add(new ListItem("A drink with jam and bread", "Te"));

            var re = list.Items.ToListItems().Where(li => li.Value == "Re").First();
            Assert.AreEqual("A drop of golden sun", re.Text);

        }
    }
}
