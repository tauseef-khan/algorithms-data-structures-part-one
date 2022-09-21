using System.Linq;
using System.Collections.Generic;
using System.IO;
using ContactManager;

namespace cm
{
    internal class ListCommand : Command
    {
        public ListCommand(ContactStore store, IReadOnlyDictionary<string, string> args)
            : base(Commands.List, args, store)
        {
        }

        public override IEnumerable<Contact> Execute()
        {
            return Store.Contacts;
        }
    }

    internal class AddCommand : Command
    {
        public AddCommand(ContactStore store, IReadOnlyDictionary<string, string> args)
            : base(Commands.Add, args, store)
        {
        }

        public override IEnumerable<Contact> Execute()
        {
            return new List<Contact> { Store.Add(CommandArgParser.ContactFromArgs(Args)) };
        }
    }

    internal class RemoveCommand : Command
    {
        internal RemoveCommand(ContactStore store, IReadOnlyDictionary<string, string> args)
            : base(Commands.Remove, args, store)
        {
        }

        public override IEnumerable<Contact> Execute()
        {
            Contact toRemove = Store.Search(CommandArgParser.FilterFromArgs(Args)).FirstOrDefault();

            if (toRemove != null)
            {
                Contact removed = Store.Remove(toRemove);

                if (removed != null)
                {
                    return new List<Contact> { removed };
                }
            }

            return new List<Contact>(0);
        }
    }

    internal class FindCommand : Command
    {
        internal FindCommand(ContactStore store, IReadOnlyDictionary<string, string> args)
            : base(Commands.Find, args, store)
        {
        }

        public override IEnumerable<Contact> Execute()
        {
            return Store.Search(CommandArgParser.FilterFromArgs(Args));
        }
    }

    internal class LoadCommand : Command
    {
        internal LoadCommand(ContactStore store, IReadOnlyDictionary<string, string> args)
            : base(Commands.Load, args, store)
        {
        }

        public override IEnumerable<Contact> Execute()
        {
            List<Contact> result = new List<Contact>(0);

            if (!Args.ContainsKey("file"))
            {
                Log.Error(@"Load command requires a 'file' argument with the path to an existing CSV file.");
                return result;
            }

            string file = Args["file"];
            if (!File.Exists(file))
            {
                Log.Error(@"Load command 'file' argument must refer to an existing CSV file.");
                return result;
            }

            using (Stream strm = File.OpenRead(file))
            {
                CsvContactReader reader = new CsvContactReader();
                return Store.Load(reader.Read(strm));
            }
        }
    }

    internal class SaveCommand : Command
    {
        internal SaveCommand(ContactStore store, IReadOnlyDictionary<string, string> args)
            : base(Commands.Save, args, store)
        {
        }

        public override IEnumerable<Contact> Execute()
        {
            if (!Args.ContainsKey("file"))
            {
                Log.Error(@"Save command requires a 'file' argument with the path to a new or existing CSV file.");
                return null;
            }

            string file = Args["file"];
            using (Stream strm = File.OpenWrite(file))
            {
                CsvContactWriter writer = new CsvContactWriter();
                writer.Write(strm, Store.Contacts);
            }

            return null;
        }
    }

    internal class SyntaxErrorCommand : ICommand
    {
        public string Verb => Commands.Error;

        public IReadOnlyDictionary<string, string> Args => new Dictionary<string, string>(0);

        public IEnumerable<Contact> Execute()
        {
            return new List<Contact>(0);
        }
    }

    internal class UnknownCommand : ICommand
    {
        public string Verb => Commands.Error;

        public IReadOnlyDictionary<string, string> Args => new Dictionary<string, string>(0);

        private string Command { get; }

        public UnknownCommand(string command)
        {
            this.Command = command;
        }

        public IEnumerable<Contact> Execute()
        {
            Log.Error("Unknown command: {0}", this.Command);
            return new List<Contact>(0);
        }
    }


    internal class QuitCommand : ICommand
    {
        public string Verb => Commands.Quit;

        public IReadOnlyDictionary<string, string> Args => new Dictionary<string, string>(0);

        public IEnumerable<Contact> Execute()
        {
            return new List<Contact>(0);
        }
    }
}