namespace QualIQon.Tools.Misscleavages

open Argu
open CLI_Parsing
open System.Reflection
open createMisscleavagePlot



module consule_MC =
    
    [<EntryPoint>]
        let main argv =
            let errorHandler = ProcessExiter(colorizer = function ErrorCode.HelpText -> None | _ -> Some System.ConsoleColor.Red)
            let parser = ArgumentParser.Create<CLIArguments>(programName =  (System.Reflection.Assembly.GetExecutingAssembly().GetName().Name),errorHandler=errorHandler)     
            let results = parser.Parse argv
            let i = results.GetResult DirectoryPath 
            let o = results.GetResult Pipeline       
            let execution = 
                MCPlot i o 
            execution
            0