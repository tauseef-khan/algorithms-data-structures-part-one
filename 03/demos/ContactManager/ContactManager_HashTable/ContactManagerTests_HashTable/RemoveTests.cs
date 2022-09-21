using System;
using System.Collections.Generic;
using System.Linq;
using ContactManager;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ContactManagerTests
{
    [TestClass]
    public class RemoveTests
    {
        [TestMethod]
        public void RemoveFirst()
        {
            ContactStore store = new ContactStore();
            Contact a = store.Add(Contact.Create("a", "ln", "street", "city", "st", "zip"));
            Contact b = store.Add(Contact.Create("b", "ln", "street", "city", "st", "zip"));
            Contact c = store.Add(Contact.Create("c", "ln", "street", "city", "st", "zip"));

            Remove(store, a);

            Assert.AreEqual(2, store.Contacts.Count());
            Assert.AreEqual("b", store.Contacts.First().FirstName);
            Assert.AreEqual("c", store.Contacts.Last().FirstName);
        }

        [TestMethod]
        public void RemoveLast()
        {
            ContactStore store = new ContactStore();
            Contact a = store.Add(Contact.Create("a", "ln", "street", "city", "st", "zip"));
            Contact b = store.Add(Contact.Create("b", "ln", "street", "city", "st", "zip"));
            Contact c = store.Add(Contact.Create("c", "ln", "street", "city", "st", "zip"));

            Remove(store, c);

            Assert.AreEqual(2, store.Contacts.Count());
            Assert.AreEqual("a", store.Contacts.First().FirstName);
            Assert.AreEqual("b", store.Contacts.Last().FirstName);
        }

        [TestMethod]
        public void RemoveMiddle()
        {
            ContactStore store = new ContactStore();
            Contact a = store.Add(Contact.Create("a", "ln", "street", "city", "st", "zip"));
            Contact b = store.Add(Contact.Create("b", "ln", "street", "city", "st", "zip"));
            Contact c = store.Add(Contact.Create("c", "ln", "street", "city", "st", "zip"));

            Remove(store, b);

            Assert.AreEqual(2, store.Contacts.Count());
            Assert.AreEqual("a", store.Contacts.First().FirstName);
            Assert.AreEqual("c", store.Contacts.Last().FirstName);
        }

        [TestMethod]
        public void RemoveAll()
        {
            ContactStore store = new ContactStore();
            Contact a = store.Add(Contact.Create("a", "ln", "street", "city", "st", "zip"));
            Contact b = store.Add(Contact.Create("b", "ln", "street", "city", "st", "zip"));
            Contact c = store.Add(Contact.Create("c", "ln", "street", "city", "st", "zip"));

            Assert.AreEqual(3, store.Contacts.Count());

            Remove(store, a);
            Remove(store, b);
            Remove(store, c);

            Assert.AreEqual(0, store.Contacts.Count());
        }

        [TestMethod]
        public void RemoveOnly()
        {
            ContactStore store = new ContactStore();
            Contact c = Contact.Create("fn", "ln", "street", "city", "st", "zip");

            c = store.Add(c);
            Assert.AreEqual(1, store.Contacts.Count());

            Remove(store, c);
            Assert.AreEqual(0, store.Contacts.Count());
        }

        [TestMethod]
        public void RemoveMissing()
        {
            ContactStore store = new ContactStore();
            Contact a = store.Add(Contact.Create("a", "ln", "street", "city", "st", "zip"));
            Contact b = store.Add(Contact.Create("b", "ln", "street", "city", "st", "zip"));
            Contact c = store.Add(Contact.Create("c", "ln", "street", "city", "st", "zip"));

            Assert.AreEqual(3, store.Contacts.Count());

            Contact d = Contact.Create("d", "ln", "street", "city", "st", "zip");

            Remove(store, d);

            Assert.AreEqual(3, store.Contacts.Count());

            CollectionAssert.AreEqual(
                new List<string> { "a", "b", "c" },
                store.Contacts.Select(c => c.FirstName).ToList());
        }

        [TestMethod]
        public void RemoveEmpty()
        {
            ContactStore store = new ContactStore();
            Remove(store, Contact.Create("f", "l", "s", "c", "s", "p"));
            Assert.AreEqual(0, store.Contacts.Count());
        }

        [TestMethod]
        public void RemoveSkipsFirstSameContentButMissingID()
        {
            ContactStore store = new ContactStore();
            Contact c1_with_id = store.Add(Contact.Create("fn", "ln", "street", "city", "st", "zip"));
            Contact c2_with_id = store.Add(Contact.Create("fn", "ln", "street", "city", "st", "zip"));
            Contact c3_with_id = store.Add(Contact.Create("fn", "ln", "street", "city", "st", "zip"));
            Contact c4_with_id = store.Add(Contact.Create("fn", "ln", "street", "city", "st", "zip"));
            Contact missing_id = Contact.Create("fn", "ln", "street", "city", "st", "zip");

            Assert.AreEqual(4, store.Contacts.Count());
            Remove(store, missing_id);

            CollectionAssert.AreEqual(
                new List<int> { 1, 2, 3, 4 },
                store.Contacts.Select(c => c.ID).ToList());
        }

        [TestMethod]
        public void RemoveRemovesExactWhenSameContentDifferentIDs()
        {
            ContactStore store = new ContactStore();
            Contact c1_with_id = store.Add(Contact.Create("fn", "ln", "street", "city", "st", "zip"));
            Contact c2_with_id = store.Add(Contact.Create("fn", "ln", "street", "city", "st", "zip"));
            Contact c3_with_id = store.Add(Contact.Create("fn", "ln", "street", "city", "st", "zip"));
            Contact c4_with_id = store.Add(Contact.Create("fn", "ln", "street", "city", "st", "zip"));

            Assert.AreEqual(4, store.Contacts.Count());
            Remove(store, c3_with_id);

            CollectionAssert.AreEqual(
                new List<int> { 1, 2, 4 },
                store.Contacts.Select(c => c.ID).ToList());
        }

        Contact Remove(ContactStore store, Contact c)
        {
            Contact removed;
            store.Remove(c, out removed);

            return removed;
        }
    }
}