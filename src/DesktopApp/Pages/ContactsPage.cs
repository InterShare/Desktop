using Eto.Forms;

namespace DesktopApp.Dialogs
{
    public class ContactsPage : Panel
    {
        private ListBox _listBox;
        
        public ContactsPage()
        {
            _listBox = new ListBox();
        }
    }
}