namespace QualIQon

open Argu

module CLIArgumentParsing = 
    open System.IO 

    type CLIArguments = 
        | [<Mandatory>] [<AltCommandLine("-i")>] DirectoryPath of path:string
        | [<Mandatory>] [<AltCommandLine("-o")>] Pipeline of string
        | [<Mandatory>] [<AltCommandLine("-p")>] LabeledData of string
        | [<Mandatory>] [<AltCommandLine("-f")>] FASTA of path:string
    with 
        interface IArgParserTemplate with 
            member s.Usage = 
                match s with 
                | DirectoryPath _ -> "filePath to output files needed"
                | Pipeline _ -> "Pipeline used needed (aviable: MaxQuant, FragPipe, ProteomIQon)"
                | LabeledData _ -> "15N needed"
                | FASTA _ -> "path to FASTA file needed"