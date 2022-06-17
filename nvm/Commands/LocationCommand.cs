//using System.CommandLine;

//namespace nvm.Commands
//{
//    internal class LocationCommand : Command
//    {
//        public LocationCommand() : base("location", 
//            "Gets the path where nvm stores the versions of node installed")
//        {
//            var setOption = new Option<string>("--set", "Sets the path where the versions of node will be installed.");
//            var moveOption = new Option<bool>("--move", "Instructs nvm to copy the existing versions across to the new location.\nWithout this, current versions of node that are installed will no longer work.");

//            AddOption(setOption);
//            AddOption(moveOption);

//            AddValidator(validator =>
//            {
//                var setValue = validator.GetValueForOption(setOption);
//                var moveValue = validator.GetValueForOption(moveOption);

//                if (string.IsNullOrEmpty(setValue) && moveValue)
//                {
//                    validator.ErrorMessage = "Cannot specify move without setting a new path";
//                }
//            });

//            this.SetHandler((string set, bool move) =>
//            {
//                Console.WriteLine("Set Directory: {0}", set);
//                Console.WriteLine("Move current: {0}", move);
//            }, setOption, moveOption);
//        }
//    }
//}
