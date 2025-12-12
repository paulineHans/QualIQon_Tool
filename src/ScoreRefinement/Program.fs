namespace QualIQon 

open Plotly.NET.TraceObjects
open Plotly.NET.LayoutObjects
open Plotly.NET
open ProteomIQon.Dto

module consule_ScoreRefinement =
    
    [<EntryPoint>]
        let main argv =
            let errorHandler = ProcessExiter(colorizer = function ErrorCode.HelpText -> None | _ -> Some System.ConsoleColor.Red)
            let parser = ArgumentParser.Create<CLIArguments>(programName =  (System.Reflection.Assembly.GetExecutingAssembly().GetName().Name),errorHandler=errorHandler)     
            let results = parser.Parse argv
            let i = results.GetResult directoryPath 
            let o = results.GetResult pipeline        
            let execution = 
                System.IO.Directory.CreateDirectory ((String.concat "" [| "./arc/runs" ;i;"/Results/ScoreRefinement" |]))
                ScoreRefinement.final_execution i o 
            0