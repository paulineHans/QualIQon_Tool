namespace QualIQon 

open System
open ProteomIQon
open Plotly.NET
open MzIO.IO.MzML
open MzIO.Processing
open MzIO.Model

module console_MS1Map = 
    [<EntryPoint>]
        let main argv =
            let errorHandler = ProcessExiter(colorizer = function ErrorCode.HelpText -> None | _ -> Some System.ConsoleColor.Red)
            let parser = ArgumentParser.Create<CLIArguments>(programName =  (System.Reflection.Assembly.GetExecutingAssembly().GetName().Name),errorHandler=errorHandler)     
            let results = parser.Parse argv
            let i = results.GetResult directoryPath 
            let o = results.GetResult pipeline        
            
            let execution = 
                System.IO.Directory.CreateDirectory ((String.concat "" [| "./arc/runs" ;i;"/Results/MS1map" |]))
                MS1Map.filesToMassSpectrum i o 
            0