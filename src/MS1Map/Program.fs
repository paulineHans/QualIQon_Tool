module QualIQon.Main

open System
open System.IO
open Argu
open System.Reflection
open CLI_Parsing 


[<EntryPoint>]
let main argv =
    let errorHandler = ProcessExiter(colorizer = function ErrorCode.HelpText -> None | _ -> Some System.ConsoleColor.Red)
    let parser = ArgumentParser.Create<CLIArguments>(programName =  (System.Reflection.Assembly.GetExecutingAssembly().GetName().Name),errorHandler=errorHandler)     
    let results = parser.Parse argv
    let i = results.GetResult DirectoryPath 
    let o = results.GetResult Pipeline        
    
    let execution = 
        System.IO.Directory.CreateDirectory ((String.concat "" [| "./arc/runs" ;i;"/Results/MS1map" |]))
        MS1Map.filesToMassSpectrum i o 
        0
    execution