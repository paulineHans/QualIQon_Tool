namespace QualIQon

open System.IO 
open Argu

module CLI_Parsing =
  
    type CLIArguments =
        | [<Mandatory>] [<AltCommandLine("-i")>] DirectoryPath of path:string 
        | [<Mandatory>] [<AltCommandLine("-o")>] Pipeline  of string 

    with
        interface IArgParserTemplate with
            member s.Usage =
                match s with
                | DirectoryPath _ -> "Directory Name needed for files (*.psm, *.tsv , *.txt)"
                | Pipeline  _ -> "pipeline used needed (FragPipe, MaxQuant, ProteomIQon)"
                            