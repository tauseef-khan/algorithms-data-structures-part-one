using System;
using System.Collections.Generic;
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
                Log(LogLevel.Error, "Add: null contact provided (skipping)");
                throw new ArgumentNullException("Contacts cannot be null");
            }

            int id = contact.ID.HasValue ? contact.ID.Value : nextId++;
            nextId = Math.Max(nextId, id + 1);

            Contact withId = Contact.CreateWithId(id, contact);

            Log(LogLevel.Info, "Add: adding new contact with ID {0} ({1} {2})", withId.ID, withId.FirstName, withId.LastName);
            contacts.Add(withId);

            Log(LogLevel.Info, "Add: complete ({0})", withId.ID);

            return withId;
        }

        public IEnumerable<Contact> Add(IEnumerable<Contact> contacts)
        {
            if (contacts == null)
            {
                throw new ArgumentNullException("contacts");
            }

            foreach (Contact c in contacts)
            {
                Add(c);
            }

            return Contacts;
        }

        public IEnumerable<Contact> Load(IEnumerable<Contact> newContacts)
        {
            nextId = 1;
            return Add(newContacts);
        }

        public bool Remove(Contact contact, out Contact removed)
        {
            if (contact == null)
            {
                Log(LogLevel.Error, "Remove: null contact provided");
                throw new ArgumentNullException("Null contact provided to Remove");
            }
            else
            {
                if (contacts.Find(contact, out removed))
                {
                    contacts.Remove(removed);
                    Log(LogLevel.Info, "Remove: removed contact {0} ({1} {2})", removed.ID.Value, removed.FirstName, removed.LastName);
                    return true;
                }
            }

            Log(LogLevel.Info, "Remove: Contact not found.  No action taken.");
            removed = null;
            return false;
        }

        public IEnumerable<Contact> Search(ContactFieldFilter filter)
        {
            return filter.Apply(this.Contacts);
        }

        protected virtual void Log(LogLevel level, string message)
        {
            OnMessageLogged(new LogMessageEventArgs(level, message));
        }

        protected virtual void Log(LogLevel level, string format, params object[] args)
        {
            OnMessageLogged(new LogMessageEventArgs(level, format, args));
        }

        protected virtual void OnMessageLogged(LogMessageEventArgs args)
        {
            MessageLogged?.Invoke(this, args);
        }

        public event EventHandler<LogMessageEventArgs> MessageLogged;

    }
}