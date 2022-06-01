using System.CommandLine;

namespace nvm.Commands
{
    internal class ListCommand : Command
    {
        public ListCommand() : base("list", 
            "Lists all versions of node installed currently")
        {
            var allOption = new Option<bool>("--all", "list all versions of node available");
            AddOption(allOption);

            var dateOption = new Option<bool>("--date", "display the date along with the version");
            AddOption(dateOption);

            this.SetHandler((bool all, bool date) =>
            {
                Console.WriteLine("All: {0}", all);
                Console.WriteLine("Date: {0}", date);
            }, allOption, dateOption);
        }
    }
}
