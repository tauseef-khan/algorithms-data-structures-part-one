using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using ContactManager;

namespace cm
{
    internal class Repl
    {
        readonly TextReader input;
        readonly TextWriter output;
        readonly CommandFactory factory;

        readonly Regex verbRegex = new Regex(@"^(?<verb>\w+)");
        readonly Regex fieldsRegex = new Regex(@"(?<field>\w+)=(?<value>[^;]+)");

        internal Repl(TextReader input, TextWriter output, ContactStore store)
        {
            this.input = input;
            this.output = output;
            this.factory = new CommandFactory(store);
            Log.MessageLogged += ConsoleLogger;
        }

        internal void Run()
        {
            bool quitSeen = false;

            while (!quitSeen)
            {
                ICommand cmd = NextCommand();

                Stopwatch timer = new Stopwatch();
                timer.Start();

                switch(cmd.Verb)
                {
                    case Commands.Quit:
                        quitSeen = true;
                        break;
                    case Commands.List:
                        PrintList(cmd.Execute());
                        break;
                    case Commands.Find:
                        PrintList(cmd.Execute());
                        break;
                    default:
                        cmd.Execute();
                        break;
                }

                timer.Stop();
                Log.Verbose("{0} completed after: {1} ms", cmd.Verb, timer.Elapsed.TotalMilliseconds);
            }
        }

        private void PrintList(IEnumerable<Contact> contacts)
        {
            foreach(Contact c in contacts)
            {
                output.WriteLine(c.ToString());
            }
        }

        private ICommand NextCommand()
        {
            Prompt();
            return TryMapToCommand(Read());
        }

        private ICommand TryMapToCommand(string line)
        {
            string verb;
            IReadOnlyDictionary<string, string> args;

            if(ParseLine(line, out verb, out args))
            {
                switch(verb)
                {
                    case Commands.Add:
                        return factory.Add(args);
                    case Commands.Remove:
                        return factory.Remove(args);
                    case Commands.Find:
                        return factory.Find(args);
                    case Commands.Quit:
                        return factory.Quit();
                    case Commands.List:
                        return factory.List(args);
                    case Commands.Save:
                        return factory.Save(args);
                    case Commands.Load:
                        return factory.Load(args);
                    default:
                        return factory.UnknownCommand(verb);
                }
            }

            return factory.SyntaxError();
        }

        private bool ParseLine(string line, out string verb, out IReadOnlyDictionary<string,string> args)
        {
            Log.Verbose("input: {0}", line);

            bool parsedVerb = false;
            Dictionary<string,string> fields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            Match verbMatch = verbRegex.Match(line.TrimStart());
            if(verbMatch.Success)
            {
                verb = verbMatch.Value;
                parsedVerb = true;
                foreach (Match m in fieldsRegex.Matches(line))
                {
                    fields[m.Groups["field"].Value] = m.Groups["value"].Value;
                }
            }
            else
            {
                verb = Commands.Error;
                fields["message"] = string.Format("Unable to parse verb. ({0})", line);
            }

            args = fields;
            return parsedVerb;
        }

        private string Read()
        {
            return input.ReadLine();
        }

        private void Prompt()
        {
            output.Write("> ");
            output.Flush();
        }

        private void ConsoleLogger(object sender, LogMessageEventArgs e)
        {
            switch (e.Level)
            {
                case LogLevel.Verbose:
                    // do nothing
                    break;
                case LogLevel.Info:
                case LogLevel.Warning:
                case LogLevel.Error:
                    this.output.WriteLine(e.Message);
                    this.output.Flush();
                    break;
            }
        }
    }
}
