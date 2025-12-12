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

module CLI_Parsing =
  
    type CLIArguments =
        | [<AltCommandLine("-i")>] DirectoryPath of path:string 
        | [<AltCommandLine("-o")>] Pipeline  of string 

    with
        interface IArgParserTemplate with
            member s.Usage =
                match s with
                | DirectoryPath _ -> "Directory Name needed for files (*.psm, *.tsv , *.txt)"
                | Pipeline  _ -> "pipeline used needed (FragPipe, MaxQuant, ProteomIQon)"
                            