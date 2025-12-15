namespace QualIQon

open ProteomIQon
open ProteomIQon.Dto
open System 
open System.IO 
open Plotly.NET 
open Plotly.NET.TraceObjects
open Plotly.NET.LayoutObjects
open Deedle
open Argu



module consule_MC =
    
    [<EntryPoint>]
        let main argv =
            let errorHandler = ProcessExiter(colorizer = function ErrorCode.HelpText -> None | _ -> Some System.ConsoleColor.Red)
            let parser = ArgumentParser.Create<CLIArguments>(programName =  (System.Reflection.Assembly.GetExecutingAssembly().GetName().Name),errorHandler=errorHandler)     
            let results = parser.Parse argv
            let i = results.GetResult directoryPath 
            let o = results.GetResult pipeline       
            let execution = 
                System.IO.Directory.CreateDirectory ((String.concat "" [| "./arc/runs" ;i;"/Results/Misscleavages" |]))
                Misscleavages.misscleavages i o 
                logger.Info "Done"
                0
            execution
