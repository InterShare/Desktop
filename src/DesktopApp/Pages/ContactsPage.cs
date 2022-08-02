using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using DesktopApp.Core;
using DesktopApp.Entities;
using Eto.Drawing;
using Eto.Forms;

namespace DesktopApp.Pages
{
    public class ContactsPage : Panel
    {
        private GridView _contactsGrid;
        private ObservableCollection<Contact> _contacts;
        private readonly List<IDisposable> _subscribedContacts = new ();

        public ContactsPage()
        {
            LoadContacts();

            _contactsGrid = new GridView();
            var layout = new DynamicLayout
            {
                DefaultSpacing = new Size(5, 5),
                Padding = new Padding(10)
            };

            layout.Add(_contactsGrid);

            DrawContactsList();

            Content = layout;
        }

        private void DrawContactsList()
        {
            _contactsGrid.Columns.Clear();
            _contactsGrid.Columns.Add(new GridColumn
            {
                HeaderText = "Name",
                DataCell = new TextBoxCell("ContactName"),
                Expand = true
            });

            _contactsGrid.Columns.Add(new GridColumn
            {
                HeaderText = "Always Allow",
                DataCell = new CheckBoxCell("AlwaysAllowReceivingContent"),
                Editable = true
            });

            _contactsGrid.Columns.Add(new GridColumn
            {
                HeaderText = "Sync Clipboard",
                DataCell = new CheckBoxCell("SyncClipboard"),
                Editable = true
            });

            _contactsGrid.DataStore = _contacts;
        }

        private void LoadContacts()
        {
            _contacts = Config<ConfigFile>.Values.Contacts;
            _contacts.CollectionChanged += ContactsChanged;

            foreach (IDisposable? s in _subscribedContacts)
            {
                s.Dispose();
            }

            _subscribedContacts.Clear();

            foreach (Contact? contact in _contacts)
            {
                _subscribedContacts.Add(
                    contact.AlwaysAllowReceivingContentObservable.Subscribe(x =>
                    {
                        Config<ConfigFile>.Update();
                    })
                );

                _subscribedContacts.Add(
                    contact.SyncClipboardObservable.Subscribe(x =>
                    {
                        Config<ConfigFile>.Update();
                    })
                );
            }
        }

        private void ContactsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            LoadContacts();
            DrawContactsList();
        }
    }
}