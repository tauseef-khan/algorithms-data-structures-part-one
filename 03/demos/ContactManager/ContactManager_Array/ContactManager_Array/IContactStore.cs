using System;
using System.Collections.Generic;

namespace ContactManager
{
    public interface IContactStore
    {
        Contact Add(Contact contact);
        Contact Remove(Contact contact);

        IEnumerable<Contact> Contacts { get; }
    }
}