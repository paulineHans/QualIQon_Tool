namespace QualIQon.Tools.ScoreRefinement

open Argu

module CLI_Parsing =
  
    type CLIArguments =
        | [<AltCommandLine("-i")>] DirectoryPath of path:string  

    with
        interface IArgParserTemplate with
            member s.Usage =
                match s with
                | DirectoryPath _ -> "Directory Name needed for files (*.psm, *.tsv , *.txt)"