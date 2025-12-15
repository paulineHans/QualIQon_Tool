namespace QualIQon 

open System
open System.IO
open Deedle
open FSharp.Stats
open Plotly.NET
open FSharpAux
open Plotly.NET.TraceObjects
open Plotly.NET.LayoutObjects
open Argu
open Corr_Light


module console_Corr_Light = 
    [<EntryPoint>]
    let main (argv : string array) : int=
        let errorHandler = ProcessExiter(colorizer = function ErrorCode.HelpText -> None | _ -> Some System.ConsoleColor.Red)
        let parser = ArgumentParser.Create<CLIArguments>(programName =  (System.Reflection.Assembly.GetExecutingAssembly().GetName().Name),errorHandler=errorHandler)     
        let results = parser.Parse argv
        let i = results.GetResult directoryPath 
        let o = results.GetResult pipeline 
        let p = results.GetResult labeldData      

        let execute_Corr_Heavy = 
            let createDirectory = System.IO.Directory.CreateDirectory ((String.concat "" [|"./arc/runs"; i; "/Results/Corr_Light"|]))
            let labeling = p
            let checkExecution = 
                if labeling = "14N" then do
                    Corr_Light.finalHeatmapQuantLight i o
                else  Corr_Light.finalHeatmapQuantLight i o |> ignore 
            checkExecution
            0
        execute_Corr_Heavy