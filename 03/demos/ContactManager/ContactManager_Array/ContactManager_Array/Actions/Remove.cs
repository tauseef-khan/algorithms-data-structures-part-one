using System;
using System.Collections.Generic;

namespace ContactManager.Actions
{
    public class Remove : Action
    {
        public Remove(IContactStore manager, Contact contact)
            : base(manager, contact)
        {
        }

        public override IEnumerable<Contact> Execute()
        {
            return new Contact[1] { manager.Remove(this.contact) };
        }
    }
}
