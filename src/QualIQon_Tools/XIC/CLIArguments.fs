namespace QualIQon 


open System
open ProteomIQon
open Plotly.NET
open MzIO.IO.MzML
open MzIO.Processing
open MzIO.Model

module CLI_Parsing =
  
    type CLIArguments =
        | [<AltCommandLine("-i")>] DirectoryPath of path:string 
        | [<AltCommandLine("-o")>] Pipeline  of path:string 

    with
        interface IArgParserTemplate with
            member s.Usage =
                match s with
                | DirectoryPath _ -> "Directory Name needed for files (*.psm, *.tsv , *.txt)"
                | Pipeline  _ -> "pipeline used needed (FragPipe, MaxQuant, ProteomIQon)"