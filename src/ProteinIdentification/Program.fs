namespace QualIQon

// open Plotly.NET.TraceObjects
// open Plotly.NET.LayoutObjects
// open ProteomIQon.Dto
// open Plotly.NET
// open System
// open BioFSharp
// open BioFSharp.IO
// open BioFSharp.PeptideClassification

open Argu
open CLIArguments
open System.Reflection

module consule_ProteinIdentification = 
        [<EntryPoint>]
            let main argv =
                let errorHandler = ProcessExiter(colorizer = function ErrorCode.HelpText -> None | _ -> Some System.ConsoleColor.Red)
                let parser = ArgumentParser.Create<CLIArguments>(programName =  (System.Reflection.Assembly.GetExecutingAssembly().GetName().Name),errorHandler=errorHandler)     
                let results = parser.Parse argv
                let i = results.GetResult directoryPath 
                let o = results.GetResult pipeline 
                let f = results.GetResult FASTA       
                let execution = 
                    let dir = System.IO.Directory.CreateDirectory ((String.concat "" [| "./arc/runs" ;i;"/Results/ProteinIdentification" |]))
                    let exe = ProteinIdentification.finalChartHisto i o f
                    exe 
                execution
                0