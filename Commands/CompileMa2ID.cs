using MaiLib;
using ManyConsole;

namespace MaichartConverter
{
    /// <summary>
    ///     Compile Ma2 Command
    /// </summary>
    public class CompileMa2ID : ConsoleCommand
    {
        /// <summary>
        ///     Return when command successfully executed
        /// </summary>
        private const int Success = 0;

        /// <summary>
        ///     Return when command failed to execute
        /// </summary>
        private const int Failed = 2;

        /// <summary>
        ///     Source file path
        /// </summary>
        public string? FileLocation { get; set; }

        /// <summary>
        ///     Difficulty
        /// </summary>
        public string? Difficulty { get; set; }

        /// <summary>
        ///     ID
        /// </summary>
        public string? ID { get; set; }

        /// <summary>
        ///     Destination of output
        /// </summary>
        public string? Destination { get; set; }

        /// <summary>
        ///     Target Format of the file
        /// </summary>
        public string? TargetFormat { get; set; }

        /// <summary>
        ///     Rotation option for charts
        /// </summary>
        /// <value>Clockwise90/180, Counterclockwise90/180, UpsideDown, LeftToRight</value>
        public string? Rotate { get; set; }

        /// <summary>
        ///     OverallTick Shift for the chart: if the shift tick exceeds the 0 Bar 0 Tick, any note before 0 bar 0 tick will be
        ///     discarded.
        /// </summary>
        /// <value>Tick, 384 tick = 1 bar</value>
        public int? ShiftTick { get; set; }

        /// <summary>
        ///     Construct Command
        /// </summary>
        public CompileMa2ID()
        {
            IsCommand("CompileMa2ID", "Compile assigned Ma2 chart to assigned format");
            HasLongDescription(
                "This function enables user to compile ma2 chart specified to the format they want. By default is simai for ma2.");
            HasRequiredOption("d|difficulty=",
                "REQUIRED: The number representing the difficulty of chart -- 0-4 for Basic to Re:Master",
                diff => Difficulty = diff);
            HasRequiredOption("i|id=", "REQUIRED: The id of the ma2", id => ID = id);
            HasRequiredOption("p|path=", "REQUIRED: Folder of A000 to override - end with a path separator",
                path => FileLocation = path);
            //FileLocation = GlobalPaths[0];
            //HasOption("a|a000=", "Folder of A000 to override - end with a path separator", path => FileLocation = path);
            HasOption("f|format=", "The target format - simai or ma2", format => TargetFormat = format);
            HasOption("r|rotate=",
                "Rotating method to rotate a chart: Clockwise90/180, Counterclockwise90/180, UpsideDown, LeftToRight",
                rotate => Rotate = rotate);
            HasOption("s|shift=", "Overall shift to the chart in unit of tick", tick => ShiftTick = int.Parse(tick));
            HasOption("o|output=", "Export compiled chart to location specified", dest => Destination = dest);
        }

        /// <summary>
        ///     Execute the command
        /// </summary>
        /// <param name="remainingArguments">Rest of the arguments</param>
        /// <returns>Code of execution indicates if the commands is successfully executed</returns>
        /// <exception cref="FileNotFoundException">Raised when the file is not found</exception>
        public override int Run(string[] remainingArguments)
        {
            try
            {
                Ma2Tokenizer tokenizer = new();
                Ma2Parser parser = new();
                //Chart good = new Ma2(@"/Users/neskol/MaiAnalysis/A000/music/music" + musicID + "/" + musicID + "_0" + difficulty + ".ma2");
                string tokenLocation = FileLocation ?? throw new FileNotFoundException("Token location not specified");
                string compensatedId =
                    Program.CompensateZero(ID ?? throw new NullReferenceException("ID shall not be null"));

                string ma2Path = Path.Combine(
                    tokenLocation,
                    "music",
                    $"music{compensatedId}",
                    $"{compensatedId}_0{Difficulty}.ma2"
                );

                if (!File.Exists(ma2Path))
                {
                    Console.WriteLine($"MA2 file not found, skipping: {ma2Path}");
                    return Success;
                }

                Chart candidate = parser.ChartOfToken(tokenizer.Tokens(ma2Path));

                if (Rotate != null)
                {
                    bool rotationIsValid = Enum.TryParse(Rotate, out NoteEnum.FlipMethod rotateMethod);
                    if (!rotationIsValid) throw new Exception("Given rotation method is not valid. Given: " + Rotate);
                    candidate.RotateNotes(rotateMethod);
                }

                if (ShiftTick != null && ShiftTick != 0) candidate.ShiftByOffset((int)ShiftTick);

                string result = "";
                switch (TargetFormat)
                {
                    case null:
                    case "":
                    case "simai":
                    case "Simai":
                    case "SimaiFes":
                        Simai resultChart = new(candidate);
                        // resultChart.Update();
                        result = resultChart.Compose();
                        if (Destination != null && !Destination.Equals(""))
                        {
                            string targetMaidataLocation = $"{Destination}/maidata.txt";
                            if (!Directory.Exists(Destination)) Directory.CreateDirectory(Destination);
                            StreamWriter sw = new(targetMaidataLocation, false);
                            {
                                sw.WriteLine(result);
                            }
                            sw.Close();
                            if (File.Exists(targetMaidataLocation))
                                Console.WriteLine("Successfully compiled at: {0}", targetMaidataLocation);
                            else
                                throw new FileNotFoundException("THE FILE IS NOT SUCCESSFULLY COMPILED.");
                        }
                        else
                        {
                            Console.WriteLine(result);
                        }

                        break;
                    case "ma2":
                    case "Ma2":
                    case "MA2":
                    case "Ma2_103":
                    case "Ma2_104":
                        if (result.Equals(""))
                        {
                            Ma2 defaultChart = TargetFormat.Equals("Ma2_104")
                                ? new Ma2(candidate) { ChartVersion = ChartEnum.ChartVersion.Ma2_104 }
                                : new Ma2(candidate);
                            result = defaultChart.Compose();
                        }

                        if (Destination != null && !Destination.Equals(""))
                        {
                            string targetMaidataLocation = $"{Destination}/result.ma2";
                            if (!Directory.Exists(Destination)) Directory.CreateDirectory(Destination);
                            StreamWriter sw = new(targetMaidataLocation, false);
                            {
                                sw.WriteLine(result);
                            }
                            sw.Close();
                            if (File.Exists(targetMaidataLocation))
                                Console.WriteLine("Successfully compiled at: {0}", targetMaidataLocation);
                            else
                                throw new FileNotFoundException("THE FILE IS NOT SUCCESSFULLY COMPILED.");
                        }
                        else
                        {
                            Console.WriteLine(result);
                        }

                        break;
                }

                return Success;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Program cannot proceed becasue of following error returned: \n{0}", ex.GetType());
                Console.Error.WriteLine(ex.Message);
                Console.Error.WriteLine(ex.StackTrace);
                Console.ReadKey();
                return Failed;
            }
        }
    }
}