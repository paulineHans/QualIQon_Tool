namespace QualIQon 


open System
open ProteomIQon
open Plotly.NET
open MzIO.IO.MzML
open MzIO.Processing
open MzIO.Model

module consule_XIC =
    
    [<EntryPoint>]
        let main argv =
            let errorHandler = ProcessExiter(colorizer = function ErrorCode.HelpText -> None | _ -> Some System.ConsoleColor.Red)
            let parser = ArgumentParser.Create<CLIArguments>(programName =  (System.Reflection.Assembly.GetExecutingAssembly().GetName().Name),errorHandler=errorHandler)     
            let results = parser.Parse argv
            let i = results.GetResult directoryPath 
            let o = results.GetResult pipeline        
            let execution = 
                let dir = System.IO.Directory.CreateDirectory ((String.concat "" [| "./arc/runs" ;i;"/Results/XIC" |]))
                let exe = XIC.filesToMassSpectrum i o 
                exe
            execution
            0