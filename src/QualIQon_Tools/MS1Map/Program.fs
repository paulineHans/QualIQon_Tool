namespace QualIQon.Tools.MS1Map

open System
open System.IO
open Argu
open System.Reflection
open CLI_Parsing 
open createMS1Mapi

module console_MS1Map = 
    [<EntryPoint>]
    let main argv =
        let errorHandler = ProcessExiter(colorizer = function ErrorCode.HelpText -> None | _ -> Some System.ConsoleColor.Red)
        let parser = ArgumentParser.Create<CLIArguments>(programName =  (System.Reflection.Assembly.GetExecutingAssembly().GetName().Name),errorHandler=errorHandler)     
        let results = parser.Parse argv
        let i = results.GetResult DirectoryPath         
        let execution = 
            MS1MapPlot i 
        execution
        0