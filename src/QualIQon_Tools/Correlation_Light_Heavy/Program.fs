namespace QualIQon.Tool.CorrLHP


open System
open Argu
open CLI_Parsing
open System.Reflection
open QualIQon.IO
open QualIQon.Plots
open createCorrelationLightHeavyPlot

module consule_Correlation_Light_Heavy = 
    [<EntryPoint>]
        let main argv =
            let errorHandler = ProcessExiter(colorizer = function ErrorCode.HelpText -> None | _ -> Some System.ConsoleColor.Red)
            let parser = ArgumentParser.Create<CLIArguments>(programName =  (System.Reflection.Assembly.GetExecutingAssembly().GetName().Name),errorHandler=errorHandler)     
            let results = parser.Parse argv
            let i = results.GetResult DirectoryPath 
            let o = results.GetResult Pipeline 
            let p = results.GetResult LabeledData        
            let exe = 
                let labeling  = p
                let check = 
                    if labeling = "15N" then
                        Some (CorrelationLightHeavy i o)
                    else 
                        None
                check
                0
            exe