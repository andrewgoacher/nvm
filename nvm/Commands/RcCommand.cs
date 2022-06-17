//using System.CommandLine;

//namespace nvm.Commands
//{
//    internal class RcCommand : Command
//    {
//        public RcCommand() : base("rc", 
//            "Allows interaction with the nearest nvmrc file")
//        {
//            var useOption = new Option<bool>("use", "Tells nvm to use the nvmrc in the current directory.  This is default");
//            useOption.SetDefaultValue(false);

//            var ignoreOption = new Option<bool>("ignore", "Tells nvm to ignore the nvmrc in the current directory.");
//            ignoreOption.SetDefaultValue(false);

//            AddOption(useOption);
//            AddOption(ignoreOption);

//            this.AddValidator(validator =>
//            {
//                var useValue = validator.GetValueForOption(useOption);
//                var ignoreValue = validator.GetValueForOption(ignoreOption);

//                if (useValue && ignoreValue || !(useValue || ignoreValue))
//                {
//                    validator.ErrorMessage = "Only 1 can be set";
//                }
//            });

//            this.SetHandler((bool use, bool ignore) =>
//            {
//                Console.WriteLine("Use: {0}", use);
//                Console.WriteLine("Ignore: {0}", ignore);
//            }, useOption, ignoreOption);
//        }
//    }
//}
