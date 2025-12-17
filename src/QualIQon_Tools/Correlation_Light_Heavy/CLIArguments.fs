namespace QualIQon.Tool.CorrLH

open Argu

module CLI_Parsing =
  
    type CLIArguments =
        | [<AltCommandLine("-i")>] DirectoryPath of path:string 
        | [<AltCommandLine("-o")>] Pipeline  of string 
        | [<AltCommandLine("-p")>] LabeledData  of string 
    with
        interface IArgParserTemplate with
            member s.Usage =
                match s with
                | DirectoryPath _ -> "Directory Name needed for files (*.psm, *.tsv , *.txt)"
                | Pipeline  _ -> "pipeline used needed (FragPipe, MaxQuant, ProteomIQon)"
                | LabeledData _ -> "15N data needed"