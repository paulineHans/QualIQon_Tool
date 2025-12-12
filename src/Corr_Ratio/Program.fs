namespace QualIQon 

open System
open System.IO
open Deedle
open FSharp.Stats
open Plotly.NET
open FSharpAux
open Plotly.NET.TraceObjects
open Plotly.NET.LayoutObjects

module console_Corr_Ratio = 
    [<EntryPoint>]
    let main argv =
            let errorHandler = ProcessExiter(colorizer = function ErrorCode.HelpText -> None | _ -> Some System.ConsoleColor.Red)
            let parser = ArgumentParser.Create<CLIArguments>(programName =  (System.Reflection.Assembly.GetExecutingAssembly().GetName().Name),errorHandler=errorHandler)     
            let results = parser.Parse argv
            let i = results.GetResult directoryPath 
            let o = results.GetResult pipeline
            let p = results.GetResult labeledData        
            let execute_Corr_Ratio = 
                let createDirectory = System.IO.Directory.CreateDirectory ((String.concat "" [|"./arc/runs"; i; "/Results/Corr_Ratio"|]))
                let labeling = p
                let checkExecution = 
                    if labeling = "15N" then do
                        Corr_Ratio.finalHeatmaRatio i o p
                    else  Corr_Ratio.finalHeatmaRatio i o p |> ignore 
                checkExecution
                0
            execute_Corr_Ratio