namespace QualIQon.Tools.XIC


open Argu 
open CLI_Parsing
open createXICPlot

module consule_XIC =
    
    [<EntryPoint>]
        let main argv =
            let errorHandler = ProcessExiter(colorizer = function ErrorCode.HelpText -> None | _ -> Some System.ConsoleColor.Red)
            let parser = ArgumentParser.Create<CLIArguments>(programName =  (System.Reflection.Assembly.GetExecutingAssembly().GetName().Name),errorHandler=errorHandler)     
            let results = parser.Parse argv
            let i = results.GetResult DirectoryPath         
            let execution = 
                let exe = XIC i 
                exe
            execution
            0