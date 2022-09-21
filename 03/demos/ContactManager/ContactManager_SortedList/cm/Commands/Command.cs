using System;
using System.Collections.Generic;
using ContactManager;

namespace cm
{
    internal interface ICommand
    {
        string Verb { get; }
        IReadOnlyDictionary<string, string> Args { get; }

        IEnumerable<Contact> Execute();
    }

    internal abstract class Command : ICommand
    {
        public string Verb { get; }

        public IReadOnlyDictionary<string, string> Args { get; }

        protected ContactStore Store { get; }

        protected Command(string verb, IReadOnlyDictionary<string, string> args, ContactStore store)
        {
            Verb = verb;
            Args = args;
            Store = store;
        }

        public abstract IEnumerable<Contact> Execute();
    }

    internal struct Commands
    {
        public const string Add = "add";
        public const string Remove = "remove";
        public const string Find = "find";
        public const string Quit = "quit";
        public const string Error = "error";
        public const string List = "list";
        public const string Load = "load";
        public const string Save = "save";
    }
}