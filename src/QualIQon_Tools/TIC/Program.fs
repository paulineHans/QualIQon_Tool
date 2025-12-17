namespace QualIQon.Tools.TIC 

open createTICPlot
open Argu 
open CLI_Parsing


module console_TIC = 
        [<EntryPoint>]
            let main argv =
                let errorHandler = ProcessExiter(colorizer = function ErrorCode.HelpText -> None | _ -> Some System.ConsoleColor.Red)
                let parser = ArgumentParser.Create<CLIArguments>(programName =  (System.Reflection.Assembly.GetExecutingAssembly().GetName().Name),errorHandler=errorHandler)     
                let results = parser.Parse argv
                let i = results.GetResult DirectoryPath        
                let execution = 
                    let exe = createTICPlot.TIC i 
                    exe
                execution
                0