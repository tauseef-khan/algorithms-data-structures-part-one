using System;
using System.Collections.Generic;
using System.Linq;
using ContactManager.Filters;
using DataStructures;

namespace ContactManager
{
    public class ContactStore : IContactStore
    {
        SortedList<Contact> contacts = new SortedList<Contact>();

        int nextId = 1;

        public IEnumerable<Contact> Contacts
        {
            get
            {
                return contacts;
            }
        }

        public Contact Add(Contact contact)
        {
            if (contact == null)
            {
                Log.Error("Add: null contact provided (skipping)");
                throw new ArgumentNullException("Contacts cannot be null");
            }

            int id = contact.ID.HasValue ? contact.ID.Value : nextId++;
            nextId = Math.Max(nextId, id + 1);

            Contact withId = Contact.CreateWithId(id, contact);

            Log.Verbose("Add: adding new contact with ID {0} ({1} {2})", withId.ID, withId.FirstName, withId.LastName);
            contacts.Add(withId);

            Log.Verbose("Add: complete ({0})", withId.ID);

            return withId;
        }

        public IEnumerable<Contact> Add(IEnumerable<Contact> contacts)
        {
            if (contacts == null)
            {
                Log.Error("Add: null contacts provided");
                throw new ArgumentNullException("contacts");
            }

            int beforeCount = this.contacts.Count;

            foreach (Contact c in contacts)
            {
                Add(c);
            }

            Log.Info("Added {0} contacts", this.contacts.Count - beforeCount);

            return Contacts;
        }

        public IEnumerable<Contact> Load(IEnumerable<Contact> newContacts)
        {
            nextId = 1;

            Add(newContacts);

            Log.Info("Loaded {0} contacts", contacts.Count);

            return contacts;
        }

        public bool Remove(ContactFieldFilter filter, out Contact removed)
        {
            Contact toRemove = Search(filter).FirstOrDefault();
            return Remove(toRemove, out removed);
        }

        public bool Remove(Contact contact, out Contact removed)
        {
            if (contacts.Remove(contact))
            {
                Log.Info("Remove: removed contact {0} ({1} {2})", contact.ID.Value, contact.FirstName, contact.LastName);
                removed = contact;
                return true;
            }

            Log.Warning("Remove: Contact not found.  No action taken.");
            removed = default;
            return false;
        }

        public IEnumerable<Contact> Search(ContactFieldFilter filter)
        {
            return filter.Apply(this.Contacts);
        }

    }
}