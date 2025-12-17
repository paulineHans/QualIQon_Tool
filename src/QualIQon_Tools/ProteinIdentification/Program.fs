namespace QualIQon.Tools.ProteinId

open Argu
open CLI_Parsing
open System.Reflection
open createProteinIdentificationPlot

module consule_ProteinIdentification = 
        [<EntryPoint>]
            let main argv =
                let errorHandler = ProcessExiter(colorizer = function ErrorCode.HelpText -> None | _ -> Some System.ConsoleColor.Red)
                let parser = ArgumentParser.Create<CLIArguments>(programName =  (System.Reflection.Assembly.GetExecutingAssembly().GetName().Name),errorHandler=errorHandler)     
                let results = parser.Parse argv
                let i = results.GetResult DirectoryPath 
                let o = results.GetResult Pipeline 
                let f = results.GetResult FASTA       
                let execution = 
                    let exe = ProteinIdentification i o f
                    exe 
                execution
                0