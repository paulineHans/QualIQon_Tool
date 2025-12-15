namespace QualIQon 

open ProteomIQon.Dto
open Plotly.NET.TraceObjects
open Plotly.NET.LayoutObjects
open Plotly.NET 
open FSharpAux.IO.SchemaReader.Attribute
open System
open Argu
open CLI_Parsing
open System.Reflection

module consule_Correlation_Light_Heavy = 
    [<EntryPoint>]
        let main argv =
            let errorHandler = ProcessExiter(colorizer = function ErrorCode.HelpText -> None | _ -> Some System.ConsoleColor.Red)
            let parser = ArgumentParser.Create<CLIArguments>(programName =  (System.Reflection.Assembly.GetExecutingAssembly().GetName().Name),errorHandler=errorHandler)     
            let results = parser.Parse argv
            let i = results.GetResult DirectoryPath 
            let o = results.GetResult Pipeline 
            let p = results.GetResult LabeledData        
            let execution = 
                let createDirectory = System.IO.Directory.CreateDirectory ((String.concat "" [|"./arc/runs"; i; "/Results/Correlation"|]))
                let labeling  = p
                if labeling = "15N" then do Correlation_Light_Heavy.finalHistoCorrelation i else ((printfn "No plot aviable because there is no 15N data used in the experiement"))
                0
            execution