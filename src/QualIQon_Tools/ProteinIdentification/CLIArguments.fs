namespace QualIQon.Tools.ProteinId 

open Argu

module CLI_Parsing =
  
    type CLIArguments =
        | [<AltCommandLine("-i")>] DirectoryPath of path:string 
        | [<AltCommandLine("-o")>] Pipeline  of path:string 
        | [<AltCommandLine("-f")>] FASTA  of path:string 

    with
        interface IArgParserTemplate with
            member s.Usage =
                match s with
                | DirectoryPath _ -> "Directory Name needed for files (*.psm, *.tsv , *.txt)"
                | Pipeline  _ -> "pipeline used needed (FragPipe, MaxQuant, ProteomIQon)"
                | FASTA _ -> "FASTA.file needed"